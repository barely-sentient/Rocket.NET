namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the flex direction of the container.
        /// </summary>
        public FlexDirection FlexDirection = FlexDirection.Row;

        /// <summary>
        /// Gets or sets whether flex items wrap onto multiple lines.
        /// </summary>
        public FlexWrap FlexWrap = FlexWrap.NoWrap;

        /// <summary>
        /// Gets or sets how flex items are distributed along the main axis.
        /// </summary>
        public JustifyContent JustifyContent = JustifyContent.FlexStart;

        /// <summary>
        /// Gets or sets how flex items are aligned along the cross axis.
        /// </summary>
        public AlignItems AlignItems = AlignItems.Stretch;

        /// <summary>
        /// Gets or sets how flex lines are distributed along the cross axis.
        /// </summary>
        public AlignContent AlignContent = AlignContent.Stretch;

        /// <summary>
        /// Gets or sets the alignment of a single flex item along the cross axis.
        /// </summary>
        public AlignSelf AlignSelf = AlignSelf.Auto;

        /// <summary>
        /// Gets or sets the flex grow factor of the element.
        /// </summary>
        public float FlexGrow { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the flex shrink factor of the element.
        /// </summary>
        public float FlexShrink { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the initial main size of the flex item in pixels.
        /// </summary>
        public float FlexBasis { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the order in which the element appears in the flex container.
        /// </summary>
        public int FlexOrder { get; set; } = 0;
    }

    /// <summary>
    /// Specifies the direction of the main axis of a flex container.
    /// </summary>
    public enum FlexDirection : byte
    {
        /// <summary>
        /// Items are laid out in a row (left to right in LTR).
        /// </summary>
        Row,

        /// <summary>
        /// Items are laid out in a row in reverse order.
        /// </summary>
        RowReverse,

        /// <summary>
        /// Items are laid out in a column (top to bottom).
        /// </summary>
        Column,

        /// <summary>
        /// Items are laid out in a column in reverse order.
        /// </summary>
        ColumnReverse,

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
    /// Specifies whether flex items wrap.
    /// </summary>
    public enum FlexWrap : byte
    {
        /// <summary>
        /// All flex items are laid out in a single line.
        /// </summary>
        NoWrap,

        /// <summary>
        /// Flex items wrap onto multiple lines from top to bottom.
        /// </summary>
        Wrap,

        /// <summary>
        /// Flex items wrap onto multiple lines from bottom to top.
        /// </summary>
        WrapReverse,

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
    /// Specifies how flex items are distributed along the main axis.
    /// </summary>
    public enum JustifyContent : byte
    {
        /// <summary>
        /// Items are packed toward the start of the main axis.
        /// </summary>
        FlexStart,

        /// <summary>
        /// Items are packed toward the end of the main axis.
        /// </summary>
        FlexEnd,

        /// <summary>
        /// Items are centered along the main axis.
        /// </summary>
        Center,

        /// <summary>
        /// Items are evenly distributed; first item at start, last at end.
        /// </summary>
        SpaceBetween,

        /// <summary>
        /// Items are evenly distributed with equal space around them.
        /// </summary>
        SpaceAround,

        /// <summary>
        /// Items are evenly distributed with equal space between and around them.
        /// </summary>
        SpaceEvenly,

        /// <summary>
        /// Items are packed toward the start (logical).
        /// </summary>
        Start,

        /// <summary>
        /// Items are packed toward the end (logical).
        /// </summary>
        End,

        /// <summary>
        /// Items are packed toward the left edge.
        /// </summary>
        Left,

        /// <summary>
        /// Items are packed toward the right edge.
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
    /// Specifies how flex items are aligned along the cross axis.
    /// </summary>
    public enum AlignItems : byte
    {
        /// <summary>
        /// Items are stretched to fill the container.
        /// </summary>
        Stretch,

        /// <summary>
        /// Items are aligned to the start of the cross axis.
        /// </summary>
        FlexStart,

        /// <summary>
        /// Items are aligned to the end of the cross axis.
        /// </summary>
        FlexEnd,

        /// <summary>
        /// Items are centered along the cross axis.
        /// </summary>
        Center,

        /// <summary>
        /// Items are aligned by their baselines.
        /// </summary>
        Baseline,

        /// <summary>
        /// Items are aligned to the start (logical).
        /// </summary>
        Start,

        /// <summary>
        /// Items are aligned to the end (logical).
        /// </summary>
        End,

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
    /// Specifies how flex lines are distributed along the cross axis.
    /// </summary>
    public enum AlignContent : byte
    {
        /// <summary>
        /// Lines are stretched to fill the container.
        /// </summary>
        Stretch,

        /// <summary>
        /// Lines are packed toward the start of the cross axis.
        /// </summary>
        FlexStart,

        /// <summary>
        /// Lines are packed toward the end of the cross axis.
        /// </summary>
        FlexEnd,

        /// <summary>
        /// Lines are centered along the cross axis.
        /// </summary>
        Center,

        /// <summary>
        /// Lines are evenly distributed; first at start, last at end.
        /// </summary>
        SpaceBetween,

        /// <summary>
        /// Lines are evenly distributed with equal space around them.
        /// </summary>
        SpaceAround,

        /// <summary>
        /// Lines are evenly distributed with equal space between and around them.
        /// </summary>
        SpaceEvenly,

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
    /// Specifies the alignment of a single flex item along the cross axis.
    /// </summary>
    public enum AlignSelf : byte
    {
        /// <summary>
        /// Inherits alignment from the parent's align-items.
        /// </summary>
        Auto,

        /// <summary>
        /// Item is stretched to fill the container.
        /// </summary>
        Stretch,

        /// <summary>
        /// Item is aligned to the start of the cross axis.
        /// </summary>
        FlexStart,

        /// <summary>
        /// Item is aligned to the end of the cross axis.
        /// </summary>
        FlexEnd,

        /// <summary>
        /// Item is centered along the cross axis.
        /// </summary>
        Center,

        /// <summary>
        /// Item is aligned by its baseline.
        /// </summary>
        Baseline,

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