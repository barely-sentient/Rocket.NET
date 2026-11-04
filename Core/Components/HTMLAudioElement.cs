namespace Rocket.Core.Components
{
    public class HTMLAudioElement
    {}

    public class HTMLAudioElementProps : IntrinsicAttributes
    {
        public string Src { get; set; }
        public bool Controls { get; set; }
        public bool Autoplay { get; set; }
        public bool Loop { get; set; }
        public bool Muted { get; set; }
        public string Preload { get; set; }
        public string CrossOrigin { get; set; }
    }
}
