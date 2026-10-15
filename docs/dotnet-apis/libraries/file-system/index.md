# FileSystem

Portable file-system contracts with physical, memory, isolated, and aggregate providers.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.FileSystem` | Defines a common file-system contract and named provider composition. | [Overview](assimalign-cohesion-filesystem/index.md) |
| `Assimalign.Cohesion.FileSystem.Aggregate` | Routes a virtual file-system tree across mounted providers. | [Overview](assimalign-cohesion-filesystem-aggregate/index.md) |
| `Assimalign.Cohesion.FileSystem.Globbing` | Matches include and exclude glob patterns over file-system abstractions. | [Overview](assimalign-cohesion-filesystem-globbing/index.md) |
| `Assimalign.Cohesion.FileSystem.InMemory` | Stores files in managed memory for tests and temporary storage. | [Overview](assimalign-cohesion-filesystem-inmemory/index.md) |
| `Assimalign.Cohesion.FileSystem.IsolatedStorage` | Provides file-system access backed by .NET isolated storage. | [Overview](assimalign-cohesion-filesystem-isolatedstorage/index.md) |
| `Assimalign.Cohesion.FileSystem.Physical` | Provides file-system access rooted in an operating-system directory. | [Overview](assimalign-cohesion-filesystem-physical/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 2. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.FileSystem` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.FileSystem.Aggregate` | `Assimalign.Cohesion.FileSystem` (CohesionProjectReference) |
| `Assimalign.Cohesion.FileSystem.Globbing` | `Assimalign.Cohesion.FileSystem` (CohesionProjectReference) |
| `Assimalign.Cohesion.FileSystem.InMemory` | `Assimalign.Cohesion.FileSystem` (CohesionProjectReference) |
| `Assimalign.Cohesion.FileSystem.IsolatedStorage` | `Assimalign.Cohesion.FileSystem` (CohesionProjectReference) |
| `Assimalign.Cohesion.FileSystem.Physical` | `Assimalign.Cohesion.FileSystem` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Assimalign.Cohesion.FileSystem.csproj`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src/Assimalign.Cohesion.FileSystem.Aggregate.csproj`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/Assimalign.Cohesion.FileSystem.Globbing.csproj`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src/Assimalign.Cohesion.FileSystem.InMemory.csproj`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src/Assimalign.Cohesion.FileSystem.IsolatedStorage.csproj`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src/Assimalign.Cohesion.FileSystem.Physical.csproj`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src`.
