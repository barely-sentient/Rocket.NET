using System;

namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the text decoration lines applied to text.
        /// </summary>
        public TextDecorationLine TextDecorationLine = TextDecorationLine.None;

        /// <summary>
        /// Gets or sets the thickness of text decoration lines.
        /// </summary>
        public string TextDecorationThickness { get; set; } = "auto";

        /// <summary>
        /// Gets or sets whether decoration lines skip glyph descenders.
        /// </summary>
        public TextDecorationSkipInk TextDecorationSkipInk = TextDecorationSkipInk.Auto;

        /// <summary>
        /// Gets or sets the legacy text decoration skip behavior.
        /// </summary>
        public string TextDecorationSkip { get; set; } = "auto";

        /// <summary>
        /// Gets or sets the inset of text decorations.
        /// </summary>
        public string TextDecorationInset { get; set; } = "auto";
    }

    /// <summary>
    /// Specifies the text decoration lines applied to text.
    /// </summary>
    [Flags]
    public enum TextDecorationLine : byte
    {
        None = 0,
        Underline = 1 << 0,
        Overline = 1 << 1,
        LineThrough = 1 << 2,
        Initial = 1 << 3,
        Inherit = 1 << 4,
        Unset = 1 << 5
    }

    public enum TextDecorationSkipInk : byte
    {
        Auto,
        None,
        All,
        Initial,
        Inherit,
        Unset
    }
}
