namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the unicode bidi behavior.
        /// </summary>
        public UnicodeBidi UnicodeBidi = UnicodeBidi.Normal;

        /// <summary>
        /// Gets or sets the writing mode.
        /// </summary>
        public WritingMode WritingMode = WritingMode.HorizontalTb;

        /// <summary>
        /// Gets or sets the ruby alignment.
        /// </summary>
        public RubyAlign RubyAlign = RubyAlign.SpaceAround;

        /// <summary>
        /// Gets or sets the ruby position.
        /// </summary>
        public RubyPosition RubyPosition = RubyPosition.Over;

        /// <summary>
        /// Gets or sets the ruby overhang behavior.
        /// </summary>
        public string RubyOverhang { get; set; } = "auto";
    }

    public enum UnicodeBidi : byte
    {
        Normal,
        Embed,
        Isolate,
        BidiOverride,
        IsolateOverride,
        Plaintext,
        Initial,
        Inherit,
        Unset
    }

    public enum WritingMode : byte
    {
        HorizontalTb,
        VerticalRl,
        VerticalLr,
        SidewaysRl,
        SidewaysLr,
        Initial,
        Inherit,
        Unset
    }

    public enum RubyAlign : byte
    {
        Start,
        Center,
        SpaceBetween,
        SpaceAround,
        Initial,
        Inherit,
        Unset
    }

    public enum RubyPosition : byte
    {
        Over,
        Under,
        InterCharacter,
        Initial,
        Inherit,
        Unset
    }
}
