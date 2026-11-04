using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLIframeElement
    {}

    public class HTMLIframeElementProps : IntrinsicAttributes
    {
        public string Src { get; set; }
        public string SrcDoc { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Sandbox { get; set; }
        public string Allow { get; set; }
        public string Loading { get; set; }
        public string ReferrerPolicy { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Iframe element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLIframeElementProps> Iframe(HTMLIframeElementProps props, params Node[] children)
        {
            return (Node<HTMLIframeElementProps>) H("iframe", props, children);
        }

        /// <summary>
        /// Shorthand method to render a IFrame element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLIframeElementProps> IFrame(HTMLIframeElementProps props, params Node[] children)
        {
            return (Node<HTMLIframeElementProps>) H("iframe", props, children);
        }

    }
}
