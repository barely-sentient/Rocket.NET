# Rocket.NET

**Build UIs in C# the way you build them for the web.** Rocket.NET combines React-style components, a virtual DOM, and CSS styling with a pluggable rendering backend, so one component tree can target HTML, a desktop canvas, a game engine, or any platform you choose.

[Getting started](GettingStarted.md) · [Components](Components.md) · [Hooks](Hooks.md) · [Context](Context.md) · [Events](Events.md) · [Styling](Styling.md) · [Rendering](Rendering.md) · [Architecture](Architecture.md)

---

## Overview

Rocket.NET is built around a single separation: **components describe what to show; renderers decide how to draw it.**

- Components are plain C# classes that return a tree of nodes built from the HTML elements you already know.
- Styling is ordinary CSS, resolved (including layout) by the built-in VantaCSS engine.
- Drawing is delegated to an `IComponentRenderer` that you implement for your target.

Because components never touch a concrete backend, changing platforms means changing the renderer. Components, state, effects, and styles stay exactly the same.

## Key capabilities

| Capability | What you get |
| --- | --- |
| **Familiar vocabulary** | Every HTML element (`Div`, `Span`, `Button`, `H1`…`H6`, `Input`, `Img`, and more) is available as a C# helper. Skills transfer in both directions. |
| **Component model** | Reusable classes with typed props, children, and hook-based state. See [Components](Components.md). |
| **Hooks** | `State`, `Effect`, `Reducer`, `Memo`, `Callback`, `Ref`, `Context`, `Transition`, and more. See [Hooks](Hooks.md). |
| **Context** | Provide values to a subtree without prop drilling. See [Context](Context.md). |
| **Events** | Typed event handlers with bubbling and propagation control. See [Events](Events.md). |
| **CSS** | Selectors, media queries, pseudo-classes, time-based transitions, and a flexbox/block layout pass. See [Styling](Styling.md). |
| **Efficient updates** | A virtual DOM reconciles changes and preserves component and hook state across re-renders. See [Architecture](Architecture.md). |
| **Renderer-agnostic** | Implement one interface, `IComponentRenderer`, to target any platform. See [Rendering](Rendering.md). |
| **Plain C#** | No markup language and no visual editor. You keep IntelliSense, refactoring, and compile-time checking. |

## How it fits together

Every render follows the same pipeline. Each stage has a dedicated guide.

```
  Components          Virtual DOM            Executor              Renderer
 ┌───────────┐      ┌─────────────┐      ┌──────────────┐      ┌──────────────┐
 │ Render()  │ ───▶ │  Node tree  │ ───▶ │ Reconcile    │ ───▶ │ Draw(root)   │
 │ + hooks   │      │ (props,     │      │ Apply CSS    │      │ IComponent-  │
 │           │      │  children)  │      │ Compute      │      │ Renderer     │
 └───────────┘      └─────────────┘      │ layout       │      └──────────────┘
   Components.md       Architecture.md   └──────────────┘         Rendering.md
   Hooks.md                              Styling.md
```

1. **Components** return `Node` trees. [Components](Components.md), [Hooks](Hooks.md), [Context](Context.md), and [Events](Events.md) cover authoring.
2. **`VirtualDomExecutor`** reconciles the tree against the previous one and runs hooks.
3. **VantaCSS** matches stylesheet rules and computes styles and layout onto each node. See [Styling](Styling.md).
4. **`IComponentRenderer.Draw(root)`** receives the fully resolved tree. See [Rendering](Rendering.md).

## Quick look

A component, styled and rendered end to end:

```csharp
using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using static Rocket.Core.UI;

public class Counter : Component<CounterProps>
{
    public override Node Render()
    {
        var (count, setCount) = State(0);

        return Div(new() { ClassName = "counter" },
            H1(new() { TextContent = $"Count: {count}" }),
            Button(new()
            {
                TextContent = "Increment",
                OnClick = _ => setCount(count + 1)
            })
        );
    }
}

public class CounterProps { }
```

```csharp
IComponentRenderer renderer = new YourRenderer();   // your target platform
var executor = new VirtualDomExecutor(renderer);

executor.Stylesheets.Add(new VantaCSS.Stylesheet("app", @"
    .counter { display: flex; flex-direction: column; gap: 8px; }
    .counter h1 { font-size: 24px; }
"));

Node root = new Counter().Render();
executor.Render(root);
```

The [Getting started](GettingStarted.md) guide builds this up step by step.

## Core concepts

These terms are used consistently across all documentation.

| Term | Meaning |
| --- | --- |
| **Component** | A class deriving from `Component<TProps>` whose `Render()` returns a `Node`. |
| **Node** | A renderer-independent description of one element: props, children, computed styles, and layout. |
| **Instance** | The executor's internal counterpart to a node. It owns hook state and persists across renders. |
| **Executor** | `VirtualDomExecutor`. Runs hooks, reconciles, applies CSS and layout, and invokes the renderer. |
| **Engine** | The VantaCSS engine that matches rules, computes styles, and performs layout. |
| **Renderer** | Your `IComponentRenderer` implementation that draws the resolved tree. |

## Documentation

| Guide | Read it to… |
| --- | --- |
| [Getting started](GettingStarted.md) | Build your first component, add state, style it, and render it. |
| [Components](Components.md) | Define components, pass props and children, and use intrinsic elements. |
| [Hooks](Hooks.md) | Manage state, effects, memoization, refs, and more. |
| [Context](Context.md) | Share values across a subtree without prop drilling. |
| [Events](Events.md) | Handle input and control event propagation. |
| [Styling](Styling.md) | Write CSS, use media queries and transitions, and read computed styles. |
| [Rendering](Rendering.md) | Implement `IComponentRenderer` and drive the executor. |
| [Architecture](Architecture.md) | Understand the virtual DOM, instances, reconciliation, and the CSS engine. |

## Installation

```bash
dotnet add package Rocket.NET
```

Requires a .NET SDK. The core library targets `netstandard2.0`.

## License

MIT