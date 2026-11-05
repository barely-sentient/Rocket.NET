using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLEmElement
    {}

    public class HTMLEmElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Em element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLEmElementProps> Em(HTMLEmElementProps props, params Node[] children)
        {
            return new Node<HTMLEmElementProps>(typeof(HTMLEmElement), props, children);
        }

    }
}
