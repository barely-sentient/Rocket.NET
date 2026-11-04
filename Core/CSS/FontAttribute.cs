using System;

namespace Rocket.Core
{
    /// <summary>
    /// Declares a font that should be loaded and registered under a name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class FontAttribute : Attribute
    {
        public string Path { get; }
        public string Name { get; }

        public FontAttribute(string path, string name)
        {
            Path = path;
            Name = name;
        }
    }
}
