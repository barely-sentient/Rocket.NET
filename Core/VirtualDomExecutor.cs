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
        
        public VirtualDomExecutor(IComponentRenderer renderer)
        {
            _renderer = renderer;    
        }
        
        /// <summary>
        /// Renders the root node tree. If a root instance already exists,
        /// the tree is reconciled against the previous state.
        /// </summary>
        /// <param name="rootNode">The root node of the virtual DOM tree to render.</param>
        public void Render<TProps>(Node<TProps> rootNode) where TProps : new()
        {
            if (rootNode == null) return;

            // Perform lifecycle: mount or update existing instance
            var instance = MountOrUpdate(rootNode);

            _renderer.Draw((Node<TProps>) (instance.Child != null ? instance.Child.VNode : instance.VNode));
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

                instance.Child = Mount(renderedNode);
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
            foreach (Node child in children)
                host.Children.Add(Mount(child));

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