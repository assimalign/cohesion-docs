# Assimalign.Cohesion.Sdk.Tasks design

The base task assembly owns the SDK's build-time generation and validation, and no application-model behavior.

> **Status:** Implemented.

## Boundaries

The base SDK is the Cohesion equivalent of `Microsoft.NET.Sdk`: defaults, framework registrations,
strongly typed settings, name-only project references, and pin validation. It generates no
application-model source, image index, OCI archive, or application image, and no other SDK in the
family does either: application-model composition is code.

## Strongly typed settings

`CohesionAppSettingsClass` is the sole opt-in. The target fingerprints the class, namespace, inputs,
and task assembly and places outputs under the target-framework intermediate directory. Generated
types are public and nullable-aware, and binding is reflection-free.

## SDK pin agreement

The task validates agreement rather than imposing the repository's canonical version, so local
identities such as `10.0.0-preview.1.local` are valid when the pins agree.

## Compatibility

Visual Studio compatibility tests load the shipped props and targets with .NET Framework MSBuild to
reject APIs unavailable in that host.

[Assembly](index.md) · [SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Assimalign.Cohesion.Sdk.Tasks.csproj`.
