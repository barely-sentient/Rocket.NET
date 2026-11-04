using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLLegendElement
    {}

    public class HTMLLegendElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Legend element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLLegendElementProps> Legend(HTMLLegendElementProps props, params Node[] children)
        {
            return (Node<HTMLLegendElementProps>) H("legend", props, children);
        }

    }
}
