using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLImageElement
    {}

    public class HTMLImageElementProps : IntrinsicAttributes
    {
        public string Src { get; set; }
        public string Alt { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Loading { get; set; }
        public string Decoding { get; set; }
        public string CrossOrigin { get; set; }
        public string ReferrerPolicy { get; set; }
        public string Sizes { get; set; }
        public string SrcSet { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Image element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLImageElementProps> Image(HTMLImageElementProps props, params Node[] children)
        {
            return (Node<HTMLImageElementProps>) H("img", props, children);
        }

        /// <summary>
        /// Shorthand method to render a Img element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLImageElementProps> Img(HTMLImageElementProps props, params Node[] children)
        {
            return (Node<HTMLImageElementProps>) H("img", props, children);
        }

    }
}
