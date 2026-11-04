using System;

namespace Rocket.Core
{
    public static partial class Hooks
    {
        /// <summary>
        /// Creates a new context with the supplied default value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Context hooks: create and consume a typed context value provided by an
        /// ancestor <see cref="Provider{T}"/>.
        /// </para>
        /// <para>
        /// Mirrors React's <c>createContext</c> / <c>useContext</c>.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Wrap a subtree in a <see cref="Provider{T}"/> to override
        /// the default value for all descendants.
        /// </para>
        /// </remarks>
        public static Context<T> CreateContext<T>(T defaultValue)
        {
            return new Context<T> { DefaultValue = defaultValue, CurrentValue = defaultValue };
        }

        /// <summary>
        /// Reads the current value of <paramref name="context"/> from the
        /// nearest provider, or its default when none exists.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Context hooks: create and consume a typed context value provided by an
        /// ancestor <see cref="Provider{T}"/>.
        /// </para>
        /// <para>
        /// Mirrors React's <c>createContext</c> / <c>useContext</c>.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Wrap a subtree in a <see cref="Provider{T}"/> to override
        /// the default value for all descendants.
        /// </para>
        /// </remarks>
        public static T Context<T>(Context<T> context)
        {
            if (context == null) return default;
            return ComponentContext.GetContext(context);
        }
    }

    /// <summary>
    /// A typed context object carrying a default value.
    /// </summary>
    public class Context<T>
    {
        /// <summary>The default value used when no provider is present.</summary>
        public T DefaultValue;
        /// <summary>Internal current value.</summary>
        internal T CurrentValue;
    }
}