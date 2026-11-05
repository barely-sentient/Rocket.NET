using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLNavElement
    {}

    public class HTMLNavElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Nav element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLNavElementProps> Nav(HTMLNavElementProps props, params Node[] children)
        {
            return new Node<HTMLNavElementProps>(typeof(HTMLNavElement), props, children);
        }

    }
}
