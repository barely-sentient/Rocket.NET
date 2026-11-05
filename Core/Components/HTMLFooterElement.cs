using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLFooterElement
    {}

    public class HTMLFooterElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Footer element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLFooterElementProps> Footer(HTMLFooterElementProps props, params Node[] children)
        {
            return new Node<HTMLFooterElementProps>(typeof(HTMLFooterElement), props, children);
        }

    }
}
