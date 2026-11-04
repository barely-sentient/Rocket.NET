using System;
using System.Collections.Generic;

namespace Rocket.Core
{
    /// <summary>
    /// Static CSS utility entry point where stylesheets can be specified.
    /// </summary>
    public static class Css
    {
        public static List<Stylesheet> Stylesheets { get; } = new List<Stylesheet>();
        public static CssEngine Engine { get; set; } = new CssEngine();

        static Css()
        {
            Engine.Stylesheets = Stylesheets;
        }

        public static void AddStylesheet(Stylesheet sheet) => Engine.AddStylesheet(sheet);
        public static void AddStylesheet(string css) => Engine.AddStylesheet(css);
    }
}