# Example: Configure retry backoff

Build a retry pipeline with an explicit attempt limit and exponential backoff.

[Examples](index.md) · [Assembly overview](../index.md)

## Code

```csharp
using System;

using Assimalign.Cohesion.Resilience;

IResiliencePipeline pipeline = new ResiliencePipelineBuilder()
    .UseRetry(options =>
    {
        options.MaxRetryAttempts = 5;
        options.Delay = TimeSpan.FromMilliseconds(200);
        options.BackoffType = DelayBackoffType.Exponential;
    })
    .Build();
```

## Walkthrough

`UseRetry` adds the retry strategy to the builder. Its options separate the retry count, base delay,
and backoff choice. This example builds the policy; the behavioral tests show callback execution and
result assertions.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/docs/DESIGN.md`.
- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/Extensions/RetryResilienceExtensions.cs`.
