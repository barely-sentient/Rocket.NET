# Hooks

Hooks must be called unconditionally and in the same order on every render.

## State

```csharp
var (count, setCount) = State(0);
setCount(count + 1);
```

## Reducer

```csharp
var (state, dispatch) = Reducer((int s, int a) => s + a, 0);
dispatch(5);
```

## Effects

```csharp
SideEffect(() =>
{
    Console.WriteLine("mounted");
    return () => Console.WriteLine("cleanup");
}, new object[] { dependency });
```

`Effect`, `LayoutEffect`, `InsertionEffect` are aliases.

## Memo & Callback

```csharp
var value = Memo(() => Expensive(), new object[] { dep });
var onClick = Callback(() => Handle(), new object[] { dep });
```

## Ref

```csharp
var r = Ref(0);
r.Current = 42;
```

## Context

```csharp
var ctx = CreateContext("default");
// provider
var provider = new Provider<string> { Props = new ProviderProps<string> { Context = ctx, Value = "provided" } };
// consumer
var value = Context(ctx);
```

## Other

- `Id()` — stable id across renders.
- `Ref<T>()` / `Ref<T>(initial)` — mutable cell.
- `DeferredValue`, `DebugValue`, `Transition`, `SyncExternalStore`.

See `Tests/Hooks/HooksTests.cs` for worked examples.
