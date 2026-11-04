using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLStrongElement
    {}

    public class HTMLStrongElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Strong element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLStrongElementProps> Strong(HTMLStrongElementProps props, params Node[] children)
        {
            return (Node<HTMLStrongElementProps>) H("strong", props, children);
        }

    }
}
