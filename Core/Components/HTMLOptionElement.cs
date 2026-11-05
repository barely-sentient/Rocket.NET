using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLOptionElement
    {}

    public class HTMLOptionElementProps : IntrinsicAttributes
    {
        public string Value { get; set; }
        public bool Selected { get; set; }
        public bool Disabled { get; set; }
        public string Label { get; set; }
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Option element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLOptionElementProps> Option(HTMLOptionElementProps props, params Node[] children)
        {
            return new Node<HTMLOptionElementProps>(typeof(HTMLOptionElement), props, children);
        }

    }
}
