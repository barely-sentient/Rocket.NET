namespace VantaCSS.Grammar
{
    /// <summary>
    /// A parsed CSS declaration (property/value pair).
    /// </summary>
    public class CssDeclaration
    {
        /// <summary>
        /// Gets or sets the property name (including CSS variables).
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initialises a new, empty instance of the <see cref="CssDeclaration"/> class.
        /// </summary>
        public CssDeclaration()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CssDeclaration"/> class.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <param name="value">The property value.</param>
        public CssDeclaration(string property, string value)
        {
            Property = property;
            Value = value;
        }
    }
}