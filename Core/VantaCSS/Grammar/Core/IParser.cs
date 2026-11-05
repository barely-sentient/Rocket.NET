using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// A parser that operates over a token stream.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Names captured by this parser (in order).
        /// </summary>
        IReadOnlyList<string> CaptureNames { get; }

        /// <summary>
        /// Attempts to parse the token stream starting at <paramref name="position"/>.
        /// </summary>
        ParseResult TryParse(Token[] tokens, int position, string source);
    }
}