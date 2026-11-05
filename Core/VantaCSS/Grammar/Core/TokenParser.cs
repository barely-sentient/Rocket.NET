using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// A parser that matches a single token of a specific <see cref="TokenKind"/>.
    /// </summary>
    public class TokenParser : IParser
    {
        /// <summary>
        /// The token kind this parser matches.
        /// </summary>
        public TokenKind Kind { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="TokenParser"/> class.
        /// </summary>
        /// <param name="kind">The token kind to match.</param>
        public TokenParser(TokenKind kind)
        {
            Kind = kind;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames => Array.Empty<string>();

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            if (position >= 0 && position < tokens.Length && tokens[position].Kind == Kind)
            {
                return new ParseResult
                {
                    Success = true,
                    Position = position,
                    Length = 1,
                    Value = tokens[position],
                    Source = source
                };
            }

            return new ParseResult
            {
                Success = false,
                Position = position,
                Length = 0,
                Error = $"Expected token {Kind}.",
                Source = source
            };
        }
    }
}