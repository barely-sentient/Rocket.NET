# Components

## Composite components

```csharp
public class MyProps { public string Title { get; set; } }

public class MyComponent : Component<MyProps>
{
    public override Node Render()
    {
        return C<Panel>(new PanelProps(), C<Text>(new TextProps { Content = Props.Title }));
    }
}
```

## Primitive (HTML) components

Use `UI.H` with a tag name:

```csharp
var button = H("button", new HTMLButtonElementProps { TextContent = "Save" });
var div = H("div", new HTMLDivElementProps(), child1, child2);
```

Or use the strongly typed HTML element classes directly via `C<T>`.

## Props

Props are plain C# objects. The framework reads public fields/properties via reflection
(cached for performance) when rendering attributes and computing styles.

```csharp
public class ButtonProps
{
    public string Text { get; set; }
    public bool Disabled { get; set; }
    public Action<Rocket.Core.Event> OnClick { get; set; }
}
```

## Children

Children are passed as the `params Node[]` argument:

```csharp
C<Panel>(new PanelProps(),
    C<Text>(new TextProps { Content = "A" }),
    C<Text>(new TextProps { Content = "B" })
);
```

## Stylesheets on a component

Declare stylesheets directly on a component (or assembly) with `StylesheetAttribute`:

```csharp
[Stylesheet("Styles/app.css")]
public class MyApp : Component<MyAppProps>
{
    public override Node Render() => ...;
}
```

See [CSS & Styling](Css.md#pointing-to-a-stylesheet-with-an-attribute) for loading details.

## Context providers

Context lets a component pass a value to all descendants without prop drilling.

### Using the built-in `Provider<T>`

```csharp
var themeCtx = CreateContext("light");

var app = new Provider<string>
{
    Props = new ProviderProps<string>
    {
        Context = themeCtx,
        Value = "dark",
        Children = new[]
        {
            C<ThemedButton>(new ThemedButtonProps())
        }
    }
}.Render();
```

Consume it anywhere in the subtree:

```csharp
public class ThemedButton : Component<ThemedButtonProps>
{
    public override Node Render()
    {
        var theme = Context(themeCtx);   // "dark"
        return C<Button>(new ButtonProps { Text = $"Theme: {theme}" });
    }
}
```

### Creating your own provider component

Implement `IContextProvider` and push/pop in the executor's render flow
(the executor calls these around the child render):

```csharp
public class ThemeProviderProps
{
    public Context<string> Context { get; set; }
    public string Value { get; set; }
    public Node[] Children { get; set; }
}

public class ThemeProvider : Component<ThemeProviderProps>, IContextProvider
{
    public void PushContext()
    {
        if (Props?.Context != null)
            ComponentContext.PushContext(Props.Context, Props.Value);
    }

    public void PopContext()
    {
        ComponentContext.PopContext();
    }

    public override Node Render()
    {
        if (Props?.Children == null || Props.Children.Length == 0)
            return null;

        if (Props.Children.Length == 1)
            return Props.Children[0];

        return H("div", new HTMLDivElementProps(), Props.Children);
    }
}
```

Notes:
- `PushContext`/`PopContext` are invoked by `VirtualDomExecutor` around the
  rendered child, so descendants see the value.
- Always pair push with pop (the executor does this for you).
- Context values are looked up by reference equality of the `Context<T>` object,
  so keep the context instance stable (create it once, e.g. as a static field).
