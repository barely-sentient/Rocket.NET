# Events

[Home](README.md) › Events

Intrinsic elements expose event-handler props on their props type, for example `OnClick`, `OnInput`, `OnChange`, and `OnKeyDown`. Handlers receive a `Rocket.Core.Event`, or a type derived from it.

## Handling an event

Assign a delegate to the handler prop when building the element:

```csharp
Button(new()
{
    TextContent = "Save",
    OnClick = e =>
    {
        e.StopPropagation();
        Save();
    }
});
```

Handlers commonly call state setters, which request a re-render. See [Hooks](Hooks.md#state).

```csharp
var (count, setCount) = State(0);

Button(new()
{
    TextContent = "Increment",
    OnClick = _ => setCount(count + 1)
});
```

To keep a handler's identity stable across renders, wrap it with [`Callback`](Hooks.md#callback).

## Event propagation

Events bubble up the node tree by default (`Bubbles = true`).

| Method | Effect |
| --- | --- |
| `StopPropagation()` | Stops the event from bubbling to parent nodes. |
| `PreventPropagation()` | Same effect as `StopPropagation()`. |
| `PreventDefault()` | Marks the event as default-prevented. |

## Event types

| Type | Members |
| --- | --- |
| `Event` | Base type for all events. |
| `MouseEvent` | `ClientX`, `ClientY`, `Button` |
| `KeyboardEvent` | `Key`, `Code`, `CtrlKey`, `ShiftKey`, `AltKey`, `MetaKey` |
| `ChangeEvent` | `Value` |
| `InputEvent` | `Data`, `Value` |

## Dispatching manually

Nodes expose `DispatchEvent(eventName, e)`. It invokes the matching `On...` handler on the node's props, then bubbles to the parent unless propagation has been stopped.

Rocket.NET does not read platform input itself, because drawing and input are the backend's concern. `DispatchEvent` is the entry point for feeding input into the tree, whether from a [renderer](Rendering.md) translating platform input or from a test.

---

← [Context](Context.md) · Next: [Styling →](Styling.md)