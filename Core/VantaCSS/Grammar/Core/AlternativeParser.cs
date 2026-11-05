using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Tries a list of parsers in order and returns the first that succeeds.
    /// </summary>
    public class AlternativeParser : IParser
    {
        /// <summary>
        /// The candidate parsers.
        /// </summary>
        public IParser[] Parsers { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="AlternativeParser"/> class.
        /// </summary>
        /// <param name="parsers">The candidate parsers.</param>
        public AlternativeParser(params IParser[] parsers)
        {
            Parsers = parsers ?? Array.Empty<IParser>();
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames =>
            Parsers.Length > 0 ? Parsers[0].CaptureNames : Array.Empty<string>();

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            foreach (var parser in Parsers)
            {
                var result = parser.TryParse(tokens, position, source);
                if (result.Success)
                {
                    return result;
                }
            }

            return new ParseResult
            {
                Success = false,
                Position = position,
                Length = 0,
                Error = "No alternative matched.",
                Source = source
            };
        }
    }
}