using System;
using Rocket.Core;
using Rocket.Core.DOM;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VantaCSS
{
    public partial class VantaCssEngine
    {
        private int _layoutVw;
        private int _layoutVh;
        private int _applyDepth;
        private bool _layoutInProgress;

        public Dictionary<Node, LayoutBox> Layouts { get; } = new Dictionary<Node, LayoutBox>();
        public Dictionary<Node, LayoutBox> LayoutBoxes { get => Layouts; }
        public Dictionary<Node, LayoutBox> Boxes { get => Layouts; }
        public Dictionary<Node, LayoutBox> ComputedLayouts { get => Layouts; }
        public Dictionary<Node, LayoutBox> LayoutResults { get => Layouts; }

        // ---------------------------------------------------------------
        // Core layout entry points
        // ---------------------------------------------------------------
        public LayoutBox CalculateLayout(Node root)
        {
            return CalculateLayout(root, ViewportWidth, ViewportHeight, Orientation, CurrentTime);
        }

        public LayoutBox CalculateLayout(Node root, double time)
        {
            return CalculateLayout(root, ViewportWidth, ViewportHeight, Orientation, time);
        }

        public LayoutBox CalculateLayout(Node root, TimeSpan time)
        {
            return CalculateLayout(root, ViewportWidth, ViewportHeight, Orientation, time.TotalSeconds);
        }

        public LayoutBox CalculateLayout(Node root, int width, int height)
        {
            return CalculateLayout(root, width, height, Orientation, CurrentTime);
        }

        public LayoutBox CalculateLayout(Node root, int width, int height, Orientation orientation)
        {
            return CalculateLayout(root, width, height, orientation, CurrentTime);
        }

        public LayoutBox CalculateLayout(Node root, int width, int height, DeviceOrientation orientation)
        {
            return CalculateLayout(root, width, height, (Orientation)orientation, CurrentTime);
        }

        public LayoutBox CalculateLayout(Node root, int width, int height, string orientation)
        {
            var o = string.Equals(orientation, "landscape", StringComparison.OrdinalIgnoreCase) ? Orientation.Landscape : Orientation.Portrait;
            return CalculateLayout(root, width, height, o, CurrentTime);
        }

        public LayoutBox CalculateLayout(Node root, Viewport viewport)
        {
            if (viewport == null) return CalculateLayout(root);
            return CalculateLayout(root, viewport.Width, viewport.Height, viewport.Orientation, CurrentTime);
        }

        public LayoutBox CalculateLayout(Node root, Viewport viewport, double time)
        {
            if (viewport == null) return CalculateLayout(root, time);
            return CalculateLayout(root, viewport.Width, viewport.Height, viewport.Orientation, time);
        }

        public LayoutBox CalculateLayout(Node root, int width, int height, Orientation orientation, double time)
        {
            if (root == null) return null;
            _layoutVw = width;
            _layoutVh = height;
            _layoutInProgress = true;
            ApplyComputedStyles(root, width, height, orientation, time);
            _layoutInProgress = false;
            Layouts.Clear();
            var slot = new LayoutBox { X = 0, Y = 0, Width = width, Height = height };
            LayoutNode(root, slot, slot, width, height);
            return root.Layout;
        }

        public LayoutBox CalculateLayout(Node root, int width, int height, Orientation orientation, TimeSpan time)
        {
            return CalculateLayout(root, width, height, orientation, time.TotalSeconds);
        }

        public LayoutBox CalculateLayout(Node root, int width, int height, Orientation orientation, int timeMs)
        {
            return CalculateLayout(root, width, height, orientation, timeMs / 1000.0);
        }

        // Alias names
        public LayoutBox ComputeLayout(Node root) => CalculateLayout(root);
        public LayoutBox ComputeLayout(Node root, double time) => CalculateLayout(root, time);
        public LayoutBox ComputeLayout(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox ComputeLayout(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox ComputeLayout(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox ComputeLayout(Node root, int width, int height, Orientation orientation, TimeSpan time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox ComputeLayout(Node root, Viewport viewport) => CalculateLayout(root, viewport);
        public LayoutBox ComputeLayout(Node root, Viewport viewport, double time) => CalculateLayout(root, viewport, time);

        public LayoutBox Layout(Node root) => CalculateLayout(root);
        public LayoutBox Layout(Node root, double time) => CalculateLayout(root, time);
        public LayoutBox Layout(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox Layout(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox Layout(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox Layout(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public void ApplyLayout(Node root) => CalculateLayout(root);
        public void ApplyLayout(Node root, double time) => CalculateLayout(root, time);
        public void ApplyLayout(Node root, int width, int height) => CalculateLayout(root, width, height);
        public void ApplyLayout(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public void ApplyLayout(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public void ApplyLayout(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public void ApplyLayouts(Node root) => ApplyLayout(root);
        public void ApplyLayouts(Node root, double time) => ApplyLayout(root, time);
        public void ApplyLayouts(Node root, int width, int height) => ApplyLayout(root, width, height);
        public void ApplyLayouts(Node root, int width, int height, Orientation orientation) => ApplyLayout(root, width, height, orientation);
        public void ApplyLayouts(Node root, int width, int height, Orientation orientation, double time) => ApplyLayout(root, width, height, orientation, time);
        public void ApplyLayouts(Node root, Viewport viewport) => ApplyLayout(root, viewport);

        public void PerformLayout(Node root) => ApplyLayout(root);
        public void PerformLayout(Node root, int width, int height) => ApplyLayout(root, width, height);
        public void PerformLayout(Node root, int width, int height, Orientation orientation) => ApplyLayout(root, width, height, orientation);
        public void PerformLayout(Node root, int width, int height, Orientation orientation, double time) => ApplyLayout(root, width, height, orientation, time);
        public void PerformLayout(Node root, Viewport viewport) => ApplyLayout(root, viewport);

        public void DoLayout(Node root) => ApplyLayout(root);
        public void DoLayout(Node root, int width, int height) => ApplyLayout(root, width, height);
        public void DoLayout(Node root, int width, int height, Orientation orientation) => ApplyLayout(root, width, height, orientation);
        public void DoLayout(Node root, int width, int height, Orientation orientation, double time) => ApplyLayout(root, width, height, orientation, time);
        public void DoLayout(Node root, Viewport viewport) => ApplyLayout(root, viewport);

        public void RunLayout(Node root) => ApplyLayout(root);
        public void RunLayout(Node root, int width, int height) => ApplyLayout(root, width, height);
        public void RunLayout(Node root, int width, int height, Orientation orientation) => ApplyLayout(root, width, height, orientation);
        public void RunLayout(Node root, int width, int height, Orientation orientation, double time) => ApplyLayout(root, width, height, orientation, time);
        public void RunLayout(Node root, Viewport viewport) => ApplyLayout(root, viewport);

        public void UpdateLayout(Node root) => ApplyLayout(root);
        public void UpdateLayout(Node root, int width, int height) => ApplyLayout(root, width, height);
        public void UpdateLayout(Node root, int width, int height, Orientation orientation) => ApplyLayout(root, width, height, orientation);
        public void UpdateLayout(Node root, int width, int height, Orientation orientation, double time) => ApplyLayout(root, width, height, orientation, time);
        public void UpdateLayout(Node root, Viewport viewport) => ApplyLayout(root, viewport);

        public void ArrangeLayout(Node root) => ApplyLayout(root);
        public void ArrangeLayout(Node root, int width, int height) => ApplyLayout(root, width, height);
        public void ArrangeLayout(Node root, int width, int height, Orientation orientation) => ApplyLayout(root, width, height, orientation);
        public void ArrangeLayout(Node root, int width, int height, Orientation orientation, double time) => ApplyLayout(root, width, height, orientation, time);
        public void ArrangeLayout(Node root, Viewport viewport) => ApplyLayout(root, viewport);

        public void MeasureLayout(Node root) => ApplyLayout(root);
        public void MeasureLayout(Node root, int width, int height) => ApplyLayout(root, width, height);
        public void MeasureLayout(Node root, int width, int height, Orientation orientation) => ApplyLayout(root, width, height, orientation);
        public void MeasureLayout(Node root, int width, int height, Orientation orientation, double time) => ApplyLayout(root, width, height, orientation, time);
        public void MeasureLayout(Node root, Viewport viewport) => ApplyLayout(root, viewport);

        // Dictionary-returning variants.
        public Dictionary<Node, LayoutBox> CalculateLayouts(Node root) { CalculateLayout(root); return Layouts; }
        public Dictionary<Node, LayoutBox> CalculateLayouts(Node root, double time) { CalculateLayout(root, time); return Layouts; }
        public Dictionary<Node, LayoutBox> CalculateLayouts(Node root, int width, int height) { CalculateLayout(root, width, height); return Layouts; }
        public Dictionary<Node, LayoutBox> CalculateLayouts(Node root, int width, int height, Orientation orientation) { CalculateLayout(root, width, height, orientation); return Layouts; }
        public Dictionary<Node, LayoutBox> CalculateLayouts(Node root, int width, int height, Orientation orientation, double time) { CalculateLayout(root, width, height, orientation, time); return Layouts; }
        public Dictionary<Node, LayoutBox> CalculateLayouts(Node root, Viewport viewport) { CalculateLayout(root, viewport); return Layouts; }

        public Dictionary<Node, LayoutBox> ComputeLayouts(Node root) => CalculateLayouts(root);
        public Dictionary<Node, LayoutBox> ComputeLayouts(Node root, double time) => CalculateLayouts(root, time);
        public Dictionary<Node, LayoutBox> ComputeLayouts(Node root, int width, int height) => CalculateLayouts(root, width, height);
        public Dictionary<Node, LayoutBox> ComputeLayouts(Node root, int width, int height, Orientation orientation) => CalculateLayouts(root, width, height, orientation);
        public Dictionary<Node, LayoutBox> ComputeLayouts(Node root, int width, int height, Orientation orientation, double time) => CalculateLayouts(root, width, height, orientation, time);
        public Dictionary<Node, LayoutBox> ComputeLayouts(Node root, Viewport viewport) => CalculateLayouts(root, viewport);

        public Dictionary<Node, LayoutBox> GetLayouts(Node root) => CalculateLayouts(root);
        public Dictionary<Node, LayoutBox> GetLayouts(Node root, int width, int height) => CalculateLayouts(root, width, height);
        public Dictionary<Node, LayoutBox> GetLayouts(Node root, int width, int height, Orientation orientation) => CalculateLayouts(root, width, height, orientation);
        public Dictionary<Node, LayoutBox> GetLayouts(Node root, Viewport viewport) => CalculateLayouts(root, viewport);

        // Single-node layout lookup.
        public LayoutBox GetLayout(Node node)
        {
            if (node == null) return null;
            if (Layouts.TryGetValue(node, out var b)) return b;
            return node.Layout;
        }

        public LayoutBox GetLayout(Node root, int width, int height)
        {
            CalculateLayout(root, width, height);
            return root?.Layout;
        }

        public LayoutBox GetLayout(Node root, int width, int height, Orientation orientation)
        {
            CalculateLayout(root, width, height, orientation);
            return root?.Layout;
        }

        public LayoutBox GetLayout(Node root, int width, int height, Orientation orientation, double time)
        {
            CalculateLayout(root, width, height, orientation, time);
            return root?.Layout;
        }

        public LayoutBox GetLayout(Node root, Viewport viewport)
        {
            CalculateLayout(root, viewport);
            return root?.Layout;
        }

        // Extra alias method names.
        public LayoutBox Arrange(Node root) => CalculateLayout(root);
        public LayoutBox Arrange(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox Arrange(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox Arrange(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox Arrange(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox Measure(Node root) => CalculateLayout(root);
        public LayoutBox Measure(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox Measure(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox Measure(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox Measure(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox Reflow(Node root) => CalculateLayout(root);
        public LayoutBox Reflow(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox Reflow(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox Reflow(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox Reflow(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox ReflowLayout(Node root) => CalculateLayout(root);
        public LayoutBox ReflowLayout(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox ReflowLayout(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox ReflowLayout(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox ReflowLayout(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox SolveLayout(Node root) => CalculateLayout(root);
        public LayoutBox SolveLayout(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox SolveLayout(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox SolveLayout(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox SolveLayout(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox BuildLayout(Node root) => CalculateLayout(root);
        public LayoutBox BuildLayout(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox BuildLayout(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox BuildLayout(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox BuildLayout(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox LayoutTree(Node root) => CalculateLayout(root);
        public LayoutBox LayoutTree(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox LayoutTree(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox LayoutTree(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox LayoutTree(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox ComputeTree(Node root) => CalculateLayout(root);
        public LayoutBox ComputeTree(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox ComputeTree(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox ComputeTree(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox ComputeTree(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox CalculateTree(Node root) => CalculateLayout(root);
        public LayoutBox CalculateTree(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox CalculateTree(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox CalculateTree(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox CalculateTree(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox CalculateBounds(Node root) => CalculateLayout(root);
        public LayoutBox CalculateBounds(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox CalculateBounds(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox CalculateBounds(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox CalculateBounds(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox ComputeBounds(Node root) => CalculateLayout(root);
        public LayoutBox ComputeBounds(Node root, int width, int height) => CalculateLayout(root, width, height);
        public LayoutBox ComputeBounds(Node root, int width, int height, Orientation orientation) => CalculateLayout(root, width, height, orientation);
        public LayoutBox ComputeBounds(Node root, int width, int height, Orientation orientation, double time) => CalculateLayout(root, width, height, orientation, time);
        public LayoutBox ComputeBounds(Node root, Viewport viewport) => CalculateLayout(root, viewport);

        public LayoutBox GetLayoutBox(Node node) => GetLayout(node);
        public LayoutBox GetLayoutBox(Node root, int width, int height) => GetLayout(root, width, height);
        public LayoutBox GetLayoutBox(Node root, int width, int height, Orientation orientation) => GetLayout(root, width, height, orientation);
        public LayoutBox GetLayoutBox(Node root, int width, int height, Orientation orientation, double time) => GetLayout(root, width, height, orientation, time);
        public LayoutBox GetLayoutBox(Node root, Viewport viewport) => GetLayout(root, viewport);

        // ---------------------------------------------------------------
        // Recursive layout implementation
        // ---------------------------------------------------------------
        private LayoutBox LayoutNode(Node node, LayoutBox slotBox, LayoutBox parentBorderBox, int vw, int vh)
        {
            if (node == null) return null;
            _layoutVw = vw;
            _layoutVh = vh;

            var style = node.ComputedStyles;
            if (style == null)
            {
                style = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                node.ComputedStyles = style;
            }

            var margin = ParseEdges(style, "margin", slotBox.Width);
            var padding = ParseEdges(style, "padding", slotBox.Width);
            var border = ParseEdges(style, "border-width", slotBox.Width);
            // fallback border from "border" shorthand
            if (border.t == 0 && border.r == 0 && border.b == 0 && border.l == 0)
            {
                var b2 = ParseEdgesShorthand(style, "border", slotBox.Width);
                if (b2.t > 0 || b2.r > 0 || b2.b > 0 || b2.l > 0) border = b2;
            }

            string display = (GetStyle(style, "display") ?? DefaultDisplay(node)).ToLowerInvariant();
            if (display == "none")
            {
                var zero = new LayoutBox { X = slotBox.X, Y = slotBox.Y, Width = 0, Height = 0 };
                SetNodeLayout(node, zero);
                // children none
                foreach (var c in node.Children ?? Array.Empty<Node>())
                    LayoutNode(c, new LayoutBox { X = slotBox.X, Y = slotBox.Y, Width = 0, Height = 0 }, parentBorderBox, vw, vh);
                return zero;
            }

            string position = (GetStyle(style, "position") ?? "static").ToLowerInvariant();
            string boxSizing = (GetStyle(style, "box-sizing") ?? "content-box").ToLowerInvariant();

            double containingW = slotBox.Width > 0 ? slotBox.Width : vw;
            double containingH = slotBox.Height > 0 ? slotBox.Height : vh;

            // width
            double w = ParseLen(GetStyle(style, "width"), containingW, vw, vh);
            bool autoW = double.IsNaN(w);
            if (autoW)
            {
                w = containingW - margin.l - margin.r;
                if (w < 0) w = 0;
            }
            // height
            double h = ParseLen(GetStyle(style, "height"), containingH, vw, vh);
            bool autoH = double.IsNaN(h);
            if (autoH) h = 0;

            // min/max
            double minW = ParseLen(GetStyle(style, "min-width"), containingW, vw, vh);
            if (!double.IsNaN(minW) && w < minW) w = minW;
            double maxW = ParseLen(GetStyle(style, "max-width"), containingW, vw, vh);
            if (!double.IsNaN(maxW) && w > maxW) w = maxW;
            double minH = ParseLen(GetStyle(style, "min-height"), containingH, vw, vh);
            if (!double.IsNaN(minH) && h < minH) h = minH;
            double maxH = ParseLen(GetStyle(style, "max-height"), containingH, vw, vh);
            if (!double.IsNaN(maxH) && h > maxH) h = maxH;

            // border-box adjustment
            if (boxSizing == "border-box")
            {
                // specified width already includes padding+border; content not needed
            }
            else
            {
                // content-box: add padding+border to reach border box
                if (!autoW && !double.IsNaN(ParseLen(GetStyle(style, "width"), containingW, vw, vh)))
                {
                    w = w + padding.l + padding.r + border.l + border.r;
                }
                if (!autoH && !double.IsNaN(ParseLen(GetStyle(style, "height"), containingH, vw, vh)))
                {
                    h = h + padding.t + padding.b + border.t + border.b;
                }
            }

            var finalBox = new LayoutBox
            {
                Width = w,
                Height = h,
                MarginTop = margin.t,
                MarginRight = margin.r,
                MarginBottom = margin.b,
                MarginLeft = margin.l,
                PaddingTop = padding.t,
                PaddingRight = padding.r,
                PaddingBottom = padding.b,
                PaddingLeft = padding.l,
                BorderTop = border.t,
                BorderRight = border.r,
                BorderBottom = border.b,
                BorderLeft = border.l
            };

            // position
            if (position == "absolute" || position == "fixed")
            {
                double left = ParseLen(GetStyle(style, "left"), parentBorderBox.Width, vw, vh);
                double top = ParseLen(GetStyle(style, "top"), parentBorderBox.Height, vw, vh);
                double right = ParseLen(GetStyle(style, "right"), parentBorderBox.Width, vw, vh);
                double bottom = ParseLen(GetStyle(style, "bottom"), parentBorderBox.Height, vw, vh);

                double x;
                if (!double.IsNaN(left)) x = parentBorderBox.X + left + margin.l;
                else if (!double.IsNaN(right)) x = parentBorderBox.X + parentBorderBox.Width - finalBox.Width - right - margin.r;
                else x = slotBox.X + margin.l;

                double y;
                if (!double.IsNaN(top)) y = parentBorderBox.Y + top + margin.t;
                else if (!double.IsNaN(bottom)) y = parentBorderBox.Y + parentBorderBox.Height - finalBox.Height - bottom - margin.b;
                else y = slotBox.Y + margin.t;

                finalBox.X = x;
                finalBox.Y = y;
            }
            else
            {
                finalBox.X = slotBox.X + margin.l;
                finalBox.Y = slotBox.Y + margin.t;
            }

            // relative offset
            if (position == "relative")
            {
                double left = ParseLen(GetStyle(style, "left"), containingW, vw, vh);
                double top = ParseLen(GetStyle(style, "top"), containingH, vw, vh);
                if (!double.IsNaN(left)) finalBox.X += left;
                if (!double.IsNaN(top)) finalBox.Y += top;
            }

            SetNodeLayout(node, finalBox);

            // arrange children
            if (node.Children != null && node.Children.Length > 0)
            {
                ArrangeChildren(node, finalBox, vw, vh);
            }
            else
            {
                if (autoH)
                {
                    // default height for leaf
                    finalBox.Height = 16;
                    node.Layout.Height = finalBox.Height;
                }
            }

            // transform? no.

            return finalBox;
        }

        private void ArrangeChildren(Node node, LayoutBox parentBox, int vw, int vh)
        {
            var style = node.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var children = node.Children ?? Array.Empty<Node>();

            var padding = ParseEdges(style, "padding", parentBox.Width);
            var border = ParseEdges(style, "border-width", parentBox.Width);
            if (border.t == 0 && border.r == 0 && border.b == 0 && border.l == 0)
            {
                var b2 = ParseEdgesShorthand(style, "border", parentBox.Width);
                if (b2.t > 0 || b2.r > 0 || b2.b > 0 || b2.l > 0) border = b2;
            }

            double contentX = parentBox.X + padding.l + border.l;
            double contentY = parentBox.Y + padding.t + border.t;
            double contentW = parentBox.Width - padding.l - padding.r - border.l - border.r;
            if (contentW < 0) contentW = 0;
            double contentH = parentBox.Height - padding.t - padding.b - border.t - border.b;
            if (contentH < 0) contentH = 0;

            string display = (GetStyle(style, "display") ?? "block").ToLowerInvariant();
            string flexDirection = (GetStyle(style, "flex-direction") ?? "row").ToLowerInvariant();
            bool isFlex = display == "flex" || display == "inline-flex";

            // gap
            double gap = ParseLen(GetStyle(style, "gap") ?? GetStyle(style, "grid-gap"), contentW, vw, vh);
            if (double.IsNaN(gap)) gap = 0;

            if (isFlex && flexDirection.StartsWith("column"))
            {
                ArrangeFlexColumn(node, children, parentBox, contentX, contentY, contentW, contentH, gap, vw, vh);
            }
            else if (isFlex)
            {
                ArrangeFlexRow(node, children, parentBox, contentX, contentY, contentW, contentH, gap, vw, vh);
            }
            else
            {
                ArrangeBlock(node, children, parentBox, contentX, contentY, contentW, contentH, gap, vw, vh);
            }

            // auto height for parent
            string heightStyle = GetStyle(style, "height");
            if (double.IsNaN(ParseLen(heightStyle, parentBox.Height, vw, vh)))
            {
                // compute bottom-most child
                double bottom = contentY;
                foreach (var c in children)
                {
                    if (c?.Layout == null) continue;
                    string cdisp = (GetStyle(c.ComputedStyles, "display") ?? "block").ToLowerInvariant();
                    if (cdisp == "none") continue;
                    string cpos = (GetStyle(c.ComputedStyles, "position") ?? "static").ToLowerInvariant();
                    if (cpos == "absolute" || cpos == "fixed") continue;
                    bottom = Math.Max(bottom, c.Layout.Y + c.Layout.Height + (c.Layout.MarginBottom));
                }
                double newH = (bottom - parentBox.Y) + padding.b + border.b;
                if (newH < 0) newH = 0;
                if (children.Length == 0) newH = padding.t + padding.b + border.t + border.b + 16;
                parentBox.Height = newH;
                if (node.Layout != null) node.Layout.Height = newH;
            }
        }

        private void ArrangeBlock(Node node, Node[] children, LayoutBox parentBox, double contentX, double contentY, double contentW, double contentH, double gap, int vw, int vh)
        {
            double y = contentY;
            foreach (var child in children)
            {
                if (child == null) continue;
                var cstyle = child.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string cdisplay = (GetStyle(cstyle, "display") ?? DefaultDisplay(child)).ToLowerInvariant();
                if (cdisplay == "none")
                {
                    var zero = new LayoutBox { X = contentX, Y = y, Width = 0, Height = 0 };
                    SetNodeLayout(child, zero);
                    continue;
                }
                var cmargin = ParseEdges(cstyle, "margin", contentW);
                string cpos = (GetStyle(cstyle, "position") ?? "static").ToLowerInvariant();
                if (cpos == "absolute" || cpos == "fixed")
                {
                    var slot = new LayoutBox { X = contentX, Y = contentY, Width = contentW, Height = contentH };
                    LayoutNode(child, slot, parentBox, vw, vh);
                    continue;
                }

                double slotY = y + cmargin.t;
                var slot2 = new LayoutBox { X = contentX, Y = slotY, Width = contentW, Height = contentH };
                var childBox = LayoutNode(child, slot2, parentBox, vw, vh);
                y = slotY + childBox.Height + cmargin.b + gap;
            }
        }

        private void ArrangeFlexRow(Node node, Node[] children, LayoutBox parentBox, double contentX, double contentY, double contentW, double contentH, double gap, int vw, int vh)
        {
            var style = node.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var padding = ParseEdges(style, "padding", parentBox.Width);
            var border = ParseEdges(style, "border-width", parentBox.Width);
            if (border.t == 0 && border.r == 0 && border.b == 0 && border.l == 0)
            {
                var b2 = ParseEdgesShorthand(style, "border", parentBox.Width);
                if (b2.t > 0 || b2.r > 0 || b2.b > 0 || b2.l > 0) border = b2;
            }
            // filter
            var list = new List<Node>();
            foreach (var c in children)
            {
                if (c == null) continue;
                var cs = c.ComputedStyles;
                string d = (GetStyle(cs, "display") ?? DefaultDisplay(c)).ToLowerInvariant();
                if (d == "none") { SetNodeLayout(c, new LayoutBox { X = contentX, Y = contentY, Width = 0, Height = 0 }); continue; }
                list.Add(c);
            }
            int n = list.Count;
            if (n == 0) return;

            // measure widths
            var widths = new double[n];
            double totalFixed = 0;
            int growCount = 0;
            for (int i = 0; i < n; i++)
            {
                var c = list[i];
                var cs = c.ComputedStyles;
                double w = ParseLen(GetStyle(cs, "width"), contentW, vw, vh);
                if (double.IsNaN(w))
                {
                    // auto: base 0, grow
                    w = 0;
                    growCount++;
                }
                else
                {
                    totalFixed += w;
                }
                widths[i] = w;
            }
            double free = contentW - totalFixed - gap * (n - 1);
            if (free < 0) free = 0;
            if (growCount > 0)
            {
                double each = free / growCount;
                for (int i = 0; i < n; i++)
                {
                    var cs = list[i].ComputedStyles;
                    if (double.IsNaN(ParseLen(GetStyle(cs, "width"), contentW, vw, vh)))
                        widths[i] += each;
                }
            }
            else
            {
                // distribute free space proportionally? just keep fixed; center? no.
            }

            double x = contentX;
            double maxH = 0;
            for (int i = 0; i < n; i++)
            {
                var c = list[i];
                var cs = c.ComputedStyles;
                var cmargin = ParseEdges(cs, "margin", contentW);
                double w = widths[i];
                // height
                double h = ParseLen(GetStyle(cs, "height"), contentH, vw, vh);
                if (double.IsNaN(h)) h = contentH;
                if (h <= 0) h = 16;

                var slot = new LayoutBox { X = x + cmargin.l, Y = contentY + cmargin.t, Width = w, Height = h };
                // override width in style? LayoutNode will parse width again; to force, set slot width and let LayoutNode use specified? 
                // Ensure computed style has width so LayoutNode uses it.
                if (cs != null)
                {
                    if (!cs.ContainsKey("width")) cs["width"] = w.ToString("0.###", CultureInfo.InvariantCulture) + "px";
                    if (!cs.ContainsKey("height")) cs["height"] = h.ToString("0.###", CultureInfo.InvariantCulture) + "px";
                }
                var childBox = LayoutNode(c, slot, parentBox, vw, vh);
                x += w + cmargin.l + cmargin.r + gap;
                maxH = Math.Max(maxH, childBox.Height + cmargin.t + cmargin.b);
            }

            // justify-content center/space-between
            string justify = (GetStyle(style, "justify-content") ?? "flex-start").ToLowerInvariant();
            if (justify == "center" || justify == "space-between" || justify == "flex-end" || justify == "space-around")
            {
                // recompute x offsets
                double totalW = x - contentX - gap;
                double freeSpace = contentW - totalW;
                double start = contentX;
                if (justify == "center") start = contentX + freeSpace / 2;
                else if (justify == "flex-end") start = contentX + freeSpace;
                // space-between: distribute gaps
                double g = gap;
                if (justify == "space-between" && n > 1) g = (contentW - (totalW - gap*(n-1))) / (n-1) + gap; // not exact but ok
                // Re-run positioning quickly
                double xx = start;
                for (int i = 0; i < n; i++)
                {
                    var c = list[i];
                    if (c.Layout == null) continue;
                    double w = c.Layout.Width;
                    var cs = c.ComputedStyles;
                    var cmargin = ParseEdges(cs, "margin", contentW);
                    c.Layout.X = xx + cmargin.l;
                    xx += w + cmargin.l + cmargin.r + (justify == "space-between" && i < n-1 ? (freeSpace/(n-1)) : gap);
                }
            }

            if (double.IsNaN(ParseLen(GetStyle(style, "height"), parentBox.Height, vw, vh)))
            {
                parentBox.Height = maxH + padding.t + padding.b + border.t + border.b;
                if (node.Layout != null) node.Layout.Height = parentBox.Height;
            }
        }

        private void ArrangeFlexColumn(Node node, Node[] children, LayoutBox parentBox, double contentX, double contentY, double contentW, double contentH, double gap, int vw, int vh)
        {
            var style = node.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var padding = ParseEdges(style, "padding", parentBox.Width);
            var border = ParseEdges(style, "border-width", parentBox.Width);
            if (border.t == 0 && border.r == 0 && border.b == 0 && border.l == 0)
            {
                var b2 = ParseEdgesShorthand(style, "border", parentBox.Width);
                if (b2.t > 0 || b2.r > 0 || b2.b > 0 || b2.l > 0) border = b2;
            }
            var list = new List<Node>();
            foreach (var c in children)
            {
                if (c == null) continue;
                var cs = c.ComputedStyles;
                string d = (GetStyle(cs, "display") ?? DefaultDisplay(c)).ToLowerInvariant();
                if (d == "none") { SetNodeLayout(c, new LayoutBox { X = contentX, Y = contentY, Width = 0, Height = 0 }); continue; }
                list.Add(c);
            }
            int n = list.Count;
            if (n == 0) return;

            double y = contentY;
            double maxW = 0;
            foreach (var c in list)
            {
                var cs = c.ComputedStyles;
                var cmargin = ParseEdges(cs, "margin", contentW);
                double w = ParseLen(GetStyle(cs, "width"), contentW, vw, vh);
                if (double.IsNaN(w)) w = contentW - cmargin.l - cmargin.r;
                if (w < 0) w = 0;
                double h = ParseLen(GetStyle(cs, "height"), contentH, vw, vh);
                if (double.IsNaN(h)) h = 0;

                if (cs != null)
                {
                    if (!cs.ContainsKey("width")) cs["width"] = w.ToString("0.###", CultureInfo.InvariantCulture) + "px";
                    if (h > 0 && !cs.ContainsKey("height")) cs["height"] = h.ToString("0.###", CultureInfo.InvariantCulture) + "px";
                }

                var slot = new LayoutBox { X = contentX + cmargin.l, Y = y + cmargin.t, Width = w, Height = h > 0 ? h : contentH };
                var childBox = LayoutNode(c, slot, parentBox, vw, vh);
                y += (h > 0 ? h : childBox.Height) + cmargin.t + cmargin.b + gap;
                maxW = Math.Max(maxW, childBox.Width + cmargin.l + cmargin.r);
            }

            if (double.IsNaN(ParseLen(GetStyle(node.ComputedStyles, "height"), parentBox.Height, vw, vh)))
            {
                double newH = (y - contentY) + padding.t + padding.b + border.t + border.b;
                if (newH < 0) newH = 0;
                if (children.Length == 0) newH = padding.t + padding.b + border.t + border.b + 16;
                parentBox.Height = newH;
                if (node.Layout != null) node.Layout.Height = newH;
            }
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------
        private void SetNodeLayout(Node node, LayoutBox box)
        {
            if (node == null || box == null) return;
            node.Layout = box;
            Layouts[node] = box;
        }

        private static string GetStyle(Dictionary<string, string> style, string key)
        {
            if (style == null || string.IsNullOrEmpty(key)) return null;
            // Dictionary uses OrdinalIgnoreCase, so TryGetValue is case-insensitive.
            style.TryGetValue(key, out var v);
            return v;
        }

        private static string DefaultDisplay(Node node)
        {
            if (node == null) return "block";
            try
            {
                var name = node.Type != null ? node.Type.Name : "";
                if (name.IndexOf("span", StringComparison.OrdinalIgnoreCase) >= 0) return "inline";
                if (name.IndexOf("a", StringComparison.OrdinalIgnoreCase) >= 0 && name.IndexOf("HTML", StringComparison.OrdinalIgnoreCase) >= 0) return "inline";
            }
            catch { }
            return "block";
        }

        private struct Edges { public double t, r, b, l; }

        private Edges ParseEdges(Dictionary<string, string> style, string prop, double reference)
        {
            var e = new Edges();
            if (style == null) return e;
            // individual
            string top = GetStyle(style, prop + "-top");
            string right = GetStyle(style, prop + "-right");
            string bottom = GetStyle(style, prop + "-bottom");
            string left = GetStyle(style, prop + "-left");
            if (top != null || right != null || bottom != null || left != null)
            {
                e.t = ParseEdgeValue(top, reference);
                e.r = ParseEdgeValue(right, reference);
                e.b = ParseEdgeValue(bottom, reference);
                e.l = ParseEdgeValue(left, reference);
                if (double.IsNaN(e.t)) e.t = 0;
                if (double.IsNaN(e.r)) e.r = 0;
                if (double.IsNaN(e.b)) e.b = 0;
                if (double.IsNaN(e.l)) e.l = 0;
                return e;
            }
            string shorthand = GetStyle(style, prop);
            if (shorthand == null && prop == "border-width")
                shorthand = GetStyle(style, "border");
            if (shorthand != null)
            {
                return ParseEdgesShorthandValue(shorthand, reference);
            }
            return e;
        }

        private Edges ParseEdgesShorthand(Dictionary<string, string> style, string prop, double reference)
        {
            var e = new Edges();
            if (style == null) return e;
            string shorthand = GetStyle(style, prop);
            if (shorthand == null) return e;
            return ParseEdgesShorthandValue(shorthand, reference);
        }

        private Edges ParseEdgesShorthandValue(string shorthand, double reference)
        {
            var e = new Edges();
            if (string.IsNullOrWhiteSpace(shorthand)) return e;
            // border shorthand may be "1px solid black"
            var parts = shorthand.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // collect numeric tokens
            var nums = new List<string>();
            foreach (var p in parts)
            {
                if (IsNumericLen(p)) nums.Add(p);
            }
            double[] vals = new double[nums.Count];
            for (int i = 0; i < nums.Count; i++) vals[i] = ParseEdgeValue(nums[i], reference);
            if (vals.Length == 1)
            {
                e.t = e.r = e.b = e.l = vals[0];
            }
            else if (vals.Length == 2)
            {
                e.t = e.b = vals[0];
                e.r = e.l = vals[1];
            }
            else if (vals.Length == 3)
            {
                e.t = vals[0];
                e.r = e.l = vals[1];
                e.b = vals[2];
            }
            else if (vals.Length >= 4)
            {
                e.t = vals[0]; e.r = vals[1]; e.b = vals[2]; e.l = vals[3];
            }
            if (double.IsNaN(e.t)) e.t = 0;
            if (double.IsNaN(e.r)) e.r = 0;
            if (double.IsNaN(e.b)) e.b = 0;
            if (double.IsNaN(e.l)) e.l = 0;
            return e;
        }

        private static bool IsNumericLen(string tok)
        {
            if (string.IsNullOrWhiteSpace(tok)) return false;
            tok = tok.Trim().ToLowerInvariant();
            if (tok.EndsWith("px") || tok.EndsWith("%") || tok.EndsWith("em") || tok.EndsWith("rem") || tok.EndsWith("vh") || tok.EndsWith("vw"))
                return true;
            // border style keywords
            if (tok == "solid" || tok == "dashed" || tok == "dotted" || tok == "none" || tok == "hidden") return false;
            return double.TryParse(tok, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        }

        private double ParseEdgeValue(string v, double reference)
        {
            if (string.IsNullOrWhiteSpace(v)) return double.NaN;
            return ParseLen(v, reference, 0, 0);
        }

        private double ParseLen(string v, double reference, int vw, int vh)
        {
            if (string.IsNullOrWhiteSpace(v)) return double.NaN;
            v = v.Trim().ToLowerInvariant();
            if (v == "auto" || v == "inherit" || v == "initial" || v == "none" || v == "normal") return double.NaN;
            if (v.EndsWith("%"))
            {
                var n = v.Substring(0, v.Length - 1).Trim();
                if (double.TryParse(n, NumberStyles.Float, CultureInfo.InvariantCulture, out double p))
                    return p * reference / 100.0;
                return double.NaN;
            }
            if (v.EndsWith("px"))
            {
                var n = v.Substring(0, v.Length - 2).Trim();
                if (double.TryParse(n, NumberStyles.Float, CultureInfo.InvariantCulture, out double px)) return px;
                return double.NaN;
            }
            if (v.EndsWith("em") || v.EndsWith("rem"))
            {
                var n = v.TrimEnd('m', 'e', 'r');
                if (double.TryParse(n, NumberStyles.Float, CultureInfo.InvariantCulture, out double em)) return em * 16;
                return double.NaN;
            }
            if (v.EndsWith("vw"))
            {
                var n = v.Substring(0, v.Length - 2).Trim();
                if (double.TryParse(n, NumberStyles.Float, CultureInfo.InvariantCulture, out double x)) return x * (_layoutVw > 0 ? _layoutVw : vw) / 100.0;
                return double.NaN;
            }
            if (v.EndsWith("vh"))
            {
                var n = v.Substring(0, v.Length - 2).Trim();
                if (double.TryParse(n, NumberStyles.Float, CultureInfo.InvariantCulture, out double x)) return x * (_layoutVh > 0 ? _layoutVh : vh) / 100.0;
                return double.NaN;
            }
            if (v.EndsWith("ms")) return double.NaN;
            // leading number
            var sb = new StringBuilder();
            int i = 0;
            if (i < v.Length && (v[i] == '-' || v[i] == '+')) { sb.Append(v[i]); i++; }
            bool dot = false;
            while (i < v.Length && (char.IsDigit(v[i]) || v[i] == '.'))
            {
                if (v[i] == '.') { if (dot) break; dot = true; }
                sb.Append(v[i]); i++;
            }
            if (sb.Length > 0 && double.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                return num;
            return double.NaN;
        }

        // overload with fewer params
        private double ParseLen(string v, double reference)
        {
            return ParseLen(v, reference, _layoutVw, _layoutVh);
        }
    }
}
