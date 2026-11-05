using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLTableHeaderCellElement
    {}

    public class HTMLTableHeaderCellElementProps : IntrinsicAttributes
    {
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string Headers { get; set; }
        public string Scope { get; set; }
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Th element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLTableHeaderCellElementProps> Th(HTMLTableHeaderCellElementProps props, params Node[] children)
        {
            return new Node<HTMLTableHeaderCellElementProps>(typeof(HTMLTableHeaderCellElement), props, children);
        }

    }
}
