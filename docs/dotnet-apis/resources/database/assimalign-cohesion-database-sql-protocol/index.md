# Assimalign.Cohesion.Database.Sql.Protocol

Shared Sql wire messages, codecs and transferred values for engines and remote clients.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Sql.Protocol`. Assembly: `Assimalign.Cohesion.Database.Sql.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql.Protocol`.

This package owns 5 public types formerly shipped in `Assimalign.Cohesion.Database.Sql`.
Both engine and client consume these same contracts and the single model family instance. Update
imports and package references and recompile. Namespaces and assembly identities change without
forwarders; wire bytes, disk formats, limits and validation behavior stay the same.

## Reference pages

- **[Design](design.md)** — Contract ownership and dependencies.
- **[Examples](examples/index.md)** — Codec use without an engine reference.
- **[ProtocolExecuteMessage](protocol-execute-message.md)** — Public wire contract.
- **[ProtocolResultCompleteMessage](protocol-result-complete-message.md)** — Public wire contract.
- **[ProtocolResultHeaderMessage](protocol-result-header-message.md)** — Public wire contract.
- **[SqlProtocol](sql-protocol.md)** — Public wire contract.
- **[SqlProtocolMessageType](sql-protocol-message-type.md)** — Public wire contract.

## Project references

| Reference | Kind |
| --- | --- |
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |

[Engine](../assimalign-cohesion-database-sql/index.md) · [Client](../assimalign-cohesion-database-sql-client/index.md) · [Database](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/docs/OVERVIEW.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/Assimalign.Cohesion.Database.Sql.Protocol.csproj`.
