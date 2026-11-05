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

        /// <summary>
        /// Gets or sets the font shorthand.
        /// </summary>
        public string Font { get; set; } = "";

        /// <summary>
        /// Gets or sets the font stretch.
        /// </summary>
        public string FontStretch { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font kerning behavior.
        /// </summary>
        public FontKerning FontKerning = FontKerning.Auto;

        /// <summary>
        /// Gets or sets the font optical sizing behavior.
        /// </summary>
        public FontOpticalSizing FontOpticalSizing = FontOpticalSizing.Auto;

        /// <summary>
        /// Gets or sets the font feature settings.
        /// </summary>
        public string FontFeatureSettings { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font variation settings.
        /// </summary>
        public string FontVariationSettings { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font variant shorthand.
        /// </summary>
        public string FontVariant { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font variant alternates.
        /// </summary>
        public string FontVariantAlternates { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font variant caps.
        /// </summary>
        public FontVariantCaps FontVariantCaps = FontVariantCaps.Normal;

        /// <summary>
        /// Gets or sets the font variant east asian.
        /// </summary>
        public string FontVariantEastAsian { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font variant emoji.
        /// </summary>
        public FontVariantEmoji FontVariantEmoji = FontVariantEmoji.Normal;

        /// <summary>
        /// Gets or sets the font variant ligatures.
        /// </summary>
        public string FontVariantLigatures { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font variant numeric.
        /// </summary>
        public string FontVariantNumeric { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font variant position.
        /// </summary>
        public FontVariantPosition FontVariantPosition = FontVariantPosition.Normal;

        /// <summary>
        /// Gets or sets the font size adjust metric.
        /// </summary>
        public string FontSizeAdjust { get; set; } = "none";

        /// <summary>
        /// Gets or sets the font palette.
        /// </summary>
        public string FontPalette { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font synthesis shorthand.
        /// </summary>
        public string FontSynthesis { get; set; } = "";

        /// <summary>
        /// Gets or sets the font synthesis small caps behavior.
        /// </summary>
        public FontSynthesisSmallCaps FontSynthesisSmallCaps = FontSynthesisSmallCaps.Auto;

        /// <summary>
        /// Gets or sets the font synthesis style behavior.
        /// </summary>
        public string FontSynthesisStyle { get; set; } = "auto";

        /// <summary>
        /// Gets or sets the font synthesis weight behavior.
        /// </summary>
        public FontSynthesisWeight FontSynthesisWeight = FontSynthesisWeight.Auto;

        /// <summary>
        /// Gets or sets the font synthesis position behavior.
        /// </summary>
        public FontSynthesisPosition FontSynthesisPosition = FontSynthesisPosition.Auto;

        /// <summary>
        /// Gets or sets the font language override.
        /// </summary>
        public string FontLanguageOverride { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the font variant ordinal.
        /// </summary>
        public FontVariantOrdinal FontVariantOrdinal = FontVariantOrdinal.Normal;
    }

    public enum FontKerning : byte
    {
        Auto,
        Normal,
        None,
        Initial,
        Inherit,
        Unset
    }

    public enum FontOpticalSizing : byte
    {
        Auto,
        None,
        Initial,
        Inherit,
        Unset
    }

    public enum FontVariantCaps : byte
    {
        Normal,
        SmallCaps,
        AllSmallCaps,
        PetiteCaps,
        AllPetiteCaps,
        Unicase,
        TitlingCaps,
        Initial,
        Inherit,
        Unset
    }

    public enum FontVariantEmoji : byte
    {
        Normal,
        Text,
        Emoji,
        Unicode,
        Initial,
        Inherit,
        Unset
    }

    public enum FontVariantPosition : byte
    {
        Normal,
        Sub,
        Super,
        Initial,
        Inherit,
        Unset
    }

    public enum FontSynthesisSmallCaps : byte
    {
        Auto,
        None,
        Initial,
        Inherit,
        Unset
    }

    public enum FontSynthesisWeight : byte
    {
        Auto,
        None,
        Initial,
        Inherit,
        Unset
    }

    public enum FontSynthesisPosition : byte
    {
        Auto,
        None,
        Initial,
        Inherit,
        Unset
    }

    public enum FontVariantOrdinal : byte
    {
        Normal,
        Ordinal,
        Initial,
        Inherit,
        Unset
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