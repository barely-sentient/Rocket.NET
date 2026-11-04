# Rocket.NET Documentation

Rocket.NET is a declarative, React-style UI framework for .NET. You describe
your interface as a tree of components using plain C#, reconcile it against the
previous tree, apply CSS, compute layout, and render it through a pluggable
backend.

## Contents

- [Getting Started](GettingStarted.md)
- [Components](Components.md)
- [Hooks](Hooks.md)
- [CSS & Styling](Css.md)
- [Layout](Layout.md)
- [Rendering Backends](Rendering.md)
- [Performance](Performance.md)

## Core concepts

- **Nodes** (`Rocket.Core.DOM.Node`) describe what to render. They are
  renderer-independent.
- **Components** (`Rocket.Core.Component<TProps>`) are composite nodes that
  implement `Render()` and may use hooks.
- **Hooks** (`Rocket.Core.Hooks`) provide state, effects, memoization, refs,
  context, reducers, and more.
- **Virtual DOM executor** (`Rocket.Core.VirtualDomExecutor`) mounts, reconciles,
  and drives hook state.
- **CSS engine** (`Rocket.Core.CssEngine`) resolves styles and layout.

## Minimal example

```csharp
using Rocket.Core;
using Rocket.Core.DOM;
using static Rocket.Core.UI;

// Build a tree
Node app = C<Panel>(
    new PanelProps { Title = "Hello" },
    C<Text>(new TextProps { Content = "Welcome to Rocket.NET" }),
    C<Button>(new ButtonProps { Text = "Click me", OnClick = e => { } })
);

// Render
var renderer = new MyRenderer();
var executor = new VirtualDomExecutor(renderer);
executor.Render(app, 1024, 768, Orientation.Landscape);
```
