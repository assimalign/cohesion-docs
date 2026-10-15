# Example: Configure a timeout

Build a pipeline with a fixed timeout duration.

[Examples](index.md) · [Assembly overview](../index.md)

## Code

```csharp
using System;

using Assimalign.Cohesion.Resilience;

IResiliencePipeline pipeline = new ResiliencePipelineBuilder()
    .UseTimeout(options =>
    {
        options.Timeout = TimeSpan.FromSeconds(2);
    })
    .Build();
```

## Walkthrough

`UseTimeout` attaches the timeout strategy with a two-second duration. Dynamic duration selection
uses `TimeoutGenerator` instead; the package design and behavioral tests describe that separate
option.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/docs/DESIGN.md`.
- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/Extensions/TimeoutResilienceExtensions.cs`.
