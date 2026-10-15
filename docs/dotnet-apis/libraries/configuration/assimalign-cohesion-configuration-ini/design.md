# Assimalign.Cohesion.Configuration.Ini design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Configuration.Ini`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Section and key names project onto the same colon-separated path model used by the other providers.
File watching and reload errors delegate to the FileSystem provider layer. Writing INI and multiline
continuations are outside the implemented loading contract.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Configuration.FileSystem`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src/Assimalign.Cohesion.Configuration.Ini.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/src`.
