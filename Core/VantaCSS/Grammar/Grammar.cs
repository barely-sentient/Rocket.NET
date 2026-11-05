using System;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Provides CSS grammar rules and parsing entry points.
    /// </summary>
    public static partial class Grammar
    {
        // Factories
        public static IParser Token(TokenKind kind) => new TokenParser(kind);

        public static IParser RuleName() => new TokenParser(TokenKind.RuleName);

        public static IParser ClassName() => new TokenParser(TokenKind.ClassName);

        public static IParser TagName() => new TokenParser(TokenKind.TagName);

        public static IParser Identifier() => new TokenParser(TokenKind.Identifier);

        public static IParser AtKeyword() => new TokenParser(TokenKind.AtKeyword);

        public static IParser VariableName() => new TokenParser(TokenKind.VariableName);

        public static IParser AnyUntil(TokenKind kind) => new AnyUntilParser(kind);

        public static IParser AnyUntil(IParser parser) => new AnyUntilParser(parser);

        public static IParser Sequence(params IParser[] parsers) => new SequenceParser(parsers);

        public static IParser Optional(IParser parser) => new OptionalParser(parser);

        public static IParser ZeroOrMore(IParser parser) => new ZeroOrMoreParser(parser);

        public static IParser Or(params IParser[] parsers) => new AlternativeParser(parsers);

        public static IParser Alternative(params IParser[] parsers) => new AlternativeParser(parsers);

        // Property name can be a normal rule name or a CSS variable (--foo)
        private static IParser PropertyName() => new AlternativeParser(new TokenParser(TokenKind.RuleName), new TokenParser(TokenKind.VariableName));

        // Default CSS declaration rule
        public static IParser Declaration = Sequence(
            PropertyName().As("Property"),
            Token(TokenKind.Colon),
            AnyUntil(TokenKind.Semicolon).As("Value")
        ).Builder((string property, string value) => new CssDeclaration(property.Trim(), value.Trim()));

        // Parsing entry points
        public static Token[] Tokenise(string css)
        {
            return Tokeniser.Tokeniser.Parse(css).ToArray();
        }

        public static ParseResult Parse(IParser parser, string css)
        {
            var tokens = Tokenise(css);
            return parser.TryParse(tokens, 0, css);
        }

        public static ParseResult Parse(IParser parser, Token[] tokens, string source = null)
        {
            return parser.TryParse(tokens, 0, source ?? string.Empty);
        }

        /// <summary>
        /// Parses a list of CSS declarations.
        /// </summary>
        /// <param name="css">The CSS source.</param>
        /// <returns>A list of parsed declarations.</returns>
        public static List<CssDeclaration> ParseDeclarations(string css)
        {
            var tokens = Tokenise(css);
            return ParseDeclarations(tokens, css);
        }

        public static List<CssDeclaration> ParseDeclarations(Token[] tokens, string source)
        {
            var declarations = new List<CssDeclaration>();
            int i = 0;
            int n = tokens.Length;
            while (i < n)
            {
                while (i < n && IsTrivia(tokens[i])) i++;
                if (i >= n || tokens[i].Kind == TokenKind.EndOfFile) break;
                if (tokens[i].Kind == TokenKind.CloseBrace) { i++; continue; }
                var result = Declaration.TryParse(tokens, i, source);
                if (result.Success)
                {
                    if (result.Value is CssDeclaration decl) declarations.Add(decl);
                    i += result.Length;
                    if (i < n && tokens[i].Kind == TokenKind.Semicolon) i++;
                }
                else
                {
                    i++;
                }
            }
            return declarations;
        }

        /// <summary>
        /// Parses a CSS stylesheet into a list of rules.
        /// </summary>
        /// <param name="css">The CSS source.</param>
        /// <returns>A list of parsed rules.</returns>
        public static List<CssRule> ParseRules(string css)
        {
            var tokens = Tokenise(css);
            return ParseRules(tokens, css);
        }

        public static List<CssRule> ParseRules(Token[] tokens, string source)
        {
            var rules = new List<CssRule>();
            int i = 0;
            int n = tokens.Length;
            while (i < n)
            {
                while (i < n && IsTrivia(tokens[i])) i++;
                if (i >= n || tokens[i].Kind == TokenKind.EndOfFile) break;
                if (tokens[i].Kind == TokenKind.CloseBrace) { i++; continue; }

                // At-rule (@media, @import, ...)
                if (tokens[i].Kind == TokenKind.AtKeyword)
                {
                    int headerStart = i;
                    while (i < n && tokens[i].Kind != TokenKind.OpenBrace &&
                           tokens[i].Kind != TokenKind.Semicolon &&
                           tokens[i].Kind != TokenKind.EndOfFile)
                    {
                        i++;
                    }
                    string header = Slice(source, tokens, headerStart, i).Trim();

                    if (i < n && tokens[i].Kind == TokenKind.Semicolon)
                    {
                        i++;
                        rules.Add(new CssRule { Selector = header, IsAtRule = true });
                        continue;
                    }

                    if (i >= n || tokens[i].Kind != TokenKind.OpenBrace) break;

                    int depth = 0;
                    int j = i;
                    for (; j < n; j++)
                    {
                        if (tokens[j].Kind == TokenKind.OpenBrace) depth++;
                        else if (tokens[j].Kind == TokenKind.CloseBrace) { depth--; if (depth == 0) break; }
                    }
                    var innerTokens = tokens.Skip(i + 1).Take(Math.Max(0, j - i - 1)).ToArray();
                    var innerRules = ParseRules(innerTokens, source);
                    string media = null;
                    if (header.StartsWith("@media", StringComparison.OrdinalIgnoreCase))
                    {
                        media = header.Substring(6).Trim();
                    }
                    rules.Add(new CssRule
                    {
                        Selector = header,
                        IsAtRule = true,
                        Media = media,
                        Rules = innerRules
                    });
                    i = (j < n) ? j + 1 : n;
                    continue;
                }

                // Normal rule: selector until {
                int selStart = i;
                while (i < n && tokens[i].Kind != TokenKind.OpenBrace && tokens[i].Kind != TokenKind.EndOfFile)
                {
                    i++;
                }
                if (i >= n || tokens[i].Kind != TokenKind.OpenBrace) break;
                string selector = Slice(source, tokens, selStart, i).Trim();

                int depth2 = 0;
                int k = i;
                for (; k < n; k++)
                {
                    if (tokens[k].Kind == TokenKind.OpenBrace) depth2++;
                    else if (tokens[k].Kind == TokenKind.CloseBrace) { depth2--; if (depth2 == 0) break; }
                }
                var declTokens = tokens.Skip(i + 1).Take(Math.Max(0, k - i - 1)).ToArray();
                var declarations = ParseDeclarations(declTokens, source);
                rules.Add(new CssRule { Selector = selector, Declarations = declarations });
                i = (k < n) ? k + 1 : n;
            }
            return rules;
        }

        private static bool IsTrivia(Token token)
        {
            return token.Kind == TokenKind.Whitespace || token.Kind == TokenKind.Comment;
        }

        private static string Slice(string source, Token[] tokens, int start, int end)
        {
            if (start >= end || start >= tokens.Length) return string.Empty;
            int first = (int)tokens[start].StartIndex;
            int last = (int)tokens[end - 1].StartIndex + (int)tokens[end - 1].Length;
            if (first < 0) first = 0;
            if (last > source.Length) last = source.Length;
            if (last <= first) return string.Empty;
            return source.Substring(first, last - first);
        }
    }
}