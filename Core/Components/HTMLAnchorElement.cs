namespace Rocket.Core.Components
{
    public class HTMLAnchorElement
    {}

    public class HTMLAnchorElementProps : IntrinsicAttributes
    {
        public string href { get; set; }
        public string Href { get => href; set => href = value; }
        public string Target { get; set; }
        public string Rel { get; set; }
        public string Download { get; set; }
        public string HrefLang { get; set; }
        public string Type { get; set; }
        public string Ping { get; set; }
        public string ReferrerPolicy { get; set; }
        public string TextContent { get; set; }
    }
}
