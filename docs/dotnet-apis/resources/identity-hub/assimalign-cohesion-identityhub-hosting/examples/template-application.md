# Template application

The shipped `cohesion-identityhub` template demonstrates the IdentityHub executable entry point.

> **Status:** Implemented.

This is the `Program.cs` from `cohesion-identityhub`. `Use` the matching
`Assimalign.Cohesion.Sdk.IdentityHub` project from that template.

```csharp
using Assimalign.Cohesion.IdentityHub;
using Assimalign.Cohesion.IdentityHub.Hosting;

IdentityHubApplicationBuilder builder = IdentityHubApplication.CreateBuilder(args);

await using IdentityHubApplication application = builder.Build();
await application.RunAsync();
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-identityhub/Program.cs`.
