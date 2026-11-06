using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// One media query from a media query list, such as <c>not screen and (min-width: 500px)</c>.
    /// </summary>
    public sealed class MediaQuery
    {
        /// <summary>
        /// Whether the query is prefixed with <c>not</c>.
        /// </summary>
        public bool Negated { get; set; }

        /// <summary>
        /// The media type in lower case (for example <c>screen</c>), or null when omitted.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// The parenthesised features that must all match.
        /// </summary>
        public List<MediaFeature> Features { get; } = new List<MediaFeature>();
    }

    /// <summary>
    /// A parenthesised media feature such as <c>(min-width: 500px)</c>. <see cref="Value"/> is null for boolean features.
    /// </summary>
    public sealed class MediaFeature
    {
        /// <summary>
        /// The feature name in lower case.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The feature value as written, or null.
        /// </summary>
        public string Value { get; set; }
    }

    public static partial class Grammar
    {
        private static readonly ConcurrentDictionary<string, List<MediaQuery>> MediaCache =
            new ConcurrentDictionary<string, List<MediaQuery>>(StringComparer.Ordinal);

        /// <summary>
        /// Parses a media query list. Returns null when the list is invalid.
        /// </summary>
        /// <param name="media">The media query text.</param>
        /// <returns>The queries in source order, or null.</returns>
        public static List<MediaQuery> ParseMediaQueries(string media)
        {
            if (string.IsNullOrWhiteSpace(media)) return new List<MediaQuery>();
            if (MediaCache.TryGetValue(media, out var cached)) return cached;
            if (MediaCache.Count >= MaxCacheEntries) MediaCache.Clear();

            var queries = ParseWhole(MediaQueryListParser(), ValueTokens(media), media) as List<MediaQuery>;
            MediaCache[media] = queries;
            return queries;
        }

        private static IParser MediaQueryListParser() =>
            Sequence(
                Optional(new TokenMatchParser("@media", t => t.Kind == TokenKind.AtKeyword && string.Equals(t.Text, "@media", StringComparison.OrdinalIgnoreCase))),
                MediaQueryParser().As("first"),
                ZeroOrMore(
                    Sequence(Token(TokenKind.Comma), MediaQueryParser().As("next"))
                        .Builder((MediaQuery next) => next)).As("more"))
            .Builder((MediaQuery first, object[] more) => BuildMediaQueryList(first, more));

        private static List<MediaQuery> BuildMediaQueryList(MediaQuery first, object[] more)
        {
            var list = new List<MediaQuery> { first };
            list.AddRange(more.Cast<MediaQuery>());
            return list;
        }

        private static IParser MediaQueryParser() =>
            Or(
                Sequence(
                        Optional(Word("not", "only")).As("modifier"),
                        new TokenMatchParser("media type", IsMediaTypeToken).As("type"),
                        AndFeaturesParser().As("features"))
                    .Builder((Token modifier, Token type, object[] features) =>
                        BuildMediaQuery(modifier, type.Text.ToLowerInvariant(), null, features)),
                Sequence(
                        Optional(Word("not", "only")).As("modifier"),
                        MediaFeatureParser().As("feature"),
                        AndFeaturesParser().As("features"))
                    .Builder((Token modifier, MediaFeature feature, object[] features) =>
                        BuildMediaQuery(modifier, null, feature, features)));

        private static MediaQuery BuildMediaQuery(Token modifier, string mediaType, MediaFeature first, object[] features)
        {
            var query = new MediaQuery
            {
                Negated = modifier != null && string.Equals(modifier.Text, "not", StringComparison.OrdinalIgnoreCase),
                MediaType = mediaType
            };
            if (first != null) query.Features.Add(first);
            query.Features.AddRange(features.Cast<MediaFeature>());
            return query;
        }

        private static IParser AndFeaturesParser() =>
            ZeroOrMore(
                Sequence(Word("and"), MediaFeatureParser().As("feature"))
                    .Builder((MediaFeature feature) => feature));

        private static IParser MediaFeatureParser() =>
            Sequence(
                Token(TokenKind.OpenParen),
                new TokenMatchParser("media feature name", t => IsIdentKind(t.Kind)).As("name"),
                Optional(Sequence(
                    Token(TokenKind.Colon),
                    new TokenMatchParser("media feature value", IsMediaValueToken).As("value"))),
                Token(TokenKind.CloseParen))
            .Builder((Token name, Token value) => new MediaFeature
            {
                Name = name.Text.ToLowerInvariant(),
                Value = value == null ? null : value.Text
            });

        private static bool IsMediaTypeToken(Token t) =>
            IsIdentKind(t.Kind) &&
            !string.Equals(t.Text, "and", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(t.Text, "not", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(t.Text, "only", StringComparison.OrdinalIgnoreCase);

        private static bool IsMediaValueToken(Token t) =>
            t.Kind != TokenKind.OpenParen && t.Kind != TokenKind.CloseParen &&
            t.Kind != TokenKind.Comma && t.Kind != TokenKind.Colon;
    }
}
