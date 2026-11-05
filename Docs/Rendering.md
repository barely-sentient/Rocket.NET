# Rendering

[Home](README.md) › Rendering

Rocket.NET separates describing a UI from drawing it. [Components](Components.md) produce a virtual DOM tree. A renderer turns that tree into pixels or markup.

```
Component tree ──▶ VirtualDomExecutor ──▶ VantaCSS ──▶ IComponentRenderer.Draw(root)
```

**On this page**
[IComponentRenderer](#icomponentrenderer) · [VirtualDomExecutor](#virtualdomexecutor) · [Mounting vs. updating](#mounting-vs-updating) · [Stylesheets](#stylesheets) · [Supplying a CSS engine](#supplying-a-css-engine) · [HtmlRenderer](#htmlrenderer-test-helper) · [Reconciliation notes](#reconciliation-notes)

## IComponentRenderer

Implement this interface for your target platform:

```csharp
using Rocket.Core;
using Rocket.Core.DOM;

public class MyRenderer : IComponentRenderer
{
    public void Draw(Node rootNode)
    {
        // Walk the node tree and draw or emit each node.
    }
}
```

`Draw` receives the resolved root node after reconciliation and CSS and layout processing. Each node carries:

| Property | Description |
| --- | --- |
| `ComputedStyle` | Merged CSS declarations as a string. |
| `ComputedStyles` | The same declarations as a dictionary. |
| `Layout` | The computed layout box. |

See [Styling](Styling.md#computed-styles-on-nodes) for how these are produced.

Typical targets and what `Draw` does for each:

| Target | Approach |
| --- | --- |
| HTML | Emit markup for each node. |
| Desktop canvas | Draw resolved nodes to a 2D surface. |
| Game engine | Map nodes to the engine's UI primitives. |

## VirtualDomExecutor

The executor owns the instance tree, runs hooks, reconciles changes, and applies CSS and layout:

```csharp
using Rocket.Core;

IComponentRenderer renderer = new MyRenderer();
var executor = new VirtualDomExecutor(renderer);

Node root = new MyApp().Render();
executor.Render(root);
```

Overloads accept a viewport, which drives [media queries](Styling.md#media-queries):

```csharp
executor.Render(root, 800, 600, VantaCSS.Orientation.Portrait);
executor.Render(root, viewport);
```

### Mounting vs. updating

The first `Render` call mounts the tree. Subsequent calls with a compatible root (same component type) reconcile against the previous tree, preserving hook state and DOM identity where possible.

## Stylesheets

Add stylesheets to the executor before, or between, renders:

```csharp
executor.Stylesheets.Add(new VantaCSS.Stylesheet("app", css));
```

The executor's `CssEngine` applies computed styles and layout during `Render`.

## Supplying a CSS engine

You can pass your own engine:

```csharp
var engine = new VantaCSS.VantaCssEngine();
var executor = new VirtualDomExecutor(renderer, engine);
```

The executor syncs its `Stylesheets` list to the engine during rendering.

## HtmlRenderer (test helper)

The repository includes an `HtmlRenderer` in the test project. It implements `IComponentRenderer` and emits HTML, which makes it a useful reference implementation when writing your own renderer.

## Reconciliation notes

- Composite components (classes deriving from `Component<T>`) are expanded to their rendered host node.
- Primitive (host) nodes build an instance tree of their children.
- Hook state lives on the `Instance` and only the cursor is reset at the start of each render.
- On unmount, side-effect cleanup delegates are invoked. See [`Effect`](Hooks.md#effect--sideeffect).

For the full model, see [Architecture](Architecture.md#reconciliation).

---

← [Styling](Styling.md) · Next: [Architecture →](Architecture.md)