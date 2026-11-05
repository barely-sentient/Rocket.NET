using System;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// A parser that matches a sequence of parsers in order.
    /// </summary>
    public class SequenceParser : IParser
    {
        /// <summary>
        /// The parsers that make up this sequence.
        /// </summary>
        public IParser[] Parsers { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="SequenceParser"/> class.
        /// </summary>
        /// <param name="parsers">The parsers to run in sequence.</param>
        public SequenceParser(params IParser[] parsers)
        {
            Parsers = parsers ?? Array.Empty<IParser>();
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames =>
            Parsers.SelectMany(p => p.CaptureNames).ToList();

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            int pos = position;
            var captures = new Dictionary<string, object>();
            foreach (var parser in Parsers)
            {
                var result = parser.TryParse(tokens, pos, source);
                if (!result.Success)
                {
                    return new ParseResult
                    {
                        Success = false,
                        Position = position,
                        Length = 0,
                        Error = result.Error,
                        Source = source
                    };
                }

                pos += result.Length;
                if (result.Captures != null)
                {
                    foreach (var kvp in result.Captures)
                    {
                        captures[kvp.Key] = kvp.Value;
                    }
                }
            }

            return new ParseResult
            {
                Success = true,
                Position = position,
                Length = pos - position,
                Value = captures,
                Captures = captures,
                Source = source
            };
        }
    }
}