# Wire protocol

Key-Value Pair carries text commands and scalar results over its model-owned protocol family.

> **Status:** Implemented. The transaction message identifier is reserved without an implemented payload.

`KeyValueProtocol.Family` is fixed when the endpoint accepts a connection. Protocol version
`1.0` retains the established identifiers, payload bytes, ordering, authentication, and error
codes. The shared Database protocol owns framing, startup, errors, and lifecycle; the model
owns execution and tabular result codecs.

## Exchange

After `Startup`, `Authenticate`, `AuthenticateResponse`, and `Ready`, the client executes
one command at a time. A result-bearing command returns `ResultHeader`, zero or more
`ResultRow` messages, and `ResultComplete`. A plain result returns completion alone.
`Error` ends the exchange without completion.

`ParseFailure` and `ExecutionFailure` leave the session ready. Malformed payloads, unknown
messages, and invalid ordering close it. There is no pipelining or multiplexing.
`Terminate` closes the channel; `Ping` receives `Pong` while ready.

## Envelope and payloads

The frame is a four-byte unsigned payload length, one-byte message type, then the payload.
The maximum payload is `16,777,216` bytes. Fixed integers are big-endian. A string is a
nonnegative signed 32-bit UTF-8 byte length followed by its bytes. Counts are nonnegative
signed 32-bit integers. Payloads contain no padding.

| Identifier | Direction | Payload |
|---|---|---|
| `Execute` (`5`) | Client to server | Statement string; parameter count; repeated parameter name string, encoded-value length, and one scalar component |
| `ResultHeader` (`6`) | Server to client | Column count; repeated name string and one-byte `DatabaseType` |
| `ResultRow` (`7`) | Server to client | One scalar component per column, concatenated without a count prefix |
| `ResultComplete` (`8`) | Server to client | Signed 64-bit affected count; `-1` means not applicable |
| `Transaction` (`9`) | Reserved | No implemented payload |

Parameter names are unique. Each encoded parameter contains exactly one scalar component.
Rows contain exactly one component per header column, including a tagged null where needed.
`PUT` preserves affected counts `1` or `0`; `GET` and `SCAN` use `-1`.
Explicit transaction verbs are absent from this model's command grammar.

## Components used by commands

Each scalar begins with a one-byte `DatabaseType` tag. These components cover the command
operands and the ordinary entry and discovery results:

| Tag | Type | Bytes after the tag |
|---|---|---|
| `0` | `Null` | None |
| `1` | `Boolean` | One byte: `0` or `1` |
| `4` | `Int32` | Four bytes, sign bit XORed, most-significant byte first |
| `5` | `Int64` | Eight bytes, sign bit XORed, most-significant byte first |
| `9` | `String` | Collation byte `0`, then escaped UTF-8 bytes |
| `10` | `Binary` | Escaped binary bytes |

Escaping replaces each `00` byte with `00 FF` and terminates the component with `00 00`.
The signed-integer transformation applies to scalar components, not to the fixed envelope,
count, or completion fields. Reverse the sign-bit XOR to decode an integer component.

The family also defines tags `2`, `3`, `6`, `7`, `8`, and `11` through `16` for other scalar
types in its source wire specification. Their presence does not widen the grammar's accepted
operand types. JavaScript Object Notation (JSON) identities `17` and `18` are not supported
scalar components in this family; unknown tags are rejected.

## Fixed payload example

The wire specification and golden tests encode `ResultComplete` with affected count `42`
as these eight payload bytes, excluding the frame header:

```text
00 00 00 00 00 00 00 2A
```

## See also

- **[Key-Value Pair](index.md)** — engine and client.
- **[Commands](commands/index.md)** — command/result mapping.
- **[Operand types](operand-types.md)** — binding restrictions.
- **[Diagnostics](diagnostics.md)** — stable error codes.

## Sources

- **Wire contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/WIRE-PROTOCOL.md`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Golden payload tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/ProtocolMessageTests.cs`.
