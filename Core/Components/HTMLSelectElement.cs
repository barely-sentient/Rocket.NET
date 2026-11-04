using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLSelectElement
    {}

    public class HTMLSelectElementProps : IntrinsicAttributes
    {
        public bool Disabled { get; set; }
        public bool Multiple { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public int Size { get; set; }
        public string Value { get; set; }
        public bool Autofocus { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Select element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLSelectElementProps> Select(HTMLSelectElementProps props, params Node[] children)
        {
            return (Node<HTMLSelectElementProps>) H("select", props, children);
        }

    }
}
