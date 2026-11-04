using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Text;
using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Tests;

public class HtmlRenderer : IComponentRenderer
{
    private readonly StringBuilder builder = new();

    private static Dictionary<Type, string> _typeMap = new();

    private sealed class MemberCache
    {
        public PropertyInfo[] Properties = Array.Empty<PropertyInfo>();
        public FieldInfo[] Fields = Array.Empty<FieldInfo>();
    }

    private static readonly ConcurrentDictionary<Type, MemberCache> _memberCache = new ConcurrentDictionary<Type, MemberCache>();

    static HtmlRenderer()
    {
        foreach (var kv in UI.Primitives)
            _typeMap[kv.Value] = kv.Key;
    }

    public void Draw(Node rootNode)
    {
        DrawNode(rootNode, 0);
    }

    public string GetHtml() => builder.ToString();

    private void DrawNode(Node node, int depth)
    {
        var indent = new string(' ', depth * 2);
        var tag = GetTag(node.Type);

        string? textContent = null;
        if (node.UntypedProps != null)
        {
            var t = node.UntypedProps.GetType();
            var f = t.GetField("TextContent");
            var p = t.GetProperty("TextContent");
            textContent = f?.GetValue(node.UntypedProps) as string ?? p?.GetValue(node.UntypedProps) as string;
        }

        builder.Append(indent).Append('<').Append(tag);
        AppendProps(node.UntypedProps, "TextContent");

        if (!string.IsNullOrEmpty(node.ComputedStyle))
        {
            builder.Append(" style=\"")
                   .Append(WebUtility.HtmlEncode(node.ComputedStyle))
                   .Append('"');
        }

        bool hasChildren = node.Children.Length > 0 || !string.IsNullOrEmpty(textContent);
        if (!hasChildren)
        {
            builder.Append(" />\n");
            return;
        }

        builder.Append(">\n");
        if (!string.IsNullOrEmpty(textContent))
            builder.Append(indent).Append("  ").Append(WebUtility.HtmlEncode(textContent)).Append('\n');
        foreach (var child in node.Children)
            DrawNode(child, depth + 1);

        builder.Append(indent).Append("</").Append(tag).Append(">\n");
    }

    private static string GetTag(Type type)
    {
        return _typeMap.GetValueOrDefault(type, "HTMLUnknownTag");
    }

    private void AppendProps(object? props, string? skip = null)
    {
        if (props == null) return;

        var type = props.GetType();
        var cache = _memberCache.GetOrAdd(type, t =>
        {
            var c = new MemberCache();
            var propsArr = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<PropertyInfo>();
            foreach (var p in propsArr)
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                list.Add(p);
            }
            c.Properties = list.ToArray();
            c.Fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            return c;
        });

        foreach (var prop in cache.Properties)
        {
            if (prop.Name == skip) continue;
            AppendAttribute(prop.Name, prop.GetValue(props));
        }

        foreach (var field in cache.Fields)
        {
            if (field.Name == skip) continue;
            AppendAttribute(field.Name, field.GetValue(props));
        }
    }

    private void AppendAttribute(string name, object? value)
    {
        // Skip nulls and callbacks; they have no sensible HTML representation.
        if (value == null || value is Delegate) return;

        // Collections and nested objects aren't useful as attribute text.
        if (value is IEnumerable && value is not string) return;

        string text = value switch
        {
            bool b => b ? "true" : "false",
            IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };

        string attrName = name == "ClassName" ? "class" : ToKebabCase(name);

        builder.Append(' ')
               .Append(attrName)
               .Append("=\"")
               .Append(WebUtility.HtmlEncode(text))
               .Append('"');
    }

    private static string ToKebabCase(string name)
    {
        var sb = new StringBuilder(name.Length + 4);
        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0) sb.Append('-');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}