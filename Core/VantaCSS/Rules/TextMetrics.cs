namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the tab size.
        /// </summary>
        public int TabSize { get; set; } = 8;

        /// <summary>
        /// Gets or sets the initial letter (drop cap) definition.
        /// </summary>
        public string InitialLetter { get; set; } = "normal";

        /// <summary>
        /// Gets or sets the text box shorthand.
        /// </summary>
        public string TextBox { get; set; } = "";

        /// <summary>
        /// Gets or sets the text box edge metric.
        /// </summary>
        public TextBoxEdge TextBoxEdge = TextBoxEdge.Text;

        /// <summary>
        /// Gets or sets the text box trim behavior.
        /// </summary>
        public TextBoxTrim TextBoxTrim = TextBoxTrim.None;
    }

    public enum TextBoxEdge : byte
    {
        Cap,
        Ex,
        Text,
        Ideographic,
        Initial,
        Inherit,
        Unset
    }

    public enum TextBoxTrim : byte
    {
        None,
        TrimStart,
        TrimEnd,
        TrimBoth,
        Initial,
        Inherit,
        Unset
    }
}
