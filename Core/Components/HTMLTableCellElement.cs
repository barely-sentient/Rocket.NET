namespace Rocket.Core.Components
{
    public class HTMLTableCellElement
    {}

    public class HTMLTableCellElementProps : IntrinsicAttributes
    {
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string Headers { get; set; }
        public string Scope { get; set; }
        public string TextContent { get; set; }
    }
}
