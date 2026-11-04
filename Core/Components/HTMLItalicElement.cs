using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLItalicElement
    {}

    public class HTMLItalicElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Italic element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLItalicElementProps> Italic(HTMLItalicElementProps props, params Node[] children)
        {
            return (Node<HTMLItalicElementProps>) H("i", props, children);
        }

    }
}
