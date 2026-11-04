using System;

// Componentizr SideEffectState - tracks cleanup and dependencies for side effects
namespace Rocket.Core
{
    /// <summary>
    /// State tracking for a side effect (useSideEffect).
    /// </summary>
    /// <summary>
    /// Tracks cleanup and dependency state for a side effect.
    /// </summary>
    public class SideEffectState
    {
        /// <summary>
        /// cleanup action invoked when the effect is re-run or the component is unmounted.
        /// </summary>
        public Action Cleanup;

        /// <summary>
        /// Dependencies array used to determine if the effect should re-run.
        /// </summary>
        public object[] Dependencies;

        /// <summary>
        /// Whether the effect has already run.
        /// </summary>
        public bool HasRun;
    }
}