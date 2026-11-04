using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLHrElement
    {}

    public class HTMLHrElementProps : IntrinsicAttributes
    {
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Hr element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHrElementProps> Hr(HTMLHrElementProps props, params Node[] children)
        {
            return (Node<HTMLHrElementProps>) H("hr", props, children);
        }

    }
}
