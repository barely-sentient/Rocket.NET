using System;

namespace Rocket.Core
{
    public static partial class Hooks
    {
        /// <summary>
        /// Returns a stable unique id for the current hook position.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Id hook: generates a stable unique identifier for a component instance
        /// across renders.
        /// </para>
        /// <para>
        /// Mirrors React's <c>useId</c>. Useful for accessibility attributes that
        /// must stay consistent between server and client.
        /// </para>
        /// </remarks>
        public static string Id()
        {
            var instance = ComponentContext.CurrentInstance;
            if (instance == null)
            {
                return Guid.NewGuid().ToString();
            }

            if (instance.HookIndex >= instance.Hooks.Count)
                instance.Hooks.Add(null);

            int index = instance.HookIndex++;
            var hook = instance.Hooks[index] as IdHook;
            if (hook == null)
            {
                hook = new IdHook { Value = Guid.NewGuid().ToString() };
                instance.Hooks[index] = hook;
            }
            return hook.Value;
        }

    }

    /// <summary>
    /// Backing store for an <see cref="Id"/> hook.
    /// </summary>
    public class IdHook
    {
        /// <summary>The generated id.</summary>
        public string Value;
    }
}