using System;
using System.Collections.Generic;

namespace VantaCSS
{
    /// <summary>
    /// A collection of CSS rules representing a stylesheet.
    /// </summary>
    public partial class Stylesheet
    {
        public string Name { get; set; }
        public List<CssRule> Rules { get; set; } = new List<CssRule>();
        public string RawCss { get; set; }

        public Stylesheet() { }

        public Stylesheet(string name) { Name = name; }

        public Stylesheet(string name, string css)
        {
            Name = name;
            ReloadCss(css);
        }

        /// <summary>
        /// Replaces the stylesheet's CSS and re-parses its rules. Used by hot reload.
        /// </summary>
        public void ReloadCss(string css)
        {
            RawCss = css ?? string.Empty;
            if (string.IsNullOrWhiteSpace(css))
                Rules = new List<CssRule>();
            else
            {
                try { Rules = Stylesheet.FromCss(css).Rules; }
                catch { Rules = Rules ?? new List<CssRule>(); }
            }
        }

        /// <summary>
        /// Parses CSS source into a stylesheet using the VantaCSS grammar.
        /// </summary>
        public static Stylesheet FromCss(string css)
        {
            var sheet = new Stylesheet { RawCss = css };
            if (string.IsNullOrWhiteSpace(css)) return sheet;

            foreach (var parsed in VantaCSS.Grammar.Grammar.ParseRules(css))
            {
                if (!parsed.IsAtRule)
                {
                    sheet.Rules.Add(ToRule(parsed, null));
                    continue;
                }

                // At-rules without a block (for example @import) carry no style rules.
                if (parsed.Rules == null) continue;
                foreach (var nested in parsed.Rules)
                    sheet.Rules.Add(ToRule(nested, parsed.Media));
            }
            return sheet;
        }

        private static CssRule ToRule(VantaCSS.Grammar.CssRule source, string media)
        {
            var rule = new CssRule { Selector = source.Selector, Media = media, Raw = source.Raw };
            foreach (var declaration in source.Declarations) rule.Declarations[declaration.Property] = declaration.Value;
            return rule;
        }

        public void AddRule(string selector, string declarations, string media = null)
        {
            Rules.Add(new CssRule(selector, declarations) { Media = media });
        }
    }
}
