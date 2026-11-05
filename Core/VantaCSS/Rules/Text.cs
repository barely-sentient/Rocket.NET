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

        /// <summary>
        /// Gets or sets the text transform.
        /// </summary>
        public TextTransform TextTransform = TextTransform.None;

        /// <summary>
        /// Gets or sets the text overflow behavior.
        /// </summary>
        public TextOverflow TextOverflow = TextOverflow.Clip;

        /// <summary>
        /// Gets or sets the white-space handling.
        /// </summary>
        public WhiteSpace WhiteSpace = WhiteSpace.Normal;

        /// <summary>
        /// Gets or sets the word-break behavior.
        /// </summary>
        public WordBreak WordBreak = WordBreak.Normal;

        /// <summary>
        /// Gets or sets the overflow-wrap behavior.
        /// </summary>
        public OverflowWrap OverflowWrap = OverflowWrap.Normal;

        /// <summary>
        /// Gets or sets the text alignment of the last line.
        /// </summary>
        public TextAlignLast TextAlignLast = TextAlignLast.Auto;

        /// <summary>
        /// Gets or sets the inline base direction.
        /// </summary>
        public Direction Direction = Direction.Ltr;

        /// <summary>
        /// Gets or sets the text decoration style.
        /// </summary>
        public TextDecorationStyle TextDecorationStyle = TextDecorationStyle.Solid;

        /// <summary>
        /// Gets or sets the text decoration color.
        /// </summary>
        public string TextDecorationColor { get; set; } = "";

        /// <summary>
        /// Gets or sets the text underline offset in pixels.
        /// </summary>
        public float TextUnderlineOffset { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the text emphasis style.
        /// </summary>
        public string TextEmphasis { get; set; } = "";

        /// <summary>
        /// Gets or sets the text emphasis color.
        /// </summary>
        public string TextEmphasisColor { get; set; } = "";

        /// <summary>
        /// Gets or sets the text emphasis position.
        /// </summary>
        public TextEmphasisPosition TextEmphasisPosition = TextEmphasisPosition.Over;
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

    /// <summary>
    /// Specifies the text transformation applied to text.
    /// </summary>
    public enum TextTransform : byte
    {
        /// <summary>
        /// No text transformation is applied.
        /// </summary>
        None,

        /// <summary>
        /// Capitalizes the first letter of each word.
        /// </summary>
        Capitalize,

        /// <summary>
        /// Converts all text to uppercase.
        /// </summary>
        Uppercase,

        /// <summary>
        /// Converts all text to lowercase.
        /// </summary>
        Lowercase,

        /// <summary>
        /// Converts all characters to full-width form.
        /// </summary>
        FullWidth,

        /// <summary>
        /// Converts small kana to full-size kana.
        /// </summary>
        FullSizeKana,

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

    /// <summary>
    /// Specifies how overflowed text content is signaled.
    /// </summary>
    public enum TextOverflow : byte
    {
        /// <summary>
        /// Text is clipped at the boundary.
        /// </summary>
        Clip,

        /// <summary>
        /// An ellipsis is shown for clipped text.
        /// </summary>
        Ellipsis,

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

    /// <summary>
    /// Specifies how white space inside an element is handled.
    /// </summary>
    public enum WhiteSpace : byte
    {
        /// <summary>
        /// Sequences of whitespace collapse into a single space; text wraps.
        /// </summary>
        Normal,

        /// <summary>
        /// Text does not wrap; whitespace collapses.
        /// </summary>
        NoWrap,

        /// <summary>
        /// Whitespace is preserved; text does not wrap.
        /// </summary>
        Pre,

        /// <summary>
        /// Whitespace is preserved; text wraps.
        /// </summary>
        PreWrap,

        /// <summary>
        /// Sequences collapse; line breaks are preserved.
        /// </summary>
        PreLine,

        /// <summary>
        /// Like pre-wrap but with breaking opportunities after each space.
        /// </summary>
        BreakSpaces,

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

    /// <summary>
    /// Specifies how words are broken at line endings.
    /// </summary>
    public enum WordBreak : byte
    {
        /// <summary>
        /// Uses the default line-breaking rules.
        /// </summary>
        Normal,

        /// <summary>
        /// Allows breaks between any two characters.
        /// </summary>
        BreakAll,

        /// <summary>
        /// Disallows breaks within words (for CJK).
        /// </summary>
        KeepAll,

        /// <summary>
        /// Allows unbreakable words to be broken.
        /// </summary>
        BreakWord,

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

    /// <summary>
    /// Specifies whether the browser may break lines within words to prevent overflow.
    /// </summary>
    public enum OverflowWrap : byte
    {
        /// <summary>
        /// Breaks only at normal word-break points.
        /// </summary>
        Normal,

        /// <summary>
        /// Allows breaking within a word if needed.
        /// </summary>
        BreakWord,

        /// <summary>
        /// Allows breaking anywhere, including within words.
        /// </summary>
        Anywhere,

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

    /// <summary>
    /// Specifies the alignment of the last line of a block.
    /// </summary>
    public enum TextAlignLast : byte
    {
        /// <summary>
        /// Aligns the last line like the rest of the text.
        /// </summary>
        Auto,

        /// <summary>
        /// Aligns the last line to the start.
        /// </summary>
        Start,

        /// <summary>
        /// Aligns the last line to the end.
        /// </summary>
        End,

        /// <summary>
        /// Centers the last line.
        /// </summary>
        Center,

        /// <summary>
        /// Justifies the last line.
        /// </summary>
        Justify,

        /// <summary>
        /// Aligns the last line to the left.
        /// </summary>
        Left,

        /// <summary>
        /// Aligns the last line to the right.
        /// </summary>
        Right,

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

    /// <summary>
    /// Specifies the inline base direction of text.
    /// </summary>
    public enum Direction : byte
    {
        /// <summary>
        /// Left-to-right text direction.
        /// </summary>
        Ltr,

        /// <summary>
        /// Right-to-left text direction.
        /// </summary>
        Rtl,

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

    /// <summary>
    /// Specifies the style of text decoration lines.
    /// </summary>
    public enum TextDecorationStyle : byte
    {
        /// <summary>
        /// A solid text decoration line.
        /// </summary>
        Solid,

        /// <summary>
        /// A double text decoration line.
        /// </summary>
        Double,

        /// <summary>
        /// A dotted text decoration line.
        /// </summary>
        Dotted,

        /// <summary>
        /// A dashed text decoration line.
        /// </summary>
        Dashed,

        /// <summary>
        /// A wavy text decoration line.
        /// </summary>
        Wavy,

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

    /// <summary>
    /// Specifies the position of text emphasis marks.
    /// </summary>
    public enum TextEmphasisPosition : byte
    {
        /// <summary>
        /// Marks are drawn over the text.
        /// </summary>
        Over,

        /// <summary>
        /// Marks are drawn under the text.
        /// </summary>
        Under,

        /// <summary>
        /// Combination of over and a side (right).
        /// </summary>
        OverRight,

        /// <summary>
        /// Combination of under and a side (right).
        /// </summary>
        UnderRight,

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