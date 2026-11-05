using System;
using System.Collections.Generic;
using System.Text;

namespace VantaCSS
{
    /// <summary>
    /// A single CSS declaration block rule.
    /// </summary>
    public partial class CssRule
    {
        public string Selector { get; set; }
        public Dictionary<string, string> Declarations { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Optional media query condition (without @media).
        /// </summary>
        public string Media { get; set; }
        public string Raw { get; set; }

        public CssRule() { }

        public CssRule(string selector, string declarations)
        {
            Selector = selector;
            if (!string.IsNullOrWhiteSpace(declarations))
            {
                foreach (var part in declarations.Split(';'))
                {
                    var trimmed = part.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;
                    var idx = trimmed.IndexOf(':');
                    if (idx > 0)
                    {
                        var key = trimmed.Substring(0, idx).Trim();
                        var val = trimmed.Substring(idx + 1).Trim();
                        Declarations[key] = val;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Alias for <see cref="CssRule"/>.
    /// </summary>
    public class CSSRule : CssRule { }
}
