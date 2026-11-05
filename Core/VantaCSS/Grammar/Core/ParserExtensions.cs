using System;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// Extension methods for <see cref="IParser"/>.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Captures the parser's result under the given name.
        /// </summary>
        public static IParser As(this IParser parser, string name) => new NamedParser(parser, name);

        /// <summary>
        /// Makes a parser optional.
        /// </summary>
        public static IParser Optional(this IParser parser) => new OptionalParser(parser);

        /// <summary>
        /// Matches zero or more repetitions.
        /// </summary>
        public static IParser ZeroOrMore(this IParser parser) => new ZeroOrMoreParser(parser);

        /// <summary>
        /// Tries this parser followed by the supplied alternatives.
        /// </summary>
        public static IParser Or(this IParser parser, params IParser[] others)
        {
            var list = new List<IParser> { parser };
            if (others != null) list.AddRange(others);
            return new AlternativeParser(list.ToArray());
        }

        private static object ConvertValue(object value, Type targetType, string source)
        {
            if (value == null) return null;
            if (targetType.IsAssignableFrom(value.GetType())) return value;
            if (targetType == typeof(string))
            {
                if (value is Token t)
                {
                    if (!string.IsNullOrEmpty(t.Text)) return t.Text;
                    if (!string.IsNullOrEmpty(source)) return source.Substring((int)t.StartIndex, (int)t.Length);
                    return string.Empty;
                }
                if (value is Token[] arr)
                {
                    if (!string.IsNullOrEmpty(source))
                    {
                        return string.Concat(arr.Select(x => source.Substring((int)x.StartIndex, (int)x.Length)));
                    }
                    return string.Empty;
                }
                return value.ToString();
            }
            try { return Convert.ChangeType(value, targetType); }
            catch { return value; }
        }

        public static IParser Builder<TResult>(this IParser parser, Func<TResult> builder) =>
            new BuilderParser(parser, (_, __) => builder());

        public static IParser Builder<T1, TResult>(this IParser parser, Func<T1, TResult> builder) =>
            new BuilderParser(parser, (values, source) => builder((T1)ConvertValue(values.Length > 0 ? values[0] : null, typeof(T1), source)));

        public static IParser Builder<T1, T2, TResult>(this IParser parser, Func<T1, T2, TResult> builder) =>
            new BuilderParser(parser, (values, source) => builder(
                (T1)ConvertValue(values.Length > 0 ? values[0] : null, typeof(T1), source),
                (T2)ConvertValue(values.Length > 1 ? values[1] : null, typeof(T2), source)));

        public static IParser Builder<T1, T2, T3, TResult>(this IParser parser, Func<T1, T2, T3, TResult> builder) =>
            new BuilderParser(parser, (values, source) => builder(
                (T1)ConvertValue(values.Length > 0 ? values[0] : null, typeof(T1), source),
                (T2)ConvertValue(values.Length > 1 ? values[1] : null, typeof(T2), source),
                (T3)ConvertValue(values.Length > 2 ? values[2] : null, typeof(T3), source)));

        public static IParser Builder<T1, T2, T3, T4, TResult>(this IParser parser, Func<T1, T2, T3, T4, TResult> builder) =>
            new BuilderParser(parser, (values, source) => builder(
                (T1)ConvertValue(values.Length > 0 ? values[0] : null, typeof(T1), source),
                (T2)ConvertValue(values.Length > 1 ? values[1] : null, typeof(T2), source),
                (T3)ConvertValue(values.Length > 2 ? values[2] : null, typeof(T3), source),
                (T4)ConvertValue(values.Length > 3 ? values[3] : null, typeof(T4), source)));
    }
}