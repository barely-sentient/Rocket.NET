using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;
using static Rocket.Core.UI;

namespace Tests
{
    public class ReproButton
    {
        [Fact]
        public void ButtonWithItalic()
        {
            var renderer = new HtmlRenderer();
            var executor = new VirtualDomExecutor(renderer);
            var root = executor.MountRoot(
                Button(new() { TextContent = "Click " },
                    Italic(new() { TextContent = "me" })
                )
            );
            executor.Render(root, 400, 300, VantaCSS.Orientation.Landscape);
            var html = renderer.GetHtml();
            throw new Exception("HTML:\n" + html);
        }
    }
}
