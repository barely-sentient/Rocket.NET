using System;
using System.Collections.Generic;
using Rocket.Core.DOM;

namespace Rocket.Core
{
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
        public int TabIndex;

        public void ResetHooks()
        {
            Hooks.Clear();
            HookIndex = 0;
        }
    }
}