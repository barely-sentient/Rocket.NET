namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the white space collapse behavior.
        /// </summary>
        public WhiteSpaceCollapse WhiteSpaceCollapse = WhiteSpaceCollapse.Collapse;

        /// <summary>
        /// Gets or sets the hyphenation behavior.
        /// </summary>
        public Hyphens Hyphens = Hyphens.None;

        /// <summary>
        /// Gets or sets the hyphenation character.
        /// </summary>
        public string HyphenateCharacter { get; set; } = "auto";

        /// <summary>
        /// Gets or sets the hyphenation limit characters.
        /// </summary>
        public string HyphenateLimitChars { get; set; } = "auto";

        /// <summary>
        /// Gets or sets the line break behavior.
        /// </summary>
        public LineBreak LineBreak = LineBreak.Auto;

        /// <summary>
        /// Gets or sets the hanging punctuation behavior.
        /// </summary>
        public string HangingPunctuation { get; set; } = "none";

        /// <summary>
        /// Alias of overflow-wrap.
        /// </summary>
        public OverflowWrap WordWrap = OverflowWrap.Normal;

        /// <summary>
        /// Gets or sets the white space trim behavior.
        /// </summary>
        public string WhiteSpaceTrim { get; set; } = "none";

        /// <summary>
        /// Gets or sets the word space transform.
        /// </summary>
        public string WordSpaceTransform { get; set; } = "none";
    }

    public enum WhiteSpaceCollapse : byte
    {
        Collapse,
        Preserve,
        PreserveBreaks,
        PreserveSpaces,
        BreakSpaces,
        Initial,
        Inherit,
        Unset
    }

    public enum Hyphens : byte
    {
        None,
        Manual,
        Auto,
        Initial,
        Inherit,
        Unset
    }

    public enum LineBreak : byte
    {
        Auto,
        Loose,
        Normal,
        Strict,
        Anywhere,
        Initial,
        Inherit,
        Unset
    }
}
