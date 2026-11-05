namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the grid auto-flow direction.
        /// </summary>
        public GridAutoFlow GridAutoFlow = GridAutoFlow.Row;

        /// <summary>
        /// Gets or sets the number of columns for an auto-placed grid.
        /// </summary>
        public int GridAutoColumns { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of rows for an auto-placed grid.
        /// </summary>
        public int GridAutoRows { get; set; } = 0;

        /// <summary>
        /// Gets or sets the column start line for the element.
        /// </summary>
        public int GridColumnStart { get; set; } = 0;

        /// <summary>
        /// Gets or sets the column end line for the element.
        /// </summary>
        public int GridColumnEnd { get; set; } = 0;

        /// <summary>
        /// Gets or sets the row start line for the element.
        /// </summary>
        public int GridRowStart { get; set; } = 0;

        /// <summary>
        /// Gets or sets the row end line for the element.
        /// </summary>
        public int GridRowEnd { get; set; } = 0;

        /// <summary>
        /// Gets or sets the justify-items alignment for grid items.
        /// </summary>
        public JustifyItems JustifyItems = JustifyItems.Stretch;

        /// <summary>
        /// Gets or sets the align-items alignment for grid items (alias of flex align-items).
        /// </summary>
        public AlignItems GridAlignItems = AlignItems.Stretch;

        /// <summary>
        /// Gets or sets the place-content shorthand alignment.
        /// </summary>
        public AlignContent PlaceContent = AlignContent.Stretch;

        /// <summary>
        /// Gets or sets the justify-self alignment for a single grid item.
        /// </summary>
        public JustifySelf JustifySelf = JustifySelf.Auto;

        /// <summary>
        /// Gets or sets the align-self alignment for a single grid item.
        /// </summary>
        public AlignSelf GridAlignSelf = AlignSelf.Auto;
    }

    /// <summary>
    /// Specifies the auto-placement algorithm direction for a grid container.
    /// </summary>
    public enum GridAutoFlow : byte
    {
        /// <summary>
        /// Items are placed by filling each row in turn.
        /// </summary>
        Row,

        /// <summary>
        /// Items are placed by filling each column in turn.
        /// </summary>
        Column,

        /// <summary>
        /// Items are placed to fill holes in the grid (dense packing).
        /// </summary>
        Dense,

        /// <summary>
        /// Items are placed by filling each row with dense packing.
        /// </summary>
        RowDense,

        /// <summary>
        /// Items are placed by filling each column with dense packing.
        /// </summary>
        ColumnDense,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }

    /// <summary>
    /// Specifies how grid items are justified within their cells.
    /// </summary>
    public enum JustifyItems : byte
    {
        /// <summary>
        /// Items are stretched to fill the cell.
        /// </summary>
        Stretch,

        /// <summary>
        /// Items are aligned to the start of the cell.
        /// </summary>
        Start,

        /// <summary>
        /// Items are aligned to the end of the cell.
        /// </summary>
        End,

        /// <summary>
        /// Items are centered within the cell.
        /// </summary>
        Center,

        /// <summary>
        /// Items are aligned to the left edge.
        /// </summary>
        Left,

        /// <summary>
        /// Items are aligned to the right edge.
        /// </summary>
        Right,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }

    /// <summary>
    /// Specifies the self-justification of a single grid item.
    /// </summary>
    public enum JustifySelf : byte
    {
        /// <summary>
        /// Inherits from the container's justify-items.
        /// </summary>
        Auto,

        /// <summary>
        /// Item is stretched to fill the cell.
        /// </summary>
        Stretch,

        /// <summary>
        /// Item is aligned to the start of the cell.
        /// </summary>
        Start,

        /// <summary>
        /// Item is aligned to the end of the cell.
        /// </summary>
        End,

        /// <summary>
        /// Item is centered within the cell.
        /// </summary>
        Center,

        /// <summary>
        /// Item is aligned to the left edge.
        /// </summary>
        Left,

        /// <summary>
        /// Item is aligned to the right edge.
        /// </summary>
        Right,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }
}