namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the overflow behavior on both axes.
        /// </summary>
        public Overflow Overflow = Overflow.Visible;

        /// <summary>
        /// Gets or sets the overflow behavior on the horizontal axis.
        /// </summary>
        public Overflow OverflowX = Overflow.Visible;

        /// <summary>
        /// Gets or sets the overflow behavior on the vertical axis.
        /// </summary>
        public Overflow OverflowY = Overflow.Visible;

        /// <summary>
        /// Gets or sets whether the element is visible.
        /// </summary>
        public Visibility Visibility = Visibility.Visible;

        /// <summary>
        /// Gets or sets the stacking order of the element.
        /// </summary>
        public int ZIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the opacity of the element, where 0 is fully transparent and 1 is fully opaque.
        /// </summary>
        public float Opacity { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the float positioning of the element.
        /// </summary>
        public Float Float = Float.None;

        /// <summary>
        /// Gets or sets which sides floating elements are cleared.
        /// </summary>
        public Clear Clear = Clear.None;

        /// <summary>
        /// Gets or sets the vertical alignment of the element.
        /// </summary>
        public VerticalAlignment VerticalAlign = VerticalAlignment.Baseline;

        /// <summary>
        /// Uniform border width in pixels applied to all sides.
        /// </summary>
        public float BorderWidth
        {
            set
            {
                BorderTopWidth = value;
                BorderRightWidth = value;
                BorderBottomWidth = value;
                BorderLeftWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the top border width in pixels.
        /// </summary>
        public float BorderTopWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the right border width in pixels.
        /// </summary>
        public float BorderRightWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the bottom border width in pixels.
        /// </summary>
        public float BorderBottomWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the left border width in pixels.
        /// </summary>
        public float BorderLeftWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the border style applied to all sides.
        /// </summary>
        public BorderStyle BorderStyle = BorderStyle.None;

        /// <summary>
        /// Gets or sets the border color applied to all sides.
        /// </summary>
        public string BorderColor { get; set; } = "";

        /// <summary>
        /// Uniform border radius in pixels applied to all corners.
        /// </summary>
        public float BorderRadius
        {
            set
            {
                BorderRadiusTopLeft = value;
                BorderRadiusTopRight = value;
                BorderRadiusBottomRight = value;
                BorderRadiusBottomLeft = value;
            }
        }

        /// <summary>
        /// Gets or sets the top-left corner border radius in pixels.
        /// </summary>
        public float BorderRadiusTopLeft { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the top-right corner border radius in pixels.
        /// </summary>
        public float BorderRadiusTopRight { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the bottom-right corner border radius in pixels.
        /// </summary>
        public float BorderRadiusBottomRight { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the bottom-left corner border radius in pixels.
        /// </summary>
        public float BorderRadiusBottomLeft { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the box shadow definition (CSS shorthand).
        /// </summary>
        public string BoxShadow { get; set; } = "";

        /// <summary>
        /// Gets or sets the outline width in pixels.
        /// </summary>
        public float OutlineWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the outline style.
        /// </summary>
        public BorderStyle OutlineStyle = BorderStyle.None;

        /// <summary>
        /// Gets or sets the outline color.
        /// </summary>
        public string OutlineColor { get; set; } = "";

        /// <summary>
        /// Gets or sets the outline offset in pixels.
        /// </summary>
        public float OutlineOffset { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the scroll margin in pixels.
        /// </summary>
        public float ScrollMargin { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the scroll padding in pixels.
        /// </summary>
        public float ScrollPadding { get; set; } = 0f;
    }

    /// <summary>
    /// Specifies how an element's total width and height are calculated.
    /// </summary>
    public enum BoxSizing : byte
    {
        /// <summary>
        /// Width and height apply to the content box only; padding and border are added on top.
        /// </summary>
        ContentBox,

        /// <summary>
        /// Width and height include content, padding, and border.
        /// </summary>
        BorderBox,

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
    /// Specifies how overflow content is handled.
    /// </summary>
    public enum Overflow : byte
    {
        /// <summary>
        /// Content is not clipped and may overflow the container.
        /// </summary>
        Visible,

        /// <summary>
        /// Content is clipped and no scrollbars are provided.
        /// </summary>
        Hidden,

        /// <summary>
        /// Content is clipped without scrollbars (newer, stricter than hidden).
        /// </summary>
        Clip,

        /// <summary>
        /// Content is clipped and scrollbars are always shown.
        /// </summary>
        Scroll,

        /// <summary>
        /// Scrollbars appear only when needed.
        /// </summary>
        Auto,

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
    /// Specifies whether an element is visible.
    /// </summary>
    public enum Visibility : byte
    {
        /// <summary>
        /// The element is visible.
        /// </summary>
        Visible,

        /// <summary>
        /// The element is invisible but still occupies layout space.
        /// </summary>
        Hidden,

        /// <summary>
        /// Collapses table rows/columns (or acts like hidden for other elements).
        /// </summary>
        Collapse,

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
    /// Specifies the float positioning of an element.
    /// </summary>
    public enum Float : byte
    {
        /// <summary>
        /// The element does not float.
        /// </summary>
        None,

        /// <summary>
        /// The element floats to the left.
        /// </summary>
        Left,

        /// <summary>
        /// The element floats to the right.
        /// </summary>
        Right,

        /// <summary>
        /// Floats to the inline start edge.
        /// </summary>
        InlineStart,

        /// <summary>
        /// Floats to the inline end edge.
        /// </summary>
        InlineEnd,

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
    /// Specifies which sides floating elements are cleared.
    /// </summary>
    public enum Clear : byte
    {
        /// <summary>
        /// No clearing is applied.
        /// </summary>
        None,

        /// <summary>
        /// Clears left floats.
        /// </summary>
        Left,

        /// <summary>
        /// Clears right floats.
        /// </summary>
        Right,

        /// <summary>
        /// Clears both left and right floats.
        /// </summary>
        Both,

        /// <summary>
        /// Clears floats on the inline start side.
        /// </summary>
        InlineStart,

        /// <summary>
        /// Clears floats on the inline end side.
        /// </summary>
        InlineEnd,

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
    /// Specifies the vertical alignment of an inline or table-cell element.
    /// </summary>
    public enum VerticalAlignment : byte
    {
        /// <summary>
        /// Aligns the baseline of the element with the baseline of the parent.
        /// </summary>
        Baseline,

        /// <summary>
        /// Subscripts the element.
        /// </summary>
        Sub,

        /// <summary>
        /// Superscripts the element.
        /// </summary>
        Super,

        /// <summary>
        /// Aligns the top of the element with the top of the parent's font.
        /// </summary>
        TextTop,

        /// <summary>
        /// Aligns the bottom of the element with the bottom of the parent's font.
        /// </summary>
        TextBottom,

        /// <summary>
        /// Centers the element vertically.
        /// </summary>
        Middle,

        /// <summary>
        /// Aligns the top of the element with the top of the line.
        /// </summary>
        Top,

        /// <summary>
        /// Aligns the bottom of the element with the bottom of the line.
        /// </summary>
        Bottom,

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
    /// Specifies the style of a border or outline.
    /// </summary>
    public enum BorderStyle : byte
    {
        /// <summary>
        /// No border is rendered.
        /// </summary>
        None,

        /// <summary>
        /// A solid border.
        /// </summary>
        Solid,

        /// <summary>
        /// A dashed border.
        /// </summary>
        Dashed,

        /// <summary>
        /// A dotted border.
        /// </summary>
        Dotted,

        /// <summary>
        /// A double border.
        /// </summary>
        Double,

        /// <summary>
        /// A grooved border.
        /// </summary>
        Groove,

        /// <summary>
        /// A ridged border.
        /// </summary>
        Ridge,

        /// <summary>
        /// An inset border.
        /// </summary>
        Inset,

        /// <summary>
        /// An outset border.
        /// </summary>
        Outset,

        /// <summary>
        /// A hidden border (same as none but with different cascade behavior).
        /// </summary>
        Hidden,

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