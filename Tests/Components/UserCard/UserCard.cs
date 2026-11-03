using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using static Rocket.Core.UI;

namespace Tests.Components;

public class UserCard : Component<UserCardProps>
{
    public override Node Render()
    {

        if (Props.IsUserLoggedIn)
        {
            return (
                H("p", new HTMLParagraphElementProps()
                {
                    TextContent = "You're logged in",
                })
            );
        }
        
        return (
            H("div", new HTMLDivElementProps()
            {
                ClassName = "user-card-preview"
            }, 
                H("div", new HTMLDivElementProps()
                {
                    ClassName = "card-main"  
                },
                    H("h1", new HTMLHeadingElementProps()
                    {
                        TextContent = "Hello"
                    }),
                    H("p", new HTMLParagraphElementProps()
                    {
                        TextContent = "You're not logged in"
                    })
                )
            )
        );
    }
}

public class UserCardProps
{
    public bool IsUserLoggedIn { get; set; } = false;
}