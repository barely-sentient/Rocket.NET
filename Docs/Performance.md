# Performance

## What the framework already does

- **Reconciliation** reuses component instances across renders, preserving hook
  state and avoiding full remounts.
- **Reflection caching** for component metadata, props, and CSS id/class lookups.
- **Compiled constructor delegates** replace `Activator.CreateInstance`.
- **CSS rule cache** memoizes filtered rule lists per node identity/viewport.
- **Style result cache** short-circuits `ComputeStyle` when nothing changed.
- **Fast event dispatch** invokes `Action<Event>` directly.
- **Debug logging removed** from the render path.

## Tips

- Keep component `Render` methods cheap; push heavy work into `Memo`.
- Use `Callback` to stabilize delegate identity across renders.
- Pass dependency arrays to `SideEffect`/`Memo`/`Callback` to avoid re-runs.
- Avoid allocating new child arrays unnecessarily; reuse where possible.
- Profile before optimizing further: `dotnet-trace`, `GC.GetAllocatedBytesForCurrentThread`.

## Node arenas

A pooled/arena allocator for nodes can reduce GC pressure for very large trees.
It is not enabled by default because nodes are mutable and retained by the tree;
pooling requires a strict reset contract and render-scoped lifetime. See the
audit notes in the repository for the recommended design.
