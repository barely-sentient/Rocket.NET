using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLDetailsElement
    {}

    public class HTMLDetailsElementProps : IntrinsicAttributes
    {
        public bool Open { get; set; }
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Details element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLDetailsElementProps> Details(HTMLDetailsElementProps props, params Node[] children)
        {
            return new Node<HTMLDetailsElementProps>(typeof(HTMLDetailsElement), props, children);
        }

    }
}
