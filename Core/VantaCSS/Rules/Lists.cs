namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the list style type.
        /// </summary>
        public ListStyleType ListStyleType = ListStyleType.Disc;

        /// <summary>
        /// Gets or sets the list style position.
        /// </summary>
        public ListStylePosition ListStylePosition = ListStylePosition.Outside;

        /// <summary>
        /// Gets or sets the list style image URL.
        /// </summary>
        public string ListStyleImage { get; set; } = "";

        /// <summary>
        /// Gets or sets the starting value of an ordered list.
        /// </summary>
        public int ListStart { get; set; } = 1;

        /// <summary>
        /// Gets or sets whether list numbering is reversed.
        /// </summary>
        public bool Reversed { get; set; } = false;
    }

    /// <summary>
    /// Specifies the marker style of a list.
    /// </summary>
    public enum ListStyleType : byte
    {
        Disc, Circle, Square, Decimal, DecimalLeadingZero, LowerRoman, UpperRoman,
        LowerGreek, LowerLatin, UpperLatin, Armenian, Georgian, LowerAlpha, UpperAlpha,
        None, Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies the position of list markers.
    /// </summary>
    public enum ListStylePosition : byte
    {
        Inside, Outside, Initial, Inherit, Unset
    }
}