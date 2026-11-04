using System;

namespace Rocket.Core
{
    /// <summary>
    /// A computed layout box (position and size) produced by the CSS engine.
    /// </summary>
    public class LayoutBox
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        // Aliases for position.
        public double Left { get => X; set => X = value; }
        public double Top { get => Y; set => Y = value; }
        public double Right { get => X + Width; set { if (Width > 0) X = value - Width; } }
        public double Bottom { get => Y + Height; set { if (Height > 0) Y = value - Height; } }

        // Aliases for size.
        public double W { get => Width; set => Width = value; }
        public double H { get => Height; set => Height = value; }

        // Box-model extras.
        public double MarginTop { get; set; }
        public double MarginRight { get; set; }
        public double MarginBottom { get; set; }
        public double MarginLeft { get; set; }
        public double PaddingTop { get; set; }
        public double PaddingRight { get; set; }
        public double PaddingBottom { get; set; }
        public double PaddingLeft { get; set; }
        public double BorderTop { get; set; }
        public double BorderRight { get; set; }
        public double BorderBottom { get; set; }
        public double BorderLeft { get; set; }

        public double Margin { get => MarginTop; set { MarginTop = MarginRight = MarginBottom = MarginLeft = value; } }
        public double Padding { get => PaddingTop; set { PaddingTop = PaddingRight = PaddingBottom = PaddingLeft = value; } }

        public override string ToString() => $"X={X}, Y={Y}, W={Width}, H={Height}";
    }

    // Aliases for the layout box type.
    public class Layout : LayoutBox { }
    public class Rect : LayoutBox { }
    public class Rectangle : LayoutBox { }
    public class Bounds : LayoutBox { }
    public class Box : LayoutBox { }
    public class Frame : LayoutBox { }
    public class CssLayout : LayoutBox { }
    public class LayoutInfo : LayoutBox { }
    public class LayoutResult : LayoutBox { }
    public class BoxModel : LayoutBox { }
    public class ComputedLayout : LayoutBox { }
    public class ComputedBox : LayoutBox { }
    public class ComputedBounds : LayoutBox { }
    public class ComputedRect : LayoutBox { }
}