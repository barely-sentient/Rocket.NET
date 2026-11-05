# Components

[Home](README.md) › Components

Components are the building blocks of a Rocket.NET UI. Each one describes a subtree of nodes and can hold state and effects. Components are composed of other components and of intrinsic elements (the familiar HTML tags).

**On this page**
[Defining a component](#defining-a-component) · [Props](#props) · [Composing components](#composing-components) · [Children](#children) · [Intrinsic elements](#intrinsic-elements-html-tags) · [Context providers](#context-providers) · [Resetting hooks](#resetting-hooks)

## Defining a component

Derive from `Component<TProps>`, where `TProps` is a plain class (usually with a parameterless constructor), and override `Render()`:

```csharp
using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using static Rocket.Core.UI;

public class UserCard : Component<UserCardProps>
{
    public override Node Render()
    {
        if (Props.IsUserLoggedIn)
        {
            return Paragraph(new() { TextContent = "You're logged in" });
        }

        return Div(new() { ClassName = "user-card" },
            H1(new() { TextContent = "Hello" }),
            Paragraph(new() { TextContent = "You're not logged in" })
        );
    }
}

public class UserCardProps
{
    public bool IsUserLoggedIn { get; set; } = false;
}
```

- `Render()` returns a `Node`, or `null` to render nothing.
- `Props` is set by the framework before rendering.
- Hooks such as `State` and `Effect` are called at the top of `Render()`. See [Hooks](Hooks.md).

## Props

Props are ordinary C# properties on your `TProps` class. `TProps` must have a parameterless constructor (the framework creates one if needed).

Pass props when building a node:

```csharp
Node node = C<UserCard>(new UserCardProps { IsUserLoggedIn = true });
```

Or set them directly when constructing a component instance, as tests commonly do:

```csharp
var card = new UserCard { Props = new UserCardProps { IsUserLoggedIn = false } };
var node = card.Render();
```

## Composing components

`C<T>(props)` produces a node for a component, so components nest inside intrinsic elements and inside each other:

```csharp
Div(new() { ClassName = "page" },
    C<UserCard>(new UserCardProps { IsUserLoggedIn = true })
);
```

During rendering the executor expands each component node into a child instance and preserves its hook state between renders. See [Architecture](Architecture.md#instances) and [Rendering](Rendering.md#reconciliation-notes).

## Children

For intrinsic elements, children are passed as trailing arguments:

```csharp
Div(new() { ClassName = "container" },
    H1(new() { TextContent = "Title" }),
    Paragraph(new() { TextContent = "Body" })
);
```

Components can also accept children. The base `Component<TProps>` declares a `Node[] Children` property, and the framework sets it from the node's children when mounting or updating, before `Render()` is called.

## Intrinsic elements (HTML tags)

Every HTML tag has a shorthand helper generated into the partial `UI` class: `Div`, `Span`, `Button`, `H1`…`H6`, `Paragraph`, `Input`, `Img`, `Ul`, `Li`, and many more. Each takes a props object and a `params Node[]` of children.

The lower-level `H(tagName, props, children)` is also available:

```csharp
H("div", new HTMLDivElementProps { ClassName = "box" },
    H("span", new HTMLSpanElementProps { TextContent = "hi" })
);
```

Element props carry the attributes you expect (`ClassName`, `TextContent`, `Id`) and event handlers such as `OnClick`. Class names and ids are what [Styling](Styling.md#selectors) selectors match against. Handlers are covered in [Events](Events.md).

## Context providers

`Provider<T>` implements `IContextProvider` and pushes a value into the context stack for its subtree:

```csharp
var ctx = CreateContext("default");

C<Provider<string>>(new ProviderProps<string>
{
    Context = ctx,
    Value = "provided",
    Children = new[] { /* subtree */ }
});
```

Descendants read the value with the `Context` hook. See [Context](Context.md).

## Resetting hooks

`ResetHooks()` is called automatically at the start of each render. It resets the hook cursor while preserving stored state, so you do not normally need to call it yourself. See [Architecture](Architecture.md#hooks).

---

← [Getting started](GettingStarted.md) · Next: [Hooks →](Hooks.md)