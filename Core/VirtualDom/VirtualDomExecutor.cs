using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Rocket.Core.DOM;
using VantaCSS;

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
        private Instance _rootInstance;

        private sealed class ComponentMeta
        {
            public bool IsComposite;
            public Type PropsType;
            public PropertyInfo PropsProperty;
            public PropertyInfo ChildrenProperty;
            public MethodInfo RenderMethod;
            public MethodInfo ResetHooksMethod;
        }

        private static readonly ConcurrentDictionary<Type, ComponentMeta> _metaCache =
            new ConcurrentDictionary<Type, ComponentMeta>();

        private static readonly ConcurrentDictionary<Type, Func<object>> _factoryCache =
            new ConcurrentDictionary<Type, Func<object>>();

        private static Func<object> GetFactory(Type type)
        {
            if (type == null) return null;
            return _factoryCache.GetOrAdd(type, t =>
            {
                var ctor = t.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                    return (Func<object>)(() => Activator.CreateInstance(t));
                var newExpr = Expression.New(ctor);
                var conv = Expression.Convert(newExpr, typeof(object));
                return Expression.Lambda<Func<object>>(conv).Compile();
            });
        }

        public List<VantaCSS.Stylesheet> Stylesheets { get; } = new List<Stylesheet>();
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
        public VantaCSS.VantaCssEngine CssEngine { get; set; } = new VantaCSS.VantaCssEngine();

        public VirtualDomExecutor(IComponentRenderer renderer)
        {
            _renderer = renderer;
            CssEngine.Stylesheets = Stylesheets;
        }

        public VirtualDomExecutor(IComponentRenderer renderer, VantaCSS.VantaCssEngine cssEngine)
        {
            _renderer = renderer;
            CssEngine = cssEngine ?? new VantaCSS.VantaCssEngine();
            CssEngine.Stylesheets = Stylesheets;
        }
        
        /// <summary>
        /// Renders the root node tree. If a root instance already exists,
        /// the tree is reconciled against the previous state.
        /// </summary>
        /// <param name="rootNode">The root node of the virtual DOM tree to render.</param>
        public void Render(Node rootNode)
        {
            Render(rootNode, 0, 0, VantaCSS.Orientation.Portrait);
        }

        public void Render(Node rootNode, int viewportWidth, int viewportHeight, VantaCSS.Orientation orientation)
        {
            if (rootNode == null) return;

            var instance = MountOrUpdate(rootNode);
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

        public void Render<TProps>(Node<TProps> rootNode, int viewportWidth, int viewportHeight, VantaCSS.Orientation orientation) where TProps : new()
        {
            Render((Node)rootNode, viewportWidth, viewportHeight, orientation);
        }

        public void Render(Node rootNode, VantaCSS.Viewport viewport)
        {
            if (viewport == null) Render(rootNode);
            else Render(rootNode, viewport.Width, viewport.Height, viewport.Orientation);
        }

        public void Render<TProps>(Node<TProps> rootNode, VantaCSS.Viewport viewport) where TProps : new()
        {
            Render((Node)rootNode, viewport);
        }

        /// <summary>
        /// Mounts or updates the instance tree based on the node.
        /// Reuses the previous instance tree when component types match,
        /// preserving hook state across re-renders.
        /// </summary>
        private Instance MountOrUpdate(Node node)
        {
            if (node == null) return null;

            if (_rootInstance == null || !Compatible(_rootInstance, node))
            {
                if (_rootInstance != null) Unmount(_rootInstance);
                _rootInstance = Mount(node);
                return _rootInstance;
            }

            UpdateInstance(_rootInstance, node);
            return _rootInstance;
        }

        private static bool Compatible(Instance instance, Node node)
        {
            if (instance == null || node == null) return false;
            return instance.ComponentType == node.Type;
        }

        /// <summary>
        /// Mounts a node tree into the instance hierarchy.
        /// </summary>
        private Instance Mount(Node node)
        {
            if (node == null) return null;

            Type type = node.Type;

            // Check if this is a composite component (inherits from Component<TProps>)
            var meta = GetMeta(type);
            if (meta.IsComposite)
            {
                object component = GetFactory(type)();
                SetComponentProps(component, meta, CastProps(meta, GetNodeProps(node)));
                SetComponentChildren(component, meta, node.Children);

                Instance instance = new Instance
                {
                    VNode = node,
                    ComponentType = type,
                    ComponentInstance = component,
                    Children = new List<Instance>()
                };

                // Hook state management
                ComponentContext.CurrentInstance = instance;
                instance.ResetHooks();
                Node renderedNode = meta.RenderMethod != null ? meta.RenderMethod.Invoke(component, null) as Node : RenderComponent(component);
                ComponentContext.CurrentInstance = null;
                // (debug logging removed for performance)

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

        private void UpdateInstance(Instance instance, Node node)
        {
            if (instance == null || node == null) return;

            instance.VNode = node;
            Type type = node.Type;
            var meta = GetMeta(type);

            if (meta.IsComposite)
            {
                object component = instance.ComponentInstance;
                SetComponentProps(component, meta, CastProps(meta, GetNodeProps(node)));
                SetComponentChildren(component, meta, node.Children);

                ComponentContext.CurrentInstance = instance;
                instance.ResetHooks();
                Node renderedNode = meta.RenderMethod != null
                    ? meta.RenderMethod.Invoke(component, null) as Node
                    : RenderComponent(component);
                ComponentContext.CurrentInstance = null;

                bool isProvider = component is IContextProvider;
                if (isProvider) ((IContextProvider)component).PushContext();

                if (instance.Child == null || !Compatible(instance.Child, renderedNode))
                {
                    if (instance.Child != null) Unmount(instance.Child);
                    instance.Child = Mount(renderedNode);
                }
                else
                {
                    UpdateInstance(instance.Child, renderedNode);
                }

                if (instance.Child != null && instance.Child.VNode != null)
                    instance.Child.VNode.Parent = node;

                if (isProvider) ((IContextProvider)component).PopContext();

                instance.Children.Clear();
                if (instance.Child != null) instance.Children.Add(instance.Child);
            }
            else
            {
                instance.ComponentType = type;

                Node[] newChildren = node.Children ?? Array.Empty<Node>();
                var newInstances = new List<Instance>();
                var resolved = new List<Node>();
                int oldCount = instance.Children.Count;

                for (int i = 0; i < newChildren.Length; i++)
                {
                    var childNode = newChildren[i];
                    Instance childInst = (i < oldCount) ? instance.Children[i] : null;

                    if (childInst != null && Compatible(childInst, childNode))
                    {
                        UpdateInstance(childInst, childNode);
                    }
                    else
                    {
                        if (childInst != null) Unmount(childInst);
                        childInst = Mount(childNode);
                    }

                    var curInst = childInst;
                    while (curInst != null && curInst.Child != null && curInst.Child.VNode != null)
                        curInst = curInst.Child;
                    Node drawChild = curInst != null ? curInst.VNode : null;
                    if (drawChild != null)
                    {
                        drawChild.Parent = node;
                        resolved.Add(drawChild);
                    }
                    newInstances.Add(childInst);
                }

                for (int i = newChildren.Length; i < oldCount; i++)
                {
                    Unmount(instance.Children[i]);
                }

                instance.Children = newInstances;
                node.Children = resolved.ToArray();
            }
        }

        private static ComponentMeta GetMeta(Type type)
        {
            if (type == null) return new ComponentMeta();
            return _metaCache.GetOrAdd(type, t =>
            {
                var meta = new ComponentMeta();
                Type current = t.BaseType;
                while (current != null)
                {
                    if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(Component<>))
                    {
                        meta.IsComposite = true;
                        meta.PropsType = current.GetGenericArguments()[0];
                        break;
                    }
                    current = current.BaseType;
                }
                if (meta.IsComposite)
                {
                    meta.PropsProperty = t.GetProperty("Props");
                    meta.ChildrenProperty = t.GetProperty("Children");
                    meta.RenderMethod = t.GetMethod("Render", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    meta.ResetHooksMethod = t.GetMethod("ResetHooks", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }
                return meta;
            });
        }

        /// <summary>
        /// Returns true if the component type is composite (inherits from Component<T>).
        /// </summary>
        private static bool IsComposite(Type type)
        {
            return GetMeta(type).IsComposite;
        }

        /// <summary>
        /// Casts props to the component's expected prop type.
        /// </summary>
        private static object CastProps(Type componentType, object props)
        {
            return CastProps(GetMeta(componentType), props);
        }

        private static object CastProps(ComponentMeta meta, object props)
        {
            Type propsType = meta.PropsType;
            if (propsType == null) return props;
            if (props == null) return GetFactory(propsType)();
            if (propsType.IsInstanceOfType(props)) return props;
            return GetFactory(propsType)();
        }

        /// <summary>
        /// Sets component properties from a props object.
        /// </summary>
        private static void SetComponentProps(object component, ComponentMeta meta, object props)
        {
            if (component == null) return;
            PropertyInfo property = meta.PropsProperty ?? component.GetType().GetProperty("Props");
            if (property != null && property.CanWrite) property.SetValue(component, props);
        }

        private static void SetComponentProps(object component, object props)
        {
            SetComponentProps(component, GetMeta(component.GetType()), props);
        }

        /// <summary>
        /// Sets component children from a Node array.
        /// </summary>
        private static void SetComponentChildren(object component, ComponentMeta meta, Node[] children)
        {
            if (component == null) return;
            PropertyInfo property = meta.ChildrenProperty ?? component.GetType().GetProperty("Children");
            if (property != null && property.CanWrite) property.SetValue(component, children);
        }

        private static void SetComponentChildren(object component, Node[] children)
        {
            SetComponentChildren(component, GetMeta(component.GetType()), children);
        }

        /// <summary>
        /// Resets component hooks for a new render cycle.
        /// </summary>
        private static void ResetComponentHooks(object component)
        {
            if (component == null) return;
            var meta = GetMeta(component.GetType());
            MethodInfo method = meta.ResetHooksMethod ?? component.GetType().GetMethod("ResetHooks", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null) method.Invoke(component, null);
        }

        /// <summary>
        /// Invokes the Render method on a component instance.
        /// </summary>
        private static Node RenderComponent(object component)
        {
            if (component == null) return null;
            var meta = GetMeta(component.GetType());
            MethodInfo method = meta.RenderMethod ?? component.GetType().GetMethod("Render", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null) return null;
            return method.Invoke(component, null) as Node;
        }

        /// <summary>
        /// Gets props from a node, supporting both base Node and Node{TProps}.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo> _nodePropsCache = new ConcurrentDictionary<Type, PropertyInfo>();

        private static object GetNodeProps(Node node)
        {
            if (node == null) return null;
            var type = node.GetType();
            var propsProperty = _nodePropsCache.GetOrAdd(type, t => t.GetProperty("Props"));
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