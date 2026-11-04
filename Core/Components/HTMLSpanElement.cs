using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLSpanElement
    {}

    public class HTMLSpanElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Span element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLSpanElementProps> Span(HTMLSpanElementProps props, params Node[] children)
        {
            return (Node<HTMLSpanElementProps>) H("span", props, children);
        }

    }
}
