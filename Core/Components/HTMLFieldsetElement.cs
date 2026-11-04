using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLFieldsetElement
    {}

    public class HTMLFieldsetElementProps : IntrinsicAttributes
    {
        public bool Disabled { get; set; }
        public string Form { get; set; }
        public string Name { get; set; }
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Fieldset element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLFieldsetElementProps> Fieldset(HTMLFieldsetElementProps props, params Node[] children)
        {
            return (Node<HTMLFieldsetElementProps>) H("fieldset", props, children);
        }

    }
}
