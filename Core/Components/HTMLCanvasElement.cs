using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLCanvasElement
    {}

    public class HTMLCanvasElementProps : IntrinsicAttributes
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Canvas element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLCanvasElementProps> Canvas(HTMLCanvasElementProps props, params Node[] children)
        {
            return (Node<HTMLCanvasElementProps>) H("canvas", props, children);
        }

    }
}
