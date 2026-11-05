using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;

namespace Tests
{
    public class VantaCssEngineTests
    {
        // ---------- Parsing / stylesheets ----------

        [Fact]
        public void FromCss_ParsesRules()
        {
            var css = "div { color: red; width: 10px; } span { color: blue; }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var sheet = engine.Stylesheets[engine.Stylesheets.Count - 1];
            Assert.True(sheet.Rules.Count >= 2);
        }

        [Fact]
        public void ComputedStyle_AppliesMatchingRule()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { color: red; width: 100px; height: 50px; }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("red", node.ComputedStyles["color"]);
            Assert.Equal("100px", node.ComputedStyles["width"]);
        }

        [Fact]
        public void Variables_AreParsed()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { --x: 10px; color: red; }");
            var sheet = engine.Stylesheets[engine.Stylesheets.Count - 1];
            Assert.Contains(sheet.Rules, r => r.Declarations.ContainsKey("--x"));
        }

        // ---------- Selectors ----------

        [Fact]
        public void ClassSelector_Matches()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(".foo { color: green; }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "foo" });
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("green", node.ComputedStyles["color"]);
        }

        [Fact]
        public void IdSelector_Matches()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("#bar { color: purple; }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { Id = "bar" });
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("purple", node.ComputedStyles["color"]);
        }

        [Fact]
        public void HoverPseudo_AppliesWhenHovered()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("button { color: black; } button:hover { color: red; }");
            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("black", node.ComputedStyles["color"]);

            node.Hovered = true;
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("red", node.ComputedStyles["color"]);
        }

        // ---------- Media queries ----------

        [Fact]
        public void MediaQuery_MinWidth_Filters()
        {
            var css = "@media (min-width: 500px) { div { color: red; } }";
            var engine = new VantaCssEngine(300, 600, "portrait");
            engine.AddStylesheet(css);
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.ApplyComputedStyles(node, 300, 600, Orientation.Portrait);
            Assert.False(node.ComputedStyles.ContainsKey("color"));
        }

        [Fact]
        public void MediaQuery_MinWidth_AppliesWhenWide()
        {
            var css = "@media (min-width: 500px) { div { color: red; } }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("red", node.ComputedStyles["color"]);
        }

        [Fact]
        public void MediaQuery_Orientation()
        {
            var css = "@media (orientation: landscape) { div { color: blue; } }";
            var engine = new VantaCssEngine(800, 600, "landscape");
            engine.AddStylesheet(css);
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Landscape);
            Assert.Equal("blue", node.ComputedStyles["color"]);
        }

        // ---------- Layout ----------

        [Fact]
        public void Layout_ComputesDimensions()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { width: 120px; height: 40px; }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.CalculateLayout(node, 800, 600, Orientation.Portrait);
            Assert.Equal(120, node.Layout.Width);
            Assert.Equal(40, node.Layout.Height);
        }

        [Fact]
        public void Layout_AutoHeight_ExpandsToChildren()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { width: 100px; } .child { height: 30px; }");
            var parent = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            var child = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "child" });
            parent.Children = new[] { child };
            engine.CalculateLayout(parent, 800, 600, Orientation.Portrait);
            Assert.True(parent.Layout.Height >= 30);
        }

        [Fact]
        public void Layout_FlexRow_DistributesWidth()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { display: flex; width: 200px; } .item { }");
            var parent = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            var c1 = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "item" });
            var c2 = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "item" });
            parent.Children = new[] { c1, c2 };
            engine.CalculateLayout(parent, 800, 600, Orientation.Portrait);
            Assert.True(c1.Layout.Width > 0);
            Assert.True(c2.Layout.Width > 0);
        }

        [Fact]
        public void Layout_Margins_AffectPosition()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { margin: 10px; width: 50px; height: 50px; }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.CalculateLayout(node, 800, 600, Orientation.Portrait);
            Assert.Equal(10, node.Layout.X);
            Assert.Equal(10, node.Layout.Y);
        }

        [Fact]
        public void Layout_MinMax_Constraints()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { width: 10px; min-width: 50px; max-width: 80px; height: 10px; }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.CalculateLayout(node, 800, 600, Orientation.Portrait);
            Assert.Equal(50, node.Layout.Width);
        }

        [Fact]
        public void Layout_AbsolutePositioning()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { position: absolute; left: 20px; top: 30px; width: 40px; height: 40px; }");
            var parent = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            parent.Children = new[] { node };
            // parent size
            parent.ComputedStyles = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase) { { "width", "200px" }, { "height", "200px" } };
            engine.CalculateLayout(parent, 800, 600, Orientation.Portrait);
            // child layout computed during parent
            Assert.True(node.Layout != null);
        }

        // ---------- Timing / transitions ----------

        [Fact]
        public void Tick_AdvancesClock()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            Assert.Equal(0, engine.CurrentTime);
            engine.Tick(0.25);
            Assert.Equal(0.25, engine.CurrentTime, 3);
            engine.Tick(System.TimeSpan.FromSeconds(0.25));
            Assert.Equal(0.5, engine.CurrentTime, 3);
        }

        [Fact]
        public void Transition_StartsAtFromValue()
        {
            var css = "button { color: black; transition: color 1s linear; } button:hover { color: white; }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.ComputeStyle(node, 0);

            node.Hovered = true;
            var at0 = engine.ComputeStyles(node, 0);
            Assert.Equal("rgb(0, 0, 0)", at0["color"]);
        }

        [Fact]
        public void Transition_InterpolatesMidpoint()
        {
            var css = "button { color: black; transition: color 1s linear; } button:hover { color: white; }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.ComputeStyle(node, 0);

            node.Hovered = true;
            engine.ComputeStyle(node, 0);
            var atHalf = engine.ComputeStyles(node, 0.5);
            Assert.Contains("128", atHalf["color"]);
        }

        [Fact]
        public void Transition_CompletesAtEnd()
        {
            var css = "button { color: black; transition: color 1s linear; } button:hover { color: white; }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var node = new Node<HTMLButtonElementProps>(typeof(HTMLButtonElement), new HTMLButtonElementProps());
            engine.ComputeStyle(node, 0);

            node.Hovered = true;
            engine.ComputeStyle(node, 0);
            var atEnd = engine.ComputeStyles(node, 2);
            Assert.Equal("white", atEnd["color"]);
        }

        [Fact]
        public void NumericTransition_Interpolates()
        {
            var css = "div { opacity: 0; transition: opacity 1s linear; } div:hover { opacity: 1; }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.ComputeStyle(node, 0);

            node.Hovered = true;
            engine.ComputeStyle(node, 0);
            var atHalf = engine.ComputeStyles(node, 0.5);
            Assert.Contains("0.5", atHalf["opacity"]);
        }

        [Fact]
        public void FocusPseudo_Applies()
        {
            var css = "input { border-color: gray; } input:focus { border-color: blue; }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var node = new Node<HTMLInputElementProps>(typeof(HTMLInputElement), new HTMLInputElementProps());
            engine.Focus(node);
            var style = engine.ComputeStyles(node);
            Assert.Equal("blue", style["border-color"]);
        }

        // ---------- Global defaults ----------

        [Fact]
        public void GlobalDefaults_BoxSizing()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("border-box", node.ComputedStyles["box-sizing"]);
        }
    }
}