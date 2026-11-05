namespace VantaCSS.Tokeniser
{
    public enum TokenKind
    {
        /// <summary>
        /// The current token is a class name
        /// </summary>
        /// <remarks>
        /// [.user-card] p
        /// </remarks>
        ClassName,

        /// <summary>
        /// The current token is an ID
        /// </summary>
        /// <remarks>
        /// [#user] p
        /// </remarks>
        Identifier,

        /// <summary>
        /// The current token is a tag name
        /// </summary>
        /// <remarks>
        /// .user-card [p]
        /// </remarks>
        TagName,

        /// <summary>
        /// The current token is a double colon.
        /// </summary>
        /// <remarks>
        /// Not much I plan on supporting at the moment
        /// </remarks>
        DoubleColon,

        /// <summary>
        /// The current token is a rule name, color, border-radius etc...
        /// </summary>
        RuleName,

        /// <summary>
        /// The current token is a single colon, used for pseudo classes etc...
        /// </summary>
        Colon,

        /// <summary>
        /// The current token is an opening brace.
        /// </summary>
        OpenBrace,

        /// <summary>
        /// The current token is a closing brace.
        /// </summary>
        CloseBrace,

        /// <summary>
        /// The current token is a variable (starts with --)
        /// </summary>
        VariableName,

        /// <summary>
        /// The current token is {number}px
        /// </summary>
        PixelUnit,

        /// <summary>
        /// The current token is {number}%
        /// </summary>
        PercentUnit,

        /// <summary>
        /// The current token is {number}em
        /// </summary>
        EmUnit,

        /// <summary>
        /// The current token is {number}rem
        /// </summary>
        RemUnit,

        /// <summary>
        /// The current token is {number}vw
        /// </summary>
        ViewportWidthUnit,

        /// <summary>
        /// The current token is {number}vh
        /// </summary>
        ViewportHeightUnit,

        /// <summary>
        /// The current token is a comma, used to separate selectors or values.
        /// </summary>
        Comma,

        /// <summary>
        /// The current token is a semicolon, used to terminate declarations.
        /// </summary>
        Semicolon,

        /// <summary>
        /// The current token is an opening parenthesis.
        /// </summary>
        OpenParen,

        /// <summary>
        /// Alias for <see cref="OpenParen"/>.
        /// </summary>
        OpenParenthesis,

        /// <summary>
        /// Alias for <see cref="OpenParen"/>.
        /// </summary>
        LeftParen,

        /// <summary>
        /// The current token is a closing parenthesis.
        /// </summary>
        CloseParen,

        /// <summary>
        /// Alias for <see cref="CloseParen"/>.
        /// </summary>
        CloseParenthesis,

        /// <summary>
        /// Alias for <see cref="CloseParen"/>.
        /// </summary>
        RightParen,

        /// <summary>
        /// The current token is an opening bracket, used in attribute selectors.
        /// </summary>
        OpenBracket,

        /// <summary>
        /// Alias for <see cref="OpenBracket"/>.
        /// </summary>
        OpenSquareBracket,

        /// <summary>
        /// Alias for <see cref="OpenBracket"/>.
        /// </summary>
        LeftBracket,

        /// <summary>
        /// The current token is a closing bracket, used in attribute selectors.
        /// </summary>
        CloseBracket,

        /// <summary>
        /// Alias for <see cref="CloseBracket"/>.
        /// </summary>
        CloseSquareBracket,

        /// <summary>
        /// Alias for <see cref="CloseBracket"/>.
        /// </summary>
        RightBracket,

        /// <summary>
        /// The current token is the universal selector (*).
        /// </summary>
        UniversalSelector,

        /// <summary>
        /// The current token is an asterisk.
        /// </summary>
        Asterisk,

        /// <summary>
        /// The current token is a greater-than combinator (&gt;).
        /// </summary>
        GreaterThan,

        /// <summary>
        /// The current token is a plus combinator (+).
        /// </summary>
        Plus,

        /// <summary>
        /// The current token is a tilde combinator (~).
        /// </summary>
        Tilde,

        /// <summary>
        /// The current token is an ampersand, used for nesting.
        /// </summary>
        Ampersand,

        /// <summary>
        /// The current token is a hash (#), used for IDs or hex colors.
        /// </summary>
        Hash,

        /// <summary>
        /// The current token is a hex color value.
        /// </summary>
        HexColor,

        /// <summary>
        /// The current token is a color value or keyword.
        /// </summary>
        Color,

        /// <summary>
        /// The current token is a numeric value.
        /// </summary>
        Number,

        /// <summary>
        /// The current token is an integer value.
        /// </summary>
        Integer,

        /// <summary>
        /// The current token is a dimension value with a unit.
        /// </summary>
        Dimension,

        /// <summary>
        /// The current token is a quoted string.
        /// </summary>
        String,

        /// <summary>
        /// Alias for <see cref="String"/>.
        /// </summary>
        StringLiteral,

        /// <summary>
        /// The current token is a url(...) value.
        /// </summary>
        Url,

        /// <summary>
        /// The current token is a function call.
        /// </summary>
        Function,

        /// <summary>
        /// The current token is an !important declaration.
        /// </summary>
        Important,

        /// <summary>
        /// The current token is a CSS keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// The current token is an at-keyword (@media, @import, ...).
        /// </summary>
        AtKeyword,

        /// <summary>
        /// Alias for <see cref="AtKeyword"/>.
        /// </summary>
        AtRule,

        /// <summary>
        /// The current token is whitespace.
        /// </summary>
        Whitespace,

        /// <summary>
        /// The current token is a comment.
        /// </summary>
        Comment,

        /// <summary>
        /// The current token marks the end of the input.
        /// </summary>
        EndOfFile,

        /// <summary>
        /// Alias for <see cref="EndOfFile"/>.
        /// </summary>
        EOF,

        /// <summary>
        /// The current token is unknown or invalid.
        /// </summary>
        Unknown,

        /// <summary>
        /// Alias for <see cref="Unknown"/>.
        /// </summary>
        Invalid,

        /// <summary>
        /// The current token is a delimiter.
        /// </summary>
        Delimiter,

        /// <summary>
        /// Alias for <see cref="Delimiter"/>.
        /// </summary>
        Delim,

        /// <summary>
        /// The current token is a CSS CDC (--&gt;) marker.
        /// </summary>
        Cdc,

        /// <summary>
        /// The current token is a CSS CDO (&lt;!--) marker.
        /// </summary>
        Cdo,

        /// <summary>
        /// The current token is {number}pt
        /// </summary>
        PointUnit,

        /// <summary>
        /// Alias for <see cref="PointUnit"/>.
        /// </summary>
        PtUnit,

        /// <summary>
        /// The current token is {number}pc
        /// </summary>
        PicaUnit,

        /// <summary>
        /// Alias for <see cref="PicaUnit"/>.
        /// </summary>
        PcUnit,

        /// <summary>
        /// The current token is {number}in
        /// </summary>
        InchUnit,

        /// <summary>
        /// Alias for <see cref="InchUnit"/>.
        /// </summary>
        InUnit,

        /// <summary>
        /// The current token is {number}cm
        /// </summary>
        CentimeterUnit,

        /// <summary>
        /// Alias for <see cref="CentimeterUnit"/>.
        /// </summary>
        CmUnit,

        /// <summary>
        /// The current token is {number}mm
        /// </summary>
        MillimeterUnit,

        /// <summary>
        /// Alias for <see cref="MillimeterUnit"/>.
        /// </summary>
        MmUnit,

        /// <summary>
        /// The current token is {number}Q
        /// </summary>
        QuarterMillimeterUnit,

        /// <summary>
        /// Alias for <see cref="QuarterMillimeterUnit"/>.
        /// </summary>
        QUnit,

        /// <summary>
        /// The current token is {number}ex
        /// </summary>
        ExUnit,

        /// <summary>
        /// The current token is {number}ch
        /// </summary>
        ChUnit,

        /// <summary>
        /// The current token is {number}vmin
        /// </summary>
        ViewportMinUnit,

        /// <summary>
        /// Alias for <see cref="ViewportMinUnit"/>.
        /// </summary>
        VminUnit,

        /// <summary>
        /// The current token is {number}vmax
        /// </summary>
        ViewportMaxUnit,

        /// <summary>
        /// Alias for <see cref="ViewportMaxUnit"/>.
        /// </summary>
        VmaxUnit,

        /// <summary>
        /// The current token is {number}fr
        /// </summary>
        FractionUnit,

        /// <summary>
        /// Alias for <see cref="FractionUnit"/>.
        /// </summary>
        FrUnit,

        /// <summary>
        /// The current token is {number}deg
        /// </summary>
        DegreeUnit,

        /// <summary>
        /// Alias for <see cref="DegreeUnit"/>.
        /// </summary>
        DegUnit,

        /// <summary>
        /// The current token is {number}rad
        /// </summary>
        RadianUnit,

        /// <summary>
        /// Alias for <see cref="RadianUnit"/>.
        /// </summary>
        RadUnit,

        /// <summary>
        /// The current token is {number}grad
        /// </summary>
        GradianUnit,

        /// <summary>
        /// Alias for <see cref="GradianUnit"/>.
        /// </summary>
        GradUnit,

        /// <summary>
        /// The current token is {number}turn
        /// </summary>
        TurnUnit,

        /// <summary>
        /// The current token is {number}s
        /// </summary>
        SecondUnit,

        /// <summary>
        /// Alias for <see cref="SecondUnit"/>.
        /// </summary>
        SUnit,

        /// <summary>
        /// The current token is {number}ms
        /// </summary>
        MillisecondUnit,

        /// <summary>
        /// Alias for <see cref="MillisecondUnit"/>.
        /// </summary>
        MsUnit,

        /// <summary>
        /// The current token is {number}Hz
        /// </summary>
        HertzUnit,

        /// <summary>
        /// Alias for <see cref="HertzUnit"/>.
        /// </summary>
        HzUnit,

        /// <summary>
        /// The current token is {number}kHz
        /// </summary>
        KilohertzUnit,

        /// <summary>
        /// Alias for <see cref="KilohertzUnit"/>.
        /// </summary>
        KhzUnit,

        /// <summary>
        /// The current token is {number}dpi
        /// </summary>
        DotsPerInchUnit,

        /// <summary>
        /// Alias for <see cref="DotsPerInchUnit"/>.
        /// </summary>
        DpiUnit,

        /// <summary>
        /// The current token is {number}dpcm
        /// </summary>
        DotsPerCentimeterUnit,

        /// <summary>
        /// Alias for <see cref="DotsPerCentimeterUnit"/>.
        /// </summary>
        DpcmUnit,

        /// <summary>
        /// The current token is {number}dppx
        /// </summary>
        DotsPerPixelUnit,

        /// <summary>
        /// Alias for <see cref="DotsPerPixelUnit"/>.
        /// </summary>
        DppxUnit,

        /// <summary>
        /// The current token is {number}cap
        /// </summary>
        CapUnit,

        /// <summary>
        /// The current token is {number}ic
        /// </summary>
        IcUnit,

        /// <summary>
        /// The current token is {number}lh
        /// </summary>
        LhUnit,

        /// <summary>
        /// The current token is {number}rlh
        /// </summary>
        RlhUnit,

        /// <summary>
        /// The current token is {number}vi
        /// </summary>
        ViUnit,

        /// <summary>
        /// The current token is {number}vb
        /// </summary>
        VbUnit,

        /// <summary>
        /// The current token is {number}cqw
        /// </summary>
        ContainerQueryWidthUnit,

        /// <summary>
        /// Alias for <see cref="ContainerQueryWidthUnit"/>.
        /// </summary>
        CqwUnit,

        /// <summary>
        /// The current token is {number}cqh
        /// </summary>
        ContainerQueryHeightUnit,

        /// <summary>
        /// Alias for <see cref="ContainerQueryHeightUnit"/>.
        /// </summary>
        CqhUnit,

        /// <summary>
        /// Alias for <see cref="Identifier"/>.
        /// </summary>
        Ident,

        /// <summary>
        /// Alias for <see cref="AtKeyword"/>.
        /// </summary>
        AtIdent,

        /// <summary>
        /// Alias for <see cref="Function"/>.
        /// </summary>
        FunctionName,

        /// <summary>
        /// Alias for <see cref="Url"/>.
        /// </summary>
        Uri,

        /// <summary>
        /// Alias for <see cref="Number"/> (integer).
        /// </summary>
        Int,

        /// <summary>
        /// A percentage token ({number}%).
        /// </summary>
        Percentage,

        /// <summary>
        /// Alias for <see cref="UniversalSelector"/>.
        /// </summary>
        Universal,

        /// <summary>
        /// Alias for <see cref="Asterisk"/>.
        /// </summary>
        Star,

        /// <summary>
        /// Alias for <see cref="OpenBrace"/>.
        /// </summary>
        OpenCurlyBrace,

        /// <summary>
        /// Alias for <see cref="CloseBrace"/>.
        /// </summary>
        CloseCurlyBrace,

        /// <summary>
        /// Alias for <see cref="OpenParen"/>.
        /// </summary>
        OpenParens,

        /// <summary>
        /// Alias for <see cref="OpenParen"/>.
        /// </summary>
        OpenParentheses,

        /// <summary>
        /// Alias for <see cref="OpenParen"/>.
        /// </summary>
        LeftParenthesis,

        /// <summary>
        /// Alias for <see cref="CloseParen"/>.
        /// </summary>
        CloseParens,

        /// <summary>
        /// Alias for <see cref="CloseParen"/>.
        /// </summary>
        CloseParentheses,

        /// <summary>
        /// Alias for <see cref="CloseParen"/>.
        /// </summary>
        RightParenthesis,

        /// <summary>
        /// Alias for <see cref="HexColor"/> (British spelling).
        /// </summary>
        Colour,

        /// <summary>
        /// Alias for <see cref="HexColor"/>.
        /// </summary>
        ColourHex,

        /// <summary>
        /// Alias for <see cref="HexColor"/>.
        /// </summary>
        Hex,

        /// <summary>
        /// Alias for <see cref="HexColor"/>.
        /// </summary>
        ColorHex
    }
}