using System;

namespace Rocket.Core
{
    public static partial class Hooks
    {
        /// <summary>
        /// Creates (or retrieves) a state cell holding <paramref name="initial"/>
        /// on the first render, and returns its current value plus a setter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Mirrors React's <c>useState</c>. Call <see cref="State{T}"/> at the
        /// top level of a component's <c>Render</c> method, in the same order on
        /// every render.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Always call hooks unconditionally and in the same order;
        /// conditional hook calls break the internal hook index tracking.
        /// </para>
        /// <example>
        /// <code>
        /// var (count, setCount) = State(0);
        /// // later: setCount(count + 1);
        /// </code>
        /// </example>
        /// </remarks>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="initial">The initial value used on first render.</param>
        /// <returns>
        /// A tuple of the current value and an <see cref="Action{T}"/> setter
        /// that updates the value and requests a re-render.
        /// </returns>
        public static (T Value, Action<T> SetValue) State<T>(T initial)
        {
            var instance = ComponentContext.CurrentInstance;
            if (instance == null)
            {
                var standalone = new StateHook<T> { Value = initial };
                Action<T> setter = v => standalone.Value = v;
                standalone.Setter = setter;
                return (standalone.Value, setter);
            }

            if (instance.HookIndex >= instance.Hooks.Count)
                instance.Hooks.Add(null);

            int index = instance.HookIndex++;
            var hook = instance.Hooks[index] as StateHook<T>;
            if (hook == null)
            {
                hook = new StateHook<T> { Value = initial, Instance = instance };
                instance.Hooks[index] = hook;
                hook.Setter = newValue =>
                {
                    hook.Value = newValue;
                    ComponentContext.RequestUpdate(instance);
                };
            }
            return (hook.Value, hook.Setter);
        }
    }

    /// <summary>
    /// Backing store for a <see cref="State{T}"/> hook.
    /// </summary>
    public class StateHook<T>
    {
        /// <summary>The current state value.</summary>
        public T Value;
        /// <summary>The setter that updates <see cref="Value"/> and requests a re-render.</summary>
        public Action<T> Setter;
        /// <summary>The owning component instance.</summary>
        public Instance Instance;
    }
}