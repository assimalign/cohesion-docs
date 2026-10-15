# Assimalign.Cohesion.Configuration.Xml

Loads XML documents into the common configuration model.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Configuration](../index.md)

## Scope

The parser flattens XML structure into composite keys and keeps XML-specific decryption inside this
package. The shared file-system provider owns file watching and reload behavior. File and stream
registration use the core builder seam.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Configuration.FileSystem`](../../configuration/assimalign-cohesion-configuration-filesystem/index.md) | `CohesionProjectReference` |
| `System.Security.Cryptography.Xml` | `CohesionPackageReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ConfigurationBuilderExtensions` | `src/Extensions/ConfigurationBuilderExtensions.cs` |
| `ConfigurationXmlOptions` | `src/ConfigurationXmlOptions.cs` |
| `ConfigurationXmlProvider` | `src/ConfigurationXmlProvider.cs` |
| `ConfigurationXmlStreamProvider` | `src/ConfigurationXmlStreamProvider.cs` |
| `XmlDocumentDecryptor` | `src/XmlDocumentDecryptor.cs` |

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src/Assimalign.Cohesion.Configuration.Xml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src/Extensions/ConfigurationBuilderExtensions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src/ConfigurationXmlOptions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src/ConfigurationXmlProvider.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src/ConfigurationXmlStreamProvider.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src/XmlDocumentDecryptor.cs`.
