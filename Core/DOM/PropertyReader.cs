using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Rocket.Core.DOM
{
    /// <summary>
    /// Reads named public members from props objects, caching the reflection lookup per type.
    /// </summary>
    internal static class PropertyReader
    {
        private sealed class Member
        {
            public PropertyInfo Property;
            public FieldInfo Field;
        }

        private static readonly ConcurrentDictionary<(Type, string), Member> _cache =
            new ConcurrentDictionary<(Type, string), Member>();

        public static string GetString(object target, string name)
        {
            if (target == null) return null;
            var member = _cache.GetOrAdd((target.GetType(), name), key => new Member
            {
                Property = key.Item1.GetProperty(key.Item2),
                Field = key.Item1.GetField(key.Item2)
            });

            if (member.Field != null && member.Field.GetValue(target) is string fromField) return fromField;
            return member.Property?.GetValue(target) as string;
        }
    }
}
