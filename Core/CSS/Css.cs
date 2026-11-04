using System;
using System.Collections.Generic;

namespace Rocket.Core
{
    /// <summary>
    /// Static CSS utility entry point where stylesheets can be specified.
    /// </summary>
    public static partial class Css
    {
        public static List<Stylesheet> Stylesheets { get; } = new List<Stylesheet>();
        public static CssEngine Engine { get; set; } = new CssEngine();

        static Css()
        {
            Engine.Stylesheets = Stylesheets;
            try
            {
                StylesheetLoader.AutoImport(Engine);
                AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
                {
                    try { StylesheetLoader.AutoImport(args.LoadedAssembly, Engine); }
                    catch { }
                };
            }
            catch { }
        }

        public static void AddStylesheet(Stylesheet sheet) => Engine.AddStylesheet(sheet);
        public static void AddStylesheet(string css) => Engine.AddStylesheet(css);
    }
}