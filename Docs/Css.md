# CSS & Styling

## Stylesheets

```csharp
var sheet = new Stylesheet();
sheet.AddRule("button", "color: black; padding: 8px;");
sheet.AddRule("button:hover", "color: red;");
UI.Stylesheets.Add(sheet);

// or from raw CSS
UI.Stylesheets.Add(Stylesheet.FromCss("div { display: flex; }"));
```

## Compute styles

```csharp
var engine = new CssEngine();
engine.AddStylesheet(sheet);

var node = Button(new HTMLButtonElementProps());
engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);

Console.WriteLine(node.ComputedStyle);        // "color: black; padding: 8px;"
Console.WriteLine(node.ComputedStyles["color"]); // "black"
```

## Pseudo-states

```csharp
node.Hovered = true;
engine.ApplyComputedStyles(node, 800, 600, Orientation.Portrait);
// button:hover rule now applies
```

Or use helpers:

```csharp
engine.Hover(node);
engine.Focus(node);
engine.SetPseudoState(node, "hover");
```

## Media queries

```csharp
sheet.AddRule("@media (max-width: 600px)", "div { flex-direction: column; }");
```

`MatchesMedia` handles `min-width`, `max-width`, `orientation`, etc.

## Transitions

```csharp
var css = "button { color: black; transition: color 1s linear; } button:hover { color: white; }";
var engine = new CssEngine();
engine.AddStylesheet(css);

var node = Button(new HTMLButtonElementProps());
engine.ComputeStyle(node, 0);          // base

node.Hovered = true;
var atStart = engine.ComputeStyles(node, 0);   // from value
var atHalf  = engine.ComputeStyles(node, 0.5); // interpolated

## Pointing to a stylesheet with an attribute

You can declare stylesheets declaratively on a component or assembly using
`StylesheetAttribute` (aliases: `StyleSheetAttribute`, `CSSStylesheetAttribute`).

```csharp
using Rocket.Core;

[Stylesheet("Styles/app.css")]
[Stylesheet("Styles/theme.css", Name = "theme")]
public class MyApp : Component<MyAppProps>
{
    public override Node Render() => ...;
}
```

The loader resolves the path at runtime, trying in order:
1. Embedded manifest resources (recommended for cross-platform)
2. Application base directory
3. Current working directory
4. Unity `StreamingAssets` (when present)

### Embedding (recommended)

Set the CSS file's Build Action to **Embedded Resource**. Then the loader
finds it by manifest resource name.

### Loading manually

```csharp
// Load one stylesheet
var sheet = StylesheetLoader.Load("Styles/app.css", typeof(MyApp).Assembly);
if (sheet != null) UI.Stylesheets.Add(sheet);

// Load all attributes on a type
StylesheetLoader.LoadFor(typeof(MyApp));

// Auto-import for an assembly (scans component types)
StylesheetLoader.AutoImport(typeof(MyApp).Assembly);

// Auto-import across all loaded assemblies
StylesheetLoader.AutoImport();
```

### Tips

- Prefer embedded resources for reliable cross-platform behaviour.
- `Embedded = false` disables the manifest-resource lookup.
- Attributes are inherited, so a base component can declare shared styles.
- Multiple attributes are allowed (`AllowMultiple = true`).

