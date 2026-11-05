using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.Core;
using Rocket.Core.DOM;

namespace VantaCSS
{
    /// <summary>
    /// Resolves <see cref="StylesheetAttribute"/> declarations into
    /// <see cref="Stylesheet"/> instances across .NET hosts.
    /// </summary>
    public static class StylesheetLoader
    {
        /// <summary>
        /// Loads a stylesheet from a path or embedded resource name.
        /// </summary>
        public static Stylesheet Load(string path, Assembly assembly = null, string name = null)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            assembly = assembly ?? Assembly.GetCallingAssembly();

            string css = TryReadEmbedded(path, assembly);
            if (css == null) css = TryReadFile(path);
            if (css == null) return null;

            var sheet = new Stylesheet(name ?? path, css);
            return sheet;
        }

        /// <summary>
        /// Loads every <see cref="StylesheetAttribute"/> declared on a type.
        /// </summary>
        public static void LoadFor(Type type, VantaCssEngine engine = null)
        {
            if (type == null) return;
            engine = engine ?? Css.Engine;
            foreach (var attr in type.GetCustomAttributes(typeof(StylesheetAttribute), true).Cast<StylesheetAttribute>())
            {
                var sheet = Load(attr.Path, type.Assembly, attr.Name);
                if (sheet != null) (engine ?? Css.Engine).AddStylesheet(sheet);
            }
        }

        /// <summary>
        /// Loads every <see cref="StylesheetAttribute"/> declared on an assembly.
        /// </summary>
        public static void LoadFor(Assembly assembly, VantaCssEngine engine = null)
        {
            if (assembly == null) return;
            engine = engine ?? Css.Engine;
            var attrs = assembly.GetCustomAttributes(typeof(StylesheetAttribute));
            foreach (StylesheetAttribute attr in attrs)
            {
                var sheet = Load(attr.Path, assembly, attr.Name);
                if (sheet != null) (engine ?? Css.Engine).AddStylesheet(sheet);
            }
        }

        /// <summary>
        /// Scans a single assembly for component types carrying
        /// <see cref="StylesheetAttribute"/> and imports their stylesheets.
        /// </summary>
        public static void AutoImport(Assembly assembly, VantaCssEngine engine = null)
        {
            if (assembly == null) return;
            if (IsSystemAssembly(assembly)) return;
            engine = engine ?? Css.Engine;
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!IsComponentType(type)) continue;
                    if (type.GetCustomAttributes(typeof(StylesheetAttribute), true).Length == 0) continue;
                    LoadFor(type, engine);
                }
                LoadFor(assembly, engine);
            }
            catch { /* ignore reflection errors */ }
        }

        /// <summary>
        /// Scans all currently loaded, non-system assemblies for types deriving
        /// from <see cref="Component{TProps}"/> that carry a
        /// <see cref="StylesheetAttribute"/>, and imports their stylesheets.
        /// </summary>
        public static void AutoImport(VantaCssEngine engine = null)
        {
            engine = engine ?? Css.Engine;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                AutoImport(asm, engine);
            }
        }

        /// <summary>
        /// Scans all types in an assembly for <see cref="StylesheetAttribute"/>.
        /// </summary>
        public static void LoadAll(Assembly assembly, VantaCssEngine engine = null)
        {
            if (assembly == null) return;
            engine = engine ?? Css.Engine;
            try
            {
                foreach (var type in assembly.GetTypes())
                    LoadFor(type, engine);
            }
            catch { /* ignore reflection errors */ }
            LoadFor(assembly, engine);
        }

        private static string TryReadEmbedded(string path, Assembly assembly)
        {
            if (assembly == null) return null;
            var names = assembly.GetManifestResourceNames();
            // Direct match.
            if (names.Contains(path)) return ReadStream(assembly.GetManifestResourceStream(path));
            // Try assembly-prefixed / folder variants.
            string file = Path.GetFileName(path);
            var candidate = names.FirstOrDefault(n => n.Equals(path, StringComparison.OrdinalIgnoreCase) ||
                                                     n.EndsWith(path.Replace('/', '.'), StringComparison.OrdinalIgnoreCase) ||
                                                     n.EndsWith(file, StringComparison.OrdinalIgnoreCase));
            if (candidate != null) return ReadStream(assembly.GetManifestResourceStream(candidate));

            // If path has no extension or uses ./, try replacing separators.
            string alt = path.Replace('/', '.').Replace('\\', '.').TrimStart('.');
            candidate = names.FirstOrDefault(n => n.EndsWith(alt, StringComparison.OrdinalIgnoreCase));
            if (candidate != null) return ReadStream(assembly.GetManifestResourceStream(candidate));
            return null;
        }

        private static string TryReadFile(string path)
        {
            try
            {
                // Absolute or relative to current directory.
                if (File.Exists(path)) return File.ReadAllText(path);

                var baseDir = AppContext.BaseDirectory;
                var combined = Path.Combine(baseDir, path.TrimStart('.', '/', '\\'));
                if (File.Exists(combined)) return File.ReadAllText(combined);

                var cwd = Directory.GetCurrentDirectory();
                combined = Path.Combine(cwd, path.TrimStart('.', '/', '\\'));
                if (File.Exists(combined)) return File.ReadAllText(combined);
            }
            catch { }
            return null;
        }

        private static bool IsComponentType(Type type)
        {
            if (type == null || !type.IsClass || type.IsAbstract) return false;
            var baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                if (baseType.IsGenericType)
                {
                    try
                    {
                        if (baseType.GetGenericTypeDefinition() == typeof(Component<>)) return true;
                    }
                    catch { }
                }
                baseType = baseType.BaseType;
            }
            return false;
        }

        private static bool IsSystemAssembly(Assembly assembly)
        {
            if (assembly == null || assembly.IsDynamic) return true;
            var name = assembly.GetName().Name ?? string.Empty;
            string[] prefixes = { "System", "Microsoft", "mscorlib", "netstandard", "WindowsBase", "PresentationCore", "PresentationFramework" };
            foreach (var p in prefixes)
            {
                if (name.Equals(p, StringComparison.OrdinalIgnoreCase)) return true;
                if (name.StartsWith(p + ".", StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        private static string ReadStream(Stream stream)
        {
            if (stream == null) return null;
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
