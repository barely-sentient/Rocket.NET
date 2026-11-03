using System.Collections;
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

    static HtmlRenderer()
    {
        _typeMap.TryAdd(typeof(HTMLParagraphElement), "p");
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

        builder.Append(indent).Append('<').Append(tag);
        AppendProps(node.UntypedProps);

        if (node.Children.Length == 0)
        {
            builder.Append(" />\n");
            return;
        }

        builder.Append(">\n");

        foreach (var child in node.Children)
            DrawNode(child, depth + 1);

        builder.Append(indent).Append("</").Append(tag).Append(">\n");
    }

    private static string GetTag(Type type)
    {
        return _typeMap.GetValueOrDefault(type, "HTMLUnknownTag");
    }

    private void AppendProps(object? props)
    {
        if (props == null) return;

        var type = props.GetType();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || prop.GetIndexParameters().Length > 0) continue;
            AppendAttribute(prop.Name, prop.GetValue(props));
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
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

        builder.Append(' ')
               .Append(ToKebabCase(name))
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