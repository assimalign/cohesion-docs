# Assimalign.Cohesion.Database.Protocol

The shared mechanism for every Cohesion database model: bounded framing, handshake, session lifecycle, errors, version negotiation, and immutable endpoint family binding.

> **Status:** Partial.

## Reference pages

- **[ProtocolMessageType](protocol-message-type.md)** — Public contract and usage.
- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[ProtocolVersion](protocol-version.md)** — Signature, behavior and validation.
- **[ProtocolCapability](protocol-capability.md)** — Signature, behavior and validation.
- **[ProtocolStartupMessage](protocol-startup-message.md)** — Signature, behavior and validation.
- **[ProtocolReadyMessage](protocol-ready-message.md)** — Signature, behavior and validation.
- **[ProtocolDiagnostic](protocol-diagnostic.md)** — Signature, behavior and validation.
- **[ProtocolDiagnosticsMessage](protocol-diagnostics-message.md)** — Signature, behavior and validation.
- **[ProtocolMessageFamily](protocol-message-family.md)** — Signature, behavior and validation.
- **[ProtocolChannel](protocol-channel.md)** — Signature, behavior and validation.
- **[ProtocolDiagnosticSeverity](protocol-diagnostic-severity.md)** — Values and usage.

The shared mechanism for every Cohesion database model: bounded framing, handshake, session
lifecycle, errors, version negotiation, and immutable endpoint family binding. Model Protocol packages own
their message identifiers and payload codecs.

- **`ProtocolFrameHeader` / `ProtocolFrame`** — five-byte envelope and payload.
- **`ProtocolMessageType`** — core handshake, error, liveness, and termination identifiers.
- **`ProtocolMessageFamily` / `ProtocolChannel`** — immutable endpoint family and validated I/O.
- **`ProtocolFrameReader` / `ProtocolFrameWriter`** — raw stream Create factories, optional
  [borrowed reads and bounded CreateBatched writes](design.md#bounded-batches-and-borrowed-payloads).
- **`ProtocolPayload`** — big-endian integers and length-prefixed UTF-8 string primitives.
- **`ProtocolVersion`** — current version 2.1 negotiation.
- **`ProtocolErrorCode` / `ProtocolException`** — shared error taxonomy and framing failures.

This child root has no dependencies on models, the area root, or transports. The area root
references it. See [design and wire contract](design.md) .

[Parent: Database](../index.md)

## Terminal delivery

Ordinary writers send each complete frame in one Stream write; opt-in batching coalesces complete
frames with unchanged bytes. All five servers complete Output and drain
terminal Errors/capacity refusals under ShutdownDrainTimeout, with a separate bounded peer-close
linger. Clients consume a queued Error before the next request. See
[the contract and TLS limitation](design.md#terminal-delivery-and-frame-writes).

[Protocol counters](design.md#protocol-eventsource) measure batched transport attempts and pooled
buffer ownership, including work before a listener attaches.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/Assimalign.Cohesion.Database.Protocol.csproj`.
