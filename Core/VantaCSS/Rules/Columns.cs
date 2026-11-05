namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the number of columns.
        /// </summary>
        public int ColumnCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the preferred column width in pixels.
        /// </summary>
        public float ColumnWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the gap between columns in pixels.
        /// </summary>
        public float ColumnGapWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the column rule width in pixels.
        /// </summary>
        public float ColumnRuleWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the column rule style.
        /// </summary>
        public BorderStyle ColumnRuleStyle = BorderStyle.None;

        /// <summary>
        /// Gets or sets the column rule color.
        /// </summary>
        public string ColumnRuleColor { get; set; } = "";

        /// <summary>
        /// Gets or sets the column span behavior.
        /// </summary>
        public ColumnSpan ColumnSpan = ColumnSpan.None;

        /// <summary>
        /// Gets or sets the column fill strategy.
        /// </summary>
        public ColumnFill ColumnFill = ColumnFill.Balance;
    }

    /// <summary>
    /// Specifies how an element spans columns.
    /// </summary>
    public enum ColumnSpan : byte
    {
        None, All, Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies how columns are filled.
    /// </summary>
    public enum ColumnFill : byte
    {
        Auto, Balance, BalanceAll, Initial, Inherit, Unset
    }
}