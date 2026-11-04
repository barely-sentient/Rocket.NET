using System;

namespace Rocket.Core
{
    /// <summary>
    /// Shared helpers for the hook implementations.
    /// </summary>
    public static partial class Hooks
    {
        /// <summary>
        /// Compares two dependency arrays for equality, element by element.
        /// </summary>
        private static bool DependenciesEqual(object[] a, object[] b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (!Equals(a[i], b[i])) return false;
            }
            return true;
        }
    }
}