using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
    /// A type selector with the ids, classes, attributes and pseudo-classes that apply to one node.
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
        /// Attribute constraints that must match.
        /// </summary>
        public List<AttributeSelector> Attributes { get; } = new List<AttributeSelector>();

        /// <summary>
        /// Pseudo-classes that must match. Pseudo-elements that do not affect the node are omitted.
        /// </summary>
        public List<PseudoSelector> Pseudos { get; } = new List<PseudoSelector>();
    }

    /// <summary>
    /// An attribute selector constraint such as <c>[hidden]</c> or <c>[type="text"]</c>.
    /// </summary>
    public sealed class AttributeSelector
    {
        /// <summary>The attribute (property) name, as written.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>The comparison operator.</summary>
        public AttrOp Op { get; set; } = AttrOp.Exists;
        /// <summary>The value to compare against, or null for <see cref="AttrOp.Exists"/>.</summary>
        public string Value { get; set; }
        /// <summary>Whether matching is case-insensitive.</summary>
        public bool CaseInsensitive { get; set; } = true;
        /// <summary>Whether the match result should be negated (for !=).</summary>
        public bool Negate { get; set; }
    }

    /// <summary>
    /// Attribute selector comparison operators.
    /// </summary>
    public enum AttrOp
    {
        Exists,
        Equals,
        Word,
        Hyphen,
        Prefix,
        Suffix,
        Substring
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

        // Bootstrap plus Font Awesome exceed 4096 distinct selectors; a smaller
        // limit makes the cache clear mid-layout and re-parse every rule per node.
        private const int MaxSelectorCacheEntries = 65536;

        /// <summary>
        /// Parses a selector list. Invalid selectors yield an empty list.
        /// </summary>
        /// <param name="selector">The selector text.</param>
        /// <returns>The parsed selector list.</returns>
        public static SelectorList ParseSelectors(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector)) return new SelectorList();
            if (SelectorCache.TryGetValue(selector, out var cached)) return cached;
            if (SelectorCache.Count >= MaxSelectorCacheEntries) SelectorCache.Clear();

            var rawTokens = Tokenise(selector);
            var tokens = new List<Token>(rawTokens.Length);
            for (int i = 0; i < rawTokens.Length; i++)
                if (rawTokens[i].Kind != TokenKind.EndOfFile) tokens.Add(rawTokens[i]);
            var tokenArray = tokens.ToArray();
            var parsed = ParseSelectorTokens(tokenArray, selector) ?? new SelectorList();
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

        // Attribute selectors are not evaluated yet. They must not match every element
        // (e.g. [hidden] would hide the whole page), so the compound is marked as one
        // that never matches.
        private static IParser AttributeSelectorParser() =>
            Sequence(
                Token(TokenKind.OpenBracket).As("open"),
                AnyUntil(TokenKind.CloseBracket).As("args"),
                Token(TokenKind.CloseBracket).As("close"))
            .Builder<Token, Token[], Token, Action<CompoundSelector>>((open, args, close) =>
            {
                var attr = ParseAttribute(args);
                if (attr == null) return (compound) => { };
                return (compound) => compound.Attributes.Add(attr);
            });

        private static AttributeSelector ParseAttribute(Token[] args)
        {
            string raw = string.Empty;
            if (args != null)
            {
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < args.Length; i++) sb.Append(args[i].Text ?? string.Empty);
                raw = sb.ToString().Trim();
            }
            if (string.IsNullOrEmpty(raw)) return null;

            bool caseInsensitive = true;
            if (raw.Length >= 2 && raw[raw.Length - 2] == ' ')
            {
                char flag = raw[raw.Length - 1];
                if (flag == 'i' || flag == 'I') { caseInsensitive = true; raw = raw.Substring(0, raw.Length - 2).Trim(); }
                else if (flag == 's' || flag == 'S') { caseInsensitive = false; raw = raw.Substring(0, raw.Length - 2).Trim(); }
            }

            string[] ops = { "!=", "~=", "|=", "^=", "$=", "*=", "=" };
            string op = null; int opIndex = -1; int opLen = 0;
            for (int i = 0; i < raw.Length; i++)
            {
                if (raw[i] == ' ' || raw[i] == '\t') continue;
                if (raw[i] == '!' || raw[i] == '~' || raw[i] == '|' || raw[i] == '^' ||
                    raw[i] == '$' || raw[i] == '*' || raw[i] == '=')
                {
                    foreach (var candidate in ops)
                    {
                        if (i + candidate.Length <= raw.Length &&
                            string.Compare(raw, i, candidate, 0, candidate.Length, StringComparison.Ordinal) == 0)
                        { op = candidate; opIndex = i; opLen = candidate.Length; break; }
                    }
                    if (op != null) break;
                }
            }

            if (op == null)
                return new AttributeSelector { Name = raw, Op = AttrOp.Exists, CaseInsensitive = caseInsensitive };

            string name = raw.Substring(0, opIndex).Trim();
            string value = raw.Substring(opIndex + opLen).Trim();
            if (value.Length >= 2 && ((value[0] == '"' && value[value.Length - 1] == '"') ||
                                      (value[0] == '\'' && value[value.Length - 1] == '\'')))
                value = value.Substring(1, value.Length - 2);

            AttrOp attrOp;
            if (op == "~=") attrOp = AttrOp.Word;
            else if (op == "|=") attrOp = AttrOp.Hyphen;
            else if (op == "^=") attrOp = AttrOp.Prefix;
            else if (op == "$=") attrOp = AttrOp.Suffix;
            else if (op == "*=") attrOp = AttrOp.Substring;
            else attrOp = AttrOp.Equals;

            return new AttributeSelector
            {
                Name = name,
                Op = attrOp,
                Value = value,
                CaseInsensitive = caseInsensitive,
                Negate = op == "!="
            };
        }



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
