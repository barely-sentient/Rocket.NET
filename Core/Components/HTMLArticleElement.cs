using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLArticleElement
    {}

    public class HTMLArticleElementProps : IntrinsicAttributes
    {
    }
}


namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render an article
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLArticleElementProps> Article(HTMLArticleElementProps props, params Node[] children)
        {
            return (Node<HTMLArticleElementProps>) H("article", props, children);
        }
    }
}
