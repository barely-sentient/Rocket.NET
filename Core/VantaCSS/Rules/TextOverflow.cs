namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the line clamp value.
        /// </summary>
        public string LineClamp { get; set; } = "none";

        /// <summary>
        /// Gets or sets the maximum number of lines.
        /// </summary>
        public string MaxLines { get; set; } = "none";

        /// <summary>
        /// Gets or sets the block ellipsis behavior.
        /// </summary>
        public string BlockEllipsis { get; set; } = "auto";

        /// <summary>
        /// Gets or sets the continue behavior.
        /// </summary>
        public string @Continue { get; set; } = "auto";

        /// <summary>
        /// Gets or sets the minimum number of lines at the bottom of a page.
        /// </summary>
        public int Orphans { get; set; } = 2;

        /// <summary>
        /// Gets or sets the minimum number of lines at the top of a page.
        /// </summary>
        public int Widows { get; set; } = 2;
    }
}
