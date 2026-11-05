using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLInputElement
    {}

    public class HTMLInputElementProps : IntrinsicAttributes
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }
        public bool Disabled { get; set; }
        public bool Checked { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool ReadOnly { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Step { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public string Pattern { get; set; }
        public int Size { get; set; }
        public bool Autofocus { get; set; }
        public string AutoComplete { get; set; }
        public bool Multiple { get; set; }
        public string Accept { get; set; }
        public string Src { get; set; }
        public string Alt { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Input element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLInputElementProps> Input(HTMLInputElementProps props, params Node[] children)
        {
            return new Node<HTMLInputElementProps>(typeof(HTMLInputElement), props, children);
        }

    }
}
