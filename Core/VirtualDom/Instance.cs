using System;
using System.Collections.Generic;
using Rocket.Core.DOM;

namespace Rocket.Core
{
    /// <summary>
    /// Runtime instance of a component or host node in the virtual DOM tree.
    /// </summary>
    public class Instance
    {
        public Node VNode;
        public Type ComponentType;
        public object ComponentInstance;
        public Instance Child;
        public List<Instance> Children = new List<Instance>();
        public List<object> Hooks = new List<object>();
        public int HookIndex;
        public Instance Parent;
        public bool Dirty;

        public void ResetHooks()
        {
            // Preserve stored hook state across re-renders; only reset the cursor.
            HookIndex = 0;
        }

        public void ClearHooks()
        {
            Hooks.Clear();
            HookIndex = 0;
        }
    }
}