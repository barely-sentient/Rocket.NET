
using System;
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
