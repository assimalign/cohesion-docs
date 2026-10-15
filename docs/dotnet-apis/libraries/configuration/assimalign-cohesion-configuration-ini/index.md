# Assimalign.Cohesion.Configuration.Ini

Loads INI streams and files into hierarchical configuration paths.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`ConfigurationBuilderExtensions`](configuration-builder-extensions.md)** — type reference.

- **[`ConfigurationIniOptions`](configuration-ini-options.md)** — type reference.

- **[`ConfigurationIniProvider`](configuration-ini-provider.md)** — type reference.

- **[`ConfigurationIniStreamProvider`](configuration-ini-stream-provider.md)** — type reference.

[Configuration](../index.md)

## Scope

Section and key names project onto the same colon-separated path model used by the other providers.
File watching and reload errors delegate to the FileSystem provider layer. Writing INI and multiline
continuations are outside the implemented loading contract.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Configuration.FileSystem`](../../configuration/assimalign-cohesion-configuration-filesystem/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ConfigurationBuilderExtensions` | `src/Extensions/ConfigurationBuilderExtensions.cs` |
| `ConfigurationIniOptions` | `src/ConfigurationIniOptions.cs` |
| `ConfigurationIniProvider` | `src/ConfigurationIniProvider.cs` |
| `ConfigurationIniStreamProvider` | `src/ConfigurationIniStreamProvider.cs` |

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src/Assimalign.Cohesion.Configuration.Ini.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src/Extensions/ConfigurationBuilderExtensions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src/ConfigurationIniOptions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src/ConfigurationIniProvider.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src/ConfigurationIniStreamProvider.cs`.
