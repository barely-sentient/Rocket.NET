using VantaCSS.Rules;

namespace VantaCSS.Rules
{
    /// <summary>
    /// Specifies how an element is positioned in a document.
    /// </summary>
    public enum Position : byte
    {
        /// <summary>
        /// The position rule has no bearing on positioning.
        /// </summary>
        Inherit,
        
        /// <summary>
        /// The element is positioned according to the normal flow of the document (default).
        /// </summary>
        Static,

        /// <summary>
        /// The element is positioned according to the normal flow, then offset relative to itself.
        /// </summary>
        Relative,

        /// <summary>
        /// The element is removed from normal flow and positioned relative to its closest positioned ancestor.
        /// </summary>
        Absolute,

        /// <summary>
        /// The element is removed from normal flow and positioned relative to the viewport.
        /// </summary>
        Fixed,

        /// <summary>
        /// The element acts like relative positioning until it crosses a specified threshold, then acts like fixed.
        /// </summary>
        Sticky
    }
}

namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// The positioning mechanism used.
        /// </summary>
        public Position Position { get; set; } = Position.Inherit;

        /// <summary>
        /// Offset top
        /// </summary>
        public float Top { get; set; } = 0f;
        
        /// <summary>
        /// Offset Bottom
        /// </summary>
        public float Bottom { get; set; } = 0f;
        
        /// <summary>
        /// Offset Left
        /// </summary>
        public float Left { get; set; } = 0f;
        
        /// <summary>
        /// Offset Right
        /// </summary>
        public float Right { get; set; } = 0f;
        
        /// <summary>
        /// Spacing between the top of the component, and it's previous vertical item
        /// </summary>
        public float MarginTop { get; set; } = 0f;
        
        /// <summary>
        /// Spacing between the bottom of the component, and it's next vertical item
        /// </summary>
        public float MarginBottom { get; set; } = 0f;
        
        /// <summary>
        /// Spacing between the left of the component, and the element to it's left
        /// </summary>
        public float MarginLeft { get; set; } = 0f;
        
        /// <summary>
        /// Spacing between the right of the component, and the element to it's right
        /// </summary>
        public float MarginRight { get; set; } = 0f;
        
        /// <summary>
        /// Set the margin all the way round
        /// </summary>
        public float Margin
        {
            set
            {
                MarginTop = MarginBottom = value;
                MarginLeft = MarginRight = value;
            }
        }
    }
}