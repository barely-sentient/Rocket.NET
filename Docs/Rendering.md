# Rendering Backends

Implement `IComponentRenderer` to target any backend:

```csharp
public interface IComponentRenderer
{
    void Draw(Node rootNode);
}
```

## HTML renderer (test helper)

`Tests/Rendering/HtmlRenderer.cs` is a reference implementation that emits HTML:

```csharp
var renderer = new HtmlRenderer();
var executor = new VirtualDomExecutor(renderer);
executor.Render(app, 800, 600, Orientation.Portrait);
Console.WriteLine(renderer.GetHtml());
```

## Custom backend

```csharp
public class MyRenderer : IComponentRenderer
{
    public void Draw(Node rootNode)
    {
        // walk rootNode and its Children, read ComputedStyle/Layout, draw.
    }
}
```

## VirtualDomExecutor

```csharp
var executor = new VirtualDomExecutor(renderer);
// optional: supply a shared CSS engine
var executor2 = new VirtualDomExecutor(renderer, new CssEngine());

executor.Stylesheets.Add(sheet);
executor.Render(rootNode, width, height, orientation);
Node toDraw = executor.LastNodeToDraw;
```

## Mount / reconcile

`Render` reuses the instance tree when component types match, preserving hook state.
Call `MountRoot` if you need the resolved node without drawing.
