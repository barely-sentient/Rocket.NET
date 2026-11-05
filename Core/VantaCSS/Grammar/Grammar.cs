using System;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
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

        // Default CSS declaration rule
        public static IParser Declaration = Sequence(
            RuleName().As("Property"),
            Token(TokenKind.Colon),
            AnyUntil(TokenKind.Semicolon).As("Value")
        ).Builder((string property, string value) => new CssDeclaration(property, value));

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

        public static List<CssDeclaration> ParseDeclarations(string css)
        {
            var tokens = Tokenise(css);
            var declarations = new List<CssDeclaration>();
            int i = 0;
            while (i < tokens.Length)
            {
                // skip trivia
                while (i < tokens.Length && (tokens[i].Kind == TokenKind.Whitespace || tokens[i].Kind == TokenKind.Comment || tokens[i].Kind == TokenKind.Semicolon))
                {
                    i++;
                }
                if (i >= tokens.Length || tokens[i].Kind == TokenKind.EndOfFile)
                {
                    break;
                }
                if (tokens[i].Kind == TokenKind.CloseBrace)
                {
                    i++;
                    continue;
                }
                var result = Declaration.TryParse(tokens, i, css);
                if (result.Success)
                {
                    if (result.Value is CssDeclaration decl)
                    {
                        declarations.Add(decl);
                    }
                    i += result.Length;
                    // consume trailing semicolon if present
                    if (i < tokens.Length && tokens[i].Kind == TokenKind.Semicolon)
                    {
                        i++;
                    }
                }
                else
                {
                    i++;
                }
            }
            return declarations;
        }
    }

    /// <summary>
    /// A simple parsed CSS declaration.
    /// </summary>
    public class CssDeclaration
    {
        public string Property { get; set; }
        public string Value { get; set; }

        public CssDeclaration() { }

        public CssDeclaration(string property, string value)
        {
            Property = property;
            Value = value;
        }
    }
}