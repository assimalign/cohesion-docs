# Template application

The shipped `cohesion-configurationstore` template demonstrates the ConfigurationStore executable entry point.

> **Status:** Partial.

This is the `Program.cs` from `cohesion-configurationstore`. `Use` the matching
`Assimalign.Cohesion.Sdk.ConfigurationStore` project from that template.

```csharp
using Assimalign.Cohesion.ConfigurationStore;
using Assimalign.Cohesion.ConfigurationStore.Hosting;

ConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder(args);

await using ConfigurationStoreApplication application = builder.Build();
await application.RunAsync();
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-configurationstore/Program.cs`.
