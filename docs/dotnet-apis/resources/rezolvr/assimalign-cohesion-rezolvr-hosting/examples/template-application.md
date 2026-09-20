# Template application

The shipped `cohesion-rezolvr` template demonstrates the Rezolvr executable entry point.

> **Status:** Partial.

This is the `Program.cs` from `cohesion-rezolvr`. `Use` the matching
`Assimalign.Cohesion.Sdk.Rezolvr` project from that template.

```csharp
using Assimalign.Cohesion.Rezolvr;
using Assimalign.Cohesion.Rezolvr.Hosting;

RezolvrApplicationBuilder builder = RezolvrApplication.CreateBuilder(args);

await using RezolvrApplication application = builder.Build();
await application.RunAsync();
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-rezolvr/Program.cs`.
