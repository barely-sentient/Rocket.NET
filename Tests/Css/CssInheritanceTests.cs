using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;
using static Rocket.Core.UI;

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

        [Fact]
        public void Var_RootResolvesThroughExecutor()
        {
            var renderer = new HtmlRenderer();
            var executor = new VirtualDomExecutor(renderer);
            executor.Stylesheets.Add(new VantaCSS.Stylesheet("app", ":root { --h: 60px; } .c { top: var(--h); }"));
            var root = executor.MountRoot(Div(new() { ClassName = "wrap" }, Div(new() { ClassName = "c" })));
            executor.Render(root, 800, 600, VantaCSS.Orientation.Portrait);
            System.Func<Node, string, Node> find = null;
            find = (n, cls) => {
                if (n == null) return null;
                var cn = n.UntypedProps?.GetType().GetProperty("ClassName")?.GetValue(n.UntypedProps)?.ToString();
                if (cn == cls) return n;
                foreach (var ch in n.Children ?? new Node[0]) { var r = find(ch, cls); if (r != null) return r; }
                return null;
            };
            var c = find(root, "c");
            Assert.NotNull(c);
            Assert.Equal("60px", c.ComputedStyles["top"]);
        }
    }
}
