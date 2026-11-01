# CertificateAuthorityOptions

The `CertificateAuthorityOptions` type belongs to `Assimalign.Cohesion.SecretStore`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.SecretStore` Assembly: `Assimalign.Cohesion.SecretStore`

## Purpose

`CertificateAuthorityOptions` is the dependency-free declaration captured by
`ISecretStoreApplicationBuilder.AddCertificateAuthority`. It describes how an empty durable store
obtains its first certificate authority; it does not perform certificate or network operations.

## Properties

- **`CommonName`** — sets the self-signed root certificate's subject common name. It defaults to
  `Cohesion SecretStore Certificate Authority` and must not be empty or whitespace.
- **`SelfSeed`** — defaults to `true`. It allows a self-signed development root when no initial
  material is supplied.
- **`InitialCertificate`** — and `InitialPrivateKey` supply paired PEM certificate and PKCS#8 private-key
  material, such as material resolved from deployment parameters.

## Selection and validation

Existing durable authority state takes precedence. On first start, paired initial material wins,
then self-seeding. Initial certificate and key values must be supplied together, any supplied
material must be non-empty, and a configuration that disables self-seeding without supplying
initial material is invalid.

The store issues no intermediate authorities and enrolls with no parent authority.

## Compatibility

Compared with `10.0.0-preview.1`, this type is a source and binary break:
`PlatformEnrollmentEndpoint` and `PlatformCertificate` are removed with the gateway-mediated
Platform enrollment they configured, and `SelfSeedWhenNoPlatform` is renamed `SelfSeed`. The
default and the self-seeding behavior are unchanged.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore/src/CertificateAuthorityOptions.cs`.
- **Design** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore/docs/DESIGN.md`.
