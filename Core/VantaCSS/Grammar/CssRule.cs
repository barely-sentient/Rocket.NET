using System.Collections.Generic;

namespace VantaCSS.Grammar
{
    /// <summary>
    /// A parsed CSS rule, containing a selector and its declarations.
    /// </summary>
    public class CssRule
    {
        /// <summary>
        /// Gets or sets the selector (or at-rule header).
        /// </summary>
        public string Selector { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the media query condition for at-rules, if applicable.
        /// </summary>
        public string Media { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this rule is an at-rule.
        /// </summary>
        public bool IsAtRule { get; set; }

        /// <summary>
        /// Gets or sets the declarations contained in this rule.
        /// </summary>
        public List<CssDeclaration> Declarations { get; set; } = new List<CssDeclaration>();

        /// <summary>
        /// Gets or sets the nested rules (used by at-rules such as @media).
        /// </summary>
        public List<CssRule> Rules { get; set; } = new List<CssRule>();

        /// <summary>
        /// Gets or sets the raw source text for this rule.
        /// </summary>
        public string Raw { get; set; }
    }
}