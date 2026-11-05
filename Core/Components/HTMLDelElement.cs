using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLDelElement
    {}

    public class HTMLDelElementProps : IntrinsicAttributes
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
        /// Shorthand method to render a Del element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLDelElementProps> Del(HTMLDelElementProps props, params Node[] children)
        {
            return new Node<HTMLDelElementProps>(typeof(HTMLDelElement), props, children);
        }

    }
}
