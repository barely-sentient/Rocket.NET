using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLParagraphElement
    {}

    public class HTMLParagraphElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Paragraph element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLParagraphElementProps> Paragraph(HTMLParagraphElementProps props, params Node[] children)
        {
            return new Node<HTMLParagraphElementProps>(typeof(HTMLParagraphElement), props, children);
        }

    }
}
