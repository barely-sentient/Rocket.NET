using System;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Matches a single token of a given kind.
    /// </summary>
    public class TokenParser : IParser
    {
        public TokenKind Kind { get; }

        public TokenParser(TokenKind kind)
        {
            Kind = kind;
        }

        public IReadOnlyList<string> CaptureNames => Array.Empty<string>();

        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            if (position >= 0 && position < tokens.Length && tokens[position].Kind == Kind)
            {
                return new ParseResult
                {
                    Success = true,
                    Position = position,
                    Length = 1,
                    Value = tokens[position],
                    Source = source
                };
            }
            return new ParseResult
            {
                Success = false,
                Position = position,
                Length = 0,
                Error = $"Expected token {Kind}.",
                Source = source
            };
        }
    }

    /// <summary>
    /// Matches a sequence of parsers in order.
    /// </summary>
    public class SequenceParser : IParser
    {
        public IParser[] Parsers { get; }

        public SequenceParser(params IParser[] parsers)
        {
            Parsers = parsers ?? Array.Empty<IParser>();
        }

        public IReadOnlyList<string> CaptureNames =>
            Parsers.SelectMany(p => p.CaptureNames).ToList();

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

    /// <summary>
    /// Consumes tokens until a terminator is encountered.
    /// </summary>
    public class AnyUntilParser : IParser
    {
        public object Terminator { get; }

        public AnyUntilParser(object terminator)
        {
            Terminator = terminator;
        }

        public IReadOnlyList<string> CaptureNames => Array.Empty<string>();

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
            var text = source != null
                ? string.Join("", collected.Select(t => source.Substring((int)t.StartIndex, (int)t.Length)))
                : string.Empty;
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

    /// <summary>
    /// Wraps a parser and captures its result under a name.
    /// </summary>
    public class NamedParser : IParser
    {
        public IParser Inner { get; }
        public string Name { get; }

        public NamedParser(IParser inner, string name)
        {
            Inner = inner;
            Name = name;
        }

        public IReadOnlyList<string> CaptureNames => new[] { Name };

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

    /// <summary>
    /// Wraps a parser and transforms its captures via a builder delegate.
    /// </summary>
    public class BuilderParser : IParser
    {
        public IParser Inner { get; }
        public Func<object[], string, object> Builder { get; }

        public BuilderParser(IParser inner, Func<object[], string, object> builder)
        {
            Inner = inner;
            Builder = builder;
        }

        public IReadOnlyList<string> CaptureNames => Inner.CaptureNames;

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

    /// <summary>
    /// Makes a parser optional.
    /// </summary>
    public class OptionalParser : IParser
    {
        public IParser Inner { get; }

        public OptionalParser(IParser inner)
        {
            Inner = inner;
        }

        public IReadOnlyList<string> CaptureNames => Inner.CaptureNames;

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

    /// <summary>
    /// Matches zero or more repetitions of a parser.
    /// </summary>
    public class ZeroOrMoreParser : IParser
    {
        public IParser Inner { get; }

        public ZeroOrMoreParser(IParser inner)
        {
            Inner = inner;
        }

        public IReadOnlyList<string> CaptureNames => Array.Empty<string>();

        public ParseResult TryParse(Token[] tokens, int position, string source)
        {
            int pos = position;
            var collected = new List<object>();
            var captures = new Dictionary<string, object>();
            while (pos < tokens.Length)
            {
                var result = Inner.TryParse(tokens, pos, source);
                if (!result.Success)
                {
                    break;
                }
                if (result.Length == 0)
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