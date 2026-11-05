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
        /// Gets or sets the top offset of the positioned element in pixels.
        /// </summary>
        public float Top { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the bottom offset of the positioned element in pixels.
        /// </summary>
        public float Bottom { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the left offset of the positioned element in pixels.
        /// </summary>
        public float Left { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the right offset of the positioned element in pixels.
        /// </summary>
        public float Right { get; set; } = 0f;

        /// <summary>
        /// Sets all four offsets (Top, Right, Bottom, Left) simultaneously.
        /// </summary>
        public float Inset
        {
            set
            {
                Top = value;
                Right = value;
                Bottom = value;
                Left = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the top margin in pixels.
        /// </summary>
        public float MarginTop { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the bottom margin in pixels.
        /// </summary>
        public float MarginBottom { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the left margin in pixels.
        /// </summary>
        public float MarginLeft { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the right margin in pixels.
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