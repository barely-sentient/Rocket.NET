using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Consumes tokens up to (not including) a terminator that is not nested inside parentheses.
    /// </summary>
    public class BalancedUntilParser : IParser
    {
        /// <summary>
        /// The terminator kind.
        /// </summary>
        public TokenKind Terminator { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="BalancedUntilParser"/> class.
        /// </summary>
        /// <param name="terminator">The terminator kind, matched only at nesting depth zero.</param>
        public BalancedUntilParser(TokenKind terminator)
        {
            Terminator = terminator;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames => Array.Empty<string>();

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            int pos = position;
            int depth = 0;
            var collected = new List<Token>();
            while (pos < tokens.Length)
            {
                var kind = tokens[pos].Kind;
                if (kind == TokenKind.EndOfFile) break;
                if (depth == 0 && kind == Terminator) break;
                if (kind == TokenKind.OpenParen) depth++;
                else if (kind == TokenKind.CloseParen && depth > 0) depth--;
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
