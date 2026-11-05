# Hooks

[Home](README.md) › Hooks

Hooks let components hold state and run side effects. They are static methods on `Rocket.Core.UI` (and `Rocket.Core.Hooks`) and are called at the top of `Render()`.

Import them with either of:

```csharp
using static Rocket.Core.UI;
using static Rocket.Core.Hooks;
```

## Reference

| Hook | Purpose |
| --- | --- |
| [`State`](#state) | Hold a value and request re-renders when it changes. |
| [`Reducer`](#reducer) | Manage state transitions with a reducer function. |
| [`Effect`](#effect--sideeffect) | Run side effects when dependencies change. |
| [`Memo`](#memo) | Cache an expensive computation. |
| [`Callback`](#callback) | Keep a delegate's identity stable across renders. |
| [`Ref`](#ref) | Hold a mutable value without triggering re-renders. |
| [`Context`](#context) | Read a value supplied by an ancestor provider. |
| [`Id`](#id) | Get a stable unique id per hook position. |
| [`DeferredValue` / `DebugValue`](#deferredvalue--debugvalue) | Compatibility hooks. |
| [`Transition`](#transition) | Mark updates as transitions. |
| [`SyncExternalStore`](#syncexternalstore) | Read a snapshot from an external store. |

## State

```csharp
var (count, setCount) = State(0);
```

- Returns the current value and a setter.
- The setter updates the stored value and requests a re-render.
- State is preserved across re-renders by hook position.

## Reducer

```csharp
var (state, dispatch) = Reducer((int s, int a) => s + a, 0);
dispatch(5); // state becomes 5
```

An overload computes the initial state from an argument:

```csharp
var (state, dispatch) = Reducer(reducer, 5, x => x * 2); // initial state is 10
```

## Effect / SideEffect

```csharp
Effect(() =>
{
    // runs when dependencies change
    return () => { /* cleanup */ }; // optional
}, new object[] { count });
```

- Pass `null` (or omit) the dependency array to run once on mount.
- The effect may return a cleanup delegate.
- Cleanup delegates are invoked when the instance unmounts. See [Rendering](Rendering.md#reconciliation-notes).
- `SideEffect`, `LayoutEffect`, and `InsertionEffect` are aliases.

## Memo

```csharp
var value = Memo(() => ExpensiveComputation(input), new object[] { input });
```

Caches the result and recomputes only when the dependencies change.

## Callback

```csharp
var onClick = Callback(() => HandleClick(id), new object[] { id });
```

Returns a stable delegate instance across renders while the dependencies are unchanged. This is useful for event handler props. See [Events](Events.md).

## Ref

```csharp
var inputRef = Ref<HTMLInputElementProps>(null);
```

A mutable container (`Current` / `Value`) whose identity is stable across renders. Mutating it does not trigger a re-render. Use it for timers, previous values, or handles.

## Context

```csharp
var theme = CreateContext("light");

// read:
var current = Context(theme);
```

`CreateContext` defines a context, and `Context(...)` reads the nearest provided value. See [Context](Context.md) for providers and matching rules.

## Id

```csharp
var id = Id(); // stable unique id per hook position
```

## DeferredValue / DebugValue

```csharp
var v = DeferredValue(value); // returns value unchanged
DebugValue(value);             // no-op debug hook
```

These exist for familiarity and API compatibility. In this implementation they have no scheduling or debugging effect.

## Transition

```csharp
var (isPending, startTransition) = Transition();
startTransition(() => setTab(nextTab));
```

In this implementation `isPending` is always `false` and the action runs synchronously.

> Not to be confused with CSS transitions, which are covered in [Styling](Styling.md#transitions).

## SyncExternalStore

```csharp
var snapshot = SyncExternalStore(subscribe, getSnapshot, getServerSnapshot);
```

Reads a snapshot from an external store. Subscription handling is left to the caller.

## Rules of hooks

- Call hooks unconditionally.
- Call them in the same order on every render.
- Do not call them inside loops or conditionals.

Hook state is stored on the owning instance in a list indexed by call order, which is why these rules are required. See [Architecture](Architecture.md#hooks).

---

← [Components](Components.md) · Next: [Context →](Context.md)