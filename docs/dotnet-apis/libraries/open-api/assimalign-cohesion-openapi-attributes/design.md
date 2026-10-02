# Assimalign.Cohesion.OpenApi.Attributes design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Attributes`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Attributes describe operations and schemas, while flat metadata records form the seam to generation.
The mapper reports invalid combinations. Compile-time discovery lives in the separate analyzer
project, keeping discovery out of the runtime model.

## Pass-through schemas (#152)

The flat vocabulary is the attributes' vocabulary, and it cannot say what a serialization contract
says: an array of a component, a dictionary, an enum, a nullable reference, a nested inline object.
A producer that already holds a complete schema therefore passes it through instead of flattening it:
`OpenApiParameterMetadata`, `OpenApiRequestBodyMetadata`, `OpenApiResponseMetadata` and
`OpenApiSchemaMetadata` each have an optional `Schema` (`OpenApiSchema?`). When it is set, generation
places that model schema as it is and ignores the flat type, format, reference or property fields
beside it.

This is the one place the metadata holds a model object, and it is deliberately narrow:

- **Only runtime producers set it.** The Web OpenAPI adapter
  ([`Assimalign.Cohesion.Web.OpenApi`](../../../resources/web/assimalign-cohesion-web-openapi/index.md))
  derives schemas from System.Text.Json contracts with `JsonSchemaExporter`. The attribute mapper and
  the source generator never set it, so their output, and the plain object initializers the generator
  emits, are unchanged.
- **It is optional and additive.** Existing producers and consumers compile and behave as before.
- **It keeps one assembler.** The schema still reaches the document through the description provider
  and the generator; the producer does not patch a generated document afterwards, which would split
  document assembly across packages.

A producer that passes schemas through writes them for the line it targets, since the model's writer
adapts nullability and the 3.1 vocabulary per line but cannot know a producer's intent: the Web
adapter builds its source per document line.

## The provider contract: metadata across assemblies (#1169)

An application's annotated endpoints are usually spread over several assemblies, and each assembly's
metadata is generated in that assembly's own compile. Two public, hand-written types carry it across:

- **`IOpenApiMetadataProvider`** (`src/Abstractions/`) — the operations, schemas, tags, and security
  schemes one assembly contributes.
- **`OpenApiMetadataProviderAttribute`** (`src/Attributes/`) —
  `[assembly: OpenApiMetadataProvider(typeof(T))]` advertises a provider type to every compilation
  that references the assembly.

The source generator implements the interface once per annotated assembly, as a public provider
class with an assembly-unique name, and applies the attribute for that implementation. In each
referencing compilation it reads the advertised attributes from metadata at compile time and emits an
internal `OpenApiMetadataRegistry` that constructs every provider directly: referenced assemblies
first, ordered by assembly name, then the compilation's own. Nothing is discovered at run time, and
trimming keeps each provider. Because each registry is internal, any number of annotated assemblies
compose without CS0433 or CS0436; the one exception is `InternalsVisibleTo`, where a friend assembly
that names the registry gets CS0436 and still binds its own registry, which is the complete one.

The attribute is not generator-only: an assembly can advertise a hand-written provider the same way.
The provider type must be declared in the assembly that carries the attribute and must be a public,
non-abstract class that is not an open generic type, implements `IOpenApiMetadataProvider`, and has a
public parameterless constructor; the generator skips a provider that does not qualify and reports
warning `OPENAPIGEN0001`.

The contract lives here, not in a package of its own, because the generator ships here: every
compilation that runs the generator references this package, so the code it emits always compiles
against the contract.

## The package carries the source generator

The project declares `CohesionAnalyzerReference` for `Assimalign.Cohesion.OpenApi.SourceGeneration`,
so the generator DLL ships in this package at `analyzers/dotnet/cs/` and runs in every project that
references the package, directly or through `OpenApi.Generation` or `OpenApi.Integration`. Shipping
the two together means the generator cannot version apart from what its output compiles against, the
metadata records and the provider contract: renaming or reshaping one of them is a change to the
generator's emitted code in the same package.

The alternatives were rejected: carrying the generator in `OpenApi.Generation` (an assembly that only
declares annotated endpoints has no reason to reference Generation, so its annotations would compile
with no registry), a package of its own (no analyzer in the repository ships alone, and an opt-in
generator lets a project apply the attributes and silently emit nothing), and a shared framework's
`CohesionFrameworkAnalyzer` entry (OpenApi is in no framework).

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi` and the
`Assimalign.Cohesion.OpenApi.SourceGeneration` analyzer. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Assimalign.Cohesion.OpenApi.Attributes.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src`.

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.OpenApi.SourceGeneration/docs/DESIGN.md`.
