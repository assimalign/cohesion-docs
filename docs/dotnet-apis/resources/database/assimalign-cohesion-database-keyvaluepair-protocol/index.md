# Assimalign.Cohesion.Database.KeyValuePair.Protocol

Shared KeyValuePair wire messages, codecs and transferred values for engines and remote clients.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`. Assembly: `Assimalign.Cohesion.Database.KeyValuePair.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`.

This package owns 5 public types formerly shipped in `Assimalign.Cohesion.Database.KeyValuePair`.
Both engine and client consume these same contracts and the single model family instance. Update
imports and package references and recompile. Namespaces and assembly identities change without
forwarders; wire bytes, disk formats, limits and validation behavior stay the same.

## Reference pages

- **[Design](design.md)** — Contract ownership and dependencies.
- **[Examples](examples/index.md)** — Codec use without an engine reference.
- **[KeyValueProtocol](key-value-protocol.md)** — Public wire contract.
- **[KeyValueProtocolMessageType](key-value-protocol-message-type.md)** — Public wire contract.
- **[ProtocolExecuteMessage](protocol-execute-message.md)** — Public wire contract.
- **[ProtocolResultCompleteMessage](protocol-result-complete-message.md)** — Public wire contract.
- **[ProtocolResultHeaderMessage](protocol-result-header-message.md)** — Public wire contract.

## Project references

| Reference | Kind |
| --- | --- |
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |

[Engine](../assimalign-cohesion-database-keyvaluepair/index.md) · [Client](../assimalign-cohesion-database-keyvaluepair-client/index.md) · [Database](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/docs/OVERVIEW.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/src/Assimalign.Cohesion.Database.KeyValuePair.Protocol.csproj`.
