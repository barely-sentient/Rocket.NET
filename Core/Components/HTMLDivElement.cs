using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLDivElement
    {}

    public class HTMLDivElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Div element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLDivElementProps> Div(HTMLDivElementProps props, params Node[] children)
        {
            return (Node<HTMLDivElementProps>) H("div", props, children);
        }

    }
}
