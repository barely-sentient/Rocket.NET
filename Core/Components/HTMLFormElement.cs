namespace Rocket.Core.Components
{
    public class HTMLFormElement
    {}

    public class HTMLFormElementProps : IntrinsicAttributes
    {
        public string Action { get; set; }
        public string Method { get; set; }
        public string Enctype { get; set; }
        public string Target { get; set; }
        public bool NoValidate { get; set; }
        public string AutoComplete { get; set; }
        public string Name { get; set; }
    }
}
