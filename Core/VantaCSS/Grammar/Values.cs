using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// One transition from a <c>transition</c> shorthand. Unset parts are null.
    /// </summary>
    public sealed class TransitionSpec
    {
        public string Property { get; set; }
        public string Duration { get; set; }
        public string Delay { get; set; }
        public string Timing { get; set; }
    }

    /// <summary>
    /// One shadow from a <c>text-shadow</c> value.
    /// </summary>
    public sealed class TextShadowSpec
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Blur { get; set; }
        public string Color { get; set; }
    }

    /// <summary>
    /// A <c>var(--name, fallback)</c> reference.
    /// </summary>
    public sealed class VarReference
    {
        public string Name { get; set; }
        public string Fallback { get; set; }
    }

    public static partial class Grammar
    {
        /// <summary>
        /// Parses a margin, padding or border-width value into one to four length slots.
        /// Returns null when the value is not one to four lengths or <c>auto</c>.
        /// </summary>
        /// <param name="value">The declaration value.</param>
        /// <returns>The slot texts in top, right, bottom, left order, or null.</returns>
        public static string[] ParseEdgeSlots(string value)
        {
            return ParseWhole(EdgeBoxParser(), ValueTokens(value), value) as string[];
        }

        /// <summary>
        /// Parses a border shorthand and returns its length components (for example <c>1px</c>).
        /// Colours and style keywords are accepted and ignored.
        /// </summary>
        /// <param name="value">The declaration value.</param>
        /// <returns>The length texts, in source order.</returns>
        public static string[] ParseBorderLengths(string value)
        {
            return ParseWhole(BorderParser(), ValueTokens(value), value) as string[] ?? new string[0];
        }

        /// <summary>
        /// Parses a <c>transition</c> shorthand into its comma-separated transitions.
        /// Returns an empty list when the value is invalid.
        /// </summary>
        /// <param name="value">The declaration value.</param>
        /// <returns>The transitions in source order.</returns>
        public static List<TransitionSpec> ParseTransitionList(string value)
        {
            var items = ParseWhole(TransitionListParser(), ValueTokens(value), value) as object[];
            return items == null ? new List<TransitionSpec>() : items.OfType<TransitionSpec>().ToList();
        }

        /// <summary>
        /// Parses a <c>text-shadow</c> list. Returns null when the value is invalid.
        /// The value <c>none</c> is not a shadow list and is handled by the caller.
        /// </summary>
        /// <param name="value">The declaration value.</param>
        /// <returns>The shadows in source order, or null.</returns>
        public static List<TextShadowSpec> ParseTextShadows(string value)
        {
            var items = ParseWhole(TextShadowListParser(), ValueTokens(value), value) as object[];
            if (items == null) return null;
            return items.OfType<TextShadowSpec>().ToList();
        }

        /// <summary>
        /// Matches a <c>var(...)</c> reference that starts at <paramref name="position"/>.
        /// </summary>
        /// <param name="tokens">The tokens of the value, including whitespace.</param>
        /// <param name="position">The index of the <c>var</c> function token.</param>
        /// <param name="source">The source text of the value.</param>
        /// <param name="consumed">The number of tokens the reference spans.</param>
        /// <returns>The reference, or null when there is none at that position.</returns>
        public static VarReference MatchVarReference(Token[] tokens, int position, string source, out int consumed)
        {
            var result = VarReferenceParser().TryParse(tokens, position, source);
            if (!result.Success)
            {
                consumed = 0;
                return null;
            }
            consumed = result.Length;
            return result.Value as VarReference;
        }

        /// <summary>
        /// Returns the first <c>url(...)</c> in a font <c>src</c> value.
        /// </summary>
        /// <param name="src">The <c>src</c> descriptor value.</param>
        /// <returns>The URL without quotes, or null when there is none.</returns>
        public static string ParseFontUrl(string src)
        {
            if (string.IsNullOrWhiteSpace(src)) return null;
            var result = Parse(FontUrlParser(), src);
            return result.Success ? result.Value as string : null;
        }

        private static IParser LengthOrAutoParser() => Or(LengthTokenParser(), Word("auto"));

        private static IParser EdgeBoxParser() =>
            Sequence(
                LengthOrAutoParser().As("top"),
                Optional(LengthOrAutoParser()).As("right"),
                Optional(LengthOrAutoParser()).As("bottom"),
                Optional(LengthOrAutoParser()).As("left"))
            .Builder((Token top, Token right, Token bottom, Token left) =>
                new[] { top, right, bottom, left }.Where(t => t != null).Select(t => t.Text).ToArray());

        private static IParser BorderParser() =>
            ZeroOrMore(BorderComponentParser()).As("components")
                .Builder((object[] components) => components.OfType<Token>()
                    .Where(t => IsLengthKind(t.Kind))
                    .Select(t => t.Text)
                    .ToArray());

        private static IParser BorderComponentParser() =>
            Or(
                LengthTokenParser(),
                Sequence(
                        new TokenMatchParser("function", t => t.Kind == TokenKind.Function).As("function"),
                        Token(TokenKind.OpenParen),
                        BalancedUntil(TokenKind.CloseParen).As("args"),
                        Token(TokenKind.CloseParen))
                    .Builder((Token function) => function),
                new TokenMatchParser("keyword or colour", t => true));

        private static IParser TransitionListParser() =>
            ZeroOrMore(
                Sequence(TransitionSpecParser().As("spec"), Optional(Token(TokenKind.Comma)))
                    .Builder((TransitionSpec spec) => spec));

        private static IParser TransitionSpecParser() =>
            ZeroOrMore(TransitionPartParser()).As("parts")
                .Builder((object[] parts) => BuildTransitionSpec(parts));

        private static TransitionSpec BuildTransitionSpec(object[] parts)
        {
            var spec = new TransitionSpec();
            foreach (var part in parts)
            {
                if (part is Action<TransitionSpec> apply) apply(spec);
            }
            return spec;
        }

        private static IParser TransitionPartParser() =>
            Or(TransitionTimeParser(), TransitionTimingParser(), TransitionPropertyParser());

        private static IParser TransitionTimeParser() =>
            new TokenMatchParser("time", t =>
                    t.Kind == TokenKind.SecondUnit || t.Kind == TokenKind.MillisecondUnit ||
                    t.Kind == TokenKind.Integer || t.Kind == TokenKind.Number)
                .As("time")
                .Builder((Token time) => (Action<TransitionSpec>)(spec =>
                {
                    if (spec.Duration == null) spec.Duration = time.Text;
                    else if (spec.Delay == null) spec.Delay = time.Text;
                }));

        private static IParser TransitionTimingParser() =>
            Or(
                new TokenMatchParser("timing keyword", t => IsIdentKind(t.Kind) && IsTimingKeyword(t.Text)).As("keyword")
                    .Builder((Token keyword) => (Action<TransitionSpec>)(spec => spec.Timing = keyword.Text)),
                Sequence(
                        new TokenMatchParser("timing function", t => t.Kind == TokenKind.Function && IsTimingFunction(t.Text)).As("function"),
                        Token(TokenKind.OpenParen),
                        BalancedUntil(TokenKind.CloseParen).As("args"),
                        Token(TokenKind.CloseParen))
                    .Builder((Token function, Token[] args) =>
                        (Action<TransitionSpec>)(spec => spec.Timing = function.Text + "(" + JoinTokens(args) + ")")));

        private static IParser TransitionPropertyParser() =>
            new TokenMatchParser("property name", t => IsIdentKind(t.Kind))
                .As("property")
                .Builder((Token property) => (Action<TransitionSpec>)(spec => spec.Property = property.Text));

        private static bool IsTimingKeyword(string text)
        {
            switch (text.ToLowerInvariant())
            {
                case "linear":
                case "ease":
                case "ease-in":
                case "ease-out":
                case "ease-in-out":
                case "step-start":
                case "step-end":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsTimingFunction(string text)
        {
            return string.Equals(text, "cubic-bezier", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(text, "steps", StringComparison.OrdinalIgnoreCase);
        }

        private static IParser TextShadowListParser() =>
            ZeroOrMore(
                Sequence(TextShadowParser().As("shadow"), Optional(Token(TokenKind.Comma)))
                    .Builder((TextShadowSpec shadow) => shadow));

        private static IParser TextShadowParser() =>
            Sequence(
                Optional(ShadowColorParser()).As("before"),
                LengthTokenParser().As("x"),
                LengthTokenParser().As("y"),
                Optional(LengthTokenParser()).As("blur"),
                Optional(ShadowColorParser()).As("after"))
            .Builder((string before, Token x, Token y, Token blur, string after) => new TextShadowSpec
            {
                X = x.Text,
                Y = y.Text,
                Blur = blur == null ? "0" : blur.Text,
                Color = before ?? after
            });

        private static IParser ShadowColorParser() =>
            Or(
                new TokenMatchParser("colour", t => IsIdentKind(t.Kind) || t.Kind == TokenKind.HexColor)
                    .As("colour")
                    .Builder((Token colour) => colour.Text),
                Sequence(
                        new TokenMatchParser("colour function", t => t.Kind == TokenKind.Function).As("function"),
                        Token(TokenKind.OpenParen),
                        BalancedUntil(TokenKind.CloseParen).As("args"),
                        Token(TokenKind.CloseParen))
                    .Builder((Token function, Token[] args) => function.Text + "(" + JoinTokens(args) + ")"));

        private static IParser VarReferenceParser() =>
            Sequence(
                new TokenMatchParser("var()", t => t.Kind == TokenKind.Function && string.Equals(t.Text, "var", StringComparison.OrdinalIgnoreCase)).As("function"),
                Token(TokenKind.OpenParen),
                Trivia(),
                new TokenMatchParser("custom property name", t => t.Kind == TokenKind.VariableName).As("name"),
                Trivia(),
                Optional(Sequence(Token(TokenKind.Comma), BalancedUntil(TokenKind.CloseParen).As("fallback"))),
                Token(TokenKind.CloseParen))
            .Builder((Token function, Token name, Token[] fallback) => new VarReference
            {
                Name = name.Text,
                Fallback = fallback == null ? null : JoinTokens(fallback).Trim()
            });

        private static IParser FontUrlParser() =>
            Sequence(
                ZeroOrMore(new TokenMatchParser("non-url token", t => t.Kind != TokenKind.Url)),
                new TokenMatchParser("url()", t => t.Kind == TokenKind.Url).As("url"))
            .Builder((Token url) => ExtractUrl(url.Text));

        private static string ExtractUrl(string text)
        {
            int open = text.IndexOf('(');
            int close = text.LastIndexOf(')');
            if (open < 0 || close <= open) return string.Empty;
            return text.Substring(open + 1, close - open - 1).Trim().Trim('"', '\'').Trim();
        }
    }
}
