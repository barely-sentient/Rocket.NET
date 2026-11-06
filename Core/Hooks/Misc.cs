using System;

namespace Rocket.Core
{
    public static partial class Hooks
    {
        /// <summary>
        /// Returns <paramref name="value"/> unchanged (no concurrent rendering
        /// is performed by this framework).
        /// </summary>
        /// <remarks>Miscellaneous hooks: deferred values, debug values, transitions and external-store subscriptions.</remarks>
        public static T DeferredValue<T>(T value) => value;

        /// <summary>No-op hook for attaching a debug label to a value.</summary>
        /// <remarks>Miscellaneous hooks: deferred values, debug values, transitions and external-store subscriptions.</remarks>
        public static void DebugValue<T>(T value) { }

        /// <summary>
        /// Returns a non-pending state and a function that runs a transition
        /// action synchronously.
        /// </summary>
        /// <remarks>Miscellaneous hooks: deferred values, debug values, transitions and external-store subscriptions.</remarks>
        public static (bool IsPending, Action<Action> StartTransition) Transition()
        {
            return (false, _transitionStarter);
        }

        private static readonly Action<Action> _transitionStarter = action => { try { action(); } catch { } };

        /// <summary>
        /// Reads a snapshot from an external store. Subscription handling is
        /// left to the caller in this minimal implementation.
        /// </summary>
        /// <remarks>Miscellaneous hooks: deferred values, debug values, transitions and external-store subscriptions.</remarks>
        public static T SyncExternalStore<T>(Func<Action, Action> subscribe, Func<T> getSnapshot, Func<T> getServerSnapshot = null)
        {
            if (getSnapshot == null) return default;
            return getSnapshot();
        }

    }
}