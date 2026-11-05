using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;
using static Rocket.Core.UI;

namespace Tests
{
    public class ReproExec
    {
        [Fact]
        public void ExecVarAndColor()
        {
            var renderer = new HtmlRenderer();
            var executor = new VirtualDomExecutor(renderer);
            executor.Stylesheets.Add(new VantaCSS.Stylesheet("app", @"
                :root { --topbar-height: 60px; }
                .page { position: fixed; top: 0px; left: 0px; right: 0px; bottom: 0px; background-color: #000000; }
                .topbar { position: fixed; top: 0px; left: 0px; right: 0px; height: var(--topbar-height); background-color:#111; }
                .content { position: absolute; top: var(--topbar-height); left: 0px; right: 0px; bottom: 0px; color:#fff; }
            "));
            var root = executor.MountRoot(
                Div(new() { ClassName = "page" },
                    Div(new() { ClassName = "topbar" }),
                    Div(new() { ClassName = "content" },
                        Paragraph(new() { TextContent = "Hello" })
                    )
                )
            );
            executor.Render(root, 800, 600, VantaCSS.Orientation.Landscape);
            var page = root;
            // find content
            // root is page
            Assert.NotNull(page.Layout);
            // find content node
            Node content = null, p = null;
            foreach (var c in page.Children) {
                if (c.UntypedProps?.GetType().GetProperty("ClassName")?.GetValue(c.UntypedProps)?.ToString() == "content") content = c;
            }
            // Actually Children may have been replaced; search recursively
            System.Func<Node, string, Node> find = null;
            find = (n, cls) => {
                if (n == null) return null;
                var cn = n.UntypedProps?.GetType().GetProperty("ClassName")?.GetValue(n.UntypedProps)?.ToString();
                if (cn == cls) return n;
                foreach (var ch in n.Children ?? new Node[0]) { var r = find(ch, cls); if (r != null) return r; }
                return null;
            };
            content = find(page, "content");
            foreach (var ch in content.Children ?? new Node[0]) p = ch;
            // p may be paragraph node
            Assert.NotNull(content);
            Assert.Equal("60px", content.ComputedStyles["top"]);
            Assert.Equal(60, content.Layout.Y);
            if (p != null) Assert.Equal("#fff", p.ComputedStyles["color"]);
        }
    }
}
