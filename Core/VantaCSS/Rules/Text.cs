using System;

namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Where should any text align.
        /// </summary>
        public TextAlignment TextAlign = TextAlignment.Initial;
        
        /// <summary>
        /// Any text decoration.
        /// </summary>
        public TextDecoration TextDecoration = TextDecoration.None;

        /// <summary>
        /// Thickness of the text, defaults to 1px.
        /// </summary>
        public int TextThickness = 1;
        
        /// <summary>
        /// Text underline position
        /// </summary>
        public TextUnderlinePosition TextUnderlinePosition = TextUnderlinePosition.Auto;
    }

    /// <summary>
    /// Specifies the horizontal alignment of inline-level content within a block container.
    /// </summary>
    public enum TextAlignment : byte
    {
        /// <summary>
        /// Aligns text to the left edge of the containing block.
        /// </summary>
        Left,

        /// <summary>
        /// Centers text within the containing block.
        /// </summary>
        Center,

        /// <summary>
        /// Aligns text to the right edge of the containing block.
        /// </summary>
        Right,

        /// <summary>
        /// Distributes text so that each line, except the last line, stretches
        /// to align with both edges of the containing block.
        /// </summary>
        Justify,

        /// <summary>
        /// Aligns text to the logical start edge of the containing block,
        /// based on the text direction.
        /// </summary>
        Start,

        /// <summary>
        /// Aligns text to the logical end edge of the containing block,
        /// based on the text direction.
        /// </summary>
        End,

        /// <summary>
        /// Aligns text to the left edge, regardless of text direction.
        /// </summary>
        MatchParent,

        /// <summary>
        /// Uses the alignment defined by the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Resets alignment to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Uses the inherited value if the property is inherited,
        /// otherwise uses the initial value.
        /// </summary>
        Unset,

        /// <summary>
        /// Reverts the property to the browser's user-agent stylesheet value.
        /// </summary>
        Revert,

        /// <summary>
        /// Reverts the property to the value established by the previous cascade layer.
        /// </summary>
        RevertLayer
    }
    
    /// <summary>
    /// Specifies the text decoration lines applied to text.
    /// </summary>
    [Flags]
    public enum TextDecoration : byte
    {
        /// <summary>
        /// Applies no text decoration.
        /// </summary>
        None = 0,

        /// <summary>
        /// Draws a line beneath the text.
        /// </summary>
        Underline = 1 << 0,

        /// <summary>
        /// Draws a line above the text.
        /// </summary>
        Overline = 1 << 1,

        /// <summary>
        /// Draws a line through the middle of the text.
        /// </summary>
        LineThrough = 1 << 2
    }
    
    /// <summary>
    /// Specifies the position of an underline relative to the text.
    /// </summary>
    [Flags]
    public enum TextUnderlinePosition : byte
    {
        /// <summary>
        /// Uses the default underline position.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Positions the underline below the text baseline.
        /// </summary>
        Under = 1 << 0,

        /// <summary>
        /// Positions the underline on the left side of the text.
        /// </summary>
        Left = 1 << 1,

        /// <summary>
        /// Positions the underline on the right side of the text.
        /// </summary>
        Right = 1 << 2
    }
}