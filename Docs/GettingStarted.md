# Getting Started

## Install

```bash
dotnet add package Rocket.NET
```

Or reference the `Core` project directly.

## Create a component

Composite components inherit from `Component<TProps>` and override `Render`.

```csharp
using Rocket.Core;
using Rocket.Core.DOM;
using static Rocket.Core.UI;

public class CounterProps { public string Label { get; set; } = "Count"; }

public class Counter : Component<CounterProps>
{
    public override Node Render()
    {
        var (count, setCount) = State(0);

        return C<Panel>(
            new PanelProps(),
            C<Text>(new TextProps { Content = $"{Props.Label}: {count}" }),
            C<Button>(new ButtonProps
            {
                Text = "+",
                OnClick = _ => setCount(count + 1)
            })
        );
    }
}
```

## Render it

```csharp
using Rocket.Core;

var renderer = new HtmlRenderer();          // or your own IComponentRenderer
var executor = new VirtualDomExecutor(renderer);

var node = new Counter { Props = new CounterProps() }.Render();
executor.Render(node, 800, 600, Orientation.Portrait);

Console.WriteLine(renderer.GetHtml());
```

## Re-render

Calling `Render` again reuses the instance tree and preserves hook state:

```csharp
executor.Render(node, 800, 600, Orientation.Portrait); // state preserved
```

## Target framework & compatibility

Rocket.NET's `Core` library targets **.NET Standard 2.0**. This means it can be
consumed by a wide range of runtimes and frameworks without forcing an upgrade:

- .NET 6, 7, 8, 9, 10+
- .NET Framework 4.6.1+
- Mono / Xamarin
- Unity (with a compatible scripting backend)

The test suite targets `net10.0` and uses the latest SDK, but the shipped
library stays on `netstandard2.0` for maximum reach. If you need APIs only
available in newer TFMs, multi-target the consumer project rather than the
library.

## Project layout

```
Core/        # framework
Tests/       # tests + example renderer
Docs/        # this documentation
```
