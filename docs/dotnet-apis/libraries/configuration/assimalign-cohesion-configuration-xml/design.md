# Assimalign.Cohesion.Configuration.Xml design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Configuration.Xml`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The parser flattens XML structure into composite keys and keeps XML-specific decryption inside this
package. The shared file-system provider owns file watching and reload behavior. File and stream
registration use the core builder seam.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Configuration.FileSystem`,
`System.Security.Cryptography.Xml`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src/Assimalign.Cohesion.Configuration.Xml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src`.
