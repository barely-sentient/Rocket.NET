using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLUListElement
    {}

    public class HTMLUListElementProps : IntrinsicAttributes
    {
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a UList element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLUListElementProps> UList(HTMLUListElementProps props, params Node[] children)
        {
            return (Node<HTMLUListElementProps>) H("ul", props, children);
        }

        /// <summary>
        /// Shorthand method to render a Ul element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLUListElementProps> Ul(HTMLUListElementProps props, params Node[] children)
        {
            return (Node<HTMLUListElementProps>) H("ul", props, children);
        }

    }
}
