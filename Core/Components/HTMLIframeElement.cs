namespace Rocket.Core.Components
{
    public class HTMLIframeElement
    {}

    public class HTMLIframeElementProps : IntrinsicAttributes
    {
        public string Src { get; set; }
        public string SrcDoc { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Sandbox { get; set; }
        public string Allow { get; set; }
        public string Loading { get; set; }
        public string ReferrerPolicy { get; set; }
    }
}
