
using System;
using System.Collections.Generic;
using Rocket.Core.Components;
using Rocket.Core.DOM;

namespace Rocket.Core
{
    /// <summary>
    /// Provides the primary API for declaratively constructing user
    /// interfaces using composable component definitions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="UI"/> class provides a lightweight, developer-friendly
    /// approach to describing user interfaces directly in C#.
    /// Components are constructed through generic method calls, allowing
    /// developers to express interface hierarchies using familiar language
    /// features rather than requiring a separate markup language, visual
    /// editor, or imperative element construction API.
    /// </para>
    ///
    /// <para>
    /// The primary construction method is <see cref="C{T}(object, Node[])"/>.
    /// It accepts a component type, a properties object, and an optional
    /// collection of child nodes. This creates a composable description
    /// of the desired interface that can be processed by the framework's
    /// DOM and rendering infrastructure.
    /// </para>
    ///
    /// <para>
    /// Developers are encouraged to import this class using the C#
    /// <c>using static Rocket.Core.UI;</c> directive. This exposes the
    /// component construction methods directly within the current source
    /// file, allowing <c>C&lt;T&gt;(...)</c> to be used without repeatedly
    /// qualifying calls with <c>UI.</c>.
    /// </para>
    ///
    /// <para>
    /// This approach makes component trees concise and naturally readable.
    /// Each component can contain other components as children, allowing
    /// interfaces to be composed incrementally from small, reusable
    /// building blocks.
    /// </para>
    ///
    /// <para>
    /// Component declarations are independent of the rendering backend.
    /// Developers describe the structure and configuration of an interface,
    /// while the framework is responsible for processing that description
    /// and coordinating the corresponding rendering operations.
    /// </para>
    ///
    /// <para>
    /// The result is a declarative UI development experience entirely within
    /// C#, benefiting from standard language tooling such as IntelliSense,
    /// type navigation, refactoring, debugging, and compile-time syntax
    /// validation.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// <b>Recommended usage: importing the component construction API.</b>
    /// </para>
    /// <code>
    /// using Rocket.Core;
    /// using static Rocket.Core.UI;
    ///
    /// Node button = C&lt;Button&gt;(
    ///     new ButtonProps
    ///     {
    ///         Text = "Hello World"
    ///     }
    /// );
    /// </code>
    ///
    /// By importing <see cref="UI"/> statically, component declarations
    /// can be written directly using <c>C&lt;T&gt;()</c>.
    /// </example>
    /// <example>
    /// <para>
    /// <b>Composing an interface using nested components.</b>
    /// </para>
    /// <code>
    /// using static Rocket.Core.UI;
    ///
    /// Node application = C&lt;Panel&gt;(
    ///     new PanelProps
    ///     {
    ///         Title = "Dashboard"
    ///     },
    ///
    ///     C&lt;Text&gt;(
    ///         new TextProps
    ///         {
    ///             Content = "Welcome to the application."
    ///         }
    ///     ),
    ///
    ///     C&lt;Button&gt;(
    ///         new ButtonProps
    ///         {
    ///             Text = "Continue"
    ///         }
    ///     )
    /// );
    /// </code>
    ///
    /// Each nested <c>C&lt;T&gt;()</c> call creates another component
    /// description, allowing the entire interface to be expressed as
    /// a single, readable composition.
    /// </example>
    /// <example>
    /// <para>
    /// <b>Organising reusable component descriptions.</b>
    /// </para>
    /// <code>
    /// using static Rocket.Core.UI;
    ///
    /// public static Node CreateToolbar()
    /// {
    ///     return C&lt;Toolbar&gt;(new ToolbarProps(),
    ///         C&lt;Button&gt;(
    ///             new ButtonProps { Text = "Save" }
    ///         ),
    ///
    ///         C&lt;Button&gt;(
    ///             new ButtonProps { Text = "Refresh" }
    ///         )
    ///     );
    /// }
    ///
    /// public static Node CreateApplication()
    /// {
    ///     return C&lt;Panel&gt;(new PanelProps(),
    ///         CreateToolbar()
    ///     );
    /// }
    /// </code>
    ///
    /// Component descriptions can be extracted into ordinary C# methods,
    /// making it straightforward to organise larger interfaces into
    /// reusable sections and independently maintained components.
    /// </example>
    public static class UI
    {
        /// <summary>
        /// Holds all the supported HTML elements
        /// </summary>
        private static Dictionary<string, Type> _primitives = new Dictionary<string, Type>();
        
        /// <summary>
        /// Sets up the primitive dictionary
        /// </summary>
        static UI()
        {
            RegisterPrimitive("p", typeof(HTMLParagraphElement));
            RegisterPrimitive("a", typeof(HTMLAnchorElement));
            RegisterPrimitive("article", typeof(HTMLArticleElement));
            RegisterPrimitive("audio", typeof(HTMLAudioElement));
            RegisterPrimitive("button", typeof(HTMLButtonElement));
            RegisterPrimitive("div", typeof(HTMLDivElement));
            RegisterPrimitive("footer", typeof(HTMLFooterElement));
            RegisterPrimitive("form", typeof(HTMLFormElement));
            RegisterPrimitive("header", typeof(HTMLHeaderElement));
            RegisterPrimitive("h1", typeof(HTMLHeadingElement));
            RegisterPrimitive("h2", typeof(HTMLHeadingElement));
            RegisterPrimitive("h3", typeof(HTMLHeadingElement));
            RegisterPrimitive("h4", typeof(HTMLHeadingElement));
            RegisterPrimitive("h5", typeof(HTMLHeadingElement));
            RegisterPrimitive("h6", typeof(HTMLHeadingElement));
            RegisterPrimitive("img", typeof(HTMLImageElement));
            RegisterPrimitive("input", typeof(HTMLInputElement));
            RegisterPrimitive("label", typeof(HTMLLabelElement));
            RegisterPrimitive("li", typeof(HTMLListItemElement));
            RegisterPrimitive("main", typeof(HTMLMainElement));
            RegisterPrimitive("nav", typeof(HTMLNavElement));
            RegisterPrimitive("ol", typeof(HTMLOListElement));
            RegisterPrimitive("option", typeof(HTMLOptionElement));
            RegisterPrimitive("section", typeof(HTMLSectionElement));
            RegisterPrimitive("select", typeof(HTMLSelectElement));
            RegisterPrimitive("span", typeof(HTMLSpanElement));
            RegisterPrimitive("td", typeof(HTMLTableCellElement));
            RegisterPrimitive("table", typeof(HTMLTableElement));
            RegisterPrimitive("th", typeof(HTMLTableHeaderCellElement));
            RegisterPrimitive("tr", typeof(HTMLTableRowElement));
            RegisterPrimitive("textarea", typeof(HTMLTextAreaElement));
            RegisterPrimitive("ul", typeof(HTMLUListElement));
            RegisterPrimitive("video", typeof(HTMLVideoElement));
            RegisterPrimitive("br", typeof(HTMLBrElement));
            RegisterPrimitive("hr", typeof(HTMLHrElement));
            RegisterPrimitive("strong", typeof(HTMLStrongElement));
            RegisterPrimitive("em", typeof(HTMLEmElement));
            RegisterPrimitive("code", typeof(HTMLCodeElement));
            RegisterPrimitive("pre", typeof(HTMLPreElement));
            RegisterPrimitive("blockquote", typeof(HTMLBlockquoteElement));
            RegisterPrimitive("aside", typeof(HTMLAsideElement));
            RegisterPrimitive("canvas", typeof(HTMLCanvasElement));
            RegisterPrimitive("iframe", typeof(HTMLIframeElement));
            RegisterPrimitive("small", typeof(HTMLSmallElement));
            RegisterPrimitive("b", typeof(HTMLBoldElement));
            RegisterPrimitive("i", typeof(HTMLItalicElement));
            RegisterPrimitive("u", typeof(HTMLUnderlineElement));
            RegisterPrimitive("mark", typeof(HTMLMarkElement));
            RegisterPrimitive("del", typeof(HTMLDelElement));
            RegisterPrimitive("ins", typeof(HTMLInsElement));
            RegisterPrimitive("sub", typeof(HTMLSubElement));
            RegisterPrimitive("sup", typeof(HTMLSupElement));
            RegisterPrimitive("time", typeof(HTMLTimeElement));
            RegisterPrimitive("details", typeof(HTMLDetailsElement));
            RegisterPrimitive("summary", typeof(HTMLSummaryElement));
            RegisterPrimitive("progress", typeof(HTMLProgressElement));
            RegisterPrimitive("fieldset", typeof(HTMLFieldsetElement));
            RegisterPrimitive("legend", typeof(HTMLLegendElement));
        }

        private static void RegisterPrimitive(string tagName, Type type)
        {
            _primitives.Add(tagName, type);
        }

        /// <summary>
        /// Creates a declarative UI node representing the specified
        /// component type, its configuration, and its child components.
        /// </summary>
        /// <typeparam name="T">
        /// The component type to instantiate within the UI description.
        /// This identifies the component implementation responsible for
        /// interpreting the supplied properties during rendering.
        /// </typeparam>
        /// <typeparam name="TProps"></typeparam>
        /// <param name="props">
        /// The properties object containing the configuration for the
        /// component. Typically, this is an instance of the component's
        /// dedicated properties type, providing a structured and
        /// discoverable way to configure its behaviour and presentation.
        /// </param>
        /// <param name="children">
        /// An optional collection of child nodes that define the nested
        /// content of the component. Additional components can be supplied
        /// directly through nested <c>C&lt;T&gt;()</c> calls, allowing
        /// complete interface hierarchies to be constructed naturally.
        /// </param>
        /// <returns>
        /// A <see cref="Node{TProps}"/> representing the requested
        /// component, including its component type, configuration
        /// properties, and child node hierarchy.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <see cref="C{T}(object, Node[])"/> is the fundamental building
        /// block of the Rocket UI component construction API.
        /// </para>
        /// 
        /// <para>
        /// Component construction follows a simple and consistent pattern:
        /// specify the component type, provide its properties, and
        /// optionally supply child components.
        /// </para>
        /// 
        /// <code>
        /// C&lt;ComponentType&gt;(
        ///     new ComponentProps
        ///     {
        ///         // Component configuration.
        ///     },
        /// 
        ///     // Optional child components.
        /// );
        /// </code>
        /// 
        /// <para>
        /// The generic type parameter makes component selection explicit
        /// at the call site. The properties object keeps configuration
        /// separate from the component's structural declaration, while
        /// the child arguments provide a natural mechanism for composition.
        /// </para>
        /// 
        /// <para>
        /// The method returns a DOM node description rather than directly
        /// constructing or manipulating a backend-specific visual element.
        /// This allows developers to focus entirely on describing their
        /// intended interface without managing rendering details.
        /// </para>
        /// 
        /// <para>
        /// For the cleanest syntax, import <see cref="UI"/> using
        /// <c>using static Rocket.Core.UI;</c>. This makes the method
        /// available directly as <c>C&lt;T&gt;()</c>, removing the need
        /// for the <c>UI.</c> qualifier throughout component declarations.
        /// </para>
        /// 
        /// <para>
        /// Because the method accepts child nodes through the
        /// <c>params</c> parameter, developers can compose arbitrarily
        /// nested interface descriptions using ordinary C# expressions.
        /// Individual sections can also be extracted into reusable
        /// methods, keeping larger component trees organised and readable.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>Creating a simple component.</b>
        /// </para>
        /// <code>
        /// using static Rocket.Core.UI;
        /// 
        /// Node button = C&lt;Button&gt;(
        ///     new ButtonProps
        ///     {
        ///         Text = "Hello World"
        ///     }
        /// );
        /// </code>
        /// </example>
        /// <example>
        /// <para>
        /// <b>Creating a component with nested children.</b>
        /// </para>
        /// <code>
        /// using static Rocket.Core.UI;
        /// 
        /// Node panel = C&lt;Panel&gt;(
        ///     new PanelProps
        ///     {
        ///         Title = "My Application"
        ///     },
        /// 
        ///     C&lt;Text&gt;(
        ///         new TextProps
        ///         {
        ///             Content = "Application settings"
        ///         }
        ///     ),
        /// 
        ///     C&lt;Button&gt;(
        ///         new ButtonProps
        ///         {
        ///             Text = "Save Changes"
        ///         }
        ///     )
        /// );
        /// </code>
        /// </example>
        /// <example>
        /// <para>
        /// <b>Building reusable interface sections.</b>
        /// </para>
        /// <code>
        /// using static Rocket.Core.UI;
        /// 
        /// public static Node CreateHeader()
        /// {
        ///     return C&lt;Header&gt;(new HeaderProps(),
        ///         C&lt;Text&gt;(
        ///             new TextProps
        ///             {
        ///                 Content = "My Application"
        ///             }
        ///         )
        ///     );
        /// }
        /// 
        /// public static Node CreatePage()
        /// {
        ///     return C&lt;Page&gt;(new PageProps(),
        ///         CreateHeader(),
        ///         C&lt;Button&gt;(
        ///             new ButtonProps
        ///             {
        ///                 Text = "Get Started"
        ///             }
        ///         )
        ///     );
        /// }
        /// </code>
        /// </example>
        public static Node C<T,TProps>(TProps props, params Node[] children) where TProps : new()
        {
            return new Node<TProps>(typeof(T), props, children);
        }
        /// <summary>
        /// Use a HTML element
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="props"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static Node H(string tagName, object props, params Node[] children)
        {
            if (!_primitives.TryGetValue(tagName, out var type))
            {
                 throw new InvalidOperationException("Unknown tag: " + tagName);  
            }
            
            return new Node<object>(type, props, children);
        }
    }
}
