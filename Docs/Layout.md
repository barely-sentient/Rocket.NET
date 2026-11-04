# Layout

The CSS engine computes a `LayoutBox` for every node.

```csharp
var engine = new CssEngine();
engine.SetViewport(1024, 768, Orientation.Landscape);

LayoutBox box = engine.CalculateLayout(root, 1024, 768, Orientation.Landscape);
Console.WriteLine($"x={box.X} y={box.Y} w={box.Width} h={box.Height}");
```

## Supported layout modes

- Block flow (default)
- Flexbox: `display: flex`, `flex-direction: row|column`
- `justify-content`: flex-start, center, flex-end, space-between, space-around
- `gap`
- Absolute/fixed positioning (`position`, `top/right/bottom/left`)
- `margin`, `padding`, `border`
- `width`/`height` with `px`, `%`, `em`, `rem`, `vw`, `vh`
- `min-*` / `max-*`
- `box-sizing: border-box|content-box`

## Reading results

```csharp
LayoutBox b = node.Layout;          // canonical
var all = engine.Layouts;           // Dictionary<Node, LayoutBox>
```

Many alias properties (`Bounds`, `Rect`, `Frame`, ...) exist for convenience but all
return the same `LayoutBox`.
