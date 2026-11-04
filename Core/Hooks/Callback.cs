using System;

namespace Rocket.Core
{
    public static partial class Hooks
    {
        /// <summary>Memoizes an <see cref="Action"/> delegate.</summary>
        public static Action Callback(Action callback, object[] dependencies = null) => Callback<Action>(callback, dependencies);
        /// <summary>Memoizes a <see cref="Func{TResult}"/> delegate.</summary>
        public static Func<TResult> Callback<TResult>(Func<TResult> callback, object[] dependencies = null) => Callback<Func<TResult>>(callback, dependencies);
        /// <summary>Memoizes an <see cref="Action{T}"/> delegate.</summary>
        public static Action<T> Callback<T>(Action<T> callback, object[] dependencies = null) => Callback<Action<T>>(callback, dependencies);
        /// <summary>Memoizes a <see cref="Func{T,TResult}"/> delegate.</summary>
        public static Func<T, TResult> Callback<T, TResult>(Func<T, TResult> callback, object[] dependencies = null) => Callback<Func<T, TResult>>(callback, dependencies);

        /// <summary>
        /// Memoizes a delegate of any type, returning a stable instance across
        /// renders when <paramref name="dependencies"/> are unchanged.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Callback hook: memoizes a delegate so its identity stays stable across
        /// renders unless its dependencies change.
        /// </para>
        /// <para>
        /// Mirrors React's <c>useCallback</c>. Useful for passing stable callbacks
        /// to child components to avoid unnecessary re-renders.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Prefer <c>Memo</c> for values and <c>Callback</c> for
        /// delegates. The returned delegate is the memoized instance, not the
        /// freshly passed one, when dependencies are unchanged.
        /// </para>
        /// </remarks>
        public static TDelegate Callback<TDelegate>(TDelegate callback, object[] dependencies = null) where TDelegate : Delegate
        {
            var instance = ComponentContext.CurrentInstance;
            if (instance == null)
            {
                return callback;
            }

            if (instance.HookIndex >= instance.Hooks.Count)
                instance.Hooks.Add(null);

            int index = instance.HookIndex++;
            var hook = instance.Hooks[index] as CallbackHook<TDelegate>;
            if (hook == null)
            {
                hook = new CallbackHook<TDelegate> { Value = callback, Dependencies = dependencies, HasValue = true };
                instance.Hooks[index] = hook;
                return hook.Value;
            }

            bool shouldUpdate = !hook.HasValue || !DependenciesEqual(hook.Dependencies, dependencies);
            if (shouldUpdate)
            {
                hook.Value = callback;
                hook.Dependencies = dependencies;
                hook.HasValue = true;
            }
            return hook.Value;
        }

    }

    /// <summary>
    /// Backing store for a <see cref="Callback{TDelegate}"/> hook.
    /// </summary>
    public class CallbackHook<TDelegate> where TDelegate : Delegate
    {
        /// <summary>The memoized delegate.</summary>
        public TDelegate Value;
        /// <summary>The dependency array used for invalidation.</summary>
        public object[] Dependencies;
        /// <summary>Whether a value has been stored yet.</summary>
        public bool HasValue;
    }
}