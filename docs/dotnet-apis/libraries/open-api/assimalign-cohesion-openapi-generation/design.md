# Assimalign.Cohesion.OpenApi.Generation design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Generation`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Generation consumes explicit input and target-version options. Metadata can come from generated
registries or attribute mapping, so document assembly does not require runtime reflection discovery.
The resulting document remains a normal version-aware model.

`OpenApiGenerationInput` is the seam: the source generator emits a registry of the metadata records,
and this pipeline consumes them. The registry is internal to each assembly and already combines every
annotated assembly that assembly references, so one input covers an application whose endpoints span
several libraries.

## Pass-through schemas (#152)

A parameter, request body, response or schema component whose metadata carries a complete `Schema`
(see [Attributes](../assimalign-cohesion-openapi-attributes/design.md#pass-through-schemas-152)) is
placed as it is: the parameter's schema, the media type's schema, or the component, with the flat
fields beside it ignored. Generation does not copy or rewrite it, so a producer that wants a schema in
several documents supplies one instance per document; the Web OpenAPI adapter builds a fresh source,
and fresh schemas, per document build. Structural schema inference from CLR types stays out of scope:
a body schema is referenced by component name, and the matching `[OpenApiSchema]` model produces the
component, or a producer passes a complete schema through.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`,
`Assimalign.Cohesion.OpenApi.Attributes`. The [overview](index.md#dependencies) distinguishes
project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src/Assimalign.Cohesion.OpenApi.Generation.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src`.
