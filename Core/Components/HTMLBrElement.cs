using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLBrElement
    {}

    public class HTMLBrElementProps : IntrinsicAttributes
    {
    }
}
namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a br tag
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLBrElementProps> Br(HTMLBrElementProps props, params Node[] children)
        {
            return (Node<HTMLBrElementProps>) H("br", props, children);
        }
    }
}
