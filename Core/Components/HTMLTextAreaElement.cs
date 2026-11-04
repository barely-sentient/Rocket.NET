namespace Rocket.Core.Components
{
    public class HTMLTextAreaElement
    {}

    public class HTMLTextAreaElementProps : IntrinsicAttributes
    {
        public string Placeholder { get; set; }
        public string Value { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
        public bool Disabled { get; set; }
        public bool Required { get; set; }
        public bool ReadOnly { get; set; }
        public string Name { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public string Wrap { get; set; }
        public bool Autofocus { get; set; }
        public string TextContent { get; set; }
    }
}
