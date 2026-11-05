using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLFormElement
    {}

    public class HTMLFormElementProps : IntrinsicAttributes
    {
        public string Action { get; set; }
        public string Method { get; set; }
        public string Enctype { get; set; }
        public string Target { get; set; }
        public bool NoValidate { get; set; }
        public string AutoComplete { get; set; }
        public string Name { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render a Form element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLFormElementProps> Form(HTMLFormElementProps props, params Node[] children)
        {
            return new Node<HTMLFormElementProps>(typeof(HTMLFormElement), props, children);
        }

    }
}
