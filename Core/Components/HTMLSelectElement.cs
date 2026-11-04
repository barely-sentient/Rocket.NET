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
