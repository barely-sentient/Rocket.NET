using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLSectionElement
    {}

    public class HTMLSectionElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Section element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLSectionElementProps> Section(HTMLSectionElementProps props, params Node[] children)
        {
            return (Node<HTMLSectionElementProps>) H("section", props, children);
        }

    }
}
