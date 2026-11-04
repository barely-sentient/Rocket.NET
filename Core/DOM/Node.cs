
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace Rocket.Core.DOM
{
    /// <summary>
    /// Represents a node in the Rocket UI document tree.
    /// </summary>
    /// <remarks>
    /// A node describes a component and its position within the UI hierarchy.
    /// It contains the component's runtime type and an ordered collection of
    /// child nodes.
    ///
    /// Nodes are renderer-independent. They describe what the interface
    /// contains rather than how it should be displayed. A rendering backend
    /// can interpret the tree and translate it into native UI elements,
    /// browser DOM elements, or another platform-specific representation.
    ///
    /// Use <see cref="Node{TProps}"/> when a component also requires properties.
    /// </remarks>
    public class Node
    {
        private sealed class MemberRef
        {
            public PropertyInfo Property;
            public FieldInfo Field;
        }

        private static readonly ConcurrentDictionary<string, MemberRef> _memberRefCache =
            new ConcurrentDictionary<string, MemberRef>();

        /// <summary>
        /// Computed CSS style string for this node at a given viewport.
        /// Passed through to the renderer to apply.
        /// </summary>
        public string ComputedStyle { get; set; }

        /// <summary>
        /// Computed CSS style declarations for this node.
        /// </summary>
        public Dictionary<string, string> ComputedStyles { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Alias for <see cref="ComputedStyle"/>.
        /// </summary>
        public string ComputedCss { get => ComputedStyle; set => ComputedStyle = value; }

        // -----------------------------------------------------------------
        // Layout results computed by the CSS engine.
        // -----------------------------------------------------------------
        private Rocket.Core.LayoutBox _layout;
        public Rocket.Core.LayoutBox Layout { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox LayoutBox { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox LayoutBounds { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox LayoutFrame { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox LayoutRect { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox LayoutRectangle { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox RenderBounds { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox FrameBounds { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox ComputedLayout { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox BoxLayout { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox Box { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox Bounds { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox Rect { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox Rectangle { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox Frame { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox Position { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox LayoutInfo { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox LayoutResult { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox CssLayout { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox BoxModel { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox ComputedBox { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox ComputedBounds { get => _layout; set => _layout = value; }
        public Rocket.Core.LayoutBox ComputedRect { get => _layout; set => _layout = value; }

        /// <summary>
        /// Alias for <see cref="ComputedStyle"/>.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Interaction state used by the CSS engine for pseudo-class matching.
        /// These flags mirror the CSS pseudo-classes :hover, :focus, :active, etc.
        /// </summary>
        public bool Hovered { get; set; }
        public bool Focused { get; set; }
        public bool Active { get; set; }
        public bool Visited { get; set; }
        public bool Checked { get; set; }
        public bool Disabled { get; set; }
        public bool Enabled { get; set; } = true;

        // Alias properties for pseudo-state.
        public bool IsHovered { get => Hovered; set => Hovered = value; }
        public bool IsFocused { get => Focused; set => Focused = value; }
        public bool IsActive { get => Active; set => Active = value; }
        public bool IsVisited { get => Visited; set => Visited = value; }
        public bool IsChecked { get => Checked; set => Checked = value; }
        public bool IsDisabled { get => Disabled; set => Disabled = value; }

        /// <summary>
        /// Optional explicit pseudo-state string (e.g. "hover", "focus").
        /// </summary>
        public string PseudoState { get; set; }

        /// <summary>
        /// Returns true when the given pseudo-class (without leading colon) matches.
        /// </summary>
        public virtual bool MatchesPseudo(string pseudo)
        {
            if (string.IsNullOrWhiteSpace(pseudo)) return true;
            pseudo = pseudo.Trim().TrimStart(':').ToLowerInvariant();
            switch (pseudo)
            {
                case "hover": return Hovered;
                case "focus": return Focused;
                case "active": return Active;
                case "visited": return Visited;
                case "checked": return Checked;
                case "disabled": return Disabled;
                case "enabled": return !Disabled && Enabled;
                default:
                    if (string.Equals(pseudo, PseudoState, StringComparison.OrdinalIgnoreCase)) return true;
                    return false;
            }
        }
        /// <summary>
        /// Hacky, but it works.
        /// </summary>
        public virtual object UntypedProps => null;
        
        /// <summary>
        /// Gets or sets the ordered collection of child nodes.
        /// </summary>
        /// <remarks>
        /// Children represent the nested structure of the interface.
        /// Their ordering is preserved and is significant to the rendering backend.
        /// </remarks>
        public Node[] Children { get; set; }

        /// <summary>
        /// Parent node in the virtual DOM tree, used for event bubbling.
        /// </summary>
        public Node Parent { get; set; }

        // Style computation cache (internal perf aid).
        internal int _styleCacheW;
        internal int _styleCacheH;
        internal Rocket.Core.Orientation _styleCacheO;
        internal double _styleCacheTime;
        internal string _styleCachePseudoKey;
        internal string _styleCacheResult;
        internal bool _styleCacheValid;
        internal int _styleCacheSheetVersion;

        /// <summary>
        /// Gets or sets the component type represented by this node.
        /// </summary>
        /// <remarks>
        /// Typically a component type such as Button, Text, or Panel.
        /// The renderer uses this type to determine how the component should
        /// be instantiated.
        /// </remarks>
        public Type Type { get; set; }

        /// <summary>
        /// Initialises a node with a component type and optional child nodes.
        /// </summary>
        /// <param name="type">The component type represented by this node.</param>
        /// <param name="children">The child nodes belonging to this component.</param>
        public Node(Type type, params Node[] children)
        {
            Type = type;
            Children = children ?? Array.Empty<Node>();
        }

        /// <summary>
        /// Writes a human-readable representation of this node and its
        /// descendants into the supplied string builder.
        /// </summary>
        /// <param name="tabIndent">
        /// The current indentation depth. Each level represents four spaces.
        /// </param>
        /// <param name="builder">
        /// The destination builder receiving the generated representation.
        /// </param>
        /// <remarks>
        /// This method provides a way to inspect the structure of a generated
        /// UI tree during development and debugging.
        ///
        /// Derived node types can override this method to include additional
        /// information, such as component properties.
        /// </remarks>
        /// <summary>
        /// Dispatches an event to this node and bubbles up the tree unless
        /// propagation is stopped.
        /// </summary>
        public void DispatchEvent(string eventName, Rocket.Core.Event e)
        {
            if (e == null || string.IsNullOrWhiteSpace(eventName)) return;
            e.Target = e.Target ?? this;

            string pascal = eventName.Trim();
            if (pascal.Length > 0)
                pascal = char.ToUpperInvariant(pascal[0]) + pascal.Substring(1);
            // common aliases
            if (string.Equals(pascal, "Dblclick", StringComparison.OrdinalIgnoreCase)) pascal = "DblClick";
            if (string.Equals(pascal, "Mouseenter", StringComparison.OrdinalIgnoreCase)) pascal = "MouseEnter";
            if (string.Equals(pascal, "Mouseleave", StringComparison.OrdinalIgnoreCase)) pascal = "MouseLeave";
            if (string.Equals(pascal, "Mousedown", StringComparison.OrdinalIgnoreCase)) pascal = "MouseDown";
            if (string.Equals(pascal, "Mouseup", StringComparison.OrdinalIgnoreCase)) pascal = "MouseUp";
            if (string.Equals(pascal, "Mouseover", StringComparison.OrdinalIgnoreCase)) pascal = "MouseOver";
            if (string.Equals(pascal, "Mouseout", StringComparison.OrdinalIgnoreCase)) pascal = "MouseOut";
            if (string.Equals(pascal, "Mousemove", StringComparison.OrdinalIgnoreCase)) pascal = "MouseMove";
            if (string.Equals(pascal, "Keydown", StringComparison.OrdinalIgnoreCase)) pascal = "KeyDown";
            if (string.Equals(pascal, "Keyup", StringComparison.OrdinalIgnoreCase)) pascal = "KeyUp";
            if (string.Equals(pascal, "Keypress", StringComparison.OrdinalIgnoreCase)) pascal = "KeyPress";
            if (string.Equals(pascal, "Contextmenu", StringComparison.OrdinalIgnoreCase)) pascal = "ContextMenu";

            string propName = "On" + pascal;

            if (!e.IsPropagationStopped && UntypedProps != null)
            {
                var props = UntypedProps;
                var t = props.GetType();
                string cacheKey = t.FullName + "|" + propName;
                var mr = _memberRefCache.GetOrAdd(cacheKey, _ => new MemberRef
                {
                    Property = t.GetProperty(propName),
                    Field = t.GetField(propName)
                });

                object handler = mr.Property != null ? mr.Property.GetValue(props) : null;
                if (handler == null && mr.Field != null) handler = mr.Field.GetValue(props);

                if (handler is Delegate del)
                {
                    try
                    {
                        // Fast path for the common handler signature.
                        if (del is Action<Rocket.Core.Event> action) action(e);
                        else del.DynamicInvoke(e);
                    }
                    catch { }
                }
            }

            if (e.Bubbles && !e.IsPropagationStopped && Parent != null)
            {
                Parent.DispatchEvent(eventName, e);
            }
        }

        public virtual void Dump(int tabIndent, StringBuilder builder)
        {
            builder.Append(new string(' ', tabIndent * 4));
            builder.Append($"{{ \"type\": \"{Type.Name}\", \"children\": [");

            for (var i = 0; i < Children.Length; i++)
            {
                if (i > 0)
                    builder.Append(',');

                builder.AppendLine();
                Children[i].Dump(tabIndent + 1, builder);
            }

            if (Children.Length > 0)
            {
                builder.AppendLine();
                builder.Append(new string(' ', tabIndent * 4));
            }

            builder.Append("]}");
        }
    }

    /// <summary>
    /// Represents a UI node that carries a strongly typed component properties
    /// object.
    /// </summary>
    /// <typeparam name="TProps">
    /// The component properties type. Typically a simple C# class containing
    /// the values required to configure the component.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// Component properties provide the configuration for a UI element.
    /// For example, a button might expose properties for its text, enabled
    /// state, and click callback, while a text component might expose its
    /// content and formatting options.
    /// </para>
    ///
    /// <para>
    /// Properties are represented by ordinary C# types, allowing developers
    /// to use familiar language features such as properties, constructors,
    /// default values, inheritance, and IntelliSense.
    /// </para>
    ///
    /// <para>
    /// The generic type preserves the component's property structure without
    /// requiring the core DOM to understand individual component types.
    /// This keeps the document model extensible and independent of rendering
    /// implementations.
    /// </para>
    ///
    /// <para>
    /// The <see cref="Dump"/> implementation includes the properties in its
    /// output, making it possible to inspect both the structure and
    /// configuration of a generated interface.
    /// </para>
    /// </remarks>
    public class Node<TProps> : Node
        where TProps : new()
    {
        /// <summary>
        /// Gets or sets the strongly typed properties associated with this
        /// component.
        /// </summary>
        /// <remarks>
        /// These values describe how the component should be configured.
        /// Their interpretation is left to the renderer responsible for
        /// creating the actual UI element.
        /// </remarks>
        public TProps Props { get; set; }
        public override object UntypedProps => Props;

        /// <summary>
        /// Initialises a component node with its type, properties, and
        /// optional child nodes.
        /// </summary>
        /// <param name="t">The component type represented by this node.</param>
        /// <param name="props">The strongly typed configuration object.</param>
        /// <param name="children">The child nodes belonging to this component.</param>
        public Node(Type t, TProps props, params Node[] children)
            : base(t, children)
        {
            Props = props;
        }

        /// <summary>
        /// Writes this component, its properties, and its descendants into
        /// the supplied string builder.
        /// </summary>
        /// <param name="tabIndent">
        /// The current indentation depth. Each level represents four spaces.
        /// </param>
        /// <param name="builder">
        /// The destination builder receiving the generated representation.
        /// </param>
        /// <remarks>
        /// Public instance properties are inspected using reflection and
        /// written into the output. This avoids requiring component properties
        /// classes to implement custom diagnostic or serialization logic.
        ///
        /// Child nodes are written recursively, preserving the complete
        /// hierarchical structure.
        /// </remarks>
        public override void Dump(int tabIndent, StringBuilder builder)
        {
            builder.Append(new string(' ', tabIndent * 4));
            builder.Append($"{{ \"type\": \"{Type.Name}\", ");

            builder.Append("\"props\": {");

            var properties = typeof(TProps).GetProperties(
                BindingFlags.Instance | BindingFlags.Public
            );

            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                if (!property.CanRead || property.GetIndexParameters().Length > 0)
                    continue;

                var value = property.GetValue(Props);

                builder.Append($"\"{property.Name}\": ");
                AppendValue(builder, value);

                if (i < properties.Length - 1)
                    builder.Append(", ");
            }

            builder.Append("}, \"children\": [");

            for (var i = 0; i < Children.Length; i++)
            {
                if (i > 0)
                    builder.Append(',');

                builder.AppendLine();
                Children[i].Dump(tabIndent + 1, builder);
            }

            if (Children.Length > 0)
            {
                builder.AppendLine();
                builder.Append(new string(' ', tabIndent * 4));
            }

            builder.Append("]}");
        }

        /// <summary>
        /// Writes an individual property value in a JSON-like format.
        /// </summary>
        /// <param name="builder">The destination string builder.</param>
        /// <param name="value">The value to write.</param>
        private static void AppendValue(StringBuilder builder, object value)
        {
            if (value == null)
            {
                builder.Append("null");
                return;
            }

            if (value is string text)
            {
                builder.Append('"');
                builder.Append(text.Replace("\\", "\\\\").Replace("\"", "\\\""));
                builder.Append('"');
                return;
            }

            if (value is bool boolean)
            {
                builder.Append(boolean ? "true" : "false");
                return;
            }

            if (value is Enum)
            {
                builder.Append('"');
                builder.Append(value);
                builder.Append('"');
                return;
            }

            if (value is IFormattable formattable)
            {
                builder.Append(formattable.ToString(null, CultureInfo.InvariantCulture));
                return;
            }

            builder.Append('"');
            builder.Append(value.ToString().Replace("\\", "\\\\").Replace("\"", "\\\""));
            builder.Append('"');
        }
    }
}
