using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLLabelElement
    {}

    public class HTMLLabelElementProps : IntrinsicAttributes
    {
        public string For { get; set; }
        public string Form { get; set; }
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Label element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLLabelElementProps> Label(HTMLLabelElementProps props, params Node[] children)
        {
            return (Node<HTMLLabelElementProps>) H("label", props, children);
        }

    }
}
