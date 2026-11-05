using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;

namespace Tests
{
    public class LayoutPositionTests
    {
        [Fact]
        public void RelativeParent_OffsetsStaticChild()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(".parent { position: relative; left: 50px; top: 60px; width: 200px; height: 200px; } .child { width: 40px; height: 40px; }");
            var parent = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "parent" });
            var child = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "child" });
            parent.Children = new[] { child };
            engine.CalculateLayout(parent, 800, 600, Orientation.Portrait);
            Assert.Equal(50, parent.Layout.X);
            Assert.Equal(60, parent.Layout.Y);
            Assert.Equal(50, child.Layout.X);
            Assert.Equal(60, child.Layout.Y);
        }

        [Fact]
        public void AbsoluteChild_UsesNearestPositionedAncestorAsContainingBlock()
        {
            var css = @"
                .outer { position: relative; left: 10px; top: 20px; width: 300px; height: 300px; }
                .inner { width: 100px; height: 100px; }
                .abs { position: absolute; left: 5px; top: 5px; width: 20px; height: 20px; }
            ";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var outer = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "outer" });
            var inner = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "inner" });
            var abs = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "abs" });
            inner.Children = new[] { abs };
            outer.Children = new[] { inner };
            engine.CalculateLayout(outer, 800, 600, Orientation.Portrait);
            // containing block for abs should be outer (nearest positioned ancestor), not inner
            Assert.Equal(outer.Layout.X + 5, abs.Layout.X);
            Assert.Equal(outer.Layout.Y + 5, abs.Layout.Y);
        }

        [Fact]
        public void AbsoluteEdges_DeriveSize()
        {
            var css = ".page { position: fixed; top: 0px; left: 0px; right: 0px; bottom: 0px; }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var page = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "page" });
            engine.CalculateLayout(page, 800, 600, Orientation.Portrait);
            Assert.Equal(0, page.Layout.X);
            Assert.Equal(0, page.Layout.Y);
            Assert.Equal(800, page.Layout.Width);
            Assert.Equal(600, page.Layout.Height);
        }

        [Fact]
        public void AbsoluteTopBottom_DeriveHeight()
        {
            var css = ".parent { position: relative; width: 200px; height: 200px; } .box { position: absolute; top: 10px; bottom: 20px; left: 0px; right: 0px; }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var parent = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "parent" });
            var box = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "box" });
            parent.Children = new[] { box };
            engine.CalculateLayout(parent, 800, 600, Orientation.Portrait);
            Assert.Equal(parent.Layout.Y + 10, box.Layout.Y);
            Assert.Equal(200 - 10 - 20, box.Layout.Height);
        }

        [Fact]
        public void CssVariable_InheritsAndResolvesInLayout()
        {
            var css = @"
                .page { position: fixed; top: 0px; left: 0px; right: 0px; bottom: 0px; --topbar-height: 60px; }
                .content { position: absolute; top: var(--topbar-height); left: 0px; right: 0px; bottom: 0px; }
            ";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var page = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "page" });
            var content = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "content" });
            page.Children = new[] { content };
            engine.CalculateLayout(page, 800, 600, Orientation.Portrait);
            Assert.Equal(60, content.Layout.Y);
            Assert.Equal(600 - 60, content.Layout.Height);
            Assert.Equal("60px", content.ComputedStyles["top"]);
        }

        [Fact]
        public void CssVariable_FallbackUsedWhenMissing()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(".x { top: var(--missing, 12px); }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "x" });
            engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
            Assert.Equal("12px", node.ComputedStyles["top"]);
        }

        [Fact]
        public void CssVariable_ComputeStylesResolves()
        {
            var css = ".a { --x: 60px; } .b { top: var(--x); }";
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(css);
            var parent = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "a" });
            var child = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "b" });
            parent.Children = new[] { child };
            engine.ApplyComputedStyles(parent, 800, 600, Orientation.Portrait);
            Assert.Equal("60px", child.ComputedStyles["top"]);
        }
    }
}
