using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core.Components
{
    public class HTMLAudioElement
    {}

    public class HTMLAudioElementProps : IntrinsicAttributes
    {
        public string Src { get; set; }
        public bool Controls { get; set; }
        public bool Autoplay { get; set; }
        public bool Loop { get; set; }
        public bool Muted { get; set; }
        public string Preload { get; set; }
        public string CrossOrigin { get; set; }
    }
}

namespace Rocket.Core
{
    public static partial class UI
    {
        /// <summary>
        /// Shorthand method to render an audio element
        /// </summary>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node<HTMLAudioElementProps> Audio(HTMLAudioElementProps props, params Node[] children)
        {
            return new Node<HTMLAudioElementProps>(typeof(HTMLAudioElement), props, children);
        }
    }
}
