using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VantaCSS.Tokeniser;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// A comma-separated list of complex selectors.
    /// </summary>
    public sealed class SelectorList
    {
        /// <summary>
        /// The complex selectors in source order.
        /// </summary>
        public List<ComplexSelector> Items { get; } = new List<ComplexSelector>();
    }

    /// <summary>
    /// Compound selectors joined by combinators. <see cref="Combinators"/> holds one entry per gap:
    /// ' ' (descendant), '&gt;' (child), '+' (adjacent sibling) or '~' (general sibling).
    /// </summary>
    public sealed class ComplexSelector
    {
        /// <summary>
        /// The compound selectors, left to right.
        /// </summary>
        public List<CompoundSelector> Compounds { get; } = new List<CompoundSelector>();

        /// <summary>
        /// The combinators between consecutive compounds.
        /// </summary>
        public List<char> Combinators { get; } = new List<char>();
    }

    /// <summary>
    /// A type selector with the ids, classes and pseudo-classes that apply to one node.
    /// </summary>
    public sealed class CompoundSelector
    {
        /// <summary>
        /// The type name, or null for the universal selector or no type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Required ids (without the leading '#').
        /// </summary>
        public List<string> Ids { get; } = new List<string>();

        /// <summary>
        /// Required classes (without the leading '.').
        /// </summary>
        public List<string> Classes { get; } = new List<string>();

        /// <summary>
        /// Pseudo-classes that must match. Pseudo-elements that do not affect the node are omitted.
        /// </summary>
        public List<PseudoSelector> Pseudos { get; } = new List<PseudoSelector>();
    }

    /// <summary>
    /// A pseudo-class or pseudo-element.
    /// </summary>
    public sealed class PseudoSelector
    {
        /// <summary>
        /// The pseudo name including its colon(s), for example ":hover" or ":is".
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The pseudo as written, including any arguments.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// The selector list of a functional selector pseudo-class such as :is(...).
        /// </summary>
        public SelectorList Argument { get; set; }
    }

    public static partial class Grammar
    {
        private static readonly ConcurrentDictionary<string, SelectorList> SelectorCache =
            new ConcurrentDictionary<string, SelectorList>(StringComparer.Ordinal);

        /// <summary>
        /// Parses a selector list. Invalid selectors yield an empty list.
        /// </summary>
        /// <param name="selector">The selector text.</param>
        /// <returns>The parsed selector list.</returns>
        public static SelectorList ParseSelectors(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector)) return new SelectorList();
            if (SelectorCache.TryGetValue(selector, out var cached)) return cached;
            if (SelectorCache.Count >= MaxCacheEntries) SelectorCache.Clear();

            var tokens = Tokenise(selector).Where(t => t.Kind != TokenKind.EndOfFile).ToArray();
            var parsed = ParseSelectorTokens(tokens, selector) ?? new SelectorList();
            SelectorCache[selector] = parsed;
            return parsed;
        }

        private static SelectorList ParseSelectorTokens(Token[] tokens, string source)
        {
            var result = SelectorListParser().TryParse(tokens, 0, source);
            return result.Success && result.Length == tokens.Length ? result.Value as SelectorList : null;
        }

        private static IParser SelectorListParser() =>
            Sequence(
                Trivia(),
                ComplexSelectorParser().As("first"),
                ZeroOrMore(
                    Sequence(Trivia(), Token(TokenKind.Comma), Trivia(), ComplexSelectorParser().As("next"))
                        .Builder((ComplexSelector next) => next)).As("more"),
                Trivia())
            .Builder((ComplexSelector first, object[] more) => BuildSelectorList(first, more));

        private static SelectorList BuildSelectorList(ComplexSelector first, object[] more)
        {
            var list = new SelectorList();
            list.Items.Add(first);
            foreach (var item in more) list.Items.Add((ComplexSelector)item);
            return list;
        }

        private static IParser ComplexSelectorParser() =>
            Sequence(
                CompoundSelectorParser().As("first"),
                ZeroOrMore(
                    Sequence(CombinatorParser().As("combinator"), CompoundSelectorParser().As("compound"))
                        .Builder((char combinator, CompoundSelector compound) => Tuple.Create(combinator, compound))).As("links"))
            .Builder((CompoundSelector first, object[] links) => BuildComplexSelector(first, links));

        private static ComplexSelector BuildComplexSelector(CompoundSelector first, object[] links)
        {
            var complex = new ComplexSelector();
            complex.Compounds.Add(first);
            foreach (Tuple<char, CompoundSelector> link in links)
            {
                complex.Combinators.Add(link.Item1);
                complex.Compounds.Add(link.Item2);
            }
            return complex;
        }

        private static IParser CombinatorParser() =>
            Or(
                Sequence(Trivia(), new TokenMatchParser("'>', '+' or '~'", IsExplicitCombinator).As("symbol"), Trivia())
                    .Builder((Token symbol) => symbol.Text[0]),
                Sequence(TriviaToken().As("whitespace"), Trivia())
                    .Builder((Token whitespace) => ' '));

        private static bool IsExplicitCombinator(Token t) =>
            t.Kind == TokenKind.GreaterThan || t.Kind == TokenKind.Plus || t.Kind == TokenKind.Tilde;

        private static IParser CompoundSelectorParser() =>
            Or(
                Sequence(TypeSelectorParser().As("type"), SimpleSelectorsParser().As("parts"))
                    .Builder((Action<CompoundSelector> type, object[] parts) => BuildCompoundSelector(type, parts)),
                Sequence(SimpleSelectorParser().As("first"), SimpleSelectorsParser().As("parts"))
                    .Builder((Action<CompoundSelector> first, object[] parts) => BuildCompoundSelector(first, parts)));

        private static CompoundSelector BuildCompoundSelector(Action<CompoundSelector> head, object[] parts)
        {
            var compound = new CompoundSelector();
            head?.Invoke(compound);
            foreach (var part in parts)
            {
                if (part is Action<CompoundSelector> apply) apply(compound);
            }
            return compound;
        }

        private static IParser TypeSelectorParser() =>
            Or(
                new TokenMatchParser("type selector", t => IsIdentKind(t.Kind)).As("type")
                    .Builder((Token type) => (Action<CompoundSelector>)(compound => compound.Type = type.Text)),
                new TokenMatchParser("universal selector", t => t.Kind == TokenKind.UniversalSelector || t.Kind == TokenKind.Asterisk).As("universal")
                    .Builder((Token universal) => (Action<CompoundSelector>)(compound => { })));

        private static IParser SimpleSelectorsParser() => ZeroOrMore(SimpleSelectorParser());

        private static IParser SimpleSelectorParser() =>
            Or(IdSelectorParser(), ClassSelectorParser(), PseudoSelectorParser(), AttributeSelectorParser());

        private static IParser IdSelectorParser() =>
            new TokenMatchParser("id selector", t =>
                    (t.Kind == TokenKind.Identifier || t.Kind == TokenKind.HexColor) && t.Text.StartsWith("#", StringComparison.Ordinal))
                .As("id")
                .Builder((Token id) => (Action<CompoundSelector>)(compound => compound.Ids.Add(id.Text.Substring(1))));

        private static IParser ClassSelectorParser() =>
            new TokenMatchParser("class selector", t => t.Kind == TokenKind.ClassName)
                .As("class")
                .Builder((Token cls) => (Action<CompoundSelector>)(compound => compound.Classes.Add(cls.Text.Substring(1))));

        // Attribute selectors are accepted but not evaluated, so they never exclude a node.
        private static IParser AttributeSelectorParser() =>
            Sequence(Token(TokenKind.OpenBracket), AnyUntil(TokenKind.CloseBracket), Token(TokenKind.CloseBracket))
                .Builder<Action<CompoundSelector>>(() => compound => { });

        private static IParser PseudoSelectorParser() =>
            Sequence(
                new TokenMatchParser("pseudo prefix", t => t.Kind == TokenKind.Colon || t.Kind == TokenKind.DoubleColon).As("prefix"),
                new TokenMatchParser("pseudo name", t => IsIdentKind(t.Kind) || t.Kind == TokenKind.Function).As("name"),
                Optional(Sequence(
                    Token(TokenKind.OpenParen),
                    BalancedUntil(TokenKind.CloseParen).As("args"),
                    Token(TokenKind.CloseParen))))
            .Builder((Token prefix, Token name, Token[] args) => BuildPseudo(prefix, name, args));

        private static Action<CompoundSelector> BuildPseudo(Token prefix, Token name, Token[] args)
        {
            return compound =>
            {
                string pseudoName = prefix.Text + name.Text;
                if (string.Equals(pseudoName, "::before", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(pseudoName, "::after", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var pseudo = new PseudoSelector
                {
                    Name = pseudoName,
                    Raw = args == null ? pseudoName : pseudoName + "(" + JoinTokens(args) + ")"
                };
                if (args != null && IsFunctionalSelectorPseudo(pseudoName))
                {
                    pseudo.Argument = ParseSelectorTokens(args, string.Empty) ?? new SelectorList();
                }
                compound.Pseudos.Add(pseudo);
            };
        }

        private static bool IsFunctionalSelectorPseudo(string name) =>
            string.Equals(name, ":is", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(name, ":where", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(name, ":matches", StringComparison.OrdinalIgnoreCase);
    }
}
