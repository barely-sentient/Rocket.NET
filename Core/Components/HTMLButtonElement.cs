namespace Rocket.Core.Components
{
    public class HTMLButtonElement
    {}

    public class HTMLButtonElementProps : IntrinsicAttributes
    {
        public string Type { get; set; }
        public bool Disabled { get; set; }
        public string TextContent { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Form { get; set; }
        public string FormAction { get; set; }
        public string FormMethod { get; set; }
        public string FormTarget { get; set; }
        public string FormEnctype { get; set; }
        public bool Autofocus { get; set; }
    }
}
