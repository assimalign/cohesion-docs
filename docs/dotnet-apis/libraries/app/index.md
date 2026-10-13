# App

`libraries/App/` is the home of the Cohesion shared-framework family. It holds the root
`Assimalign.Cohesion.App` framework (the hosting kernel every resource-area SDK references), the
shared props every framework producer imports, and the build logic that turns a framework's
member list into targeting and runtime packs. Each area framework, `Assimalign.Cohesion.App.<Area>`,
is produced inside its own area under `resources/<Area>/`, and its members are listed by hand in
`resources/<Area>/Assimalign.Cohesion.<Area>.Runtime/Directory.Build.props`.

| Path | Role |
| --- | --- |
| `Assimalign.Cohesion.App.props` | App's own inputs (the kernel roots, the umbrella assembly, the Core source generator) and the defaults every producer shares (TFM, version prefix/suffix, package description). It lists no area framework |
| `Assimalign.Cohesion.App.targets` | Producer build logic: the App kernel closure, the `FrameworkList.xml`/`RuntimeList.xml` writer, and pack collection |
| `Assimalign.Cohesion.App.Refs` | Produces the `Assimalign.Cohesion.App.Ref` targeting pack |
| `Assimalign.Cohesion.App.Runtime` | Produces the `Assimalign.Cohesion.App.Runtime.<rid>` runtime packs. Its `tests/` check App's derived closure and each area list's coverage of its project-graph closure |

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.App.Runtime` | Produces the hosting kernel runtime packs and verifies framework membership. | [Overview](assimalign-cohesion-app-runtime/index.md) |

## Kernel roots

App derives its public reference and runtime inventory from these seventeen
`CohesionAppKernelRoot` entries and their transitive Cohesion project references, then adds the
`Assimalign.Cohesion.App` umbrella assembly. The three connection capability packages and
`Logging.EventSource` are kernel roots, so resource frameworks obtain them through App.

| Kernel root |
|---|
| `Assimalign.Cohesion.Hosting` |
| `Assimalign.Cohesion.Hosting.Health` |
| `Assimalign.Cohesion.Hosting.Resources` |
| `Assimalign.Cohesion.Connections` |
| `Assimalign.Cohesion.Connections.StreamAbort` |
| `Assimalign.Cohesion.Connections.StreamLimits` |
| `Assimalign.Cohesion.Connections.Tls` |
| `Assimalign.Cohesion.Hosting.Telemetry` |
| `Assimalign.Cohesion.Configuration.CommandLine` |
| `Assimalign.Cohesion.Configuration.EnvironmentVariables` |
| `Assimalign.Cohesion.Configuration.FileSystem` |
| `Assimalign.Cohesion.Configuration.Json` |
| `Assimalign.Cohesion.Logging.Console` |
| `Assimalign.Cohesion.Logging.EventSource` |
| `Assimalign.Cohesion.OpenTelemetry` |
| `Assimalign.Cohesion.DependencyInjection` |
| `Assimalign.Cohesion.FileSystem.Physical` |

The targeting pack also carries `Assimalign.Cohesion.SourceGeneration` under
`analyzers/dotnet/cs/`. Resource-area SDKs reference App together with their area framework; a
direct base-SDK consumer adds an explicit `FrameworkReference`.

## Project map

How the producers depend on each other and on the files they import. Every producer imports
`App.props` and `App.targets`. An area's Runtime producer reads the area's member list from its own
`Directory.Build.props`, and the sibling Refs producer's `Directory.Build.props` imports that same
file. The App runtime producer references only the kernel roots; an area runtime producer
references every listed member.

```mermaid
flowchart LR
    AppRefs["Assimalign.Cohesion.App.Refs"] --> AppRuntime["Assimalign.Cohesion.App.Runtime"]
    AppRefs --> Shared["Assimalign.Cohesion.App.props and .targets"]
    AppRuntime --> Shared
    AppRuntime --> Kernel["App kernel roots: Hosting, Connections, Configuration providers, ..."]
    AreaRefs["resources/Area: Assimalign.Cohesion.Area.Refs"] --> AreaRuntime["resources/Area: Assimalign.Cohesion.Area.Runtime"]
    AreaRefs --> Shared
    AreaRefs --> AreaList["Area.Runtime/Directory.Build.props: the member list"]
    AreaRuntime --> Shared
    AreaRuntime --> AreaList
    AreaRuntime --> Family["the area's listed libraries"]
```

## Naming convention

A framework producer project is named for the area that owns it. Its assembly, package, and
framework names keep the `App` segment and never change when the project moves:

| Framework | Producer projects | Assemblies | Packages |
| --- | --- | --- | --- |
| `Assimalign.Cohesion.App` | `libraries/App/Assimalign.Cohesion.App.Refs`, `.Runtime` | `Assimalign.Cohesion.App.Refs`, `Assimalign.Cohesion.App` | `Assimalign.Cohesion.App.Ref`, `Assimalign.Cohesion.App.Runtime.<rid>` |
| `Assimalign.Cohesion.App.<Area>` | `resources/<Area>/Assimalign.Cohesion.<Area>.Refs`, `.Runtime` | `Assimalign.Cohesion.App.<Area>.Refs`, `Assimalign.Cohesion.App.<Area>` | `Assimalign.Cohesion.App.<Area>.Ref`, `Assimalign.Cohesion.App.<Area>.Runtime.<rid>` |

The full rule, covering what each producer declares, how the resource guards treat producers, and
how to add a framework, is in
`cohesion/.claude/rules/build-system.md`, “Framework producer projects”.

## Validate

```bash
dotnet test libraries/App/Assimalign.Cohesion.App.Runtime/tests/Assimalign.Cohesion.App.Runtime.Tests.csproj
dotnet pack libraries/App/Assimalign.Cohesion.App.Refs/src/Assimalign.Cohesion.App.Refs.csproj
```

[Libraries](../index.md) · [SDKs](../../sdks/index.md)

## Sources

- **Primary source** — `cohesion/libraries/App/README.md`.
- **Kernel membership** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Build logic** — `cohesion/libraries/App/Assimalign.Cohesion.App.targets`.
