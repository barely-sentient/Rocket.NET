using System;

namespace Rocket.Core
{
    public static partial class Hooks
    {
        /// <summary>
        /// Runs <paramref name="effect"/> (which may return a cleanup action)
        /// according to the supplied <paramref name="dependencies"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Mirrors React's <c>useEffect</c>. The framework does not distinguish
        /// between layout and passive effects, so <c>LayoutEffect</c> and
        /// <c>InsertionEffect</c> are aliases.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Pass <c>null</c> dependencies to run once on mount; pass an
        /// array to re-run only when values change. Return a cleanup delegate to
        /// release resources before the next run or on unmount.
        /// </para>
        /// </remarks>
        public static void SideEffect(Action effect, object[] dependencies = null)
        {
            SideEffect(() => { effect(); return null; }, dependencies);
        }

        /// <summary>
        /// Runs <paramref name="effect"/> (which may return a cleanup action)
        /// according to the supplied <paramref name="dependencies"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Mirrors React's <c>useEffect</c>. The framework does not distinguish
        /// between layout and passive effects, so <c>LayoutEffect</c> and
        /// <c>InsertionEffect</c> are aliases.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Pass <c>null</c> dependencies to run once on mount; pass an
        /// array to re-run only when values change. Return a cleanup delegate to
        /// release resources before the next run or on unmount.
        /// </para>
        /// </remarks>
        public static void SideEffect(Func<Action> effect, object[] dependencies = null)
        {
            var instance = ComponentContext.CurrentInstance;
            if (instance == null) { effect(); return; }

            if (instance.HookIndex >= instance.Hooks.Count)
                instance.Hooks.Add(null);

            int index = instance.HookIndex++;
            var state = instance.Hooks[index] as SideEffectState;
            if (state == null)
            {
                state = new SideEffectState { Dependencies = dependencies, HasRun = false };
                instance.Hooks[index] = state;
            }

            bool shouldRun = !state.HasRun || !DependenciesEqual(state.Dependencies, dependencies);
            if (shouldRun)
            {
                try { state.Cleanup?.Invoke(); } catch { }
                state.Dependencies = dependencies;
                state.HasRun = true;
                try { state.Cleanup = effect(); } catch { state.Cleanup = null; }
            }
        }

        /// <summary>Short alias for <see cref="SideEffect(Action, object[])"/>.</summary>
        public static void Effect(Action effect, object[] dependencies = null) => SideEffect(effect, dependencies);
        /// <summary>Short alias for <see cref="SideEffect(Func{Action}, object[])"/>.</summary>
        public static void Effect(Func<Action> effect, object[] dependencies = null) => SideEffect(effect, dependencies);
        /// <summary>Alias for <see cref="SideEffect(Action, object[])"/> (no separate layout phase).</summary>
        public static void LayoutEffect(Action effect, object[] dependencies = null) => SideEffect(effect, dependencies);
        /// <summary>Alias for <see cref="SideEffect(Func{Action}, object[])"/> (no separate layout phase).</summary>
        public static void LayoutEffect(Func<Action> effect, object[] dependencies = null) => SideEffect(effect, dependencies);
        /// <summary>Alias for <see cref="SideEffect(Action, object[])"/> (no separate insertion phase).</summary>
        public static void InsertionEffect(Action effect, object[] dependencies = null) => SideEffect(effect, dependencies);
        /// <summary>Alias for <see cref="SideEffect(Func{Action}, object[])"/> (no separate insertion phase).</summary>
        public static void InsertionEffect(Func<Action> effect, object[] dependencies = null) => SideEffect(effect, dependencies);
    }
}