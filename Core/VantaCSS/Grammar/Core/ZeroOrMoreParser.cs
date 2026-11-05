using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Matches zero or more repetitions of a parser.
    /// </summary>
    public class ZeroOrMoreParser : IParser
    {
        /// <summary>
        /// The wrapped parser.
        /// </summary>
        public IParser Inner { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ZeroOrMoreParser"/> class.
        /// </summary>
        /// <param name="inner">The wrapped parser.</param>
        public ZeroOrMoreParser(IParser inner)
        {
            Inner = inner;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames => Array.Empty<string>();

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            int pos = position;
            var collected = new List<object>();
            var captures = new Dictionary<string, object>();
            while (pos < tokens.Length)
            {
                var result = Inner.TryParse(tokens, pos, source);
                if (!result.Success || result.Length == 0)
                {
                    break;
                }

                collected.Add(result.Value);
                pos += result.Length;
                if (result.Captures != null)
                {
                    foreach (var kvp in result.Captures)
                    {
                        if (!captures.ContainsKey(kvp.Key))
                        {
                            captures[kvp.Key] = new List<object>();
                        }
                        ((List<object>)captures[kvp.Key]).Add(kvp.Value);
                    }
                }
            }

            return new ParseResult
            {
                Success = true,
                Position = position,
                Length = pos - position,
                Value = collected.ToArray(),
                Captures = captures,
                Source = source
            };
        }
    }
}