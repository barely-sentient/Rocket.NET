using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Makes a parser optional.
    /// </summary>
    public class OptionalParser : IParser
    {
        /// <summary>
        /// The wrapped parser.
        /// </summary>
        public IParser Inner { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="OptionalParser"/> class.
        /// </summary>
        /// <param name="inner">The wrapped parser.</param>
        public OptionalParser(IParser inner)
        {
            Inner = inner;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames => Inner.CaptureNames;

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            var result = Inner.TryParse(tokens, position, source);
            if (result.Success)
            {
                return result;
            }

            return new ParseResult
            {
                Success = true,
                Position = position,
                Length = 0,
                Value = null,
                Captures = new Dictionary<string, object>(),
                Source = source
            };
        }
    }
}