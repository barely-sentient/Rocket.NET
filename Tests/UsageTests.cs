using Rocket.Core.DOM;
using Tests.Components;
using Xunit.Abstractions;

namespace Tests;

public class UsageTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UsageTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        var renderer = new HtmlRenderer();
        var props = new UserCardProps { IsUserLoggedIn = false };
        var card = new UserCard { Props = props };
        var node = card.Render();

        renderer.Draw(node);

        _testOutputHelper.WriteLine(renderer.GetHtml());
    }
}