using System;

namespace Rocket.Core
{
    public static partial class Hooks
    {
        /// <summary>
        /// Creates (or retrieves) a reducer-backed state cell.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Mirrors React's <c>useReducer</c>. Useful for state logic that involves
        /// multiple sub-values or complex transitions.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Keep the reducer pure and side-effect free. Dispatch actions
        /// describing <em>what happened</em>, not how to update.
        /// </para>
        /// </remarks>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TAction">The action type accepted by dispatch.</typeparam>
        /// <param name="reducer">Pure function computing the next state.</param>
        /// <param name="initial">The initial state.</param>
        public static (TState State, Action<TAction> Dispatch) Reducer<TState, TAction>(Func<TState, TAction, TState> reducer, TState initial)
        {
            var instance = ComponentContext.CurrentInstance;
            if (instance == null)
            {
                TState state = initial;
                Action<TAction> dispatchStandalone = action => { state = reducer(state, action); };
                return (state, dispatchStandalone);
            }

            if (instance.HookIndex >= instance.Hooks.Count)
                instance.Hooks.Add(null);

            int index = instance.HookIndex++;
            var hook = instance.Hooks[index] as ReducerHook<TState, TAction>;
            if (hook == null)
            {
                hook = new ReducerHook<TState, TAction> { State = initial, Reducer = reducer };
                instance.Hooks[index] = hook;
            }
            if (hook.Reducer == null) hook.Reducer = reducer;

            Action<TAction> dispatch = action =>
            {
                hook.State = hook.Reducer(hook.State, action);
                ComponentContext.RequestUpdate(instance);
            };

            return (hook.State, dispatch);
        }

        /// <summary>
        /// Creates a reducer-backed state cell whose initial state is computed
        /// from <paramref name="initialArg"/> via <paramref name="init"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Mirrors React's <c>useReducer</c>. Useful for state logic that involves
        /// multiple sub-values or complex transitions.
        /// </para>
        /// <para>
        /// <b>Tip:</b> Keep the reducer pure and side-effect free. Dispatch actions
        /// describing <em>what happened</em>, not how to update.
        /// </para>
        /// </remarks>
        public static (TState State, Action<TAction> Dispatch) Reducer<TState, TAction, TArg>(Func<TState, TAction, TState> reducer, TArg initialArg, Func<TArg, TState> init)
        {
            TState initial = init != null ? init(initialArg) : default;
            return Reducer(reducer, initial);
        }

    }

    /// <summary>
    /// Backing store for a <see cref="Reducer{TState,TAction}"/> hook.
    /// </summary>
    public class ReducerHook<TState, TAction>
    {
        /// <summary>The current state.</summary>
        public TState State;
        /// <summary>The reducer function.</summary>
        public Func<TState, TAction, TState> Reducer;
    }
}