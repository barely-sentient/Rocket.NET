namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the alignment baseline.
        /// </summary>
        public AlignmentBaseline AlignmentBaseline = AlignmentBaseline.Auto;

        /// <summary>
        /// Gets or sets the dominant baseline.
        /// </summary>
        public DominantBaseline DominantBaseline = DominantBaseline.Auto;

        /// <summary>
        /// Gets or sets the baseline shift.
        /// </summary>
        public string BaselineShift { get; set; } = "baseline";
    }

    public enum AlignmentBaseline : byte
    {
        Auto,
        Baseline,
        BeforeEdge,
        AfterEdge,
        Central,
        Middle,
        TextBeforeEdge,
        TextAfterEdge,
        Ideographic,
        Alphabetic,
        Hanging,
        Mathematical,
        Top,
        Center,
        Bottom,
        TextTop,
        TextBottom,
        Initial,
        Inherit,
        Unset
    }

    public enum DominantBaseline : byte
    {
        Auto,
        Alphabetic,
        Central,
        Hanging,
        Ideographic,
        Mathematical,
        Middle,
        TextTop,
        TextBottom,
        Initial,
        Inherit,
        Unset
    }
}
