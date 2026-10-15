# Configuration

A shared configuration model with command-line, environment, and file-format providers.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Configuration` | Builds configuration from providers into keys, values, sections, and snapshots. | [Overview](assimalign-cohesion-configuration/index.md) |
| `Assimalign.Cohesion.Configuration.CommandLine` | Loads process arguments into the Cohesion configuration model. | [Overview](assimalign-cohesion-configuration-commandline/index.md) |
| `Assimalign.Cohesion.Configuration.EnvironmentVariables` | Loads filtered environment variables as configuration entries. | [Overview](assimalign-cohesion-configuration-environmentvariables/index.md) |
| `Assimalign.Cohesion.Configuration.FileSystem` | Shares file access, watching, and reload behavior between configuration formats. | [Overview](assimalign-cohesion-configuration-filesystem/index.md) |
| `Assimalign.Cohesion.Configuration.Ini` | Loads INI streams and files into hierarchical configuration paths. | [Overview](assimalign-cohesion-configuration-ini/index.md) |
| `Assimalign.Cohesion.Configuration.Json` | Loads JSON streams and files into configuration entries. | [Overview](assimalign-cohesion-configuration-json/index.md) |
| `Assimalign.Cohesion.Configuration.Xml` | Loads XML documents into the common configuration model. | [Overview](assimalign-cohesion-configuration-xml/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 2. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Configuration` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.Configuration.CommandLine` | `Assimalign.Cohesion.Configuration` (CohesionProjectReference) |
| `Assimalign.Cohesion.Configuration.EnvironmentVariables` | `Assimalign.Cohesion.Configuration` (CohesionProjectReference) |
| `Assimalign.Cohesion.Configuration.FileSystem` | `Assimalign.Cohesion.Configuration` (CohesionProjectReference), `Assimalign.Cohesion.FileSystem` (CohesionProjectReference) |
| `Assimalign.Cohesion.Configuration.Ini` | `Assimalign.Cohesion.Configuration.FileSystem` (CohesionProjectReference) |
| `Assimalign.Cohesion.Configuration.Json` | `Assimalign.Cohesion.Configuration.FileSystem` (CohesionProjectReference) |
| `Assimalign.Cohesion.Configuration.Xml` | `Assimalign.Cohesion.Configuration.FileSystem` (CohesionProjectReference), `System.Security.Cryptography.Xml` (CohesionPackageReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Assimalign.Cohesion.Configuration.csproj`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src/Assimalign.Cohesion.Configuration.CommandLine.csproj`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src/Assimalign.Cohesion.Configuration.EnvironmentVariables.csproj`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src/Assimalign.Cohesion.Configuration.FileSystem.csproj`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src/Assimalign.Cohesion.Configuration.Ini.csproj`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src/Assimalign.Cohesion.Configuration.Json.csproj`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src/Assimalign.Cohesion.Configuration.Xml.csproj`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Xml/src`.
