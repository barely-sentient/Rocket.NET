using System;

namespace Rocket.Core
{
    /// <summary>
    /// Declares a stylesheet that should be imported for a component or assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The path is resolved at runtime. Because different .NET hosts (desktop,
    /// web, Unity, etc.) expose files differently, the loader tries, in order:
    /// embedded manifest resources, the application base directory, the current
    /// working directory, and (when present) Unity's StreamingAssets folder.
    /// </para>
    /// <para>
    /// For reliable cross-platform behaviour, prefer embedding the CSS file as
    /// a manifest resource (Build Action = Embedded Resource in most SDKs).
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class StylesheetAttribute : Attribute
    {
        /// <summary>The stylesheet path or embedded resource name.</summary>
        public string Path { get; }

        /// <summary>Optional friendly name for the loaded stylesheet.</summary>
        public string Name { get; set; }

        /// <summary>
        /// When true (default) the loader will also try to resolve the path as
        /// an embedded manifest resource.
        /// </summary>
        public bool Embedded { get; set; } = true;

        public StylesheetAttribute(string path)
        {
            Path = path;
        }
    }

    /// <summary>Alias for <see cref="StylesheetAttribute"/>.</summary>
    public class StyleSheetAttribute : StylesheetAttribute
    {
        public StyleSheetAttribute(string path) : base(path) { }
    }

    /// <summary>Alias for <see cref="StylesheetAttribute"/>.</summary>
    public class CSSStylesheetAttribute : StylesheetAttribute
    {
        public CSSStylesheetAttribute(string path) : base(path) { }
    }
}
