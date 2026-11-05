namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the font size in pixels.
        /// </summary>
        public float FontSize { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the font weight (100-900, or named weights via CSS).
        /// </summary>
        public int FontWeight { get; set; } = 400;

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle = FontStyle.Normal;

        /// <summary>
        /// Gets or sets the font family name.
        /// </summary>
        public string FontFamily { get; set; } = "";

        /// <summary>
        /// Gets or sets the line height multiplier or pixel value.
        /// </summary>
        public float LineHeight { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the letter spacing in pixels.
        /// </summary>
        public float LetterSpacing { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the word spacing in pixels.
        /// </summary>
        public float WordSpacing { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the text indent in pixels.
        /// </summary>
        public float TextIndent { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the text shadow definition (CSS shorthand).
        /// </summary>
        public string TextShadow { get; set; } = "";

        /// <summary>
        /// Gets or sets the text stroke width in pixels.
        /// </summary>
        public float TextStrokeWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the text stroke color.
        /// </summary>
        public string TextStrokeColor { get; set; } = "";
    }

    /// <summary>
    /// Specifies the font style.
    /// </summary>
    public enum FontStyle : byte
    {
        /// <summary>
        /// Normal (upright) font style.
        /// </summary>
        Normal,

        /// <summary>
        /// Italic font style.
        /// </summary>
        Italic,

        /// <summary>
        /// Oblique font style.
        /// </summary>
        Oblique,

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