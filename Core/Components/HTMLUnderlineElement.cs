using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLUnderlineElement
    {}

    public class HTMLUnderlineElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Underline element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLUnderlineElementProps> Underline(HTMLUnderlineElementProps props, params Node[] children)
        {
            return new Node<HTMLUnderlineElementProps>(typeof(HTMLUnderlineElement), props, children);
        }

    }
}
