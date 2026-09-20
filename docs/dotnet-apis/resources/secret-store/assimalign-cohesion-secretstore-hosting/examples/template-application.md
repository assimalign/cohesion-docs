# Template application

The shipped `cohesion-secretstore` template demonstrates the SecretStore executable entry point.

> **Status:** Partial.

This is the `Program.cs` from `cohesion-secretstore`. `Use` the matching
`Assimalign.Cohesion.Sdk.SecretStore` project from that template.

```csharp
using Assimalign.Cohesion.SecretStore;
using Assimalign.Cohesion.SecretStore.Hosting;

SecretStoreApplicationBuilder builder = SecretStoreApplication.CreateBuilder(args);

await using SecretStoreApplication application = builder.Build();
await application.RunAsync();
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-secretstore/Program.cs`.
