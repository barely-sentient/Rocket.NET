using Rocket.Core.DOM;

namespace Rocket.Core
{
    /// <summary>
    /// Base class for composite components in the virtual DOM.
    /// </summary>
    /// <typeparam name="TProps">The component's properties type.</typeparam>
    public abstract class Component<TProps> where TProps : new()
    {
        public abstract Node Render();
        
        public TProps Props { get; set; } = new TProps();

        /// <summary>
        /// Resets hook state for the current render cycle.
        /// Must be called at the start of each Render() invocation.
        /// </summary>
        public void ResetHooks()
        {
            if (AssociatedInstance != null)
            {
                AssociatedInstance.ResetHooks();
            }
        }

        /// <summary>
        /// Gets the instance this component is associated with.
        /// </summary>
        internal Instance AssociatedInstance { get; set; }
    }
}