using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLAsideElement
    {}

    public class HTMLAsideElementProps : IntrinsicAttributes
    {
    }
}
namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render an aside node
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLAsideElementProps> Aside(HTMLAsideElementProps props, params Node[] children)
        {
            return new Node<HTMLAsideElementProps>(typeof(HTMLAsideElement), props, children);
        }
    }
}
