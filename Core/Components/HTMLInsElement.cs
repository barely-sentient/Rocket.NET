using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLInsElement
    {}

    public class HTMLInsElementProps : IntrinsicAttributes
    {
        public string Cite { get; set; }
        public string DateTime { get; set; }
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Ins element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLInsElementProps> Ins(HTMLInsElementProps props, params Node[] children)
        {
            return new Node<HTMLInsElementProps>(typeof(HTMLInsElement), props, children);
        }

    }
}
