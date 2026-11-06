using System.Collections.Generic;
using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;

namespace Tests
{
    public class StyleCacheTests
    {
        [Fact]
        public void ReassigningSameStylesheetList_DoesNotInvalidateCaches()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            var sheets = new List<Stylesheet> { new Stylesheet("app", "div { color: red; }") };
            engine.Stylesheets = sheets;
            int version = engine.StylesheetVersion;

            engine.Stylesheets = sheets;

            Assert.Equal(version, engine.StylesheetVersion);
        }

        [Fact]
        public void ReassigningStylesheetList_WithNewSheet_RebuildsStyles()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            var sheets = new List<Stylesheet> { new Stylesheet("app", "div { color: red; }") };
            engine.Stylesheets = sheets;
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            Assert.Contains("color: red", engine.ComputeStyle(node, 800, 600, Orientation.Portrait));

            sheets.Add(new Stylesheet("override", "div { color: blue; }"));
            engine.Stylesheets = sheets;

            Assert.Contains("color: blue", engine.ComputeStyle(node, 800, 600, Orientation.Portrait));
        }

        [Fact]
        public void PseudoState_ChangesComputedStyle_AcrossCalls()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { color: red; } div:hover { color: green; }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());

            Assert.Contains("color: red", engine.ComputeStyle(node, 800, 600, Orientation.Portrait));

            node.Hovered = true;
            Assert.Contains("color: green", engine.ComputeStyle(node, 800, 600, Orientation.Portrait));

            node.Hovered = false;
            Assert.Contains("color: red", engine.ComputeStyle(node, 800, 600, Orientation.Portrait));
        }

        [Fact]
        public void DescendantSelector_RespectsAncestorChanges()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet(".outer .inner { color: red; }");
            var outer = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "outer" });
            var inner = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "inner" });
            inner.Parent = outer;

            Assert.Contains("color: red", engine.ComputeStyle(inner, 800, 600, Orientation.Portrait));

            var other = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { ClassName = "other" });
            inner.Parent = other;

            Assert.DoesNotContain("color: red", engine.ComputeStyle(inner, 800, 600, Orientation.Portrait));
        }

        [Fact]
        public void DispatchEvent_InvokesPropertyHandler_ForLowerCaseName()
        {
            var clicks = 0;
            var props = new HTMLDivElementProps { OnClick = _ => clicks++ };
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), props);

            node.DispatchEvent("click", new Event());

            Assert.Equal(1, clicks);
        }

        [Fact]
        public void DispatchEvent_UsesAliasedHandlerName()
        {
            var entered = 0;
            var props = new HTMLDivElementProps { OnMouseEnter = _ => entered++ };
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), props);

            node.DispatchEvent("mouseenter", new Event());

            Assert.Equal(1, entered);
        }

        [Theory]
        [InlineData("30px", 30)]
        [InlineData("50%", 400)]
        [InlineData("10vw", 80)]
        [InlineData("2em", 32)]
        [InlineData("  30PX ", 30)]
        public void Width_ResolvesLengthUnits(string width, double expected)
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet($"div {{ width: {width}; }}");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());

            var box = engine.CalculateLayout(node, 800, 600, Orientation.Portrait, 0.0);

            Assert.Equal(expected, box.Width, 6);
        }

        [Fact]
        public void Width_InvalidLength_FallsBackToAuto()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            engine.AddStylesheet("div { width: 100ms; }");
            var node = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());

            var box = engine.CalculateLayout(node, 800, 600, Orientation.Portrait, 0.0);

            Assert.Equal(800, box.Width, 6);
        }

        [Fact]
        public void Image_WithoutExplicitSize_UsesEncodedDimensions()
        {
            const string png1x1 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==";
            var engine = new VantaCssEngine(800, 600, "portrait");
            var node = new Node<HTMLImageElementProps>(typeof(HTMLImageElement), new HTMLImageElementProps { Src = png1x1 });

            var first = engine.CalculateLayout(node, 800, 600, Orientation.Portrait, 0.0);
            var second = engine.CalculateLayout(node, 800, 600, Orientation.Portrait, 0.0);

            Assert.Equal(1, first.Width, 6);
            Assert.Equal(1, second.Width, 6);
        }

        [Fact]
        public void DispatchEvent_BubblesToParent_WhenNotHandled()
        {
            var clicks = 0;
            var parent = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps { OnClick = _ => clicks++ });
            var child = new Node<HTMLDivElementProps>(typeof(HTMLDivElement), new HTMLDivElementProps());
            child.Parent = parent;

            child.DispatchEvent("click", new Event());

            Assert.Equal(1, clicks);
        }
    }
}
