using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLSupElement
    {}

    public class HTMLSupElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Sup element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLSupElementProps> Sup(HTMLSupElementProps props, params Node[] children)
        {
            return new Node<HTMLSupElementProps>(typeof(HTMLSupElement), props, children);
        }

    }
}
