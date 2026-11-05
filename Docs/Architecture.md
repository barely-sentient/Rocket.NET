# Architecture

[Home](README.md) › Architecture

This page explains how Rocket.NET turns components into a drawn UI. You do not need it to use the library, but it explains the rules the other guides ask you to follow, such as the [rules of hooks](Hooks.md#rules-of-hooks).

## Overview

```
  Components          Virtual DOM             Executor                 Renderer
 ┌───────────┐      ┌──────────────┐      ┌──────────────────┐      ┌──────────────┐
 │ Render()  │ ───▶ │  Node tree   │ ───▶│ Instance tree    │ ───▶│ Draw(root)   │
 │ + hooks   │      │              │      │ Reconciliation   │      │              │
 └───────────┘      └──────────────┘      │ VantaCSS + layout│      └──────────────┘
                                          └──────────────────┘
```

| Layer | Responsibility | Guide |
| --- | --- | --- |
| Components | Describe the desired UI. | [Components](Components.md) |
| Virtual DOM | Renderer-independent node tree. | below |
| Executor | Instances, hooks, reconciliation. | [Rendering](Rendering.md) |
| CSS engine | Style matching and layout. | [Styling](Styling.md) |
| Renderer | Draws the resolved tree. | [Rendering](Rendering.md#icomponentrenderer) |

## Virtual DOM

Components return `Node` trees that describe the desired UI. Nodes are renderer-independent and carry props, children, computed styles, and layout. Because nodes know nothing about any particular backend, the same tree can be drawn by any [`IComponentRenderer`](Rendering.md#icomponentrenderer).

## Instances

`VirtualDomExecutor` maintains an instance tree (`Instance`) that mirrors the node tree.

- **Composite components** expand to a child instance.
- **Primitive nodes** hold their children.

Instances are what persist between renders. Nodes are recreated by every `Render()` call, while instances keep hook state alive.

## Hooks

Hook state is stored on the owning `Instance` in a list indexed by call order. `ResetHooks()` resets the cursor at the start of each render while preserving the stored state.

This design is the reason for the rules in [Hooks](Hooks.md#rules-of-hooks): each hook call claims the next slot in the list, so calling hooks conditionally or in a different order would shift every later hook onto the wrong slot.

## Reconciliation

On re-render, the executor reuses instances when component types match, updating props and re-running `Render()`. Otherwise it unmounts the old instance and mounts a new one.

- Reuse preserves hook state and node identity.
- Unmounting invokes side-effect cleanup delegates. See [`Effect`](Hooks.md#effect--sideeffect).

## Rendering

After reconciliation, the executor applies CSS and layout through the VantaCSS engine and calls `IComponentRenderer.Draw(root)`.

## CSS engine

VantaCSS parses stylesheets, matches rules to nodes, computes merged declarations (including transitions), and performs layout. Results are attached to the nodes as `ComputedStyle`, `ComputedStyles`, and `Layout`. See [Styling](Styling.md) for supported features and limitations.

## Life of a render

1. `executor.Render(root)` is called.
2. For each component, hook cursors are reset with `ResetHooks()` and `Render()` runs. Hooks read and write state on the instance.
3. The executor reconciles the new nodes against the instance tree, reusing instances where component types match.
4. The VantaCSS engine matches stylesheet rules and computes styles and layout for the viewport.
5. The renderer's `Draw(root)` is called with the fully resolved tree.
6. When an event handler calls a state setter, a re-render is requested and the cycle repeats from step 2. See [Events](Events.md).

---

← [Rendering](Rendering.md) · Back to [Home](README.md)