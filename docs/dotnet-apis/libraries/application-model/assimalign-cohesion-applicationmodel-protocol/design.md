# Assimalign.Cohesion.ApplicationModel.Protocol design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Protocol`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Decisions

- **P1 — The wire form is stable** — JSON keys and values are camel case with enum names
  (`"kind": "ReadWrite"`): a publication's `snapshots` and `lease`, a result's `status`, `output`,
  `reason`, and `state`, and a snapshot's `action` and `value`. The internal source-generated
  context is `ApplicationProtocolJsonContext`.
- **P2 — Kind is a capability, operation is one invocation** — `ApplicationActionKind` is a flags
  set (`Read`, `Write`, `ReadWrite`) describing what an action accepts.
  `ApplicationActionContext.Operation` is the non-flags `ApplicationActionOperation`, so an
  invocation requests exactly one operation.
- **P3 — One copy of each type** — Both runtimes reference this assembly instead of compiling its
  types. `ActionName` validates with the identity root's shared name grammar.
- **P4 — Errors** — A serializer throws the sealed `ApplicationContentException` for content that is
  not a value of the requested type; a control plane reports it as `Rejected`. Converters throw
  `JsonException` for malformed wire content.

## Content serialization

`JsonApplicationContentSerializer.Create(...)` writes JSON through source-generated contexts only,
so content handling stays NativeAOT-safe. An area puts its own context first, then the contexts the
application's added actions came with. A platform stores snapshots as opaque bytes and needs no
area's types.

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Protocol/docs/DESIGN.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
