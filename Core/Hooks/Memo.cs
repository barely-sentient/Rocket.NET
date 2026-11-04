using System;

namespace Rocket.Core
{
    
    public static partial class Hooks
    {
        /// <summary>
        /// Returns the cached result of <paramref name="factory"/>, recomputing
        /// when <paramref name="dependencies"/> change.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Memoization hook: caches the result of a computation and recomputes it
        /// only when its dependencies change.
        /// </para>
        /// <para>
        /// Mirrors React's <c>useMemo</c>. Use it to avoid expensive
        /// recalculations between renders.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Do not rely on <c>Memo</c> for correctness (only
        /// performance); React may discard memoized values. Keep the factory
        /// side-effect free.
        /// </para>
        /// </remarks>
        public static T Memo<T>(Func<T> factory, object[] dependencies = null)
        {
            var instance = ComponentContext.CurrentInstance;
            if (instance == null)
            {
                return factory();
            }

            if (instance.HookIndex >= instance.Hooks.Count)
                instance.Hooks.Add(null);

            int index = instance.HookIndex++;
            var hook = instance.Hooks[index] as MemoHook<T>;
            if (hook == null)
            {
                hook = new MemoHook<T> { Dependencies = dependencies, HasValue = false };
                instance.Hooks[index] = hook;
            }

            bool shouldCompute = !hook.HasValue || !DependenciesEqual(hook.Dependencies, dependencies);
            if (shouldCompute)
            {
                try { hook.Value = factory(); } catch { hook.Value = default; }
                hook.Dependencies = dependencies;
                hook.HasValue = true;
            }
            return hook.Value;
        }

    }

    /// <summary>
    /// Backing store for a <see cref="Memo{T}"/> hook.
    /// </summary>
    public class MemoHook<T>
    {
        /// <summary>The cached value.</summary>
        public T Value;
        /// <summary>The dependency array used for invalidation.</summary>
        public object[] Dependencies;
        /// <summary>Whether a value has been computed yet.</summary>
        public bool HasValue;
    }
}