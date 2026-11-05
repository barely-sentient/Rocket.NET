using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// The result of a parse attempt.
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// Whether the parse succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The position in the token stream where parsing started.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The number of tokens consumed.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The value produced by the parser.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Named captures produced by the parser.
        /// </summary>
        public Dictionary<string, object> Captures { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Error message when parsing failed.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// The source text that was parsed.
        /// </summary>
        public string Source { get; set; }
    }
}