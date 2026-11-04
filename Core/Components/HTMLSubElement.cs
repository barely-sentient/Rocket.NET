using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLSubElement
    {}

    public class HTMLSubElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Sub element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLSubElementProps> Sub(HTMLSubElementProps props, params Node[] children)
        {
            return (Node<HTMLSubElementProps>) H("sub", props, children);
        }

    }
}
