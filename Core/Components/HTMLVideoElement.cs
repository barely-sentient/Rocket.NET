namespace Rocket.Core.Components
{
    public class HTMLVideoElement
    {}

    public class HTMLVideoElementProps : IntrinsicAttributes
    {
        public string Src { get; set; }
        public bool Controls { get; set; }
        public bool Autoplay { get; set; }
        public bool Loop { get; set; }
        public bool Muted { get; set; }
        public string Poster { get; set; }
        public string Preload { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string CrossOrigin { get; set; }
    }
}
