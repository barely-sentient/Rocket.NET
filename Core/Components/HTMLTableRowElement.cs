using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLTableRowElement
    {}

    public class HTMLTableRowElementProps : IntrinsicAttributes
    {
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Tr element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLTableRowElementProps> Tr(HTMLTableRowElementProps props, params Node[] children)
        {
            return (Node<HTMLTableRowElementProps>) H("tr", props, children);
        }

    }
}
