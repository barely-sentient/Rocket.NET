using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Rocket.Core.DOM;

namespace Rocket.Core
{
    /// <summary>
    /// A single CSS transition description.
    /// </summary>
    public class CssTransition
    {
        public string Property { get; set; } = "all";
        public double Duration { get; set; } // seconds
        public double Delay { get; set; } // seconds
        public string TimingFunction { get; set; } = "ease";

        public CssTransition() { }
        public CssTransition(string property, double duration, double delay = 0, string timing = "ease")
        {
            Property = property; Duration = duration; Delay = delay; TimingFunction = timing;
        }
    }

    /// <summary>
    /// An in-flight transition for one property of one node.
    /// </summary>
    public class PropertyTransition
    {
        public string Property { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public double StartTime { get; set; }
        public double Duration { get; set; }
        public double Delay { get; set; }
        public string TimingFunction { get; set; } = "ease";
        public bool Active { get; set; } = true;
    }

    /// <summary>
    /// Per-node transition bookkeeping.
    /// </summary>
    public class NodeTransitionState
    {
        public Node Node { get; set; }
        public Dictionary<string, string> Target { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Computed { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, PropertyTransition> ActiveTransitions { get; set; } = new Dictionary<string, PropertyTransition>(StringComparer.OrdinalIgnoreCase);
        public List<CssTransition> Transitions { get; set; } = new List<CssTransition>();
        public double LastTime { get; set; }
    }

    public partial class CssEngine
    {
        private readonly Dictionary<Node, NodeTransitionState> _transitionStates =
            new Dictionary<Node, NodeTransitionState>();

        /// <summary>Gets or creates transition bookkeeping for a node.</summary>
        public NodeTransitionState GetTransitionState(Node node)
        {
            if (node == null) return null;
            if (!_transitionStates.TryGetValue(node, out var st))
            {
                st = new NodeTransitionState { Node = node, LastTime = CurrentTime };
                _transitionStates[node] = st;
            }
            return st;
        }

        /// <summary>Clears all transition state.</summary>
        public void ClearTransitions()
        {
            _transitionStates.Clear();
        }

        /// <summary>Removes transition state for a node.</summary>
        public void ClearTransitions(Node node)
        {
            if (node != null) _transitionStates.Remove(node);
        }

        /// <summary>Parses a time value such as "1s", "200ms", "0.5s".</summary>
        public static double ParseTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            value = value.Trim().ToLowerInvariant();
            if (value.EndsWith("ms"))
            {
                var n = value.Substring(0, value.Length - 2).Trim();
                double.TryParse(n, NumberStyles.Float, CultureInfo.InvariantCulture, out double ms);
                return ms / 1000.0;
            }
            if (value.EndsWith("s"))
            {
                var n = value.Substring(0, value.Length - 1).Trim();
                double.TryParse(n, NumberStyles.Float, CultureInfo.InvariantCulture, out double s);
                return s;
            }
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double v);
            return v;
        }

        /// <summary>Parses transition declarations from a property dictionary.</summary>
        public static List<CssTransition> ParseTransitions(Dictionary<string, string> decls)
        {
            var result = new List<CssTransition>();
            if (decls == null) return result;

            // Longhand support.
            string prop = null, dur = null, delay = null, timing = null;
            if (decls.TryGetValue("transition-property", out var tp)) prop = tp;
            if (decls.TryGetValue("transition-duration", out var td)) dur = td;
            if (decls.TryGetValue("transition-delay", out var tdel)) delay = tdel;
            if (decls.TryGetValue("transition-timing-function", out var ttf)) timing = ttf;

            if (decls.TryGetValue("transition", out var sh))
            {
                // shorthand may contain multiple comma-separated transitions.
                foreach (var part in sh.Split(','))
                {
                    var toks = part.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    var t = new CssTransition();
                    foreach (var tok in toks)
                    {
                        if (tok.Equals("all", StringComparison.OrdinalIgnoreCase) ||
                            tok.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                            !IsTimeToken(tok) && !IsTimingToken(tok))
                        {
                            // property if not time/timing
                            if (!IsTimeToken(tok) && !IsTimingToken(tok))
                            {
                                t.Property = tok;
                                continue;
                            }
                        }
                        if (IsTimeToken(tok))
                        {
                            if (t.Duration == 0 && dur == null) { t.Duration = ParseTime(tok); }
                            else if (t.Delay == 0) { t.Delay = ParseTime(tok); }
                            else { /* keep first */ }
                            continue;
                        }
                        if (IsTimingToken(tok))
                        {
                            t.TimingFunction = tok;
                            continue;
                        }
                    }
                    if (t.Duration == 0 && dur != null) t.Duration = ParseTime(dur);
                    if (t.Delay == 0 && delay != null) t.Delay = ParseTime(delay);
                    if (string.IsNullOrEmpty(t.Property) && prop != null) t.Property = prop;
                    if (string.IsNullOrEmpty(t.TimingFunction) && timing != null) t.TimingFunction = timing;
                    if (string.IsNullOrEmpty(t.Property)) t.Property = "all";
                    result.Add(t);
                }
                return result;
            }

            // Longhand-only.
            if (dur != null)
            {
                var t = new CssTransition
                {
                    Property = prop ?? "all",
                    Duration = ParseTime(dur),
                    Delay = delay != null ? ParseTime(delay) : 0,
                    TimingFunction = timing ?? "ease"
                };
                result.Add(t);
            }
            return result;
        }

        private static bool IsTimeToken(string tok)
        {
            tok = tok.Trim().ToLowerInvariant();
            return tok.EndsWith("s") || tok.EndsWith("ms") || double.TryParse(tok, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        }

        private static bool IsTimingToken(string tok)
        {
            tok = tok.Trim().ToLowerInvariant();
            return tok == "linear" || tok == "ease" || tok == "ease-in" || tok == "ease-out" ||
                   tok == "ease-in-out" || tok.StartsWith("cubic-bezier");
        }

        /// <summary>Evaluates a timing function, returning eased progress for linear input p in [0,1].</summary>
        public static double Ease(string timing, double p)
        {
            p = Math.Max(0, Math.Min(1, p));
            if (string.IsNullOrWhiteSpace(timing)) timing = "ease";
            timing = timing.Trim().ToLowerInvariant();
            switch (timing)
            {
                case "linear": return p;
                case "ease": return CubicBezier(0.25, 0.1, 0.25, 1.0, p);
                case "ease-in": return CubicBezier(0.42, 0, 1.0, 1.0, p);
                case "ease-out": return CubicBezier(0, 0, 0.58, 1.0, p);
                case "ease-in-out": return CubicBezier(0.42, 0, 0.58, 1.0, p);
            }
            if (timing.StartsWith("cubic-bezier"))
            {
                var nums = timing.Replace("cubic-bezier", "").Trim('(', ')', ' ').Split(',');
                if (nums.Length == 4 &&
                    double.TryParse(nums[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x1) &&
                    double.TryParse(nums[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y1) &&
                    double.TryParse(nums[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double x2) &&
                    double.TryParse(nums[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double y2))
                {
                    return CubicBezier(x1, y1, x2, y2, p);
                }
            }
            return p;
        }

        private static double CubicBezier(double x1, double y1, double x2, double y2, double t)
        {
            // Approximate using Newton-Raphson.
            double cx = 3 * x1;
            double bx = 3 * (x2 - x1) - cx;
            double ax = 1 - cx - bx;
            double cy = 3 * y1;
            double by = 3 * (y2 - y1) - cy;
            double ay = 1 - cy - by;

            double x = t;
            for (int i = 0; i < 8; i++)
            {
                double xest = ((ax * x + bx) * x + cx) * x;
                double dx = (3 * ax * x + 2 * bx) * x + cx;
                if (Math.Abs(dx) < 1e-6) break;
                x -= (xest - t) / dx;
            }
            x = Math.Max(0, Math.Min(1, x));
            return ((ay * x + by) * x + cy) * x;
        }

        /// <summary>Interpolates between two CSS values.</summary>
        public static string Interpolate(string from, string to, double progress)
        {
            progress = Math.Max(0, Math.Min(1, progress));
            if (string.IsNullOrEmpty(from)) from = to;
            if (string.IsNullOrEmpty(to)) to = from;
            if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase)) return to;

            // Color interpolation.
            if (TryParseColor(from, out var c1) && TryParseColor(to, out var c2))
            {
                int r = (int)Math.Round(c1.r + (c2.r - c1.r) * progress);
                int g = (int)Math.Round(c1.g + (c2.g - c1.g) * progress);
                int b = (int)Math.Round(c1.b + (c2.b - c1.b) * progress);
                if (c1.a != 1 || c2.a != 1)
                {
                    double a = c1.a + (c2.a - c1.a) * progress;
                    return $"rgba({r}, {g}, {b}, {a.ToString("0.###", CultureInfo.InvariantCulture)})";
                }
                return $"rgb({r}, {g}, {b})";
            }

            // Numeric (with unit) interpolation.
            if (TryParseNumber(from, out double n1, out string u1) && TryParseNumber(to, out double n2, out string u2) &&
                string.Equals(u1, u2, StringComparison.OrdinalIgnoreCase))
            {
                double n = n1 + (n2 - n1) * progress;
                string fmt = n.ToString("0.###", CultureInfo.InvariantCulture);
                return fmt + u1;
            }

            // Fallback: snap at midpoint.
            return progress < 0.5 ? from : to;
        }

        private static bool TryParseNumber(string value, out double number, out string unit)
        {
            number = 0; unit = "";
            if (string.IsNullOrWhiteSpace(value)) return false;
            value = value.Trim();
            int i = 0;
            // handle leading minus
            if (i < value.Length && (value[i] == '-' || value[i] == '+')) i++;
            bool dot = false;
            var sb = new StringBuilder();
            while (i < value.Length && (char.IsDigit(value[i]) || value[i] == '.'))
            {
                if (value[i] == '.') { if (dot) break; dot = true; }
                sb.Append(value[i]);
                i++;
            }
            if (sb.Length == 0) return false;
            if (!double.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out number)) return false;
            unit = value.Substring(i).Trim();
            return true;
        }

        private struct Color { public int r, g, b; public double a; }

        private static bool TryParseColor(string value, out Color color)
        {
            color = new Color { a = 1 };
            if (string.IsNullOrWhiteSpace(value)) return false;
            value = value.Trim().ToLowerInvariant();

            if (value.StartsWith("#"))
            {
                string hex = value.Substring(1);
                if (hex.Length == 3)
                {
                    hex = new string(new[] { hex[0], hex[0], hex[1], hex[1], hex[2], hex[2] });
                }
                if (hex.Length == 6 && int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgb))
                {
                    color.r = (rgb >> 16) & 0xFF;
                    color.g = (rgb >> 8) & 0xFF;
                    color.b = rgb & 0xFF;
                    color.a = 1;
                    return true;
                }
                if (hex.Length == 8 && long.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long rgba))
                {
                    color.r = (int)((rgba >> 24) & 0xFF);
                    color.g = (int)((rgba >> 16) & 0xFF);
                    color.b = (int)((rgba >> 8) & 0xFF);
                    color.a = (rgba & 0xFF) / 255.0;
                    return true;
                }
                return false;
            }

            if (value.StartsWith("rgb"))
            {
                int open = value.IndexOf('(');
                int close = value.IndexOf(')', open);
                if (open < 0 || close < 0) return false;
                var inner = value.Substring(open + 1, close - open - 1);
                var parts = inner.Split(',');
                if (parts.Length >= 3 &&
                    int.TryParse(parts[0].Trim(), out int r) &&
                    int.TryParse(parts[1].Trim(), out int g) &&
                    int.TryParse(parts[2].Trim(), out int b))
                {
                    color.r = Clamp255(r); color.g = Clamp255(g); color.b = Clamp255(b);
                    if (parts.Length >= 4 && double.TryParse(parts[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double a))
                        color.a = a;
                    else color.a = 1;
                    return true;
                }
                return false;
            }

            // Named colors (basic).
            switch (value)
            {
                case "red": color = new Color { r = 255, g = 0, b = 0, a = 1 }; return true;
                case "green": color = new Color { r = 0, g = 128, b = 0, a = 1 }; return true;
                case "blue": color = new Color { r = 0, g = 0, b = 255, a = 1 }; return true;
                case "white": color = new Color { r = 255, g = 255, b = 255, a = 1 }; return true;
                case "black": color = new Color { r = 0, g = 0, b = 0, a = 1 }; return true;
                case "transparent": color = new Color { r = 0, g = 0, b = 0, a = 0 }; return true;
                default: return false;
            }
        }

        private static int Clamp255(int v) => v < 0 ? 0 : (v > 255 ? 255 : v);
    }
}
