using System;

namespace Rocket.Core
{
    public static partial class Hooks
    {
        /// <summary>Creates a ref initialized to the default value of <typeparamref name="T"/>.</summary>
        public static Ref<T> Ref<T>() => Ref(default(T));

        /// <summary>
        /// Creates (or retrieves) a ref initialized to <paramref name="initial"/>
        /// on first render.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ref hook: returns a mutable container whose identity is stable across
        /// renders, allowing imperative access to a value.
        /// </para>
        /// <para>
        /// Mirrors React's <c>useRef</c>. Unlike <see cref="State{T}"/>, mutating
        /// <see cref="Ref{T}.Current"/> does not trigger a re-render.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Use refs for values that do not affect rendering (timers,
        /// previous values, DOM handles). Avoid reading/writing during render in
        /// ways that affect output.
        /// </para>
        /// </remarks>
        public static Ref<T> Ref<T>(T initial)
        {
            var instance = ComponentContext.CurrentInstance;
            if (instance == null)
            {
                return new Ref<T>(initial);
            }

            if (instance.HookIndex >= instance.Hooks.Count)
                instance.Hooks.Add(null);

            int index = instance.HookIndex++;
            var hook = instance.Hooks[index] as RefHook<T>;
            if (hook == null)
            {
                hook = new RefHook<T> { Value = new Ref<T>(initial) };
                instance.Hooks[index] = hook;
            }
            return hook.Value;
        }

    }

    /// <summary>
    /// Backing store for a <see cref="Ref{T}"/> hook.
    /// </summary>
    public class RefHook<T>
    {
        /// <summary>The mutable ref instance.</summary>
        public Ref<T> Value;
    }

    /// <summary>
    /// A mutable container with a <see cref="Current"/> value, mirroring
    /// React's ref object.
    /// </summary>
    public class Ref<T>
    {
        private T _current;
        /// <summary>Creates an empty ref.</summary>
        public Ref() { }
        /// <summary>Creates a ref initialized to <paramref name="initial"/>.</summary>
        public Ref(T initial) { _current = initial; }
        /// <summary>The current mutable value.</summary>
        public T Current { get { return _current; } set { _current = value; } }
        /// <summary>Alias for <see cref="Current"/>.</summary>
        public T Value { get { return _current; } set { _current = value; } }

        /// <summary>
        /// Deconstructs the ref into its current value and a setter, enabling
        /// tuple-style usage.
        /// </summary>
        public void Deconstruct(out T current, out Action<T> setValue)
        {
            current = _current;
            setValue = v => { _current = v; };
        }
    }
}