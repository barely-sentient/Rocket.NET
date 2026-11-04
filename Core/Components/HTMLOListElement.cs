using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLOListElement
    {}

    public class HTMLOListElementProps : IntrinsicAttributes
    {
        public bool Reversed { get; set; }
        public int Start { get; set; }
        public string Type { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Ol element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLOListElementProps> Ol(HTMLOListElementProps props, params Node[] children)
        {
            return (Node<HTMLOListElementProps>) H("ol", props, children);
        }

    }
}
