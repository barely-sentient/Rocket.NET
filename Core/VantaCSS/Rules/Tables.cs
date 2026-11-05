namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the table layout algorithm.
        /// </summary>
        public TableLayout TableLayout = TableLayout.Auto;

        /// <summary>
        /// Gets or sets whether table borders collapse.
        /// </summary>
        public BorderCollapse BorderCollapse = BorderCollapse.Separate;

        /// <summary>
        /// Gets or sets the spacing between table borders in pixels.
        /// </summary>
        public float BorderSpacing { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the position of the table caption.
        /// </summary>
        public CaptionSide CaptionSide = CaptionSide.Top;

        /// <summary>
        /// Gets or sets the empty-cells display behavior.
        /// </summary>
        public EmptyCells EmptyCells = EmptyCells.Show;

        /// <summary>
        /// Gets or sets the vertical alignment of table cell content.
        /// </summary>
        public VerticalAlignment TableCellVerticalAlign = VerticalAlignment.Middle;
    }

    /// <summary>
    /// Specifies the table layout algorithm.
    /// </summary>
    public enum TableLayout : byte
    {
        Auto, Fixed, Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies whether table borders collapse.
    /// </summary>
    public enum BorderCollapse : byte
    {
        Collapse, Separate, Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies the position of the table caption.
    /// </summary>
    public enum CaptionSide : byte
    {
        Top, Bottom, Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies whether empty table cells are shown.
    /// </summary>
    public enum EmptyCells : byte
    {
        Show, Hide, Initial, Inherit, Unset
    }
}