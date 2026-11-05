# Getting started

[Home](README.md) › Getting started

This guide takes you from an empty project to a rendered, stateful, styled component. By the end you will have touched every stage of the Rocket.NET pipeline: components, hooks, CSS, and rendering.

**You will build:** a counter with a button, an effect that logs changes, and a stylesheet.

## Prerequisites

- A .NET SDK. The core library targets `netstandard2.0`.
- The `Rocket.NET` package:

```bash
dotnet add package Rocket.NET
```

## Step 1: Create a component

A component derives from `Component<TProps>` and overrides `Render()`, which returns a tree of nodes.

```csharp
using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using static Rocket.Core.UI;

public class Greeting : Component<GreetingProps>
{
    public override Node Render()
    {
        return Div(new() { ClassName = "greeting" },
            H1(new() { TextContent = "Hello, Rocket" }),
            Paragraph(new() { TextContent = "Write once, render anywhere." })
        );
    }
}

public class GreetingProps { }
```

`using static Rocket.Core.UI;` brings the element helpers (`Div`, `H1`, `Paragraph`, …) into scope without qualification.

> **Go deeper:** [Components](Components.md) covers props, children, and composing components together.

## Step 2: Add state

Hooks are called at the top of `Render()`. `State` returns the current value and a setter; calling the setter requests a re-render.

```csharp
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

The `OnClick` handler is an event handler prop. See [Events](Events.md) for the full event model.

## Step 3: Add an effect

Effects run side effects when their dependencies change.

```csharp
var (count, setCount) = State(0);

Effect(() =>
{
    Console.WriteLine($"Count is now {count}");
    return () => Console.WriteLine("cleanup"); // optional
}, new object[] { count });
```

- The effect re-runs whenever a value in the dependency array changes.
- Return a cleanup delegate to release resources, or return `null`.
- Pass `null` (or omit) the dependency array to run the effect once on mount.

> **Go deeper:** [Hooks](Hooks.md) documents every hook, including `Reducer`, `Memo`, `Callback`, and `Ref`.

## Step 4: Style it

Stylesheets are added to the executor before rendering. The VantaCSS engine resolves them, along with layout, ahead of drawing.

```csharp
using VantaCSS;

var executor = new VirtualDomExecutor(renderer);
executor.Stylesheets.Add(new Stylesheet("app", @"
    .counter { display: flex; flex-direction: column; gap: 8px; }
    .counter h1 { font-size: 24px; }
"));
```

Resolved styles are stored on each node as `ComputedStyle` and `ComputedStyles`, ready for your renderer to read.

> **Go deeper:** [Styling](Styling.md) covers selectors, media queries, transitions, and limitations.

## Step 5: Render it

Implement `IComponentRenderer` for your target platform, then hand the tree to the executor.

```csharp
using Rocket.Core;

IComponentRenderer renderer = new MyRenderer();
var executor = new VirtualDomExecutor(renderer);

Node root = new Counter().Render();
executor.Render(root);
```

The executor reconciles the tree, applies CSS and layout, and calls your renderer's `Draw` method with the resolved root node.

> **Go deeper:** [Rendering](Rendering.md) explains how to write a renderer and how updates are reconciled.

## Putting it together

```csharp
IComponentRenderer renderer = new MyRenderer();
var executor = new VirtualDomExecutor(renderer);

executor.Stylesheets.Add(new VantaCSS.Stylesheet("app", @"
    .counter { display: flex; flex-direction: column; gap: 8px; }
    .counter h1 { font-size: 24px; }
"));

Node root = new Counter().Render();
executor.Render(root);
```

## Rules of hooks

These rules apply everywhere hooks are used.

- Call hooks unconditionally.
- Call them in the same order on every render.
- Call them only from a component's `Render()` or from other hooks.

Hook state is stored by call position, which is why order matters. The reasoning is explained in [Architecture](Architecture.md#hooks).

## Next steps

| If you want to… | Read |
| --- | --- |
| Compose larger UIs from smaller parts | [Components](Components.md) |
| Manage state and effects in depth | [Hooks](Hooks.md) |
| Share values across a subtree | [Context](Context.md) |
| Respond to user input | [Events](Events.md) |
| Make it look right | [Styling](Styling.md) |
| Target your own platform | [Rendering](Rendering.md) |

---

Next: [Components →](Components.md)