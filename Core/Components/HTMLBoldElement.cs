using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLBoldElement
    {}

    public class HTMLBoldElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}
namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a bold element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLBoldElementProps> Bold(HTMLBoldElementProps props, params Node[] children)
        {
            return new Node<HTMLBoldElementProps>(typeof(HTMLBoldElement), props, children);
        }
    }
}