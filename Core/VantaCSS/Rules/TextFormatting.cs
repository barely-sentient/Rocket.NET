namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the text wrapping mode.
        /// </summary>
        public TextWrap TextWrap = TextWrap.Wrap;

        /// <summary>
        /// Gets or sets the text wrap mode (wrap or nowrap).
        /// </summary>
        public TextWrapMode TextWrapMode = TextWrapMode.Wrap;

        /// <summary>
        /// Gets or sets the text wrap style.
        /// </summary>
        public TextWrapStyle TextWrapStyle = TextWrapStyle.Auto;

        /// <summary>
        /// Gets or sets the text justification strategy.
        /// </summary>
        public TextJustify TextJustify = TextJustify.Auto;

        /// <summary>
        /// Gets or sets the orientation of text within a line.
        /// </summary>
        public TextOrientation TextOrientation = TextOrientation.Mixed;

        /// <summary>
        /// Gets or sets how characters are combined upright.
        /// </summary>
        public TextCombineUpright TextCombineUpright = TextCombineUpright.None;

        /// <summary>
        /// Gets or sets the automatic spacing between character classes.
        /// </summary>
        public string TextAutospace { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the spacing trim around CJK punctuation.
        /// </summary>
        public TextSpacingTrim TextSpacingTrim = TextSpacingTrim.Normal;

        /// <summary>
        /// Gets or sets the alignment applied to all lines.
        /// </summary>
        public TextAlignment TextAlignAll = TextAlignment.Start;

        /// <summary>
        /// Gets or sets the text emphasis style.
        /// </summary>
        public string TextEmphasisStyle { get; set; } = "none";

        /// <summary>
        /// Gets or sets the experimental text fitting behavior.
        /// </summary>
        public string TextFit { get; set; } = "";

        /// <summary>
        /// Gets or sets the text group alignment.
        /// </summary>
        public string TextGroupAlign { get; set; } = "";
    }

    public enum TextWrap : byte
    {
        Wrap,
        NoWrap,
        Balance,
        Pretty,
        Stable,
        Initial,
        Inherit,
        Unset
    }

    public enum TextWrapMode : byte
    {
        Wrap,
        NoWrap,
        Initial,
        Inherit,
        Unset
    }

    public enum TextWrapStyle : byte
    {
        Auto,
        Balance,
        Pretty,
        Stable,
        Initial,
        Inherit,
        Unset
    }

    public enum TextJustify : byte
    {
        Auto,
        InterWord,
        InterCharacter,
        None,
        Distribute,
        Initial,
        Inherit,
        Unset
    }

    public enum TextOrientation : byte
    {
        Mixed,
        Upright,
        Sideways,
        Initial,
        Inherit,
        Unset
    }

    public enum TextSpacingTrim : byte
    {
        Normal,
        SpaceAll,
        TrimStart,
        Initial,
        Inherit,
        Unset
    }

    public enum TextCombineUpright : byte
    {
        None,
        All,
        Digits,
        Initial,
        Inherit,
        Unset
    }
}
