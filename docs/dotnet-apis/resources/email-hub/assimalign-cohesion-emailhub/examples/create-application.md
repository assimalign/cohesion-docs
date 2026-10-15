# Create an application

Create and build the concrete application that implements the area contracts.

> **Status:** Partial.

This composition example comes from the documented type surface. In an executable, `args` is the
top-level command-line argument array. Domain behavior depends on the area status.

```csharp
using Assimalign.Cohesion.EmailHub.Hosting;

EmailHubApplicationBuilder builder = EmailHubApplication.CreateBuilder(args);
// Add area declarations and optional hosting services before Build().
await using EmailHubApplication application = builder.Build();
await application.RunAsync();
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub/docs/Assembly/Assimalign.Cohesion.EmailHub/IEmailHubApplication/OVERVIEW.md`.
