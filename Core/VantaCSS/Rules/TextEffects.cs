namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the text rendering optimization hint.
        /// </summary>
        public TextRendering TextRendering = TextRendering.Auto;

        /// <summary>
        /// Gets or sets the text size adjustment.
        /// </summary>
        public string TextSizeAdjust { get; set; } = "auto";

        /// <summary>
        /// Gets or sets the webkit text fill color.
        /// </summary>
        public string WebkitTextFillColor { get; set; } = "";

        /// <summary>
        /// Gets or sets the webkit text stroke shorthand.
        /// </summary>
        public string WebkitTextStroke { get; set; } = "";

        /// <summary>
        /// Gets or sets the webkit text stroke color.
        /// </summary>
        public string WebkitTextStrokeColor { get; set; } = "";

        /// <summary>
        /// Gets or sets the webkit text stroke width in pixels.
        /// </summary>
        public float WebkitTextStrokeWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the webkit text security display.
        /// </summary>
        public WebkitTextSecurity WebkitTextSecurity = WebkitTextSecurity.None;
    }

    public enum TextRendering : byte
    {
        Auto,
        OptimizeSpeed,
        OptimizeLegibility,
        GeometricPrecision,
        Initial,
        Inherit,
        Unset
    }

    public enum WebkitTextSecurity : byte
    {
        None,
        Disc,
        Circle,
        Square,
        Initial,
        Inherit,
        Unset
    }
}
