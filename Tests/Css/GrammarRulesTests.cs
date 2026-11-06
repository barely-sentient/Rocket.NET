using System.Collections.Generic;
using System.Linq;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using VantaCSS.Grammar;
using Xunit;

namespace Tests
{
    public class GrammarRulesTests
    {
        [Fact]
        public void Selectors_ParseCombinatorsInOrder()
        {
            var list = Grammar.ParseSelectors("ul > li + li ~ a, .x.y");
            Assert.Equal(2, list.Items.Count);
            Assert.Equal(new[] { '>', '+', '~' }, list.Items[0].Combinators);
            Assert.Equal(new[] { "x", "y" }, list.Items[1].Compounds[0].Classes);
        }

        [Fact]
        public void Selectors_ParsePseudoClassesAndFunctionalArguments()
        {
            var compound = Grammar.ParseSelectors("a:not(.b):is(.c, .d)").Items[0].Compounds[0];
            Assert.Equal(new[] { ":not", ":is" }, compound.Pseudos.Select(p => p.Name));
            Assert.Equal(":not(.b)", compound.Pseudos[0].Raw);
            Assert.Equal(2, compound.Pseudos[1].Argument.Items.Count);
        }

        [Fact]
        public void Selectors_OmitPseudoElementsThatDoNotAffectTheNode()
        {
            var compound = Grammar.ParseSelectors("p::before").Items[0].Compounds[0];
            Assert.Empty(compound.Pseudos);
        }

        [Fact]
        public void Selectors_InvalidSelectorYieldsNoItems()
        {
            Assert.Empty(Grammar.ParseSelectors("div >").Items);
        }

        [Fact]
        public void Selectors_ChildCombinatorMatchesOnlyDirectChildren()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(".outer > .inner { color: red; }");

            var outer = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "outer" });
            var inner = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "inner" });
            var deep = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "inner" });
            var middle = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            outer.Children = new[] { inner, middle };
            inner.Parent = outer;
            middle.Parent = outer;
            middle.Children = new[] { deep };
            deep.Parent = middle;

            engine.ApplyComputedStyles(inner, 800, 600, Orientation.Portrait);
            engine.ApplyComputedStyles(deep, 800, 600, Orientation.Portrait);
            Assert.Equal("red", inner.ComputedStyles["color"]);
            Assert.False(deep.ComputedStyles.ContainsKey("color"));
        }

        [Fact]
        public void Selectors_IsPseudoMatchesAnyArgument()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div:is(.a, .b) { color: blue; }");

            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "b" });
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("blue", node.ComputedStyles["color"]);
        }

        [Fact]
        public void EdgeSlots_AcceptOneToFourLengthsAndAuto()
        {
            Assert.Equal(new[] { "10px", "20px" }, Grammar.ParseEdgeSlots("10px 20px"));
            Assert.Equal(new[] { "10px", "auto" }, Grammar.ParseEdgeSlots("10px auto"));
            Assert.Equal(new[] { "1em", "0", "2px", "3px" }, Grammar.ParseEdgeSlots("1em 0 2px 3px !important"));
        }

        [Fact]
        public void EdgeSlots_RejectMoreThanFourOrNonLengths()
        {
            Assert.Null(Grammar.ParseEdgeSlots("1px 2px 3px 4px 5px"));
            Assert.Null(Grammar.ParseEdgeSlots("red"));
        }

        [Fact]
        public void BorderLengths_IgnoreStyleAndColour()
        {
            Assert.Equal(new[] { "2px" }, Grammar.ParseBorderLengths("2px solid #808080"));
            Assert.Empty(Grammar.ParseBorderLengths("solid rgb(0, 0, 0)"));
        }

        [Fact]
        public void TransitionList_ParsesEachPart()
        {
            var specs = Grammar.ParseTransitionList("color 1s linear 200ms, opacity 0.5s ease-in");
            Assert.Equal(2, specs.Count);
            Assert.Equal("color", specs[0].Property);
            Assert.Equal("1s", specs[0].Duration);
            Assert.Equal("200ms", specs[0].Delay);
            Assert.Equal("linear", specs[0].Timing);
            Assert.Equal("opacity", specs[1].Property);
            Assert.Equal("0.5s", specs[1].Duration);
            Assert.Null(specs[1].Delay);
            Assert.Equal("ease-in", specs[1].Timing);
        }

        [Fact]
        public void TransitionList_KeepsCubicBezierTogether()
        {
            var spec = Assert.Single(Grammar.ParseTransitionList("transform 1s cubic-bezier(0.1, 0.7, 1, 0.1)"));
            Assert.Equal("cubic-bezier(0.1,0.7,1,0.1)", spec.Timing);
        }

        [Fact]
        public void TextShadows_ParseOffsetsBlurAndColour()
        {
            var shadow = Assert.Single(Grammar.ParseTextShadows("2px 3px 4px red"));
            Assert.Equal("2px", shadow.X);
            Assert.Equal("3px", shadow.Y);
            Assert.Equal("4px", shadow.Blur);
            Assert.Equal("red", shadow.Color);
        }

        [Fact]
        public void TextShadows_AllowColourFirstAndDefaultBlur()
        {
            var shadow = Assert.Single(Grammar.ParseTextShadows("red 1px 1px"));
            Assert.Equal("0", shadow.Blur);
            Assert.Equal("red", shadow.Color);
        }

        [Fact]
        public void TextShadows_RejectMissingOffsets()
        {
            Assert.Null(Grammar.ParseTextShadows("1px"));
        }

        [Fact]
        public void VarReferences_ResolveWithFallbackAndLocalValues()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            Assert.Equal("1px solid red", engine.ResolveVarValue("1px solid var(--c, red)", null));
            Assert.Equal("5px", engine.ResolveVarValue("var(--a)", null, new Dictionary<string, string> { { "--a", "5px" } }));
            Assert.Equal("", engine.ResolveVarValue("var(--missing)", null));
        }

        [Fact]
        public void FontUrl_ReturnsFirstUrlWithoutQuotes()
        {
            Assert.Equal("https://example.com/a.ttf", Grammar.ParseFontUrl("url('https://example.com/a.ttf') format('truetype')"));
            Assert.Null(Grammar.ParseFontUrl("local(Foo)"));
        }

        [Fact]
        public void FontFace_ParsesDeclarationsIntoAtRule()
        {
            var rule = Assert.Single(Grammar.ParseRules("@font-face { font-family: Foo; src: url(a.ttf); }"));
            Assert.True(rule.IsAtRule);
            Assert.Equal(2, rule.Declarations.Count);
            Assert.Equal("Foo", rule.Declarations[0].Value);
        }

        [Theory]
        [InlineData("(min-width: 500px) and (max-width: 900px)", 800, 600, Orientation.Portrait, true)]
        [InlineData("(min-width: 500px) and (max-width: 900px)", 950, 600, Orientation.Portrait, false)]
        [InlineData("print", 800, 600, Orientation.Portrait, false)]
        [InlineData("not print", 800, 600, Orientation.Portrait, true)]
        [InlineData("screen and (orientation: landscape)", 800, 600, Orientation.Landscape, true)]
        [InlineData("screen and (orientation: landscape)", 800, 600, Orientation.Portrait, false)]
        [InlineData("@media (min-width: 500px)", 800, 600, Orientation.Portrait, true)]
        [InlineData("(min-width: 2em)", 20, 600, Orientation.Portrait, false)]
        [InlineData("(min-width: 2em)", 40, 600, Orientation.Portrait, true)]
        public void MediaQueries_MatchFeatures(string media, int width, int height, Orientation orientation, bool expected)
        {
            var engine = new VantaCssEngine(width, height, orientation);
            Assert.Equal(expected, engine.MatchesMedia(media, width, height, orientation));
        }

        [Fact]
        public void MediaQueries_InvalidListDoesNotMatch()
        {
            Assert.Null(Grammar.ParseMediaQueries("(min-width: 500px"));
            var engine = new VantaCssEngine(800, 600, "portrait");
            Assert.False(engine.MatchesMedia("(min-width: 500px", 800, 600, Orientation.Portrait));
        }
    }
}
