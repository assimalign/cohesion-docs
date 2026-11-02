# Assimalign.Cohesion.Sdk.Database

The Database SDK adds its frameworks and optional C# schema compilation and SQL migrations.

> **Status:** Partial. SQL schema and migration tooling is implemented; KeyValuePair compilation and migrations fail explicitly.

Choose this SDK for Database applications. It pairs `Assimalign.Cohesion.App.Database` with the
inherited `Assimalign.Cohesion.App` framework. Schema tooling is an independent opt-in; choosing
this SDK alone does not compile a schema. The SDK emits no resource manifest or generated resource
accessors. The application becomes managed at runtime through
[Database.Hosting.ApplicationModel](../../resources/database/assimalign-cohesion-database-hosting-applicationmodel/index.md)
with `builder.Services.AddResource("orders-db", resource => resource.UseInProcess())`.
The integration is public in `App.Database`; reference each platform hosting package explicitly.

## Minimal consumer

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.Database">
</Project>
```

```json
{
  "sdk": {
    "version": "10.0.300",
    "rollForward": "latestFeature"
  },
  "msbuild-sdks": {
    "Assimalign.Cohesion.Sdk": "10.0.1-preview.3",
    "Assimalign.Cohesion.Sdk.Database": "10.0.1-preview.3"
  }
}
```

Supply a `Program.cs` entry point. To compile a retained C# schema declaration, set
`CohesionDatabaseProject=true` and `CohesionDatabaseModel=Sql`. The compiler analyzes retained
`SqlSchema.Compile`, `SqlSchema.Create`, or inline engine-builder `database.Schema(...)` declarations
without executing the entry point, and writes one schema artifact pair per database.

- **Overview** — [Properties, artifacts, and diagnostics](overview.md).
- **MSBuild** — [Imports and inner build machinery](msbuild/index.md).
- **Task assembly** —
  [`Assimalign.Cohesion.Sdk.Database.Tasks`](assimalign-cohesion-sdk-database-tasks/index.md).
- **Database** — [Product documentation](../../../database/index.md) and
  [API reference](../../resources/database/index.md).
- **Family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.KeyValuePair.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.Sql.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/global.json`
