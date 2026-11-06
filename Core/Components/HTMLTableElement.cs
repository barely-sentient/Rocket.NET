using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLTableElement
    {}

    public class HTMLTableElementProps : IntrinsicAttributes
    {
        public int Border { get; set; }
        public int CellPadding { get; set; }
        public int CellSpacing { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Table element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLTableElementProps> Table(HTMLTableElementProps props, params Node[] children)
        {
            return new Node<HTMLTableElementProps>(typeof(HTMLTableElement), props, children);
        }

    }
}
