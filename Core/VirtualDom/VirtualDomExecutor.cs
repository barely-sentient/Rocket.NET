using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.Core.DOM;

namespace Rocket.Core
{
    /// <summary>
    /// Central manager for the virtual DOM lifecycle.
    /// Responsible for mounting, reconciliation, and hook state management.
    /// Does NOT handle drawing primitives - that is the responsibility of
    /// the IVirtualDomRenderer implementor.
    /// </summary>
    public class VirtualDomExecutor
    {
        private IComponentRenderer _renderer;

        public List<Rocket.Core.Stylesheet> Stylesheets { get; } = new List<Stylesheet>();
        public Node LastNodeToDraw { get; private set; }

        /// <summary>Mounts (or re-mounts) the root node and returns the resolved node to draw, without drawing.</summary>
        public Node MountRoot(Node rootNode)
        {
            var instance = MountOrUpdate(rootNode);
            var cur = instance;
            while (cur != null && cur.Child != null && cur.Child.VNode != null) cur = cur.Child;
            LastNodeToDraw = cur != null ? cur.VNode : null;
            return LastNodeToDraw;
        }
        public CssEngine CssEngine { get; set; } = new CssEngine();

        public VirtualDomExecutor(IComponentRenderer renderer)
        {
            _renderer = renderer;
            CssEngine.Stylesheets = Stylesheets;
        }

        public VirtualDomExecutor(IComponentRenderer renderer, Rocket.Core.CssEngine cssEngine)
        {
            _renderer = renderer;
            CssEngine = cssEngine ?? new Rocket.Core.CssEngine();
            CssEngine.Stylesheets = Stylesheets;
        }
        
        /// <summary>
        /// Renders the root node tree. If a root instance already exists,
        /// the tree is reconciled against the previous state.
        /// </summary>
        /// <param name="rootNode">The root node of the virtual DOM tree to render.</param>
        public void Render(Node rootNode)
        {
            Render(rootNode, 0, 0, Rocket.Core.Orientation.Portrait);
        }

        public void Render(Node rootNode, int viewportWidth, int viewportHeight, Rocket.Core.Orientation orientation)
        {
            if (rootNode == null) return;

            var instance = MountOrUpdate(rootNode);

            Console.WriteLine($"[VDE Render] instance.Child={(instance.Child==null?"null":instance.Child.VNode==null?"nullVNode":instance.Child.VNode.Type.Name)} instance.VNode={instance.VNode.Type.Name}");
            // Unwrap nested composite components to their rendered host node.
            var cur = instance;
            while (cur.Child != null && cur.Child.VNode != null) cur = cur.Child;
            var nodeToDraw = cur.VNode;
            LastNodeToDraw = nodeToDraw;

            // Attach computed styles from the CSS engine for this viewport.
            if (CssEngine != null)
            {
                CssEngine.Stylesheets = Stylesheets;
                CssEngine.Apply(nodeToDraw, viewportWidth, viewportHeight, orientation);
            }

            _renderer.Draw(nodeToDraw);
        }

        public void Render<TProps>(Node<TProps> rootNode) where TProps : new()
        {
            Render((Node)rootNode);
        }

        public void Render<TProps>(Node<TProps> rootNode, int viewportWidth, int viewportHeight, Rocket.Core.Orientation orientation) where TProps : new()
        {
            Render((Node)rootNode, viewportWidth, viewportHeight, orientation);
        }

        public void Render(Node rootNode, Rocket.Core.Viewport viewport)
        {
            if (viewport == null) Render(rootNode);
            else Render(rootNode, viewport.Width, viewport.Height, viewport.Orientation);
        }

        public void Render<TProps>(Node<TProps> rootNode, Rocket.Core.Viewport viewport) where TProps : new()
        {
            Render((Node)rootNode, viewport);
        }

        /// <summary>
        /// Mounts or updates the instance tree based on the node.
        /// </summary>
        private Instance MountOrUpdate(Node node)
        {
            if (node == null) return null;

            // Check if this is a new root (no existing instance) or an update
            // For simplicity, we always mount from scratch; 
            // in a full implementation, would reconcile against existing state.
            return Mount(node);
        }

        /// <summary>
        /// Mounts a node tree into the instance hierarchy.
        /// </summary>
        private Instance Mount(Node node)
        {
            if (node == null) return null;

            Type type = node.Type;

            // Check if this is a composite component (inherits from Component<TProps>)
            if (IsComposite(type))
            {
                object component = Activator.CreateInstance(type);
                SetComponentProps(component, CastProps(type, GetNodeProps(node)));
                SetComponentChildren(component, node.Children);

                Instance instance = new Instance
                {
                    VNode = node,
                    ComponentType = type,
                    ComponentInstance = component,
                    Children = new List<Instance>()
                };

                // Hook state management
                ComponentContext.CurrentInstance = instance;
                ResetComponentHooks(component);
                Node renderedNode = RenderComponent(component);
                ComponentContext.CurrentInstance = null;
                Console.WriteLine($"[VDE] composite {type.Name} rendered={(renderedNode==null?"null":renderedNode.Type.Name)} children={(node.Children==null?0:node.Children.Length)}");

                bool isProvider = component is IContextProvider;
                if (isProvider) ((IContextProvider)component).PushContext();

                instance.Child = Mount(renderedNode);
                if (instance.Child != null && instance.Child.VNode != null)
                    instance.Child.VNode.Parent = node;

                if (isProvider) ((IContextProvider)component).PopContext();
                // Add rendered children
                if (instance.Child != null)
                    instance.Children.Add(instance.Child);

                return instance;
            }

            // Primitive or host component - just build the instance tree
            // Drawing is handled by the IVirtualDomRenderer implementor
            Instance host = new Instance
            {
                VNode = node,
                ComponentType = type,
                Children = new List<Instance>()
            };

            Node[] children = node.Children ?? Array.Empty<Node>();
            var resolvedChildren = new List<Node>();
            foreach (Node child in children)
            {
                var mounted = Mount(child);
                if (mounted == null) continue;
                host.Children.Add(mounted);
                // Splice composite components' rendered output into the host tree
                // so the renderer draws the resolved nodes instead of component nodes.
                var curInst = mounted;
                while (curInst.Child != null && curInst.Child.VNode != null) curInst = curInst.Child;
                Node drawChild = curInst.VNode;
                if (drawChild != null)
                {
                    drawChild.Parent = node;
                    resolvedChildren.Add(drawChild);
                }
            }
            node.Children = resolvedChildren.ToArray();

            return host;
        }

        /// <summary>
        /// Returns true if the component type is composite (inherits from Component<T>).
        /// </summary>
        private static bool IsComposite(Type type)
        {
            if (type == null) return false;
            Type current = type.BaseType;
            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(Component<>))
                    return true;
                current = current.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Casts props to the component's expected prop type.
        /// </summary>
        private static object CastProps(Type componentType, object props)
        {
            Type baseType = componentType.BaseType;
            while (baseType != null && !(baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Component<>)))
                baseType = baseType.BaseType;

            if (baseType == null) return props;

            Type propsType = baseType.GetGenericArguments()[0];
            if (props == null) return Activator.CreateInstance(propsType);
            if (propsType.IsInstanceOfType(props)) return props;
            return Activator.CreateInstance(propsType);
        }

        /// <summary>
        /// Sets component properties from a props object.
        /// </summary>
        private static void SetComponentProps(object component, object props)
        {
            if (component == null) return;
            PropertyInfo property = component.GetType().GetProperty("Props");
            if (property != null && property.CanWrite) property.SetValue(component, props);
        }

        /// <summary>
        /// Sets component children from a Node array.
        /// </summary>
        private static void SetComponentChildren(object component, Node[] children)
        {
            if (component == null) return;
            PropertyInfo property = component.GetType().GetProperty("Children");
            if (property != null && property.CanWrite) property.SetValue(component, children);
        }

        /// <summary>
        /// Resets component hooks for a new render cycle.
        /// </summary>
        private static void ResetComponentHooks(object component)
        {
            if (component == null) return;
            MethodInfo method = component.GetType().GetMethod("ResetHooks", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null) method.Invoke(component, null);
        }

        /// <summary>
        /// Invokes the Render method on a component instance.
        /// </summary>
        private static Node RenderComponent(object component)
        {
            if (component == null) return null;
            MethodInfo method = component.GetType().GetMethod("Render", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null) return null;
            return method.Invoke(component, null) as Node;
        }

        /// <summary>
        /// Gets props from a node, supporting both base Node and Node{TProps}.
        /// </summary>
        private static object GetNodeProps(Node node)
        {
            if (node == null) return null;
            var propsProperty = node.GetType().GetProperty("Props");
            return propsProperty?.GetValue(node);
        }

        /// <summary>
        /// Unmounts a component instance and its children.
        /// </summary>
        private static void Unmount(Instance instance)
        {
            if (instance == null) return;

            if (IsComposite(instance.ComponentType) && instance.ComponentInstance != null)
            {
                try
                {
                    PropertyInfo hooksProperty = instance.ComponentInstance.GetType().GetProperty("Hooks", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (hooksProperty != null)
                    {
                        System.Collections.IEnumerable hooks = hooksProperty.GetValue(instance.ComponentInstance) as System.Collections.IEnumerable;
                        if (hooks != null)
                        {
                            foreach (object hook in hooks)
                            {
                                if (hook is SideEffectState ses)
                                {
                                    try { ses.Cleanup?.Invoke(); } catch { }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            if (instance.Child != null) Unmount(instance.Child);
            foreach (Instance child in instance.Children) Unmount(child);
        }
    }
}