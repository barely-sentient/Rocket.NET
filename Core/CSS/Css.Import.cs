using System;
using VantaCSS;
using System.Reflection;

namespace Rocket.Core
{
    /// <summary>
    /// Stylesheet import helpers for <see cref="Css"/>.
    /// </summary>
    public static partial class Css
    {
        /// <summary>Loads and registers a stylesheet from a path/resource.</summary>
        public static Stylesheet Import(string path, Assembly assembly = null)
        {
            var sheet = StylesheetLoader.Load(path, assembly);
            if (sheet != null) AddStylesheet(sheet);
            return sheet;
        }

        /// <summary>Loads stylesheets declared on a type.</summary>
        public static void ImportFor(Type type, VantaCSS.VantaCssEngine engine = null)
        {
            StylesheetLoader.LoadFor(type, engine);
        }

        /// <summary>Loads stylesheets declared on an assembly.</summary>
        public static void ImportFor(Assembly assembly, VantaCSS.VantaCssEngine engine = null)
        {
            StylesheetLoader.LoadFor(assembly, engine);
        }

        /// <summary>Scans an assembly for stylesheet attributes.</summary>
        public static void ImportAll(Assembly assembly, VantaCSS.VantaCssEngine engine = null)
        {
            StylesheetLoader.LoadAll(assembly, engine);
        }

        /// <summary>
        /// Scans all loaded non-system assemblies for component types carrying
        /// <see cref="StylesheetAttribute"/> and imports their stylesheets.
        /// </summary>
        public static void AutoImport(VantaCSS.VantaCssEngine engine = null)
        {
            StylesheetLoader.AutoImport(engine);
        }
    }
}
