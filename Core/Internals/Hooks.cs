using System;
using System.Collections.Generic;

namespace Rocket.Core
{
    public static class ComponentContext
    {
        public static Instance CurrentInstance;

        public static Action<Instance> RequestUpdateHandler;

        private static readonly Stack<ContextFrame> _contextStack = new Stack<ContextFrame>();

        public static void RequestUpdate(Instance instance)
        {
            if (instance == null)
                return;

            instance.Dirty = true;
            RequestUpdateHandler?.Invoke(instance);
        }

        public static void PushContext(object context, object value)
        {
            _contextStack.Push(new ContextFrame { Context = context, Value = value });
        }

        public static void PopContext()
        {
            if (_contextStack.Count > 0) _contextStack.Pop();
        }

        public static T GetContext<T>(Context<T> context)
        {
            if (context == null) return default;
            foreach (var frame in _contextStack)
            {
                if (ReferenceEquals(frame.Context, context))
                {
                    return (T)frame.Value;
                }
            }
            return context.DefaultValue;
        }

        public class ContextFrame
        {
            public object Context;
            public object Value;
        }
    }

    public class ComponentHooks
    {
        private readonly List<object> _hooks = new List<object>();
        private int _hookIndex;

        public void Reset()
        {
            _hookIndex = 0;
            _hooks.Clear();
        }

        public int AddHook(object hook)
        {
            if (_hookIndex >= _hooks.Count)
            {
                _hooks.Add(hook);
            }

            return _hookIndex++;
        }

        public T GetHook<T>(int index) where T : class
        {
            if (index < 0 || index >= _hooks.Count) return null;
            return _hooks[index] as T;
        }

        public void SetHook(int index, object value)
        {
            if (index < 0 || index >= _hooks.Count) return;
            _hooks[index] = value;
        }

        public bool HasHook(int index) => index >= 0 && index < _hooks.Count;
    }
}