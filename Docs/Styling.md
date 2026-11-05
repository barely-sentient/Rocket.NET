# Styling (VantaCSS)

[Home](README.md) › Styling

Rocket.NET uses the VantaCSS engine to resolve styles and layout for a viewport before anything is drawn. Because styles are computed onto the nodes themselves, they travel with the tree and look the same on every [renderer](Rendering.md).

**On this page**
[Adding stylesheets](#adding-stylesheets) · [Selectors](#selectors) · [Media queries](#media-queries) · [Transitions](#transitions) · [Computed styles](#computed-styles-on-nodes) · [Layout](#layout) · [Limitations](#limitations)

## Adding stylesheets

Create a `Stylesheet` with a name and CSS text:

```csharp
using VantaCSS;

var sheet = new Stylesheet("app", @"
    .card { display: flex; padding: 16px; }
    .card:hover { background: #333; }
");
```

With a `VirtualDomExecutor`, add it to `executor.Stylesheets`. The executor syncs the list to its engine during rendering:

```csharp
executor.Stylesheets.Add(new Stylesheet("app", css));
```

When you work with an engine directly:

```csharp
var engine = new VantaCSS.VantaCssEngine();
engine.AddStylesheet(sheet);
```

You can also load stylesheets with the `[Stylesheet("path")]` attribute on component classes or assemblies. `StylesheetLoader` resolves embedded resources or files and registers them with `Css.Engine`.

> See [Rendering](Rendering.md#supplying-a-css-engine) for supplying your own engine to the executor.

## Selectors

Selectors match by tag name, id, and class name. Ids and class names come from a node's props (`Id`, `ClassName`), the same props you set when [building elements](Components.md#intrinsic-elements-html-tags).

```csharp
Div(new() { ClassName = "card" }, /* ... */)
```

```css
.card { padding: 16px; }
```

Pseudo-classes `:hover`, `:focus`, `:active`, `:checked`, and `:disabled` are supported when the corresponding flags are set on the node.

## Media queries

`@media` blocks are supported and are filtered by viewport width, height, and orientation:

```css
@media (min-width: 500px) {
    .card { width: 300px; }
}
```

Viewport dimensions come from the executor's `Render` overloads. See [Rendering](Rendering.md#virtualdomexecutor).

## Transitions

Time-based transitions are supported. Declare them with the `transition` property, and the engine interpolates between values as time advances:

```css
.card {
    transition: transform 150ms ease, box-shadow 150ms ease;
}
```

Pass time to the engine through the viewport-aware `Render` overloads, or by setting `CurrentTime`.

> Not to be confused with the [`Transition` hook](Hooks.md#transition), which is unrelated to CSS.

## Computed styles on nodes

After `Apply` / `CalculateLayout`, each node carries:

| Property | Contents |
| --- | --- |
| `ComputedStyle` | The merged CSS declarations as a string. |
| `ComputedStyles` | The same declarations as a dictionary. |
| `Layout` | The computed layout box. |

Renderers read these to draw. See [Rendering](Rendering.md#icomponentrenderer).

## Layout

The engine performs a flexbox and block layout pass. Supported properties include:

- Display and flow: `display`, `flex-direction`, `justify-content`, `gap`
- Sizing: `width`, `height`, `min/max-width`, `min/max-height`
- Box model: `margin`, `padding`, `border`
- Positioning: `position`, `left`, `top`, `right`, `bottom`

Related box-model properties are also supported.

## Limitations

- Keyframe (`@keyframes`) animations are not supported. Use [transitions](#transitions) instead.
- Not every CSS property affects layout. Unknown properties are still passed through in `ComputedStyle` so your renderer can use them.
- Stylesheet lists are not automatically shared between `UI.Stylesheets`, `Css.Stylesheets`, and an executor's `Stylesheets`. Wire them explicitly.

---

← [Events](Events.md) · Next: [Rendering →](Rendering.md)