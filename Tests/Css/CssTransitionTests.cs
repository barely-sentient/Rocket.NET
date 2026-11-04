using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using Xunit;

namespace Tests
{
    public class CssTransitionTests
    {
        [Fact]
        public void HoverRuleAppliesWhenHovered()
        {
            var sheet = new Stylesheet();
            sheet.AddRule("button", "color: black");
            sheet.AddRule("button:hover", "color: red");
            var engine = new CssEngine();
            engine.AddStylesheet(sheet);

            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("black", node.ComputedStyles["color"]);

            node.Hovered = true;
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("red", node.ComputedStyles["color"]);
        }

        [Fact]
        public void EngineHoverHelperWorks()
        {
            var css = "button { color: black; } button:hover { color: red; }";
            var engine = new CssEngine();
            engine.AddStylesheet(css);

            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.Hover(node);

            var style = engine.ComputeStyles(node);
            Assert.Equal("red", style["color"]);
        }

        [Fact]
        public void TransitionStartsAtFromValue()
        {
            var css = "button { color: black; transition: color 1s linear; } button:hover { color: white; }";
            var engine = new CssEngine();
            engine.AddStylesheet(css);

            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.ComputeStyle(node, 0); // base

            node.Hovered = true;
            var at0 = engine.ComputeStyles(node, 0);
            Assert.Equal("rgb(0, 0, 0)", at0["color"]);
        }

        [Fact]
        public void TransitionInterpolatesMidpoint()
        {
            var css = "button { color: black; transition: color 1s linear; } button:hover { color: white; }";
            var engine = new CssEngine();
            engine.AddStylesheet(css);

            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.ComputeStyle(node, 0); // base

            node.Hovered = true;
            engine.ComputeStyle(node, 0); // start
            var atHalf = engine.ComputeStyles(node, 0.5);
            Assert.Contains("128", atHalf["color"]);
        }

        [Fact]
        public void TransitionCompletesAtEnd()
        {
            var css = "button { color: black; transition: color 1s linear; } button:hover { color: white; }";
            var engine = new CssEngine();
            engine.AddStylesheet(css);

            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.ComputeStyle(node, 0);

            node.Hovered = true;
            engine.ComputeStyle(node, 0);
            var atEnd = engine.ComputeStyles(node, 2);
            Assert.Equal("white", atEnd["color"]);
        }

        [Fact]
        public void TickAdvancesClock()
        {
            var engine = new CssEngine();
            Assert.Equal(0, engine.CurrentTime);
            engine.Tick(0.25);
            Assert.Equal(0.25, engine.CurrentTime, 3);
            engine.Tick(System.TimeSpan.FromSeconds(0.25));
            Assert.Equal(0.5, engine.CurrentTime, 3);
        }

        [Fact]
        public void FocusRuleApplies()
        {
            var css = "input { border-color: gray; } input:focus { border-color: blue; }";
            var engine = new CssEngine();
            engine.AddStylesheet(css);

            var node = new Node<HTMLInputElementProps>(typeof(HTMLInputElement), new HTMLInputElementProps());
            engine.Focus(node);

            var style = engine.ComputeStyles(node);
            Assert.Equal("blue", style["border-color"]);
        }

        [Fact]
        public void GlobalPseudoStateWorks()
        {
            var css = "button { color: black; } button:hover { color: red; }";
            var engine = new CssEngine();
            engine.AddStylesheet(css);

            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.SetPseudoState("hover");

            var style = engine.ComputeStyles(node);
            Assert.Equal("red", style["color"]);
        }

        [Fact]
        public void ComputeStylePseudoOverloadWorks()
        {
            var css = "button { color: black; } button:hover { color: red; }";
            var engine = new CssEngine();
            engine.AddStylesheet(css);

            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            var style = engine.ComputeStyles(node, ":hover");
            Assert.Equal("red", style["color"]);
        }

        [Fact]
        public void NumericTransitionInterpolates()
        {
            var css = "div { opacity: 0; transition: opacity 1s linear; } div:hover { opacity: 1; }";
            var engine = new CssEngine();
            engine.AddStylesheet(css);

            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.ComputeStyle(node, 0);

            node.Hovered = true;
            engine.ComputeStyle(node, 0);
            var atHalf = engine.ComputeStyles(node, 0.5);
            Assert.Contains("0.5", atHalf["opacity"]);
        }
    }
}
