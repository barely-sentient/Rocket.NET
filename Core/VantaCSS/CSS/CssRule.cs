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
            if (string.IsNullOrWhiteSpace(declarations)) return;
            foreach (var declaration in VantaCSS.Grammar.Grammar.ParseDeclarations(declarations))
                Declarations[declaration.Property] = declaration.Value;
        }
    }
}
