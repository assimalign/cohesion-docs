# Create an application

Create and build the concrete application that implements the area contracts.

> **Status:** Partial.

This composition example comes from the documented type surface. In an executable, `args` is the
top-level command-line argument array. Domain behavior depends on the area status.

```csharp
using Assimalign.Cohesion.LoadBalancer.Hosting;

LoadBalancerApplicationBuilder builder = LoadBalancerApplication.CreateBuilder(args);
// Add area declarations and optional hosting services before Build().
await using LoadBalancerApplication application = builder.Build();
await application.RunAsync();
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer/docs/Assembly/Assimalign.Cohesion.LoadBalancer/ILoadBalancerApplication/OVERVIEW.md`.
