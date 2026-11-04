namespace Rocket.Core.Components
{
    public class HTMLTableHeaderCellElement
    {}

    public class HTMLTableHeaderCellElementProps : IntrinsicAttributes
    {
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string Headers { get; set; }
        public string Scope { get; set; }
        public string TextContent { get; set; }
    }
}
