# Assimalign.Cohesion.Content.Yaml

Parses YAML documents and emits deterministic YAML through the content contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

The document model, event pipeline, parser, and emitter are separated behind `YamlText`. Scalar
resolution follows the core schema, while writing chooses stable block output and reconstructs
anchors and aliases for shared nodes. Text encoding detection is delegated to the text layer.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Content`](../../content/assimalign-cohesion-content/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Content.Text`](../../content/assimalign-cohesion-content-text/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `YamlDocument` | `src/Nodes/YamlDocument.cs` |
| `YamlEvent` | `src/Events/YamlEvent.cs` |
| `YamlMapping` | `src/Nodes/YamlMapping.cs` |
| `YamlNode` | `src/Nodes/YamlNode.cs` |
| `YamlScalar` | `src/Nodes/YamlScalar.cs` |
| `YamlStream` | `src/Nodes/YamlStream.cs` |
| `YamlText` | `src/YamlText.cs` |
| `YamlCollectionStyle` | `src/Nodes/YamlCollectionStyle.cs` |
| `YamlEventKind` | `src/Events/YamlEvent.cs` |
| `YamlException` | `src/Exceptions/YamlException.cs` |
| `YamlMappingEntry` | `src/Nodes/YamlMapping.cs` |
| `YamlScalarKind` | `src/Nodes/YamlScalarKind.cs` |
| `YamlScalarStyle` | `src/Nodes/YamlScalarStyle.cs` |
| `YamlSequence` | `src/Nodes/YamlSequence.cs` |
| `YamlWriterOptions` | `src/YamlWriterOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Assimalign.Cohesion.Content.Yaml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlDocument.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Events/YamlEvent.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlMapping.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlNode.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlScalar.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlStream.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/YamlText.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlCollectionStyle.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Exceptions/YamlException.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlScalarKind.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlScalarStyle.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Nodes/YamlSequence.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/YamlWriterOptions.cs`.
