using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLHeaderElement
    {}

    public class HTMLHeaderElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Header element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHeaderElementProps> Header(HTMLHeaderElementProps props, params Node[] children)
        {
            return (Node<HTMLHeaderElementProps>) H("header", props, children);
        }

    }
}
