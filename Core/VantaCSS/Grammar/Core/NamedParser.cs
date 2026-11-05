using System;
using System.Collections.Generic;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Wraps a parser and captures its result under a name.
    /// </summary>
    public class NamedParser : IParser
    {
        /// <summary>
        /// The wrapped parser.
        /// </summary>
        public IParser Inner { get; }

        /// <summary>
        /// The capture name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="NamedParser"/> class.
        /// </summary>
        /// <param name="inner">The wrapped parser.</param>
        /// <param name="name">The capture name.</param>
        public NamedParser(IParser inner, string name)
        {
            Inner = inner;
            Name = name;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames => new[] { Name };

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            var result = Inner.TryParse(tokens, position, source);
            if (!result.Success)
            {
                return result;
            }

            var captures = new Dictionary<string, object> { { Name, result.Value } };
            return new ParseResult
            {
                Success = true,
                Position = result.Position,
                Length = result.Length,
                Value = result.Value,
                Captures = captures,
                Source = source
            };
        }
    }
}