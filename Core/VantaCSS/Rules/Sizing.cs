namespace VantaCSS.Engine
{
    /// <partial class="NodeComputedRuleset" />
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the computed left padding value in pixels.
        /// </summary>
        public float PaddingLeft { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the computed right padding value in pixels.
        /// </summary>
        public float PaddingRight { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the computed top padding value in pixels.
        /// </summary>
        public float PaddingTop { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the computed bottom padding value in pixels.
        /// </summary>
        public float PaddingBottom { get; set; } = 0f;
        
        /// <summary>
        /// Gets or sets the computed element width in pixels.
        /// </summary>
        public float Width { get; set; } = 0f;
        
        /// <summary>
        /// Gets or sets the computed element height in pixels.
        /// </summary>
        public float Height { get; set; } = 0f;
        
        /// <summary>
        /// Gets or sets the minimum allowed width constraint in pixels.
        /// </summary>
        public float MinWidth { get; set; } = 0f;
        
        /// <summary>
        /// Gets or sets the minimum allowed height constraint in pixels.
        /// </summary>
        public float MinHeight { get; set; } = 0f;
        
        /// <summary>
        /// Gets or sets the maximum allowed width constraint in pixels.
        /// </summary>
        public float MaxWidth { get; set; } = 0f;
        
        /// <summary>
        /// Gets or sets the maximum allowed height constraint in pixels.
        /// </summary>
        public float MaxHeight { get; set; } = 0f;

        /// <summary>
        /// Sets uniform padding across all four sides (Left, Right, Top, and Bottom) simultaneously.
        /// </summary>
        public float Padding
        {
            set
            {
                PaddingLeft = value;
                PaddingRight = value;
                PaddingTop = value;
                PaddingBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the gap between flex/grid items along both axes in pixels.
        /// </summary>
        public float Gap { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the gap between flex/grid items along the row (vertical) axis in pixels.
        /// </summary>
        public float RowGap { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the gap between flex/grid items along the column (horizontal) axis in pixels.
        /// </summary>
        public float ColumnGap { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the preferred aspect ratio of the element (width / height).
        /// </summary>
        public float AspectRatio { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the box-sizing model used to calculate element dimensions.
        /// </summary>
        public BoxSizing BoxSizing = BoxSizing.ContentBox;
    }
}