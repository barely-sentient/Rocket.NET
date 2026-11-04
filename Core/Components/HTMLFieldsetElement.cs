namespace Rocket.Core.Components
{
    public class HTMLFieldsetElement
    {}

    public class HTMLFieldsetElementProps : IntrinsicAttributes
    {
        public bool Disabled { get; set; }
        public string Form { get; set; }
        public string Name { get; set; }
        public string TextContent { get; set; }
    }
}
