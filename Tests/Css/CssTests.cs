using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;

namespace Tests
{
    public class CssTests
    {
        [Fact]
        public void GenerateCss_FiltersByMediaQuery()
        {
            var sheet = new Stylesheet();
            sheet.AddRule("div", "color: red", "@media (min-width: 500px)");
            sheet.AddRule("span", "color: blue");

            var engine = new VantaCSS.VantaCssEngine();
            engine.AddStylesheet(sheet);

            var css = engine.GenerateCss(800, 600, Orientation.Portrait);
            Assert.Contains("color: red", css);
            Assert.Contains("color: blue", css);

            var css2 = engine.GenerateCss(300, 600, Orientation.Portrait);
            Assert.DoesNotContain("color: red", css2);
            Assert.Contains("color: blue", css2);
        }

        [Fact]
        public void FromCss_ParsesMediaBlocks()
        {
            var css = "@media (min-width: 500px) { div { color: red; } } span { color: blue; }";
            var sheet = Stylesheet.FromCss(css);
            Assert.Equal(2, sheet.Rules.Count);
        }

        [Fact]
        public void ApplyComputedStyles_SetsNodeStyle()
        {
            var sheet = new Stylesheet();
            sheet.AddRule("div", "color: red");
            var engine = new VantaCSS.VantaCssEngine();
            engine.AddStylesheet(sheet);

            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "foo" });
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);

            Assert.Contains("color: red", node.ComputedStyle);
            Assert.Equal("red", node.ComputedStyles["color"]);
        }

        [Fact]
        public void Orientation_ParsedFromString()
        {
            var engine = new VantaCSS.VantaCssEngine(800, 600, "landscape");
            Assert.Equal(Orientation.Landscape, engine.Orientation);
        }
    }
}