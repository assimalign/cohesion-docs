# Assimalign.Cohesion.Content.Exe design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Exe`.

> **Status:** Not yet implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`IExecutableFile` declares invocation but the result type and implementation remain scaffolding. The
design establishes no executable parser, process-launch lifecycle, or meaningful invocation error
model. The empty test does not demonstrate execution support.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Content.Binary`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src/Assimalign.Cohesion.Content.Exe.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src`.
