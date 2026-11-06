using System;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    public static partial class Grammar
    {
        private const int MaxCacheEntries = 4096;

        private static IParser Trivia() => ZeroOrMore(TriviaToken());

        private static IParser TriviaToken() => new TokenMatchParser("whitespace or comment", IsTrivia);

        private static IParser BalancedUntil(TokenKind terminator) => new BalancedUntilParser(terminator);

        private static bool IsIdentKind(TokenKind kind) =>
            kind == TokenKind.TagName || kind == TokenKind.RuleName || kind == TokenKind.Keyword || kind == TokenKind.Color;

        // A keyword followed by '(' (as in "and (min-width: ...)") tokenises as a function, so accept both.
        private static IParser Word(params string[] words) =>
            new TokenMatchParser(string.Join(" or ", words), t =>
                (IsIdentKind(t.Kind) || t.Kind == TokenKind.Function) &&
                words.Any(w => string.Equals(w, t.Text, StringComparison.OrdinalIgnoreCase)));

        private static bool IsLengthKind(TokenKind kind) =>
            kind == TokenKind.Integer || kind == TokenKind.Number || kind == TokenKind.PixelUnit ||
            kind == TokenKind.PercentUnit || kind == TokenKind.EmUnit || kind == TokenKind.RemUnit ||
            kind == TokenKind.ViewportWidthUnit || kind == TokenKind.ViewportHeightUnit;

        private static IParser LengthTokenParser() => new TokenMatchParser("length", t => IsLengthKind(t.Kind));

        private static Token[] ValueTokens(string text) =>
            Tokenise(text ?? string.Empty)
                .Where(t => !IsTrivia(t) && t.Kind != TokenKind.Important && t.Kind != TokenKind.EndOfFile)
                .ToArray();

        private static object ParseWhole(IParser parser, Token[] tokens, string source)
        {
            var result = parser.TryParse(tokens, 0, source);
            return result.Success && result.Length == tokens.Length ? result.Value : null;
        }

        private static string JoinTokens(IEnumerable<Token> tokens) => string.Concat(tokens.Select(t => t.Text));
    }
}
