using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLSmallElement
    {}

    public class HTMLSmallElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Small element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLSmallElementProps> Small(HTMLSmallElementProps props, params Node[] children)
        {
            return (Node<HTMLSmallElementProps>) H("small", props, children);
        }

    }
}
