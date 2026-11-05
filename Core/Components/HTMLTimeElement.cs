using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLTimeElement
    {}

    public class HTMLTimeElementProps : IntrinsicAttributes
    {
        public string DateTime { get; set; }
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Time element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLTimeElementProps> Time(HTMLTimeElementProps props, params Node[] children)
        {
            return new Node<HTMLTimeElementProps>(typeof(HTMLTimeElement), props, children);
        }

    }
}
