# Context

[Home](README.md) › Context

Context lets an ancestor provide a value to all of its descendants without passing props through every level (prop drilling). Typical uses include themes, localization, and shared services.

Context has three parts: **create** a context, **provide** a value, and **consume** it.

## Creating a context

```csharp
var theme = CreateContext("light");
```

The argument is the default value, returned when no provider is present.

## Providing a value

Wrap a subtree in a `Provider<T>` node:

```csharp
C<Provider<string>>(new ProviderProps<string>
{
    Context = theme,
    Value = "dark",
    Children = new[] { /* subtree */ }
});
```

`Provider<T>` is an ordinary component (see [Components](Components.md#context-providers)) that implements `IContextProvider`. Every component rendered beneath it sees the provided value.

## Consuming a value

Call the `Context` hook inside `Render()`:

```csharp
var currentTheme = Context(theme);
```

Without a provider, the default value is returned. Like every hook, `Context` must follow the [rules of hooks](Hooks.md#rules-of-hooks).

## Example

```csharp
public class ThemedLabel : Component<ThemedLabelProps>
{
    public override Node Render()
    {
        var currentTheme = Context(AppContexts.Theme);

        return Paragraph(new()
        {
            ClassName = $"label label--{currentTheme}",
            TextContent = Props.Text
        });
    }
}

public class ThemedLabelProps
{
    public string Text { get; set; } = "";
}

public static class AppContexts
{
    // Keep the context instance stable; see Notes below.
    public static readonly Context<string> Theme = CreateContext("light");
}
```

The class name produced here can be targeted by a stylesheet. See [Styling](Styling.md#selectors).

## Notes

- `PushContext` / `PopContext` on `Provider<T>` manipulate the context stack directly. The framework calls them during rendering, so you do not need to.
- Context values are matched **by reference**, so keep the `Context<T>` instance stable (for example in a static field) rather than creating a new one each render.

---

← [Hooks](Hooks.md) · Next: [Events →](Events.md)