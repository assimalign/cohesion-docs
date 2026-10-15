# Assimalign.Cohesion.IdentityModel design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The root is the family's dependency anchor and references only the base class library. Protocol
contracts and token documents are separate branches. Claim canonicalization preserves provenance
while presenting one vocabulary to resource platforms.

## NativeAOT sample factory migration (#1465)

The family NativeAOT sample now constructs asymmetric verifiers with
`EcdsaJsonWebTokenSignatureVerifier.Create(...)` and `RsaJsonWebTokenSignatureVerifier.Create(...)`.
Rebuild the sample against the updated JWT package; its DESIGN records the factory source and
binary breaks against 10.0.0-preview.1 on decision 15's terms. This cleanup changes no API or
behavior in the IdentityModel root package and preserves the sample's verification checks.
Advisory: not applicable: API shape.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Assimalign.Cohesion.IdentityModel.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src`.
