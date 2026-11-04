using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLMainElement
    {}

    public class HTMLMainElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Main element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLMainElementProps> Main(HTMLMainElementProps props, params Node[] children)
        {
            return (Node<HTMLMainElementProps>) H("main", props, children);
        }

    }
}
