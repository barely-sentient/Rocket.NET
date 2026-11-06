using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLHeadingElement
    {}

    public class HTMLHeadingElementProps : IntrinsicAttributes
    {
        public string TextContent { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Heading element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHeadingElementProps> Heading(HTMLHeadingElementProps props, params Node[] children)
        {
            return new Node<HTMLHeadingElementProps>(typeof(HTMLHeadingElement), props, children);
        }

        /// <summary>
        /// Shorthand method to render a H1 element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHeadingElementProps> H1(HTMLHeadingElementProps props, params Node[] children)
        {
            return new Node<HTMLHeadingElementProps>(typeof(HTMLHeadingElement), props, children);
        }

        /// <summary>
        /// Shorthand method to render a H2 element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHeadingElementProps> H2(HTMLHeadingElementProps props, params Node[] children)
        {
            return new Node<HTMLHeadingElementProps>(typeof(HTMLHeadingElement), props, children);
        }

        /// <summary>
        /// Shorthand method to render a H3 element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHeadingElementProps> H3(HTMLHeadingElementProps props, params Node[] children)
        {
            return new Node<HTMLHeadingElementProps>(typeof(HTMLHeadingElement), props, children);
        }

        /// <summary>
        /// Shorthand method to render a H4 element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHeadingElementProps> H4(HTMLHeadingElementProps props, params Node[] children)
        {
            return new Node<HTMLHeadingElementProps>(typeof(HTMLHeadingElement), props, children);
        }

        /// <summary>
        /// Shorthand method to render a H5 element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHeadingElementProps> H5(HTMLHeadingElementProps props, params Node[] children)
        {
            return new Node<HTMLHeadingElementProps>(typeof(HTMLHeadingElement), props, children);
        }

        /// <summary>
        /// Shorthand method to render a H6 element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLHeadingElementProps> H6(HTMLHeadingElementProps props, params Node[] children)
        {
            return new Node<HTMLHeadingElementProps>(typeof(HTMLHeadingElement), props, children);
        }

    }
}
