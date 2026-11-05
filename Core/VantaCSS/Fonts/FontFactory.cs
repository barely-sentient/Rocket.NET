using System;
using System.Collections.Generic;

namespace VantaCSS
{
    /// <summary>
    /// Factory/registry for fonts used by the CSS engine and renderers.
    /// Fonts can be registered from raw TTF/OTF data and looked up by family name.
    /// </summary>
    public class FontFactory
    {
        public static FontFactory Instance { get; } = new FontFactory();

        private readonly Dictionary<string, byte[]> _fonts =
            new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, byte[]> Fonts => _fonts;

        public void RegisterFont(string family, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(family) || data == null || data.Length == 0) return;
            _fonts[family.Trim()] = data;
        }

        public void RegisterFont(string family, string path)
        {
            if (string.IsNullOrWhiteSpace(family) || string.IsNullOrWhiteSpace(path)) return;
            if (!System.IO.File.Exists(path)) return;
            try { RegisterFont(family, System.IO.File.ReadAllBytes(path)); }
            catch { }
        }

        public void RegisterFontAlias(string aliasFamily, string existingFamily)
        {
            if (string.IsNullOrWhiteSpace(aliasFamily) || string.IsNullOrWhiteSpace(existingFamily)) return;
            if (TryGetFont(existingFamily, out var data)) RegisterFont(aliasFamily, data);
        }

        public bool TryGetFont(string family, out byte[] data)
        {
            if (!string.IsNullOrWhiteSpace(family) && _fonts.TryGetValue(family.Trim(), out data))
                return true;
            data = null;
            return false;
        }

        public void Clear() => _fonts.Clear();
    }
}