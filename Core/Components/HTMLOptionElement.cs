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
