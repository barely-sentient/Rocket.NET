using System;
using System.Collections.Generic;
using Rocket.Core.DOM;

namespace Rocket.Core
{
    public static class Hooks
    {
        public static (T Value, Action<T> SetValue) UseState<T>(T initial) => State<T>(initial);

        public static (T Value, Action<T> SetValue) State<T>(T initial)
        {
            var instance = ComponentContext.CurrentInstance;
            if (instance == null)
            {
                // Outside of a render; return a standalone state cell.
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

        public static void UseEffect(Action effect, object[] dependencies = null) => SideEffect(effect, dependencies);

        public static void UseEffect(Func<Action> effect, object[] dependencies = null) => SideEffect(effect, dependencies);

        public static void SideEffect(Action effect, object[] dependencies = null)
        {
            SideEffect(() => { effect(); return null; }, dependencies);
        }

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

        private static bool DependenciesEqual(object[] a, object[] b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (!Equals(a[i], b[i])) return false;
            }
            return true;
        }

        public static Context<T> CreateContext<T>(T defaultValue)
        {
            return new Context<T> { DefaultValue = defaultValue, CurrentValue = defaultValue };
        }

        public static T UseContext<T>(Context<T> context)
        {
            if (context == null) return default;
            return ComponentContext.GetContext(context);
        }

        public static T Context<T>(Context<T> context)
        {
            return UseContext(context);
        }
    }

    public class StateHook<T>
    {
        public T Value;
        public Action<T> Setter;
        public Instance Instance;
    }

    public class Context<T>
    {
        public T DefaultValue;
        internal T CurrentValue;
    }
}