using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLSummaryElement
    {}

    public class HTMLSummaryElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Summary element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLSummaryElementProps> Summary(HTMLSummaryElementProps props, params Node[] children)
        {
            return new Node<HTMLSummaryElementProps>(typeof(HTMLSummaryElement), props, children);
        }

    }
}
