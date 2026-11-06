using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;

namespace Tests
{
    public class GrammarUsageTests
    {
        [Fact]
        public void FromCss_BlocklessAtRule_DoesNotSwallowFollowingRule()
        {
            var sheet = Stylesheet.FromCss("@import url('base.css'); .a { color: red; }");
            var rule = Assert.Single(sheet.Rules);
            Assert.Equal(".a", rule.Selector);
            Assert.Equal("red", rule.Declarations["color"]);
        }

        [Fact]
        public void CssRule_KeepsSemicolonsInsideUrlValues()
        {
            var rule = new CssRule(".a", "background-image: url(data:image/png;base64,AAAA); color: red");
            Assert.Equal("url(data:image/png;base64,AAAA)", rule.Declarations["background-image"]);
            Assert.Equal("red", rule.Declarations["color"]);
        }

        [Fact]
        public void ApplyComputedStyles_KeepsSemicolonsInsideUrlValues()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(".a { background-image: url(data:image/png;base64,AAAA); }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "a" });
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("url(data:image/png;base64,AAAA)", node.ComputedStyles["background-image"]);
        }
    }
}
