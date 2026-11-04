namespace Rocket.Core.Components
{
    public class HTMLImageElement
    {}

    public class HTMLImageElementProps : IntrinsicAttributes
    {
        public string Src { get; set; }
        public string Alt { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Loading { get; set; }
        public string Decoding { get; set; }
        public string CrossOrigin { get; set; }
        public string ReferrerPolicy { get; set; }
        public string Sizes { get; set; }
        public string SrcSet { get; set; }
    }
}
