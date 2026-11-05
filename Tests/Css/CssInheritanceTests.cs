using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;

namespace Tests
{
    public class CssInheritanceTests
    {
        [Fact]
        public void Color_InheritsToChild()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(".content { color: white; }");
            var content = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "content" });
            var p = new Node<HTMLParagraphElementProps>(typeof(HTMLParagraphElement), new HTMLParagraphElementProps());
            content.Children = new[] { p };
            engine.ApplyComputedStyles(content, 800, 600, Orientation.Portrait);
            Assert.Equal("white", p.ComputedStyles["color"]);
        }

        [Fact]
        public void Var_RootResolves()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(":root { --topbar-height: 60px; } .content { top: var(--topbar-height); }");
            var root = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            var content = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "content" });
            root.Children = new[] { content };
            engine.ApplyComputedStyles(root, 800, 600, Orientation.Portrait);
            Assert.Equal("60px", content.ComputedStyles["top"]);
        }
    }
}
