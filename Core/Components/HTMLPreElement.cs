using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLPreElement
    {}

    public class HTMLPreElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Pre element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLPreElementProps> Pre(HTMLPreElementProps props, params Node[] children)
        {
            return (Node<HTMLPreElementProps>) H("pre", props, children);
        }

    }
}
