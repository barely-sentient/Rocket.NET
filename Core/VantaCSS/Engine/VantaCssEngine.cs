using System;
using Rocket.Core.DOM;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VantaCSS
{
    /// <summary>
    /// Simple CSS engine. Given a viewport, filters applicable rules and
    /// generates CSS for that point in time.
    /// </summary>
    public partial class VantaCssEngine
    {
        private readonly List<Stylesheet> _stylesheets = new List<Stylesheet>();
        private bool _defaultsAdded;

        public List<Stylesheet> Stylesheets
        {
            get
            {
                EnsureDefaults();
                return _stylesheets;
            }
            set
            {
                _stylesheets.Clear();
                if (value != null) _stylesheets.AddRange(value);
                _defaultsAdded = true;
            }
        }

        private const string DefaultCss = @"
* { box-sizing: border-box; }
body { margin: 0; }
div, section, article, header, footer, main, nav, aside { display: block; }
span, a, label, strong, em, b, i { display: inline; }
img { display: inline-block; }
button { display: inline-block; }
input, textarea, select { display: inline-block; }
ul, ol { display: block; margin: 0; padding-left: 20px; }
li { display: list-item; }
table { display: table; border-collapse: collapse; }
";
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }
        public Orientation Orientation { get; set; } = Orientation.Portrait;

        public VantaCssEngine()
        {
        }

        public VantaCssEngine(int width, int height, Orientation orientation)
        {
            ViewportWidth = width;
            ViewportHeight = height;
            Orientation = orientation;
        }

        public VantaCssEngine(int width, int height, DeviceOrientation orientation)
            : this(width, height, (Orientation)orientation) { }

        public VantaCssEngine(int width, int height, string orientation)
        {
            ViewportWidth = width;
            ViewportHeight = height;
            if (string.Equals(orientation, "landscape", StringComparison.OrdinalIgnoreCase))
                Orientation = Orientation.Landscape;
            else
                Orientation = Orientation.Portrait;
        }

        public VantaCssEngine(Viewport viewport)
        {
            if (viewport != null)
            {
                ViewportWidth = viewport.Width;
                ViewportHeight = viewport.Height;
                Orientation = viewport.Orientation;
            }
        }

        private int _stylesheetVersion;

        private void EnsureDefaults()
        {
            if (_defaultsAdded) return;
            _defaultsAdded = true;
            AddStylesheet(DefaultCss);
        }

        public void AddStylesheet(Stylesheet sheet)
        {
            if (sheet != null)
            {
                EnsureDefaults();
                Stylesheets.Add(sheet);
                _stylesheetVersion++;
            }
        }

        public void AddStylesheet(string css)
        {
            EnsureDefaults();
            var sheet = new Stylesheet();
            sheet.RawCss = css;
            foreach (var vr in VantaCSS.Grammar.Grammar.ParseRules(css))
            {
                if (vr.IsAtRule && vr.Rules != null && vr.Rules.Count > 0)
                {
                    foreach (var nr in vr.Rules)
                    {
                        var r = new CssRule { Selector = nr.Selector, Media = vr.Media, Raw = nr.Raw };
                        foreach (var d in nr.Declarations) r.Declarations[d.Property] = d.Value;
                        sheet.Rules.Add(r);
                    }
                }
                else
                {
                    var r = new CssRule { Selector = vr.Selector, Media = vr.Media, Raw = vr.Raw };
                    foreach (var d in vr.Declarations) r.Declarations[d.Property] = d.Value;
                    sheet.Rules.Add(r);
                }
            }
            Stylesheets.Add(sheet);
            _stylesheetVersion++;
        }

        public void SetViewport(int width, int height, Orientation orientation)
        {
            ViewportWidth = width;
            ViewportHeight = height;
            Orientation = orientation;
        }

        public void SetViewport(Viewport viewport)
        {
            if (viewport == null) return;
            ViewportWidth = viewport.Width;
            ViewportHeight = viewport.Height;
            Orientation = viewport.Orientation;
        }

        public bool MatchesMedia(string media, int width, int height, Orientation orientation)
        {
            if (string.IsNullOrWhiteSpace(media)) return true;
            media = media.Trim();
            foreach (var part in media.Split(','))
            {
                if (MatchesSingle(part.Trim(), width, height, orientation)) return true;
            }
            return false;
        }

        private bool MatchesSingle(string media, int width, int height, Orientation orientation)
        {
            if (string.IsNullOrWhiteSpace(media)) return true;
            string lower = media.ToLowerInvariant();
            if (lower == "all" || lower == "screen") return true;
            if (lower.StartsWith("screen ") || lower.StartsWith("all "))
            {
                int sp = lower.IndexOf(' ');
                media = media.Substring(sp + 1).Trim();
                lower = media.ToLowerInvariant();
            }

            var conditions = new List<string>();
            int idx = 0;
            while (true)
            {
                int open = media.IndexOf('(', idx);
                if (open < 0) break;
                int close = media.IndexOf(')', open);
                if (close < 0) break;
                conditions.Add(media.Substring(open + 1, close - open - 1).Trim());
                idx = close + 1;
            }

            if (conditions.Count == 0)
            {
                return EvaluateCondition(media, width, height, orientation);
            }

            foreach (var c in conditions)
            {
                foreach (var sub in c.Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!EvaluateCondition(sub.Trim(), width, height, orientation)) return false;
                }
            }
            return true;
        }

        private bool EvaluateCondition(string cond, int width, int height, Orientation orientation)
        {
            cond = cond.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(cond)) return true;

            if (cond.Contains("min-width"))
            {
                int val = ParsePx(cond);
                return width >= val;
            }
            if (cond.Contains("max-width"))
            {
                int val = ParsePx(cond);
                return width <= val;
            }
            if (cond.Contains("min-height"))
            {
                int val = ParsePx(cond);
                return height >= val;
            }
            if (cond.Contains("max-height"))
            {
                int val = ParsePx(cond);
                return height <= val;
            }
            if (cond.Contains("orientation"))
            {
                string o = cond.Contains("landscape") ? "landscape" : "portrait";
                return o == orientation.ToString().ToLowerInvariant();
            }
            return true;
        }

        private static int ParsePx(string cond)
        {
            var digits = new StringBuilder();
            foreach (char c in cond)
            {
                if (char.IsDigit(c)) digits.Append(c);
                else if (digits.Length > 0 && (c == ' ' || c == ')')) break;
            }
            int.TryParse(digits.ToString(), out int v);
            return v;
        }

        public string GenerateCss()
        {
            return GenerateCss(ViewportWidth, ViewportHeight, Orientation);
        }

        public string GenerateCss(int width, int height, Orientation orientation)
        {
            return BuildCss(width, height, orientation);
        }

        public string GenerateCss(int width, int height, DeviceOrientation orientation)
        {
            return BuildCss(width, height, (Orientation)orientation);
        }

        public string GenerateCss(int width, int height, string orientation)
        {
            var o = string.Equals(orientation, "landscape", StringComparison.OrdinalIgnoreCase) ? Orientation.Landscape : Orientation.Portrait;
            return BuildCss(width, height, o);
        }

        public string GenerateCss(Viewport viewport)
        {
            if (viewport == null) return BuildCss(ViewportWidth, ViewportHeight, Orientation);
            return BuildCss(viewport.Width, viewport.Height, viewport.Orientation);
        }

        public string Generate()
        {
            return GenerateCss();
        }

        public string Generate(int width, int height, Orientation orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string Generate(int width, int height, DeviceOrientation orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string Generate(int width, int height, string orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string GenerateCSS()
        {
            return GenerateCss();
        }

        public string GenerateCSS(int width, int height, Orientation orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string GenerateCSS(int width, int height, DeviceOrientation orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string GenerateCSS(int width, int height, string orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string GetCss()
        {
            return GenerateCss();
        }

        public string GetCss(int width, int height, Orientation orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string GetCss(int width, int height, string orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string GetCSS()
        {
            return GenerateCss();
        }

        public string GetCSS(int width, int height, Orientation orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string GetCSS(int width, int height, string orientation)
        {
            return GenerateCss(width, height, orientation);
        }

        public string Build()
        {
            return GenerateCss();
        }

        public string BuildCss()
        {
            return GenerateCss();
        }

        public string BuildCss(int width, int height, Orientation orientation)
        {
            var sb = new StringBuilder();
            foreach (var sheet in Stylesheets)
            {
                if (!string.IsNullOrWhiteSpace(sheet.RawCss) && (sheet.Rules == null || sheet.Rules.Count == 0))
                {
                    var parsed = Stylesheet.FromCss(sheet.RawCss);
                    sheet.Rules = parsed.Rules;
                }

                foreach (var rule in sheet.Rules)
                {
                    if (!MatchesMedia(rule.Media, width, height, orientation)) continue;
                    sb.Append(rule.Selector ?? string.Empty);
                    sb.Append(" { ");
                    foreach (var kv in rule.Declarations)
                    {
                        sb.Append(kv.Key);
                        sb.Append(": ");
                        sb.Append(kv.Value);
                        sb.Append("; ");
                    }
                    if (!string.IsNullOrWhiteSpace(rule.Raw))
                        sb.Append(rule.Raw);
                    sb.Append("}\n");
                }
            }
            return sb.ToString();
        }

        public string BuildCss(int width, int height, DeviceOrientation orientation)
        {
            return BuildCss(width, height, (Orientation)orientation);
        }

        public string BuildCss(int width, int height, string orientation)
        {
            var o = string.Equals(orientation, "landscape", StringComparison.OrdinalIgnoreCase) ? Orientation.Landscape : Orientation.Portrait;
            return BuildCss(width, height, o);
        }

        public string BuildCss(Viewport viewport)
        {
            if (viewport == null) return BuildCss(ViewportWidth, ViewportHeight, Orientation);
            return BuildCss(viewport.Width, viewport.Height, viewport.Orientation);
        }

        public string ComputeStyle(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation)
        {
            return ComputeStyle(node, width, height, orientation, CurrentTime);
        }

        public string ComputeStyle(Rocket.Core.DOM.Node node)
        {
            return ComputeStyle(node, ViewportWidth, ViewportHeight, Orientation);
        }

        public string Compute(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation)
        {
            return ComputeStyle(node, width, height, orientation);
        }

        public string Compute(Rocket.Core.DOM.Node node)
        {
            return ComputeStyle(node);
        }

        public string GetComputedStyle(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation)
        {
            return ComputeStyle(node, width, height, orientation);
        }

        public string GetComputedStyle(Rocket.Core.DOM.Node node)
        {
            return ComputeStyle(node);
        }

        public Dictionary<string, string> ComputeStyles(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation)
        {
            var style = ComputeStyle(node, width, height, orientation);
            var dict = new Dictionary<string, string>();
            foreach (var part in style.Split(';'))
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                var idx = trimmed.IndexOf(':');
                if (idx > 0)
                {
                    dict[trimmed.Substring(0, idx).Trim()] = trimmed.Substring(idx + 1).Trim();
                }
            }
            return dict;
        }

        public Dictionary<string, string> ComputeStyles(Rocket.Core.DOM.Node node)
        {
            return ComputeStyles(node, ViewportWidth, ViewportHeight, Orientation);
        }

        public void ApplyComputedStyles(Rocket.Core.DOM.Node root, int width, int height, Orientation orientation)
        {
            if (root == null) return;
            _applyDepth++;
            var style = ComputeStyle(root, width, height, orientation);
            root.ComputedStyle = style;
            root.ComputedCss = style;
            if (root.ComputedStyles == null) root.ComputedStyles = new Dictionary<string, string>();
            root.ComputedStyles.Clear();
            foreach (var part in style.Split(';'))
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                var idx = trimmed.IndexOf(':');
                if (idx > 0)
                {
                    var key = trimmed.Substring(0, idx).Trim();
                    var val = trimmed.Substring(idx + 1).Trim();
                    root.ComputedStyles[key] = val;
                }
            }

            // Inherit CSS custom properties from parent.
            if (root.Parent != null && root.Parent.ComputedStyles != null)
            {
                foreach (var kv in root.Parent.ComputedStyles)
                {
                    if (kv.Key.StartsWith("--", StringComparison.Ordinal) && !root.ComputedStyles.ContainsKey(kv.Key))
                        root.ComputedStyles[kv.Key] = kv.Value;
                }
            }

            // Resolve var() references.
            var rkeys = new List<string>(root.ComputedStyles.Keys);
            foreach (var k in rkeys)
                root.ComputedStyles[k] = ResolveVarValue(root.ComputedStyles[k], root);

            // Rebuild computed style string.
            var sb = new StringBuilder();
            foreach (var kv in root.ComputedStyles)
            {
                sb.Append(kv.Key).Append(": ").Append(kv.Value).Append("; ");
            }
            root.ComputedStyle = sb.ToString().Trim();
            root.ComputedCss = root.ComputedStyle;

            foreach (var child in root.Children ?? Array.Empty<Rocket.Core.DOM.Node>())
            {
                child.Parent = root;
                ApplyComputedStyles(child, width, height, orientation);
            }
            if (_applyDepth == 1 && !_layoutInProgress)
            {
                _layoutInProgress = true;
                CalculateLayout(root, width, height, orientation, CurrentTime);
                _layoutInProgress = false;
            }
            _applyDepth--;
        }

        public void ApplyComputedStyles(Rocket.Core.DOM.Node root)
        {
            ApplyComputedStyles(root, ViewportWidth, ViewportHeight, Orientation);
        }

        public void Apply(Rocket.Core.DOM.Node root, int width, int height, Orientation orientation)
        {
            CalculateLayout(root, width, height, orientation, CurrentTime);
        }

        public void Apply(Rocket.Core.DOM.Node root)
        {
            CalculateLayout(root, ViewportWidth, ViewportHeight, Orientation, CurrentTime);
        }

        public void ApplyStyles(Rocket.Core.DOM.Node root, int width, int height, Orientation orientation)
        {
            CalculateLayout(root, width, height, orientation, CurrentTime);
        }

        public void ApplyStyles(Rocket.Core.DOM.Node root)
        {
            CalculateLayout(root, ViewportWidth, ViewportHeight, Orientation, CurrentTime);
        }

        public void ApplyStyle(Rocket.Core.DOM.Node root, int width, int height, Orientation orientation)
        {
            CalculateLayout(root, width, height, orientation, CurrentTime);
        }

        public void ApplyStyle(Rocket.Core.DOM.Node root)
        {
            CalculateLayout(root, ViewportWidth, ViewportHeight, Orientation, CurrentTime);
        }

        private static bool SelectorMatches(string selector, string tag, string id, string className, Rocket.Core.DOM.Node node = null, VantaCssEngine engine = null)
        {
            if (string.IsNullOrWhiteSpace(selector)) return false;
            selector = selector.Trim();
            foreach (var s in selector.Split(','))
            {
                if (SingleSelectorMatches(s.Trim(), tag, id, className, node, engine)) return true;
            }
            return false;
        }

        private static bool SingleSelectorMatches(string selector, string tag, string id, string className, Rocket.Core.DOM.Node node = null, VantaCssEngine engine = null)
        {
            var parts = selector.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var last = parts.Length > 0 ? parts[parts.Length - 1] : selector;

            string t = null, i = null, c = null;
            var pseudos = new List<string>();
            int idx = 0;
            var sb = new StringBuilder();
            while (idx < last.Length && (char.IsLetterOrDigit(last[idx]) || last[idx] == '-'))
            {
                sb.Append(last[idx++]);
            }
            if (sb.Length > 0) t = sb.ToString();
            while (idx < last.Length)
            {
                if (last[idx] == '#')
                {
                    idx++;
                    var s2 = new StringBuilder();
                    while (idx < last.Length && (char.IsLetterOrDigit(last[idx]) || last[idx] == '-' || last[idx] == '_'))
                        s2.Append(last[idx++]);
                    i = s2.ToString();
                }
                else if (last[idx] == '.')
                {
                    idx++;
                    var s2 = new StringBuilder();
                    while (idx < last.Length && (char.IsLetterOrDigit(last[idx]) || last[idx] == '-' || last[idx] == '_'))
                        s2.Append(last[idx++]);
                    c = s2.ToString();
                }
                else if (last[idx] == ':')
                {
                    idx++;
                    var s2 = new StringBuilder();
                    s2.Append(':');
                    while (idx < last.Length && (char.IsLetterOrDigit(last[idx]) || last[idx] == '-' || last[idx] == '(' || last[idx] == ')'))
                        s2.Append(last[idx++]);
                    pseudos.Add(s2.ToString());
                }
                else
                {
                    idx++;
                }
            }

            if (i != null && i != id) return false;
            if (c != null)
            {
                if (string.IsNullOrEmpty(className)) return false;
                var classes = className.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                bool found = false;
                foreach (var cl in classes) if (cl == c) { found = true; break; }
                if (!found) return false;
            }
            if (t != null && !string.IsNullOrEmpty(tag))
            {
                bool match = string.Equals(t, tag, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(t, tag.Replace("HTML", "").Replace("Element", ""), StringComparison.OrdinalIgnoreCase) ||
                             tag.ToLowerInvariant().Contains(t.ToLowerInvariant());
                if (!match) return false;
            }

            // Pseudo-class matching.
            foreach (var ps in pseudos)
            {
                bool ok;
                if (engine != null) ok = engine.MatchesPseudo(node, ps);
                else if (node != null) ok = node.MatchesPseudo(ps);
                else ok = false;
                if (!ok) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Alias for <see cref="VantaCssEngine"/>.
    /// </summary>
    public class SimpleVantaCssEngine : VantaCssEngine { }

    /// <summary>
    /// Alias for <see cref="VantaCssEngine"/>.
    /// </summary>
    public class SimpleCSSEngine : VantaCssEngine { }

    /// <summary>
    /// Alias for <see cref="VantaCssEngine"/>.
    /// </summary>
    public class CSSEngine : VantaCssEngine { }

    /// <summary>
    /// Alias for <see cref="VantaCssEngine"/>.
    /// </summary>
    public class CssProcessor : VantaCssEngine { }
}
