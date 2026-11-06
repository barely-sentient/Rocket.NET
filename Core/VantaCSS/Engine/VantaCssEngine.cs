using System;
using System.Collections.Generic;
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
                EnsureDefaults();
                if (SameUserSheets(value)) return;
                _stylesheets.Clear();
                _stylesheets.Add(DefaultStylesheet);
                if (value != null) _stylesheets.AddRange(value);
                _stylesheetVersion++;
            }
        }

        private bool SameUserSheets(List<Stylesheet> value)
        {
            int count = value?.Count ?? 0;
            if (_stylesheets.Count != count + 1) return false;
            if (!ReferenceEquals(_stylesheets[0], DefaultStylesheet)) return false;
            for (int i = 0; i < count; i++)
            {
                if (!ReferenceEquals(_stylesheets[i + 1], value[i])) return false;
            }
            return true;
        }

        private Stylesheet _defaultStylesheet;
        private Stylesheet DefaultStylesheet
        {
            get
            {
                if (_defaultStylesheet == null) _defaultStylesheet = Stylesheet.FromCss(DefaultCss);
                return _defaultStylesheet;
            }
        }

        private const string DefaultCss = @"
* { box-sizing: border-box; }
body { display: block; margin: 8px; }
div, section, article, header, footer, main, nav, aside { display: block; }
p { display: block; margin: 1em 0; }
br { display: block; }
heading { display: block; font-weight: bold; margin: 1em 0; }
h1, h2, h3, h4, h5, h6 { display: block; font-weight: bold; }
span, a, label, strong, em, b, i, u, s, del, ins, mark, small, sub, sup, code, kbd, samp { display: inline; margin: 0; padding: 0; }
strong, b { font-weight: bold; }
em, i { font-style: italic; }
u, ins { text-decoration: underline; }
s, del { text-decoration: line-through; }
mark { background-color: yellow; color: black; }
small { font-size: smaller; }
sub { vertical-align: sub; font-size: smaller; }
sup { vertical-align: super; font-size: smaller; }
code, kbd, samp, pre { font-family: monospace; }
img, image { display: inline-block; }
button { display: inline-block; margin: 0; padding: 4px 10px; background-color: #f0f0f0; color: #000000; border: 2px outset #808080; border-radius: 2px; cursor: default; text-align: center; font-size: 11px; }
button:hover { background-color: #e0e0e0; border-color: #606060; }
button:active { border-style: inset; background-color: #c0c0c0; border-color: #404040; color: #000000; }
button:disabled { color: #808080; background-color: #f0f0f0; border-color: #b0b0b0; }
input, textarea, select { display: inline-block; margin: 0; padding: 4px; background-color: #ffffff; color: #000000; border: 2px inset #808080; border-radius: 2px; }
input:disabled, textarea:disabled, select:disabled { color: #808080; background-color: #f0f0f0; border-color: #b0b0b0; }
input::placeholder, textarea::placeholder { color: #757575; opacity: 1; }
textarea { padding: 4px; }
select { display: inline-block; width: 160px; height: 24px; padding: 1px 4px; font-size: 13px; }
option { display: none; }
ul, ol, menu, dir { display: block; margin: 1em 0; padding-left: 40px; list-style-type: disc; }
ol { list-style-type: decimal; }
li { display: list-item; }
table { display: table; border-collapse: separate; border-spacing: 2px; text-indent: initial; box-sizing: border-box; border: 1px solid #808080; }
td, th { border: 1px solid #808080; }
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
            _stylesheets.Insert(0, DefaultStylesheet);
            _stylesheetVersion++;
        }

        private static System.Net.Http.HttpClient _fontHttp;

        private void ProcessFontFaces(string css)
        {
            if (string.IsNullOrWhiteSpace(css)) return;
            try
            {
                foreach (var rule in VantaCSS.Grammar.Grammar.ParseRules(css))
                {
                    if (!rule.IsAtRule || !string.Equals(rule.Selector, "@font-face", StringComparison.OrdinalIgnoreCase)) continue;

                    string family = null;
                    string src = null;
                    foreach (var d in rule.Declarations)
                    {
                        if (string.Equals(d.Property, "font-family", StringComparison.OrdinalIgnoreCase)) family = d.Value.Trim().Trim('"', '\'');
                        if (string.Equals(d.Property, "src", StringComparison.OrdinalIgnoreCase)) src = d.Value.Trim();
                    }
                    if (string.IsNullOrWhiteSpace(family) || string.IsNullOrWhiteSpace(src)) continue;

                    // Extract first url(...) from src.
                    var url = VantaCSS.Grammar.Grammar.ParseFontUrl(src);
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
                        _fontHttp = _fontHttp ?? new System.Net.Http.HttpClient();
                        var bytes = _fontHttp.GetByteArrayAsync(uri).GetAwaiter().GetResult();
                        FontFactory.RegisterFont(family, bytes);
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
            Stylesheets.Add(Stylesheet.FromCss(css));
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
            var queries = VantaCSS.Grammar.Grammar.ParseMediaQueries(media);
            if (queries == null) return false;
            foreach (var query in queries)
            {
                if (MediaQueryMatches(query, width, height, orientation)) return true;
            }
            return false;
        }

        private bool MediaQueryMatches(VantaCSS.Grammar.MediaQuery query, int width, int height, Orientation orientation)
        {
            bool matches = query.MediaType == null ||
                           string.Equals(query.MediaType, "all", StringComparison.Ordinal) ||
                           string.Equals(query.MediaType, "screen", StringComparison.Ordinal);
            foreach (var feature in query.Features)
            {
                if (!matches) break;
                matches = MediaFeatureMatches(feature, width, height, orientation);
            }
            return query.Negated ? !matches : matches;
        }

        private bool MediaFeatureMatches(VantaCSS.Grammar.MediaFeature feature, int width, int height, Orientation orientation)
        {
            double limit = ParseLen(feature.Value, 0);
            switch (feature.Name)
            {
                case "min-width": return double.IsNaN(limit) || width >= limit;
                case "max-width": return double.IsNaN(limit) || width <= limit;
                case "min-height": return double.IsNaN(limit) || height >= limit;
                case "max-height": return double.IsNaN(limit) || height <= limit;
                case "orientation": return string.Equals(feature.Value, orientation.ToString(), StringComparison.OrdinalIgnoreCase);
                default: return true;
            }
        }

        public string GenerateCss()
        {
            return GenerateCss(ViewportWidth, ViewportHeight, Orientation);
        }

        public string GenerateCss(int width, int height, Orientation orientation)
        {
            return BuildCss(width, height, orientation);
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


        public string ComputeStyle(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation)
        {
            return ComputeStyle(node, width, height, orientation, CurrentTime);
        }

        public string ComputeStyle(Rocket.Core.DOM.Node node)
        {
            return ComputeStyle(node, ViewportWidth, ViewportHeight, Orientation);
        }



        public Dictionary<string, string> ComputeStyles(Rocket.Core.DOM.Node node)
        {
            return ComputeStyles(node, ViewportWidth, ViewportHeight, Orientation, CurrentTime);
        }

        public void ApplyComputedStyles(Rocket.Core.DOM.Node root, int width, int height, Orientation orientation)
        {
            ApplyComputedStyles(root, width, height, orientation, CurrentTime);
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
            case "u": return normalizedTypeName == "underline";
            case "del": return normalizedTypeName == "del";
            case "ins": return normalizedTypeName == "ins";
            case "mark": return normalizedTypeName == "mark";
            case "small": return normalizedTypeName == "small";
            case "sub": return normalizedTypeName == "sub";
            case "sup": return normalizedTypeName == "sup";
            case "strong": return normalizedTypeName == "strong";
            case "em": return normalizedTypeName == "em";
            case "code": return normalizedTypeName == "code";
            case "pre": return normalizedTypeName == "pre";
            case "tr": return normalizedTypeName == "tablerow";
            case "td": return normalizedTypeName == "tablecell";
            case "th": return normalizedTypeName == "tableheadercell";
            case "thead": return normalizedTypeName == "tableheadergroup"; // no component, harmless
            case "tbody": return normalizedTypeName == "tablerowgroup";
            case "tfoot": return normalizedTypeName == "tablefootergroup";
            case "caption": return normalizedTypeName == "tablecaption";
            case "col": return normalizedTypeName == "tablecolumn";
            case "colgroup": return normalizedTypeName == "tablecolumngroup";
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

        private static bool SelectorMatches(string selector, string tag, string id, string className, Rocket.Core.DOM.Node node = null, VantaCssEngine engine = null)
        {
            foreach (var complex in VantaCSS.Grammar.Grammar.ParseSelectors(selector).Items)
            {
                if (MatchesComplexAt(complex, complex.Compounds.Count - 1, tag, id, className, node, engine)) return true;
            }
            return false;
        }

        private static bool MatchesComplexAt(VantaCSS.Grammar.ComplexSelector complex, int index, string tag, string id, string className, Rocket.Core.DOM.Node node, VantaCssEngine engine)
        {
            if (!CompoundMatches(complex.Compounds[index], tag, id, className, node, engine)) return false;
            if (index == 0) return true;

            switch (complex.Combinators[index - 1])
            {
                case '>':
                    return node?.Parent != null && MatchesNode(complex, index - 1, node.Parent, engine);
                case '+':
                    foreach (var sibling in PreviousSiblings(node))
                    {
                        return MatchesNode(complex, index - 1, sibling, engine);
                    }
                    return false;
                case '~':
                    foreach (var sibling in PreviousSiblings(node))
                    {
                        if (MatchesNode(complex, index - 1, sibling, engine)) return true;
                    }
                    return false;
                default:
                    for (var ancestor = node?.Parent; ancestor != null; ancestor = ancestor.Parent)
                    {
                        if (MatchesNode(complex, index - 1, ancestor, engine)) return true;
                    }
                    return false;
            }
        }

        private static bool MatchesNode(VantaCSS.Grammar.ComplexSelector complex, int index, Rocket.Core.DOM.Node node, VantaCssEngine engine)
        {
            if (node == null) return false;
            GetNodeIdentity(node, out var tag, out var id, out var className);
            return MatchesComplexAt(complex, index, tag, id, className, node, engine);
        }

        private static IEnumerable<Rocket.Core.DOM.Node> PreviousSiblings(Rocket.Core.DOM.Node node)
        {
            var siblings = node?.Parent?.Children;
            if (siblings == null) yield break;
            int index = Array.IndexOf(siblings, node);
            for (int i = index - 1; i >= 0; i--) yield return siblings[i];
        }

        private static bool CompoundMatches(VantaCSS.Grammar.CompoundSelector compound, string tag, string id, string className, Rocket.Core.DOM.Node node, VantaCssEngine engine)
        {
            if (compound.Type != null && !string.IsNullOrEmpty(tag) && !TypeMatches(compound.Type, tag)) return false;
            foreach (var wanted in compound.Ids)
            {
                if (wanted != id) return false;
            }
            foreach (var wanted in compound.Classes)
            {
                if (!HasClass(className, wanted)) return false;
            }
            foreach (var pseudo in compound.Pseudos)
            {
                if (!PseudoMatches(pseudo, node, engine)) return false;
            }
            return true;
        }

        private static bool TypeMatches(string selectorType, string tag)
        {
            string normTag = tag.Replace("HTML", "").Replace("Element", "").ToLowerInvariant();
            string sel = selectorType.ToLowerInvariant();
            return string.Equals(sel, tag, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(sel, normTag, StringComparison.OrdinalIgnoreCase) ||
                   IsCssTagAlias(sel, normTag) ||
                   (sel.Length >= 3 && normTag.Contains(sel));
        }

        private static bool HasClass(string className, string wanted)
        {
            if (string.IsNullOrEmpty(className)) return false;
            foreach (var cl in className.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (cl == wanted) return true;
            }
            return false;
        }

        private static bool PseudoMatches(VantaCSS.Grammar.PseudoSelector pseudo, Rocket.Core.DOM.Node node, VantaCssEngine engine)
        {
            if (pseudo.Argument != null)
            {
                foreach (var complex in pseudo.Argument.Items)
                {
                    if (MatchesNode(complex, complex.Compounds.Count - 1, node, engine)) return true;
                }
                return false;
            }
            if (string.Equals(pseudo.Name, ":root", StringComparison.OrdinalIgnoreCase))
            {
                return node != null && node.Parent == null;
            }
            if (engine != null) return engine.MatchesPseudo(node, pseudo.Raw);
            return node != null && node.MatchesPseudo(pseudo.Raw);
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
                id = ids.IdGetter?.Invoke(props);
                className = ids.ClassGetter?.Invoke(props);
            }
        }
    }
}
