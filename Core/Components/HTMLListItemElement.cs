using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLListItemElement
    {}

    public class HTMLListItemElementProps : IntrinsicAttributes
    {
        public int Value { get; set; }
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a ListItem element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLListItemElementProps> ListItem(HTMLListItemElementProps props, params Node[] children)
        {
            return (Node<HTMLListItemElementProps>) H("li", props, children);
        }

        /// <summary>
        /// Shorthand method to render a Li element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLListItemElementProps> Li(HTMLListItemElementProps props, params Node[] children)
        {
            return (Node<HTMLListItemElementProps>) H("li", props, children);
        }

    }
}
