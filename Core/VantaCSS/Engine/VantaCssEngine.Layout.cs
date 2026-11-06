using System;
using Rocket.Core.DOM;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

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

            // Inherit CSS custom properties from the parent node.
            if (node.Parent != null && node.Parent.ComputedStyles != null)
            {
                foreach (var kv in node.Parent.ComputedStyles)
                {
                    if (kv.Key.StartsWith("--", StringComparison.Ordinal) && !style.ContainsKey(kv.Key))
                        style[kv.Key] = kv.Value;
                }
            }

            // Resolve var() references in all values.
            var styleKeys = new List<string>(style.Keys);
            foreach (var k in styleKeys)
            {
                style[k] = ResolveVarValue(style[k], node);
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

            bool isAbs = position == "absolute" || position == "fixed";

            double containingW;
            double containingH;
            if (isAbs)
            {
                containingW = parentBorderBox != null && parentBorderBox.Width > 0 ? parentBorderBox.Width : vw;
                containingH = parentBorderBox != null && parentBorderBox.Height > 0 ? parentBorderBox.Height : vh;
            }
            else
            {
                containingW = slotBox.Width > 0 ? slotBox.Width : vw;
                containingH = slotBox.Height > 0 ? slotBox.Height : vh;
            }

            // offsets for positioned elements
            double leftCss = double.NaN;
            double topCss = double.NaN;
            double rightCss = double.NaN;
            double bottomCss = double.NaN;
            if (isAbs)
            {
                leftCss = ParseLen(GetStyle(style, "left"), containingW, vw, vh);
                topCss = ParseLen(GetStyle(style, "top"), containingH, vw, vh);
                rightCss = ParseLen(GetStyle(style, "right"), containingW, vw, vh);
                bottomCss = ParseLen(GetStyle(style, "bottom"), containingH, vw, vh);
            }

            // width
            string textContent = GetNodeTextContent(node);
            bool isInline = display == "inline" || display == "inline-block" || display == "inline-flex" || display == "inline-grid" || display == "inline-table";
            bool isImage = node != null && node.Type != null && (node.Type.Name ?? "").IndexOf("Image", StringComparison.OrdinalIgnoreCase) >= 0;

            double w = ParseLen(GetStyle(style, "width"), containingW, vw, vh);
            bool autoW = double.IsNaN(w);
            if (autoW)
            {
                if (isAbs && !double.IsNaN(leftCss) && !double.IsNaN(rightCss))
                {
                    w = containingW - leftCss - rightCss - margin.l - margin.r;
                }
                else if (isImage)
                {
                    var (iw, _) = GetImageDimensions(node);
                    w = iw > 0 ? iw : 0;
                }
                else if (isInline && !string.IsNullOrEmpty(textContent))
                {
                    w = MeasureTextWidth(textContent, style, containingW, vw, vh) + padding.l + padding.r + border.l + border.r;
                }
                else
                {
                    w = containingW - margin.l - margin.r;
                }
                if (w < 0) w = 0;
            }
            // height
            double h = ParseLen(GetStyle(style, "height"), containingH, vw, vh);
            bool autoH = double.IsNaN(h);
            if (autoH)
            {
                if (isAbs && !double.IsNaN(topCss) && !double.IsNaN(bottomCss))
                {
                    h = containingH - topCss - bottomCss - margin.t - margin.b;
                    autoH = false;
                }
                else if (isImage)
                {
                    var (_, ih) = GetImageDimensions(node);
                    h = ih > 0 ? ih : 0;
                    autoH = false;
                }
                else
                {
                    h = 0;
                }
                if (h < 0) h = 0;
            }

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
            if (isAbs)
            {
                double x;
                if (!double.IsNaN(leftCss)) x = parentBorderBox.X + leftCss + margin.l;
                else if (!double.IsNaN(rightCss)) x = parentBorderBox.X + parentBorderBox.Width - finalBox.Width - rightCss - margin.r;
                else x = slotBox.X + margin.l;

                double y;
                if (!double.IsNaN(topCss)) y = parentBorderBox.Y + topCss + margin.t;
                else if (!double.IsNaN(bottomCss)) y = parentBorderBox.Y + parentBorderBox.Height - finalBox.Height - bottomCss - margin.b;
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
                LayoutBox childContainingBlock = IsPositioned(node) ? finalBox : parentBorderBox;
                ArrangeChildren(node, finalBox, childContainingBlock, vw, vh);
            }
            else
            {
                if (autoH)
                {
                    double fs = ParseLen(GetStyle(style, "font-size"), containingH, vw, vh);
                    if (double.IsNaN(fs) || fs <= 0) fs = 16;
                    double lh = ParseLen(GetStyle(style, "line-height"), containingH, vw, vh);
                    double lhPx;
                    if (double.IsNaN(lh)) lhPx = fs * 1.2;
                    else if (lh < 10) lhPx = fs * lh;
                    else lhPx = lh;

                    if (!string.IsNullOrEmpty(textContent))
                    {
                        // estimate wrapped lines for block-level boxes
                        double contentWidth = Math.Max(1, w - padding.l - padding.r - border.l - border.r - margin.l - margin.r);
                        int lines = 1;
                        if (!isInline)
                        {
                            double lineW = 0;
                            lines = 1;
                            for (int i = 0; i < textContent.Length; i++)
                            {
                                char c = textContent[i];
                                double cw = (c == ' ') ? (fs * 0.25 + (double.IsNaN(ParseLen(GetStyle(style, "word-spacing"), contentWidth, vw, vh)) ? 0 : ParseLen(GetStyle(style, "word-spacing"), contentWidth, vw, vh))) : (fs * 0.5 + (double.IsNaN(ParseLen(GetStyle(style, "letter-spacing"), contentWidth, vw, vh)) ? 0 : ParseLen(GetStyle(style, "letter-spacing"), contentWidth, vw, vh)));
                                if (lineW + cw > contentWidth && lineW > 0)
                                {
                                    lines++;
                                    lineW = cw;
                                }
                                else
                                {
                                    lineW += cw;
                                }
                            }
                            if (lines < 1) lines = 1;
                        }
                        finalBox.Height = lines * lhPx + padding.t + padding.b + border.t + border.b;
                    }
                    else
                    {
                        finalBox.Height = lhPx + padding.t + padding.b + border.t + border.b;
                        if (finalBox.Height < 16) finalBox.Height = 16;
                    }
                    node.Layout.Height = finalBox.Height;
                }
            }

            // transform? no.

            return finalBox;
        }

        private void ArrangeChildren(Node node, LayoutBox parentBox, LayoutBox containingBlock, int vw, int vh)
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
                ArrangeFlexColumn(node, children, parentBox, contentX, contentY, contentW, contentH, gap, containingBlock, vw, vh);
            }
            else if (isFlex)
            {
                ArrangeFlexRow(node, children, parentBox, contentX, contentY, contentW, contentH, gap, containingBlock, vw, vh);
            }
            else
            {
                ArrangeBlock(node, children, parentBox, contentX, contentY, contentW, contentH, gap, containingBlock, vw, vh);
            }

            // auto height for parent
            string heightStyle = GetStyle(style, "height");
            bool autoHeight = double.IsNaN(ParseLen(heightStyle, parentBox.Height, vw, vh));
            string posStyle = (GetStyle(style, "position") ?? "static").ToLowerInvariant();
            if ((posStyle == "absolute" || posStyle == "fixed") &&
                !double.IsNaN(ParseLen(GetStyle(style, "top"), parentBox.Height, vw, vh)) &&
                !double.IsNaN(ParseLen(GetStyle(style, "bottom"), parentBox.Height, vw, vh)))
            {
                autoHeight = false;
            }
            if (autoHeight)
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

        private void ArrangeBlock(Node node, Node[] children, LayoutBox parentBox, double contentX, double contentY, double contentW, double contentH, double gap, LayoutBox containingBlock, int vw, int vh)
        {
            double y = contentY;
            double x = contentX;
            double lineH = 0;
            bool inLine = false;
            foreach (var child in children)
            {
                if (child == null) continue;
                var cstyle = child.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string cdisplay = (GetStyle(cstyle, "display") ?? DefaultDisplay(child)).ToLowerInvariant();
                if (cdisplay == "none")
                {
                    var zero = new LayoutBox { X = x, Y = y, Width = 0, Height = 0 };
                    SetNodeLayout(child, zero);
                    continue;
                }
                var cmargin = ParseEdges(cstyle, "margin", contentW);
                string cpos = (GetStyle(cstyle, "position") ?? "static").ToLowerInvariant();
                if (cpos == "absolute" || cpos == "fixed")
                {
                    var slot = new LayoutBox { X = contentX, Y = contentY, Width = contentW, Height = contentH };
                    LayoutNode(child, slot, containingBlock, vw, vh);
                    continue;
                }

                bool inline = cdisplay == "inline" || cdisplay == "inline-block" || cdisplay == "inline-flex" || cdisplay == "inline-grid" || cdisplay == "inline-table" || cdisplay.StartsWith("inline");
                // Also treat images as inline by default.
                bool isImage = child.Type != null && (child.Type.Name ?? "").IndexOf("Image", StringComparison.OrdinalIgnoreCase) >= 0;
                if (isImage) inline = true;

                if (!inline)
                {
                    // flush current inline line
                    if (inLine)
                    {
                        y += lineH + gap;
                        lineH = 0;
                        x = contentX;
                        inLine = false;
                    }
                    var slot2 = new LayoutBox { X = contentX, Y = y, Width = contentW, Height = contentH };
                    var childBox = LayoutNode(child, slot2, containingBlock, vw, vh);
                    y = childBox.Y + childBox.Height + cmargin.b + gap;
                    x = contentX;
                }
                else
                {
                    double remaining = (contentX + contentW) - x;
                    if (inLine && remaining < 1)
                    {
                        y += lineH + gap;
                        lineH = 0;
                        x = contentX;
                        inLine = false;
                        remaining = contentW;
                    }
                    var slot2 = new LayoutBox { X = x, Y = y, Width = Math.Max(1, remaining), Height = contentH };
                    var childBox = LayoutNode(child, slot2, containingBlock, vw, vh);
                    // If it didn't fit on this line, wrap.
                    if (inLine && childBox.Width > remaining + 0.5 && x > contentX)
                    {
                        y += lineH + gap;
                        lineH = 0;
                        x = contentX;
                        remaining = contentW;
                        var slot3 = new LayoutBox { X = x, Y = y, Width = Math.Max(1, remaining), Height = contentH };
                        childBox = LayoutNode(child, slot3, containingBlock, vw, vh);
                    }
                    childBox.X = x;
                    childBox.Y = y;
                    if (child.Layout != null)
                    {
                        child.Layout.X = x;
                        child.Layout.Y = y;
                    }
                    x += childBox.Width + cmargin.r + gap;
                    lineH = Math.Max(lineH, childBox.Height + cmargin.t + cmargin.b);
                    inLine = true;
                }
            }
            if (inLine)
            {
                y += lineH;
            }
        }

        private void ArrangeFlexRow(Node node, Node[] children, LayoutBox parentBox, double contentX, double contentY, double contentW, double contentH, double gap, LayoutBox containingBlock, int vw, int vh)
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
                string cpos = (GetStyle(cs, "position") ?? "static").ToLowerInvariant();
                if (cpos == "absolute" || cpos == "fixed")
                {
                    var slot = new LayoutBox { X = contentX, Y = contentY, Width = contentW, Height = contentH };
                    LayoutNode(c, slot, containingBlock, vw, vh);
                    continue;
                }
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

                var slot = new LayoutBox { X = x, Y = contentY, Width = w, Height = h };
                // override width in style? LayoutNode will parse width again; to force, set slot width and let LayoutNode use specified? 
                // Ensure computed style has width so LayoutNode uses it.
                if (cs != null)
                {
                    if (!cs.ContainsKey("width")) cs["width"] = w.ToString("0.###", CultureInfo.InvariantCulture) + "px";
                    if (!cs.ContainsKey("height")) cs["height"] = h.ToString("0.###", CultureInfo.InvariantCulture) + "px";
                }
                var childBox = LayoutNode(c, slot, containingBlock, vw, vh);
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

        private void ArrangeFlexColumn(Node node, Node[] children, LayoutBox parentBox, double contentX, double contentY, double contentW, double contentH, double gap, LayoutBox containingBlock, int vw, int vh)
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
                string cpos = (GetStyle(cs, "position") ?? "static").ToLowerInvariant();
                if (cpos == "absolute" || cpos == "fixed")
                {
                    var slot = new LayoutBox { X = contentX, Y = contentY, Width = contentW, Height = contentH };
                    LayoutNode(c, slot, containingBlock, vw, vh);
                    continue;
                }
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

                var slot = new LayoutBox { X = contentX, Y = y, Width = w, Height = h > 0 ? h : contentH };
                var childBox = LayoutNode(c, slot, containingBlock, vw, vh);
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

        private static bool IsPositioned(Node node)
        {
            if (node == null) return false;
            var style = node.ComputedStyles;
            string pos = (GetStyle(style, "position") ?? "static").ToLowerInvariant();
            return pos == "relative" || pos == "absolute" || pos == "fixed" || pos == "sticky";
        }

        private static readonly Regex _varRegex = new Regex(@"var\(\s*(--[\w-]+)(?:\s*,\s*(?<fallback>[^)]*))?\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string ResolveVarValue(string value, Node node)
        {
            return ResolveVarValue(value, node, null);
        }

        public string ResolveVarValue(string value, Node node, Dictionary<string, string> localVars)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (!value.Contains("var(")) return value;
            var visiting = new HashSet<string>(StringComparer.Ordinal);
            return _varRegex.Replace(value, m =>
            {
                string varName = m.Groups[1].Value;
                string fallback = m.Groups["fallback"].Success ? m.Groups["fallback"].Value.Trim() : null;
                string resolved = LookupVar(varName, node, visiting, localVars);
                if (resolved == null) resolved = fallback;
                if (resolved == null) resolved = string.Empty;
                if (resolved.Contains("var("))
                    resolved = ResolveVarValue(resolved, node, localVars);
                return resolved;
            });
        }

        private string LookupVar(string name, Node node, HashSet<string> visiting, Dictionary<string, string> localVars)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            if (visiting.Contains(name)) return null;
            visiting.Add(name);
            if (localVars != null && localVars.TryGetValue(name, out var lv))
            {
                if (lv != null && lv.Contains("var("))
                    lv = ResolveVarValue(lv, node, localVars);
                visiting.Remove(name);
                return lv;
            }
            Node cur = node;
            while (cur != null)
            {
                if (cur.ComputedStyles != null && cur.ComputedStyles.TryGetValue(name, out var v))
                {
                    if (v != null && v.Contains("var("))
                        v = ResolveVarValue(v, cur);
                    visiting.Remove(name);
                    return v;
                }
                cur = cur.Parent;
            }
            visiting.Remove(name);
            return null;
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

        private static string GetNodeTextContent(Node node)
        {
            if (node?.UntypedProps == null) return null;
            try
            {
                var t = node.UntypedProps.GetType();
                var f = t.GetField("TextContent");
                var p = t.GetProperty("TextContent");
                return f?.GetValue(node.UntypedProps) as string ?? p?.GetValue(node.UntypedProps) as string;
            }
            catch { return null; }
        }

        private double MeasureTextWidth(string text, Dictionary<string, string> style, double containingW, int vw, int vh)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            double fs = ParseLen(GetStyle(style, "font-size"), containingW, vw, vh);
            if (double.IsNaN(fs) || fs <= 0) fs = 16;
            double letter = ParseLen(GetStyle(style, "letter-spacing"), containingW, vw, vh);
            if (double.IsNaN(letter)) letter = 0;
            double word = ParseLen(GetStyle(style, "word-spacing"), containingW, vw, vh);
            if (double.IsNaN(word)) word = 0;

            double width = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == ' ')
                {
                    // space glyph plus word-spacing gap
                    width += fs * 0.25 + word;
                }
                else
                {
                    width += fs * 0.5 + letter;
                }
            }
            return width;
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

        private static (int W, int H) GetImageDimensions(Rocket.Core.DOM.Node node)
        {
            try
            {
                if (node?.UntypedProps == null) return (0, 0);
                var t = node.UntypedProps.GetType();
                var f = t.GetField("Src");
                var p = t.GetProperty("Src");
                string src = f?.GetValue(node.UntypedProps) as string ?? p?.GetValue(node.UntypedProps) as string;
                if (string.IsNullOrWhiteSpace(src)) return (0, 0);

                if (src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    int comma = src.IndexOf(',');
                    string b64 = comma >= 0 ? src.Substring(comma + 1) : src;
                    byte[] bytes = Convert.FromBase64String(b64.Trim());
                    using (var ms = new System.IO.MemoryStream(bytes))
                    using (var img = System.Drawing.Image.FromStream(ms))
                    {
                        return (img.Width, img.Height);
                    }
                }

                string path = src;
                if (!System.IO.Path.IsPathRooted(path))
                {
                    var rel = path.TrimStart('/', '\\');
                    foreach (var baseDir in new[] { System.IO.Directory.GetCurrentDirectory(), System.AppContext.BaseDirectory })
                    {
                        try
                        {
                            var cand = System.IO.Path.Combine(baseDir, rel);
                            if (System.IO.File.Exists(cand)) { path = cand; break; }
                        }
                        catch { }
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        foreach (var baseDir in new[] { System.IO.Directory.GetCurrentDirectory(), System.AppContext.BaseDirectory })
                        {
                            try
                            {
                                var dir = new System.IO.DirectoryInfo(baseDir);
                                while (dir != null)
                                {
                                    var cand = System.IO.Path.Combine(dir.FullName, rel);
                                    if (System.IO.File.Exists(cand)) { path = cand; break; }
                                    dir = dir.Parent;
                                }
                                if (System.IO.File.Exists(path)) break;
                            }
                            catch { }
                        }
                    }
                }
                if (System.IO.File.Exists(path))
                {
                    using (var img = System.Drawing.Image.FromFile(path))
                    {
                        return (img.Width, img.Height);
                    }
                }
            }
            catch
            {
            }
            return (0, 0);
        }
    }
}
