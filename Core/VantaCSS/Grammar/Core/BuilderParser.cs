using System;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Wraps a parser and transforms its captures via a builder delegate.
    /// </summary>
    public class BuilderParser : IParser
    {
        /// <summary>
        /// The wrapped parser.
        /// </summary>
        public IParser Inner { get; }

        /// <summary>
        /// The builder delegate.
        /// </summary>
        public Func<object[], string, object> Builder { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="BuilderParser"/> class.
        /// </summary>
        /// <param name="inner">The wrapped parser.</param>
        /// <param name="builder">The builder delegate.</param>
        public BuilderParser(IParser inner, Func<object[], string, object> builder)
        {
            Inner = inner;
            Builder = builder;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> CaptureNames => Inner.CaptureNames;

        /// <inheritdoc />
        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            var result = Inner.TryParse(tokens, position, source);
            if (!result.Success)
            {
                return result;
            }

            var values = Inner.CaptureNames.Select(n =>
            {
                if (result.Captures != null && result.Captures.TryGetValue(n, out var v))
                {
                    return v;
                }
                return null;
            }).ToArray();

            object built = null;
            try
            {
                built = Builder(values, source);
            }
            catch
            {
                built = null;
            }

            return new ParseResult
            {
                Success = true,
                Position = result.Position,
                Length = result.Length,
                Value = built,
                Captures = result.Captures,
                Source = source
            };
        }
    }
}