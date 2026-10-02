# Assimalign.Cohesion.OpenApi.Integration design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Integration`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Endpoint sources feed description providers without introducing a Web or ApiManager dependency.
Import/export implementations compose serialization and version transforms. Retargeted exports
report losses instead of silently claiming equivalent documents.

An endpoint source over the generated `OpenApiMetadataRegistry` is a trivial adapter. That registry
is internal to the assembly declaring the endpoint source and already combines every annotated
assembly it references, so one endpoint source covers endpoints spread over several libraries.

The shipped Web adapter (`Assimalign.Cohesion.Web.OpenApi`, #152) implements the contract over the
Web route table rather than over the attribute registry: its operations come from the
source-generated endpoint descriptions Web.Api attaches to routes, and its schemas from the
application's System.Text.Json contracts. Those schemas are richer than the flat metadata can spell
(arrays of components, dictionaries, enums, nullable references), so it passes complete model schemas
through the metadata's optional `Schema` members. The provider still assembles the document; the
adapter contributes data only. An application can compose the generated registry beside it as an
extra source.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`,
`Assimalign.Cohesion.OpenApi.Attributes`, `Assimalign.Cohesion.OpenApi.Generation`,
`Assimalign.Cohesion.OpenApi.Serialization`, `Assimalign.Cohesion.OpenApi.Versioning`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/Assimalign.Cohesion.OpenApi.Integration.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src`.
