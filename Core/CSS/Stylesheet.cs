using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Core
{
    /// <summary>
    /// A collection of CSS rules representing a stylesheet.
    /// </summary>
    public partial class Stylesheet
    {
        public string Name { get; set; }
        public List<CssRule> Rules { get; set; } = new List<CssRule>();
        public string RawCss { get; set; }
        public string Css { get => RawCss; set => RawCss = value; }
        public string Content { get => RawCss; set => RawCss = value; }

        public Stylesheet() { }

        public Stylesheet(string name) { Name = name; }

        public Stylesheet(string name, string css)
        {
            Name = name;
            RawCss = css;
            if (!string.IsNullOrWhiteSpace(css))
                Rules = Stylesheet.FromCss(css).Rules;
        }

        public static Stylesheet FromCss(string css)
        {
            var sheet = new Stylesheet();
            sheet.RawCss = css;
            if (string.IsNullOrWhiteSpace(css)) return sheet;
            css = css.Trim();
            int i = 0;
            while (i < css.Length)
            {
                while (i < css.Length && char.IsWhiteSpace(css[i])) i++;
                if (i >= css.Length) break;

                if (css[i] == '@')
                {
                    int mediaStart = css.IndexOf('{', i);
                    if (mediaStart < 0) break;
                    string mediaHeader = css.Substring(i, mediaStart - i).Trim();
                    int depth = 0;
                    int j = mediaStart;
                    for (; j < css.Length; j++)
                    {
                        if (css[j] == '{') depth++;
                        else if (css[j] == '}') { depth--; if (depth == 0) break; }
                    }
                    string inner = css.Substring(mediaStart + 1, j - mediaStart - 1);
                    string condition = mediaHeader;
                    if (condition.StartsWith("@media", StringComparison.OrdinalIgnoreCase))
                        condition = condition.Substring(6).Trim();
                    ParseRules(inner, sheet, condition);
                    i = j + 1;
                }
                else
                {
                    int brace = css.IndexOf('{', i);
                    if (brace < 0) break;
                    int close = css.IndexOf('}', brace);
                    if (close < 0) break;
                    string selector = css.Substring(i, brace - i).Trim();
                    string decls = css.Substring(brace + 1, close - brace - 1).Trim();
                    var rule = new CssRule(selector, decls);
                    sheet.Rules.Add(rule);
                    i = close + 1;
                }
            }
            return sheet;
        }

        private static void ParseRules(string css, Stylesheet sheet, string media)
        {
            int i = 0;
            while (i < css.Length)
            {
                while (i < css.Length && char.IsWhiteSpace(css[i])) i++;
                if (i >= css.Length) break;
                int brace = css.IndexOf('{', i);
                if (brace < 0) break;
                int close = css.IndexOf('}', brace);
                if (close < 0) break;
                string selector = css.Substring(i, brace - i).Trim();
                string decls = css.Substring(brace + 1, close - brace - 1).Trim();
                if (!string.IsNullOrEmpty(selector))
                {
                    var rule = new CssRule(selector, decls) { Media = media };
                    sheet.Rules.Add(rule);
                }
                i = close + 1;
            }
        }

        public void AddRule(string selector, string declarations, string media = null)
        {
            Rules.Add(new CssRule(selector, declarations) { Media = media });
        }
    }

    /// <summary>
    /// Alias for <see cref="Stylesheet"/>.
    /// </summary>
    public class StyleSheet : Stylesheet { }

    /// <summary>
    /// CSS stylesheet alias (all caps).
    /// </summary>
    public class CSSStylesheet : Stylesheet { }
}
