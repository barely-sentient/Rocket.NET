using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// A parser that matches a single token accepted by a predicate.
    /// </summary>
    public class TokenMatchParser : IParser
    {
        /// <summary>
        /// Describes the expected token in error messages.
        /// </summary>
        public string Expected { get; }

        /// <summary>
        /// The predicate a token must satisfy.
        /// </summary>
        public Func<Token, bool> Predicate { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="TokenMatchParser"/> class.
        /// </summary>
        /// <param name="expected">A description of the expected token.</param>
        /// <param name="predicate">The predicate a token must satisfy.</param>
        public TokenMatchParser(string expected, Func<Token, bool> predicate)
        {
            Expected = expected;
            Predicate = predicate;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames => Array.Empty<string>();

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            if (position >= 0 && position < tokens.Length && Predicate(tokens[position]))
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
                Error = $"Expected {Expected}.",
                Source = source
            };
        }
    }
}
