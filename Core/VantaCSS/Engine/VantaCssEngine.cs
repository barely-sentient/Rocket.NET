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

        public FontFactory FontFactory { get; set; } = FontFactory.Instance;

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
                _defaultsAdded = false;
                EnsureDefaults();
                if (value != null) _stylesheets.AddRange(value);
                _stylesheetVersion++;
            }
        }

        private const string DefaultCss = @"
* { box-sizing: border-box; }
body { display: block; margin: 8px; }
div, section, article, header, footer, main, nav, aside { display: block; }
p { display: block; margin: 1em 0; }
heading { display: block; font-weight: bold; margin: 1em 0; }
h1, h2, h3, h4, h5, h6 { display: block; font-weight: bold; }
span, a, label, strong, em, b, i { display: inline; margin: 0; padding: 0; }
img, image { display: inline-block; }
button { display: inline-block; margin: 0; padding: 4px 10px; background-color: #f0f0f0; color: #000000; border: 2px outset #808080; border-radius: 2px; cursor: default; text-align: center; font-size: 11px; }
button:hover { background-color: #e0e0e0; border-color: #606060; }
button:active { border-style: inset; background-color: #c0c0c0; border-color: #404040; color: #000000; }
button:disabled { color: #808080; background-color: #f0f0f0; border-color: #b0b0b0; }
input, textarea, select { display: inline-block; margin: 0; padding: 4px; background-color: #ffffff; color: #000000; border: 2px inset #808080; border-radius: 2px; }
input:disabled, textarea:disabled, select:disabled { color: #808080; background-color: #f0f0f0; border-color: #b0b0b0; }
input::placeholder, textarea::placeholder { color: #757575; opacity: 1; }
textarea { padding: 4px; }
select { padding: 4px; }
ul, ol, menu, dir { display: block; margin: 1em 0; padding-left: 40px; list-style-type: disc; }
ol { list-style-type: decimal; }
li { display: list-item; }
table { display: table; border-collapse: separate; border-spacing: 2px; text-indent: initial; box-sizing: border-box; }
thead, tbody, tfoot { display: table-row-group; vertical-align: middle; border-color: inherit; }
tr { display: table-row; vertical-align: inherit; border-color: inherit; }
td, th { display: table-cell; vertical-align: inherit; padding: 2px; }
th { font-weight: bold; }
caption { display: table-caption; text-align: center; }
blockquote { display: block; margin: 1em 40px; }
hr { display: block; margin: 0.5em 0; overflow: hidden; color: gray; border-style: inset; border-width: 1px; }
pre, code, kbd, samp { font-family: monospace; }
a { color: blue; text-decoration: underline; cursor: pointer; }
abbr[title], acronym[title] { text-decoration: dotted underline; }
fieldset { display: block; margin: 0 2px; padding: 0.35em 0.75em 0.625em; border: 2px groove gray; }
legend { padding: 0 2px; border: none; }
label { cursor: default; }
form { display: block; margin-top: 0; }
figure { display: block; margin: 1em 40px; }
figcaption { display: block; }
center { display: block; text-align: center; }
dd { display: block; margin-left: 40px; }
dl { display: block; margin: 1em 0; }
dt { display: block; }
output { display: inline; }
progress, meter { display: inline-block; }
img, image { display: inline; width: auto; height: auto; }
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
        public int StylesheetVersion => _stylesheetVersion;
        public void InvalidateStyleCache() { _stylesheetVersion++; }

        private void EnsureDefaults()
        {
            if (_defaultsAdded) return;
            _defaultsAdded = true;
            AddStylesheet(DefaultCss);
        }

        private void ProcessFontFaces(string css)
        {
            if (string.IsNullOrWhiteSpace(css)) return;
            try
            {
                foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(css, @"@font-face\s*\{([^}]*)\}", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    var decls = VantaCSS.Grammar.Grammar.ParseDeclarations(m.Groups[1].Value);
                    string family = null;
                    string src = null;
                    foreach (var d in decls)
                    {
                        if (string.Equals(d.Property, "font-family", StringComparison.OrdinalIgnoreCase)) family = d.Value.Trim().Trim('"', '\'');
                        if (string.Equals(d.Property, "src", StringComparison.OrdinalIgnoreCase)) src = d.Value.Trim();
                    }
                    if (string.IsNullOrWhiteSpace(family) || string.IsNullOrWhiteSpace(src)) continue;

                    // Extract first url(...) from src.
                    var urlMatch = System.Text.RegularExpressions.Regex.Match(src, @"url\(\s*['""]?([^'""\)]+)['""]?\s*\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (!urlMatch.Success) continue;
                    var url = urlMatch.Groups[1].Value.Trim();
                    if (string.IsNullOrWhiteSpace(url)) continue;

                    // Only absolute http(s) URLs are downloadable; relative paths are left to the host.
                    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https")) continue;

                    // woff2 cannot be decoded by the current renderer; log and skip.
                    if (url.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase))
                    {
                        System.Console.WriteLine($"[VantaCSS] @font-face '{family}' points to woff2, which is not supported yet: {url}");
                        continue;
                    }
                    if (!url.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) && !url.EndsWith(".otf", StringComparison.OrdinalIgnoreCase)) continue;

                    try
                    {
                        using (var http = new System.Net.Http.HttpClient())
                        {
                            var bytes = http.GetByteArrayAsync(uri).GetAwaiter().GetResult();
                            FontFactory.RegisterFont(family, bytes);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine($"[VantaCSS] Failed to download font '{family}' from {url}: {ex.Message}");
                    }
                }
            }
            catch { }
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
            ProcessFontFaces(css);
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
                // Inherit inheritable properties.
                foreach (var kv in root.Parent.ComputedStyles)
                {
                    if (IsInheritable(kv.Key) && !root.ComputedStyles.ContainsKey(kv.Key))
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

        private static bool IsCssTagAlias(string selectorTag, string normalizedTypeName)
    {
        switch (selectorTag)
        {
            case "a": return normalizedTypeName == "anchor";
            case "b": return normalizedTypeName == "bold";
            case "i": return normalizedTypeName == "italic";
            case "p": return normalizedTypeName == "paragraph";
            case "img": return normalizedTypeName == "image";
            case "ul": return normalizedTypeName == "ulist";
            case "ol": return normalizedTypeName == "olist";
            case "li": return normalizedTypeName == "listitem";
            case "hr": return normalizedTypeName == "hr";
            default: return false;
        }
    }

    public static bool IsInheritable(string prop)
        {
            if (string.IsNullOrWhiteSpace(prop)) return false;
            prop = prop.Trim().ToLowerInvariant();
            switch (prop)
            {
                case "color":
                case "font":
                case "font-family":
                case "font-size":
                case "font-style":
                case "font-variant":
                case "font-weight":
                case "line-height":
                case "letter-spacing":
                case "text-align":
                case "text-align-all":
                case "text-align-last":
                case "text-indent":
                case "text-transform":
                case "text-wrap":
                case "text-wrap-mode":
                case "text-wrap-style":
                case "text-justify":
                case "text-orientation":
                case "text-combine-upright":
                case "text-autospace":
                case "text-spacing-trim":
                case "text-overflow":
                case "white-space":
                case "white-space-collapse":
                case "white-space-trim":
                case "word-spacing":
                case "word-break":
                case "overflow-wrap":
                case "word-wrap":
                case "hyphens":
                case "hyphenate-character":
                case "hyphenate-limit-chars":
                case "line-break":
                case "hanging-punctuation":
                case "writing-mode":
                case "unicode-bidi":
                case "tab-size":
                case "visibility":
                case "cursor":
                case "direction":
                case "list-style":
                case "list-style-type":
                case "list-style-position":
                case "list-style-image":
                case "quotes":
                case "border-collapse":
                case "empty-cells":
                case "caption-side":
                case "orphans":
                case "widows":
                    return true;
                default:
                    return prop.StartsWith("--", StringComparison.Ordinal);
            }
        }

        private static string StripCssComments(string selector)
        {
            if (string.IsNullOrEmpty(selector) || selector.IndexOf("/*", StringComparison.Ordinal) < 0)
                return selector;
            var sb = new StringBuilder(selector.Length);
            int i = 0;
            while (i < selector.Length)
            {
                int start = selector.IndexOf("/*", i, StringComparison.Ordinal);
                if (start < 0)
                {
                    sb.Append(selector, i, selector.Length - i);
                    break;
                }
                sb.Append(selector, i, start - i);
                int end = selector.IndexOf("*/", start + 2, StringComparison.Ordinal);
                if (end < 0) break;
                i = end + 2;
            }
            return sb.ToString();
        }

        private static bool SelectorMatches(string selector, string tag, string id, string className, Rocket.Core.DOM.Node node = null, VantaCssEngine engine = null)
        {
            if (string.IsNullOrWhiteSpace(selector)) return false;
            selector = StripCssComments(selector).Trim();
            if (selector.Length == 0) return false;
            // Split only top-level commas (respect parentheses) so :is(...) selectors stay intact.
            var parts = new List<string>();
            int depth = 0; int start = 0;
            for (int i = 0; i <= selector.Length; i++)
            {
                if (i == selector.Length || (selector[i] == ',' && depth == 0))
                {
                    parts.Add(selector.Substring(start, i - start).Trim());
                    start = i + 1;
                }
                else if (i < selector.Length && selector[i] == '(') depth++;
                else if (i < selector.Length && selector[i] == ')') depth--;
            }
            foreach (var s in parts)
            {
                if (string.IsNullOrWhiteSpace(s)) continue;
                if (SingleSelectorMatches(s, tag, id, className, node, engine)) return true;
            }
            return false;
        }

        private static string NormalizeCombinators(string selector)
        {
            if (string.IsNullOrEmpty(selector)) return selector;
            var sb = new StringBuilder(selector.Length + 8);
            int depth = 0;
            foreach (char ch in selector)
            {
                if (ch == '(') { depth++; sb.Append(ch); }
                else if (ch == ')') { depth--; sb.Append(ch); }
                else if (depth == 0 && (ch == '>' || ch == '+' || ch == '~'))
                {
                    sb.Append(' '); sb.Append(ch); sb.Append(' ');
                }
                else sb.Append(ch);
            }
            return sb.ToString();
        }

        private static void GetNodeIdentity(Rocket.Core.DOM.Node node, out string tag, out string id, out string className)
        {
            tag = null; id = null; className = null;
            if (node == null) return;
            try { tag = node.Type != null ? node.Type.Name : null; } catch { }
            var props = node.UntypedProps;
            if (props != null)
            {
                var ids = GetPropIds(props.GetType());
                id = ids.IdField?.GetValue(props) as string ?? ids.IdProp?.GetValue(props) as string;
                className = ids.ClassField?.GetValue(props) as string ?? ids.ClassProp?.GetValue(props) as string;
            }
        }

        private static bool NodeMatchesSimpleSelector(string selector, Rocket.Core.DOM.Node node, VantaCssEngine engine)
        {
            if (node == null) return false;
            string tag, id, className;
            GetNodeIdentity(node, out tag, out id, out className);
            return SingleSelectorMatches(selector, tag, id, className, node, engine);
        }

        private static bool MatchesAncestorChain(string[] parts, Rocket.Core.DOM.Node node, VantaCssEngine engine)
        {
            if (parts == null || parts.Length <= 1 || node == null) return true;
            var current = node;
            for (int p = parts.Length - 2; p >= 0; p--)
            {
                char combinator = ' ';
                string token = parts[p];
                if (token == ">" || token == "+" || token == "~")
                {
                    combinator = token[0];
                    p--;
                    if (p < 0) return false;
                    token = parts[p];
                }
                if (string.IsNullOrWhiteSpace(token)) continue;

                bool found = false;
                if (combinator == '>')
                {
                    var parent = current.Parent;
                    if (parent != null && NodeMatchesSimpleSelector(token, parent, engine))
                    {
                        found = true;
                        current = parent;
                    }
                }
                else if (combinator == '+')
                {
                    var parent = current.Parent;
                    if (parent != null && parent.Children != null)
                    {
                        int idxChild = -1;
                        for (int j = 0; j < parent.Children.Length; j++)
                            if (parent.Children[j] == current) { idxChild = j; break; }
                        if (idxChild > 0 && NodeMatchesSimpleSelector(token, parent.Children[idxChild - 1], engine))
                        {
                            found = true;
                            current = parent.Children[idxChild - 1];
                        }
                    }
                }
                else if (combinator == '~')
                {
                    var parent = current.Parent;
                    if (parent != null && parent.Children != null)
                    {
                        int idxChild = -1;
                        for (int j = 0; j < parent.Children.Length; j++)
                            if (parent.Children[j] == current) { idxChild = j; break; }
                        for (int j = idxChild - 1; j >= 0; j--)
                        {
                            if (NodeMatchesSimpleSelector(token, parent.Children[j], engine))
                            {
                                found = true;
                                current = parent.Children[j];
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var ancestor = current.Parent;
                    while (ancestor != null)
                    {
                        if (NodeMatchesSimpleSelector(token, ancestor, engine))
                        {
                            found = true;
                            current = ancestor;
                            break;
                        }
                        ancestor = ancestor.Parent;
                    }
                }
                if (!found) return false;
            }
            return true;
        }

        private static bool SingleSelectorMatches(string selector, string tag, string id, string className, Rocket.Core.DOM.Node node = null, VantaCssEngine engine = null)
        {
            selector = (selector ?? string.Empty).Trim();
            // Support :is(...) functional pseudo-class (used by Font Awesome and modern CSS).
            if (selector.IndexOf(":is(", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                int isIdx = selector.IndexOf(":is(", StringComparison.OrdinalIgnoreCase);
                int open = selector.IndexOf('(', isIdx);
                int depth = 0;
                int close = -1;
                for (int ii = open; ii < selector.Length; ii++)
                {
                    if (selector[ii] == '(') depth++;
                    else if (selector[ii] == ')') { depth--; if (depth == 0) { close = ii; break; } }
                }
                if (open >= 0 && close > open)
                {
                    string inner = selector.Substring(open + 1, close - open - 1);
                    string rest = selector.Remove(isIdx, close - isIdx + 1).Trim();
                    bool any = false;
                    int d = 0; int start = 0;
                    for (int k = 0; k <= inner.Length; k++)
                    {
                        if (k == inner.Length || (inner[k] == ',' && d == 0))
                        {
                            var part = inner.Substring(start, k - start).Trim();
                            start = k + 1;
                            if (string.IsNullOrEmpty(part)) continue;
                            if (SingleSelectorMatches(part, tag, id, className, node, engine)) { any = true; break; }
                        }
                        else if (inner[k] == '(') d++;
                        else if (inner[k] == ')') d--;
                    }
                    if (!any) return false;
                    if (!string.IsNullOrWhiteSpace(rest))
                    {
                        rest = rest.Replace("::before", "").Replace("::after", "").Trim();
                        if (!string.IsNullOrWhiteSpace(rest) && !SingleSelectorMatches(rest, tag, id, className, node, engine))
                            return false;
                    }
                    return true;
                }
            }

            if (string.Equals(selector.Trim(), ":root", StringComparison.OrdinalIgnoreCase))
            {
                return node != null && node.Parent == null;
            }
            var parts = NormalizeCombinators(selector).Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
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
                    // Support pseudo-elements (::before, ::after) and pseudo-classes (:hover, ...).
                    if (idx + 1 < last.Length && last[idx + 1] == ':')
                    {
                        idx += 2;
                        var s2 = new StringBuilder();
                        s2.Append("::");
                        while (idx < last.Length && (char.IsLetterOrDigit(last[idx]) || last[idx] == '-' || last[idx] == '(' || last[idx] == ')'))
                            s2.Append(last[idx++]);
                        var pseudoEl = s2.ToString();
                        // Pseudo-elements do not affect base-node matching; they are handled separately.
                        if (!string.Equals(pseudoEl, "::before", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(pseudoEl, "::after", StringComparison.OrdinalIgnoreCase))
                        {
                            pseudos.Add(pseudoEl);
                        }
                    }
                    else
                    {
                        idx++;
                        var s2 = new StringBuilder();
                        s2.Append(':');
                        while (idx < last.Length && (char.IsLetterOrDigit(last[idx]) || last[idx] == '-' || last[idx] == '(' || last[idx] == ')'))
                            s2.Append(last[idx++]);
                        pseudos.Add(s2.ToString());
                    }
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
                string normTag = tag.Replace("HTML", "").Replace("Element", "").ToLowerInvariant();
                string sel = t.ToLowerInvariant();
                bool match = string.Equals(sel, tag, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(sel, normTag, StringComparison.OrdinalIgnoreCase) ||
                             IsCssTagAlias(sel, normTag) ||
                             (sel.Length >= 3 && normTag.Contains(sel));
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

            // Verify any ancestor/sibling qualifiers (descendant ' ', child '>',
            // adjacent '+', general sibling '~').
            return MatchesAncestorChain(parts, node, engine);
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
