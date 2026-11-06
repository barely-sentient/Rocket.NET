using System;
using Rocket.Core.DOM;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
                            double wordSpacing = ParseLen(GetStyle(style, "word-spacing"), contentWidth, vw, vh);
                            if (double.IsNaN(wordSpacing)) wordSpacing = 0;
                            double letterSpacing = ParseLen(GetStyle(style, "letter-spacing"), contentWidth, vw, vh);
                            if (double.IsNaN(letterSpacing)) letterSpacing = 0;
                            for (int i = 0; i < textContent.Length; i++)
                            {
                                char c = textContent[i];
                                double cw = (c == ' ') ? fs * 0.25 + wordSpacing : fs * 0.5 + letterSpacing;
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
            bool isTable = display == "table" || display == "inline-table";

            // gap
            double gap = ParseLen(GetStyle(style, "gap") ?? GetStyle(style, "grid-gap"), contentW, vw, vh);
            if (double.IsNaN(gap)) gap = 0;

            if (isTable)
            {
                ArrangeTable(node, children, parentBox, contentX, contentY, contentW, contentH, gap, containingBlock, vw, vh);
                return;
            }

            if (isFlex && flexDirection.StartsWith("column", StringComparison.Ordinal))
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

                bool inline = cdisplay == "inline" || cdisplay == "inline-block" || cdisplay == "inline-flex" || cdisplay == "inline-grid" || cdisplay == "inline-table" || cdisplay.StartsWith("inline", StringComparison.Ordinal);
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
        private void ArrangeTable(Node node, Node[] children, LayoutBox parentBox, double contentX, double contentY, double contentW, double contentH, double gap, LayoutBox containingBlock, int vw, int vh)
        {
            var style = node.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var padding = ParseEdges(style, "padding", parentBox.Width);
            var border = ParseEdges(style, "border-width", parentBox.Width);
            if (border.t == 0 && border.r == 0 && border.b == 0 && border.l == 0)
            {
                var b2 = ParseEdgesShorthand(style, "border", parentBox.Width);
                if (b2.t > 0 || b2.r > 0 || b2.b > 0 || b2.l > 0) border = b2;
            }

            double spacing = ParseLen(GetStyle(style, "border-spacing"), contentW, vw, vh);
            if (double.IsNaN(spacing)) spacing = 0;
            string borderCollapse = (GetStyle(style, "border-collapse") ?? "separate").ToLowerInvariant();
            if (borderCollapse == "collapse") spacing = 0;
            string tableLayout = (GetStyle(style, "table-layout") ?? "auto").ToLowerInvariant();

            var allChildren = children ?? Array.Empty<Node>();
            var captionsTop = new List<Node>();
            var captionsBottom = new List<Node>();
            var flatRows = new List<(Node group, Node row)>();

            foreach (var ch in allChildren)
            {
                if (ch == null) continue;
                var cs = ch.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string d = (GetStyle(cs, "display") ?? DefaultDisplay(ch)).ToLowerInvariant();
                if (d == "table-caption")
                {
                    string side = (GetStyle(cs, "caption-side") ?? "top").ToLowerInvariant();
                    if (side == "bottom") captionsBottom.Add(ch); else captionsTop.Add(ch);
                }
                else if (d == "table-row")
                {
                    flatRows.Add((null, ch));
                }
                else if (d == "table-row-group" || d == "table-header-group" || d == "table-footer-group")
                {
                    foreach (var r in ch.Children ?? Array.Empty<Node>())
                    {
                        if (r == null) continue;
                        var rcs = r.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        string rd = (GetStyle(rcs, "display") ?? DefaultDisplay(r)).ToLowerInvariant();
                        if (rd == "table-row") flatRows.Add((ch, r));
                    }
                }
            }

            int rowCount = flatRows.Count;
            if (rowCount == 0)
            {
                if (double.IsNaN(ParseLen(GetStyle(style, "height"), parentBox.Height, vw, vh)))
                {
                    double nh = padding.t + padding.b + border.t + border.b;
                    if (nh < 16) nh = 16;
                    parentBox.Height = nh;
                    if (node.Layout != null) node.Layout.Height = nh;
                }
                return;
            }

            int cols = 0;
            foreach (var (_, row) in flatRows)
            {
                int c = 0;
                foreach (var cell in row.Children ?? Array.Empty<Node>())
                {
                    if (cell == null) continue;
                    var cs = cell.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    string d = (GetStyle(cs, "display") ?? DefaultDisplay(cell)).ToLowerInvariant();
                    if (d != "table-cell") continue;
                    int span = GetColSpan(cell);
                    if (span < 1) span = 1;
                    c += span;
                }
                if (c > cols) cols = c;
            }
            if (cols < 1) cols = 1;

            // Compute cell origins (row/col) up front so column placement accounts for
            // rowspans from previous rows.
            var grid = new Node[rowCount][];
            for (int r = 0; r < rowCount; r++) grid[r] = new Node[cols];
            var cellOrigin = new Dictionary<Node, (int r, int c)>();
            for (int r = 0; r < rowCount; r++)
            {
                var (_, row) = flatRows[r];
                foreach (var cell in row.Children ?? Array.Empty<Node>())
                {
                    if (cell == null) continue;
                    var cs = cell.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    string d = (GetStyle(cs, "display") ?? DefaultDisplay(cell)).ToLowerInvariant();
                    if (d != "table-cell") continue;
                    int span = GetColSpan(cell);
                    int rowspan = GetRowSpan(cell);
                    if (span < 1) span = 1;
                    if (rowspan < 1) rowspan = 1;
                    int c = 0;
                    while (c < cols && grid[r][c] != null) c++;
                    if (c >= cols) break;
                    if (c + span > cols) span = cols - c;
                    for (int rr = r; rr < r + rowspan && rr < rowCount; rr++)
                    {
                        for (int cc = c; cc < c + span && cc < cols; cc++)
                        {
                            grid[rr][cc] = cell;
                        }
                    }
                    cellOrigin[cell] = (r, c);
                }
            }

            double[] colPref = new double[cols];
            for (int i = 0; i < cols; i++) colPref[i] = 1;

            if (tableLayout != "fixed")
            {
                foreach (var cell in cellOrigin.Keys)
                {
                    var (r, c) = cellOrigin[cell];
                    int span = GetColSpan(cell);
                    if (span < 1) span = 1;
                    if (c + span > cols) span = cols - c;
                    double cpref = MeasurePreferredWidth(cell, contentW, vw, vh);
                    if (span == 1)
                    {
                        if (cpref > colPref[c]) colPref[c] = cpref;
                    }
                    else
                    {
                        double each = cpref / span;
                        for (int k = 0; k < span && c + k < cols; k++)
                        {
                            if (each > colPref[c + k]) colPref[c + k] = each;
                        }
                    }
                }
            }
            else
            {
                if (flatRows.Count > 0)
                {
                    var (_, firstRow) = flatRows[0];
                    foreach (var cell in firstRow.Children ?? Array.Empty<Node>())
                    {
                        if (cell == null) continue;
                        if (!cellOrigin.ContainsKey(cell)) continue;
                        var (rr, c) = cellOrigin[cell];
                        var cs = cell.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        string d = (GetStyle(cs, "display") ?? DefaultDisplay(cell)).ToLowerInvariant();
                        if (d != "table-cell") continue;
                        int span = GetColSpan(cell);
                        if (span < 1) span = 1;
                        if (c + span > cols) span = cols - c;
                        double w = ParseLen(GetStyle(cs, "width"), contentW, vw, vh);
                        var pad = ParseEdges(cs, "padding", contentW);
                        var bor = ParseEdges(cs, "border-width", contentW);
                        if (bor.t == 0 && bor.r == 0 && bor.b == 0 && bor.l == 0)
                        {
                            var b2 = ParseEdgesShorthand(cs, "border", contentW);
                            if (b2.t > 0 || b2.r > 0 || b2.b > 0 || b2.l > 0) bor = b2;
                        }
                        double cpref;
                        if (double.IsNaN(w))
                            cpref = double.NaN;
                        else
                            cpref = w + pad.l + pad.r + bor.l + bor.r;
                        if (span == 1)
                        {
                            if (double.IsNaN(cpref)) colPref[c] = double.NaN;
                            else if (double.IsNaN(colPref[c]) || cpref > colPref[c]) colPref[c] = cpref;
                        }
                        else
                        {
                            double each = double.IsNaN(cpref) ? double.NaN : cpref / span;
                            for (int k = 0; k < span && c + k < cols; k++)
                            {
                                if (double.IsNaN(each)) colPref[c + k] = double.NaN;
                                else if (double.IsNaN(colPref[c + k]) || each > colPref[c + k]) colPref[c + k] = each;
                            }
                        }
                    }
                }
            }

            double availCols = contentW - spacing * (cols - 1);
            if (availCols < 0) availCols = 0;
            double[] colWidth = new double[cols];

            if (tableLayout == "fixed")
            {
                double totalSpec = 0;
                int autoCount = 0;
                for (int i = 0; i < cols; i++)
                {
                    if (double.IsNaN(colPref[i]))
                    {
                        autoCount++;
                    }
                    else
                    {
                        if (colPref[i] < 0) colPref[i] = 0;
                        totalSpec += colPref[i];
                    }
                }
                double remaining = availCols - totalSpec;
                if (remaining < 0) remaining = 0;
                double eachAuto = autoCount > 0 ? remaining / autoCount : 0;
                for (int i = 0; i < cols; i++)
                {
                    if (double.IsNaN(colPref[i]))
                        colWidth[i] = eachAuto;
                    else
                        colWidth[i] = colPref[i];
                    if (colWidth[i] < 0) colWidth[i] = 0;
                }
            }
            else
            {
                double totalPref = 0;
                for (int i = 0; i < cols; i++)
                {
                    if (double.IsNaN(colPref[i]) || colPref[i] <= 0) colPref[i] = 1;
                    totalPref += colPref[i];
                }
                if (totalPref <= 0) totalPref = cols;
                for (int i = 0; i < cols; i++)
                {
                    colWidth[i] = colPref[i] / totalPref * availCols;
                    if (colWidth[i] < 0) colWidth[i] = 0;
                }
            }

            double[] rowHeight = new double[rowCount];
            for (int i = 0; i < rowCount; i++) rowHeight[i] = 0;

            for (int r = 0; r < rowCount; r++)
            {
                var (_, row) = flatRows[r];
                foreach (var cell in row.Children ?? Array.Empty<Node>())
                {
                    if (cell == null) continue;
                    if (!cellOrigin.ContainsKey(cell)) continue;
                    var (rr, c) = cellOrigin[cell];
                    var cs = cell.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    string d = (GetStyle(cs, "display") ?? DefaultDisplay(cell)).ToLowerInvariant();
                    if (d != "table-cell") continue;
                    int span = GetColSpan(cell);
                    int rowspan = GetRowSpan(cell);
                    if (span < 1) span = 1;
                    if (rowspan < 1) rowspan = 1;
                    if (c + span > cols) span = cols - c;

                    double cellW = 0;
                    for (int k = 0; k < span && c + k < cols; k++) cellW += colWidth[c + k];
                    cellW += spacing * (span - 1);
                    if (cellW < 0) cellW = 0;

                    var slot = new LayoutBox { X = contentX, Y = contentY, Width = cellW, Height = contentH };
                    LayoutNode(cell, slot, containingBlock, vw, vh);
                    double measuredH = cell.Layout != null ? cell.Layout.Height : 0;
                    if (measuredH <= 0) measuredH = 16;
                    if (rowspan == 1)
                    {
                        if (measuredH > rowHeight[r]) rowHeight[r] = measuredH;
                    }
                    else
                    {
                        double per = measuredH / rowspan;
                        for (int k = 0; k < rowspan && r + k < rowCount; k++)
                        {
                            if (per > rowHeight[r + k]) rowHeight[r + k] = per;
                        }
                    }
                }
            }
            for (int r = 0; r < rowCount; r++)
            {
                if (rowHeight[r] <= 0) rowHeight[r] = 16;
            }

            double[] rowTop = new double[rowCount];

            // captions top
            double yCursor = contentY;
            foreach (var cap in captionsTop)
            {
                var capSlot = new LayoutBox { X = contentX, Y = yCursor, Width = contentW, Height = contentH };
                LayoutNode(cap, capSlot, containingBlock, vw, vh);
                double ch = cap.Layout != null ? cap.Layout.Height : 0;
                if (ch <= 0) ch = 16;
                yCursor += ch;
            }
            if (captionsTop.Count > 0 && rowCount > 0) yCursor += spacing;

            for (int r = 0; r < rowCount; r++)
            {
                rowTop[r] = yCursor;
                yCursor += rowHeight[r];
                if (r < rowCount - 1) yCursor += spacing;
            }
            if (captionsBottom.Count > 0 && rowCount > 0) yCursor += spacing;
            foreach (var cap in captionsBottom)
            {
                var capSlot = new LayoutBox { X = contentX, Y = yCursor, Width = contentW, Height = contentH };
                LayoutNode(cap, capSlot, containingBlock, vw, vh);
                double ch = cap.Layout != null ? cap.Layout.Height : 0;
                if (ch <= 0) ch = 16;
                yCursor += ch;
            }

            // group boxes
            var groupMinMax = new Dictionary<Node, (int min, int max)>();
            for (int r = 0; r < rowCount; r++)
            {
                var g = flatRows[r].group;
                if (g == null) continue;
                if (!groupMinMax.ContainsKey(g)) groupMinMax[g] = (r, r);
                else
                {
                    var (mn, mx) = groupMinMax[g];
                    if (r < mn) mn = r;
                    if (r > mx) mx = r;
                    groupMinMax[g] = (mn, mx);
                }
            }
            foreach (var kv in groupMinMax)
            {
                var g = kv.Key;
                var (mn, mx) = kv.Value;
                double gTop = rowTop[mn];
                double gBottom = rowTop[mx] + rowHeight[mx];
                var gBox = new LayoutBox { X = contentX, Y = gTop, Width = contentW, Height = gBottom - gTop };
                SetNodeLayout(g, gBox);
            }

            // row boxes
            for (int r = 0; r < rowCount; r++)
            {
                var (_, row) = flatRows[r];
                var rBox = new LayoutBox { X = contentX, Y = rowTop[r], Width = contentW, Height = rowHeight[r] };
                SetNodeLayout(row, rBox);
            }

            // cell origins already computed above

            // final cell layout
            foreach (var cell in cellOrigin.Keys)
            {
                var (r, c) = cellOrigin[cell];
                int span = GetColSpan(cell);
                int rowspan = GetRowSpan(cell);
                if (span < 1) span = 1;
                if (rowspan < 1) rowspan = 1;
                if (c + span > cols) span = cols - c;
                if (r + rowspan > rowCount) rowspan = rowCount - r;

                double cellX = contentX;
                for (int k = 0; k < c; k++) cellX += colWidth[k];
                cellX += spacing * c;
                double cellW = 0;
                for (int k = 0; k < span; k++) cellW += colWidth[c + k];
                cellW += spacing * (span - 1);

                double cellY = rowTop[r];
                double cellH = 0;
                for (int k = 0; k < rowspan; k++) cellH += rowHeight[r + k];
                cellH += spacing * (rowspan - 1);

                var cs = cell.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string valign = (GetStyle(cs, "vertical-align") ?? "middle").ToLowerInvariant();
                if (valign == "inherit" || valign == "initial" || valign == "unset" || valign == "revert") valign = "middle";

                var slot1 = new LayoutBox { X = cellX, Y = cellY, Width = cellW, Height = cellH };
                LayoutNode(cell, slot1, containingBlock, vw, vh);
                double contentHeight = cell.Layout != null ? cell.Layout.Height : 0;
                if (contentHeight <= 0) contentHeight = 16;
                if (contentHeight > cellH) contentHeight = cellH;

                double offset = 0;
                if (valign == "middle") offset = (cellH - contentHeight) / 2;
                else if (valign == "bottom") offset = cellH - contentHeight;
                if (offset < 0) offset = 0;

                var slot2 = new LayoutBox { X = cellX, Y = cellY + offset, Width = cellW, Height = contentHeight };
                LayoutNode(cell, slot2, containingBlock, vw, vh);
                if (cell.Layout != null)
                {
                    cell.Layout.Height = cellH;
                }
            }

            // auto height for table
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
                double totalH = yCursor - contentY;
                double newH = totalH + padding.t + padding.b + border.t + border.b;
                if (newH < 0) newH = 0;
                parentBox.Height = newH;
                if (node.Layout != null) node.Layout.Height = newH;
            }
        }

        private static int GetColSpan(Node cell)
        {
            try
            {
                var p = cell?.UntypedProps;
                if (p == null) return 1;
                var t = p.GetType();
                var f = t.GetField("ColSpan");
                var prop = t.GetProperty("ColSpan");
                int v = 0;
                if (f != null) v = Convert.ToInt32(f.GetValue(p) ?? 0);
                else if (prop != null) v = Convert.ToInt32(prop.GetValue(p) ?? 0);
                return v > 0 ? v : 1;
            }
            catch { return 1; }
        }

        private static int GetRowSpan(Node cell)
        {
            try
            {
                var p = cell?.UntypedProps;
                if (p == null) return 1;
                var t = p.GetType();
                var f = t.GetField("RowSpan");
                var prop = t.GetProperty("RowSpan");
                int v = 0;
                if (f != null) v = Convert.ToInt32(f.GetValue(p) ?? 0);
                else if (prop != null) v = Convert.ToInt32(prop.GetValue(p) ?? 0);
                return v > 0 ? v : 1;
            }
            catch { return 1; }
        }

        private double MeasurePreferredWidth(Node node, double containingW, int vw, int vh)
        {
            if (node == null) return 0;
            var style = node.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var padding = ParseEdges(style, "padding", containingW);
            var border = ParseEdges(style, "border-width", containingW);
            if (border.t == 0 && border.r == 0 && border.b == 0 && border.l == 0)
            {
                var b2 = ParseEdgesShorthand(style, "border", containingW);
                if (b2.t > 0 || b2.r > 0 || b2.b > 0 || b2.l > 0) border = b2;
            }
            double w = ParseLen(GetStyle(style, "width"), containingW, vw, vh);
            if (!double.IsNaN(w))
            {
                return w + padding.l + padding.r + border.l + border.r;
            }
            string display = (GetStyle(style, "display") ?? DefaultDisplay(node)).ToLowerInvariant();
            string text = GetNodeTextContent(node);
            var children = node.Children ?? Array.Empty<Node>();
            double content = 0;
            if (children.Length == 0)
            {
                if (!string.IsNullOrEmpty(text))
                    content = MeasureTextWidth(text, style, containingW, vw, vh);
                else if (node.Type != null && (node.Type.Name ?? "").IndexOf("Image", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var (iw, _) = GetImageDimensions(node);
                    content = iw > 0 ? iw : 0;
                }
            }
            else
            {
                var list = new List<Node>();
                foreach (var c in children)
                {
                    if (c == null) continue;
                    var cs = c.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    string cd = (GetStyle(cs, "display") ?? DefaultDisplay(c)).ToLowerInvariant();
                    if (cd == "none") continue;
                    list.Add(c);
                }
                if (display == "flex" || display == "inline-flex")
                {
                    string dir = (GetStyle(style, "flex-direction") ?? "row").ToLowerInvariant();
                    if (dir.StartsWith("column", StringComparison.Ordinal))
                    {
                        double max = 0;
                        foreach (var c in list) max = Math.Max(max, MeasurePreferredWidth(c, containingW, vw, vh));
                        content = max;
                    }
                    else
                    {
                        double sum = 0;
                        foreach (var c in list) sum += MeasurePreferredWidth(c, containingW, vw, vh);
                        content = sum;
                    }
                }
                else if (display == "table" || display == "inline-table")
                {
                    double maxRow = 0;
                    foreach (var child in list)
                    {
                        var cs = child.ComputedStyles ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        string cd = (GetStyle(cs, "display") ?? DefaultDisplay(child)).ToLowerInvariant();
                        Node[] rows = (cd == "table-row-group" || cd == "table-header-group" || cd == "table-footer-group")
                            ? (child.Children ?? Array.Empty<Node>())
                            : new[] { child };
                        foreach (var r in rows)
                        {
                            if (r == null) continue;
                            double rowSum = 0;
                            foreach (var cell in r.Children ?? Array.Empty<Node>())
                            {
                                rowSum += MeasurePreferredWidth(cell, containingW, vw, vh);
                            }
                            if (rowSum > maxRow) maxRow = rowSum;
                        }
                    }
                    content = maxRow;
                }
                else if (display.Contains("inline"))
                {
                    double sum = 0;
                    foreach (var c in list) sum += MeasurePreferredWidth(c, containingW, vw, vh);
                    content = sum;
                }
                else
                {
                    double max = 0;
                    foreach (var c in list) max = Math.Max(max, MeasurePreferredWidth(c, containingW, vw, vh));
                    content = max;
                }
            }
            double total = content + padding.l + padding.r + border.l + border.r;
            if (total < 0) total = 0;
            return total;
        }

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

        public string ResolveVarValue(string value, Node node)
        {
            return ResolveVarValue(value, node, null);
        }

        public string ResolveVarValue(string value, Node node, Dictionary<string, string> localVars)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (value.IndexOf("var(", StringComparison.OrdinalIgnoreCase) < 0) return value;
            var visiting = new HashSet<string>(StringComparer.Ordinal);
            var tokens = VantaCSS.Grammar.Grammar.Tokenise(value);
            var sb = new StringBuilder();
            int copied = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Kind != VantaCSS.Tokeniser.TokenKind.Function) continue;
                var reference = VantaCSS.Grammar.Grammar.MatchVarReference(tokens, i, value, out int consumed);
                if (reference == null) continue;

                var last = tokens[i + consumed - 1];
                string resolved = LookupVar(reference.Name, node, visiting, localVars) ?? reference.Fallback ?? string.Empty;
                if (resolved.Contains("var("))
                    resolved = ResolveVarValue(resolved, node, localVars);

                int start = (int)tokens[i].StartIndex;
                sb.Append(value, copied, start - copied).Append(resolved);
                copied = (int)(last.StartIndex + last.Length);
                i += consumed - 1;
            }
            sb.Append(value, copied, value.Length - copied);
            return sb.ToString();
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
                return PropertyReader.GetString(node.UntypedProps, "TextContent");
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

        private static readonly ConcurrentDictionary<string, string[]> _edgeLonghandNames =
            new ConcurrentDictionary<string, string[]>(StringComparer.Ordinal);

        private static string[] EdgeLonghandNames(string prop)
        {
            return _edgeLonghandNames.GetOrAdd(prop, p => new[] { p + "-top", p + "-right", p + "-bottom", p + "-left" });
        }

        private Edges ParseEdges(Dictionary<string, string> style, string prop, double reference)
        {
            var e = new Edges();
            if (style == null) return e;
            // individual
            string[] names = EdgeLonghandNames(prop);
            string top = GetStyle(style, names[0]);
            string right = GetStyle(style, names[1]);
            string bottom = GetStyle(style, names[2]);
            string left = GetStyle(style, names[3]);
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
                return ParseEdgesShorthandValue(shorthand, reference, prop.StartsWith("border", StringComparison.Ordinal));
            }
            return e;
        }

        private Edges ParseEdgesShorthand(Dictionary<string, string> style, string prop, double reference)
        {
            var e = new Edges();
            if (style == null) return e;
            string shorthand = GetStyle(style, prop);
            if (shorthand == null) return e;
            return ParseEdgesShorthandValue(shorthand, reference, true);
        }

        private Edges ParseEdgesShorthandValue(string shorthand, double reference, bool border)
        {
            var e = new Edges();
            if (string.IsNullOrWhiteSpace(shorthand)) return e;
            var slots = border
                ? VantaCSS.Grammar.Grammar.ParseBorderLengths(shorthand)
                : (VantaCSS.Grammar.Grammar.ParseEdgeSlots(shorthand) ?? new string[0]);
            double[] vals = new double[slots.Length];
            for (int i = 0; i < slots.Length; i++) vals[i] = ParseEdgeValue(slots[i], reference);
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

        private double ParseEdgeValue(string v, double reference)
        {
            if (string.IsNullOrWhiteSpace(v)) return double.NaN;
            return ParseLen(v, reference, 0, 0);
        }

        private enum LengthUnit : byte
        {
            Invalid,
            Percent,
            Px,
            Em,
            Vw,
            Vh,
            Number
        }

        private struct ParsedLength
        {
            public LengthUnit Unit;
            public double Number;
        }

        private const int MaxLengthCacheEntries = 4096;

        private static readonly ConcurrentDictionary<string, ParsedLength> _lengthCache =
            new ConcurrentDictionary<string, ParsedLength>(StringComparer.Ordinal);

        private double ParseLen(string v, double reference, int vw, int vh)
        {
            if (string.IsNullOrWhiteSpace(v)) return double.NaN;
            if (!_lengthCache.TryGetValue(v, out var parsed))
            {
                parsed = ParseLengthToken(v);
                if (_lengthCache.Count >= MaxLengthCacheEntries) _lengthCache.Clear();
                _lengthCache[v] = parsed;
            }

            // The number is cached; the unit is applied per call because viewport units depend on current state.
            switch (parsed.Unit)
            {
                case LengthUnit.Percent: return parsed.Number * reference / 100.0;
                case LengthUnit.Px: return parsed.Number;
                case LengthUnit.Em: return parsed.Number * 16;
                case LengthUnit.Vw: return parsed.Number * (_layoutVw > 0 ? _layoutVw : vw) / 100.0;
                case LengthUnit.Vh: return parsed.Number * (_layoutVh > 0 ? _layoutVh : vh) / 100.0;
                case LengthUnit.Number: return parsed.Number;
                default: return double.NaN;
            }
        }

        private static ParsedLength ParseLengthToken(string value)
        {
            string v = value.Trim().ToLowerInvariant();
            if (v == "auto" || v == "inherit" || v == "initial" || v == "none" || v == "normal") return default;
            if (v.EndsWith("%", StringComparison.Ordinal))
                return new ParsedLength { Unit = LengthUnit.Percent, Number = ParseInvariant(v.Substring(0, v.Length - 1).Trim()) };
            if (v.EndsWith("px", StringComparison.Ordinal))
                return new ParsedLength { Unit = LengthUnit.Px, Number = ParseInvariant(v.Substring(0, v.Length - 2).Trim()) };
            if (v.EndsWith("em", StringComparison.Ordinal) || v.EndsWith("rem", StringComparison.Ordinal))
                return new ParsedLength { Unit = LengthUnit.Em, Number = ParseInvariant(v.TrimEnd('m', 'e', 'r')) };
            if (v.EndsWith("vw", StringComparison.Ordinal))
                return new ParsedLength { Unit = LengthUnit.Vw, Number = ParseInvariant(v.Substring(0, v.Length - 2).Trim()) };
            if (v.EndsWith("vh", StringComparison.Ordinal))
                return new ParsedLength { Unit = LengthUnit.Vh, Number = ParseInvariant(v.Substring(0, v.Length - 2).Trim()) };
            if (v.EndsWith("ms", StringComparison.Ordinal)) return default;

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
                return new ParsedLength { Unit = LengthUnit.Number, Number = num };
            return default;
        }

        private static double ParseInvariant(string text)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ? value : double.NaN;
        }

        // overload with fewer params
        private double ParseLen(string v, double reference)
        {
            return ParseLen(v, reference, _layoutVw, _layoutVh);
        }

        private const int MaxImageDimensionCacheEntries = 512;

        private static readonly ConcurrentDictionary<string, (int W, int H)> _imageDimensionCache =
            new ConcurrentDictionary<string, (int W, int H)>(StringComparer.Ordinal);

        private static (int W, int H) GetImageDimensions(Node node)
        {
            string src;
            try { src = PropertyReader.GetString(node?.UntypedProps, "Src"); }
            catch { return (0, 0); }
            if (string.IsNullOrWhiteSpace(src)) return (0, 0);

            if (_imageDimensionCache.TryGetValue(src, out var cached)) return cached;
            var size = ReadImageDimensions(src);
            // Only successful reads are cached, so an image that appears later is still picked up.
            if (size.W > 0 && size.H > 0)
            {
                if (_imageDimensionCache.Count >= MaxImageDimensionCacheEntries) _imageDimensionCache.Clear();
                _imageDimensionCache[src] = size;
            }
            return size;
        }

        private static (int W, int H) ReadImageDimensions(string src)
        {
            try
            {
                if (src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    int comma = src.IndexOf(',');
                    byte[] bytes = Convert.FromBase64String((comma >= 0 ? src.Substring(comma + 1) : src).Trim());
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        return ImageDimensions.TryGetSize(ms, out int w, out int h) ? (w, h) : (0, 0);
                    }
                }

                string path = ResolveImagePath(src);
                if (path == null) return (0, 0);
                using (var fs = System.IO.File.OpenRead(path))
                {
                    return ImageDimensions.TryGetSize(fs, out int w, out int h) ? (w, h) : (0, 0);
                }
            }
            catch
            {
            }
            return (0, 0);
        }

        private static string ResolveImagePath(string src)
        {
            if (System.IO.Path.IsPathRooted(src)) return System.IO.File.Exists(src) ? src : null;

            var rel = src.TrimStart('/', '\\');
            string[] baseDirs = { System.IO.Directory.GetCurrentDirectory(), System.AppContext.BaseDirectory };

            foreach (var baseDir in baseDirs)
            {
                var cand = System.IO.Path.Combine(baseDir, rel);
                if (System.IO.File.Exists(cand)) return cand;
            }

            foreach (var baseDir in baseDirs)
            {
                for (var dir = new System.IO.DirectoryInfo(baseDir); dir != null; dir = dir.Parent)
                {
                    var cand = System.IO.Path.Combine(dir.FullName, rel);
                    if (System.IO.File.Exists(cand)) return cand;
                }
            }
            return null;
        }
    }
}
