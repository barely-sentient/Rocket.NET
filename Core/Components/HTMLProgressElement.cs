namespace Rocket.Core.Components
{
    public class HTMLProgressElement
    {}

    public class HTMLProgressElementProps : IntrinsicAttributes
    {
        public double Value { get; set; }
        public double Max { get; set; }
        public string TextContent { get; set; }
    }
}
