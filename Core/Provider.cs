using System;
using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core
{
    public interface IContextProvider
    {
        void PushContext();
        void PopContext();
    }

    public class ProviderProps<T>
    {
        public Context<T> Context { get; set; }
        public T Value { get; set; }
        public Node[] Children { get; set; }
    }

    public class Provider<T> : Component<ProviderProps<T>>, IContextProvider
    {
        public void PushContext()
        {
            if (Props != null && Props.Context != null)
                ComponentContext.PushContext(Props.Context, Props.Value);
        }

        public void PopContext()
        {
            ComponentContext.PopContext();
        }

        public override Node Render()
        {
            if (Props == null || Props.Children == null || Props.Children.Length == 0)
                return null;

            if (Props.Children.Length == 1)
                return Props.Children[0];

            return new Node<object>(typeof(HTMLDivElement), new Components.HTMLDivElementProps(), Props.Children);
        }
    }
}