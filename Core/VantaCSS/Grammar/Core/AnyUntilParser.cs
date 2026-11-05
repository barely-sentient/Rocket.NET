using System;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// A parser that consumes tokens until a terminator is encountered.
    /// </summary>
    public class AnyUntilParser : IParser
    {
        /// <summary>
        /// The terminator (a <see cref="TokenKind"/> or another <see cref="IParser"/>).
        /// </summary>
        public object Terminator { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="AnyUntilParser"/> class.
        /// </summary>
        /// <param name="terminator">The terminator.</param>
        public AnyUntilParser(object terminator)
        {
            Terminator = terminator;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames => Array.Empty<string>();

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            int pos = position;
            var collected = new List<Token>();
            while (pos < tokens.Length)
            {
                bool isTerminator = false;
                if (Terminator is TokenKind kind)
                {
                    if (tokens[pos].Kind == kind)
                    {
                        isTerminator = true;
                    }
                }
                else if (Terminator is IParser parser)
                {
                    var tr = parser.TryParse(tokens, pos, source);
                    if (tr.Success)
                    {
                        isTerminator = true;
                    }
                }

                if (isTerminator)
                {
                    break;
                }

                collected.Add(tokens[pos]);
                pos++;
            }

            return new ParseResult
            {
                Success = true,
                Position = position,
                Length = pos - position,
                Value = collected.ToArray(),
                Captures = new Dictionary<string, object>(),
                Source = source
            };
        }
    }
}