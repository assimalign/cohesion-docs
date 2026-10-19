# Diagnostics

Key-Value Pair distinguishes invalid commands, conditional misses, execution failures, and protocol failures.

> **Status:** Implemented.

The command parser raises `DatabaseParseException` with a coded diagnostic. The wire uses the shared `ProtocolErrorCode` values.
The typed client preserves the code in `KeyValueClientException.Code` and supplies a
model-specific category in `KeyValueClientException.Kind`.

## Diagnostic codes

| Code | Meaning |
| --- | --- |
| `COHKV001` | Empty command |
| `COHKV002` | Attempted catalog mutation |
| `COHKV003` | Unknown command verb |
| `COHKV004` | Request-constructor validation failed |
| `COHKV005` | Invalid PUT arity |
| `COHKV006` | PUT requires IF |
| `COHKV007` | Invalid DELETE arity |
| `COHKV008` | DELETE requires IF |
| `COHKV009` | SCAN clause operand missing |
| `COHKV010` | Repeated SCAN clause |
| `COHKV011` | Unknown SCAN clause |
| `COHKV012` | Byte operand required |
| `COHKV013` | Integer etag required |
| `COHKV014` | Nonnegative LIMIT literal required |
| `COHKV015` | Nonnegative integer LIMIT parameter required |
| `COHKV016` | Parameter reference required |
| `COHKV017` | Parameter value missing |
| `COHKV018` | Command arity mismatch |
| `COHKV019` | Execution failure without an engine diagnostic |

Parser refusals use ParseFailure and leave the session ready. Execution errors without an engine
code use COHKV019. Wire Error text is CODE: message, with the existing layout; Key-Value does not
negotiate the core Diagnostics capability. For example GET without its operand reports COHKV018;
PUT KEYSPACES @value reports COHKV002. Conditional misses remain successful outcomes.

## Command failures

| Condition | Surface | Effect |
|---|---|---|
| Unknown verb, bad token count, or unsupported clause | `DatabaseParseException` / `ParseFailure` (`5`) | Command does not execute; session remains ready |
| Missing parameter or wrong runtime operand type | `DatabaseParseException` / `ParseFailure` (`5`) | Command does not execute; session remains ready |
| Duplicate scan clause or incompatible scan bounds | `DatabaseParseException` / `ParseFailure` (`5`) | Command does not execute; session remains ready |
| Conditional write misses | `applied=false` or affected count `0` | Successful outcome without mutation |
| Concurrent committed write to the same key | Retryable transaction abort / `ExecutionFailure` (`6`) | Transaction conflicts; session remains usable |
| Malformed frame, message, or ordering | `ProtocolViolation` (`8`) | Connection is broken |

The engine also exposes `DatabaseTransactionDeadlockException` for retryable deadlocks and
`DatabaseNotFoundException` when opening absent storage. Kernel exceptions are translated at
the model boundary.

## Read-only catalog diagnostic

Attempts such as `PUT KEYSPACES @value` or `delete keyspaces` produce the stable message:

```text
COHKV002: The KEYSPACES catalog surface is read-only.
```

This check concerns the command text. A parameter-bound key containing those bytes remains
ordinary data.

## Client categories

`KeyValueClientErrorKind` defines `Internal`, `ConnectionFailure`, `AuthenticationFailure`,
`NotAuthorized`, `ParseFailure`, `ExecutionFailure`, `TransactionAborted`,
`ProtocolViolation`, `Unavailable`, and `MalformedResult`.

`KeyValueClientException.ConnectionUsable` is true for `ParseFailure`, `ExecutionFailure`,
`TransactionAborted`, `NotAuthorized`, and `MalformedResult`. It is false for the other
categories. Conditional misses never become this exception.

The shared wire enum also contains `Internal` (`0`), `UnsupportedVersion` (`1`),
`AuthenticationFailed` (`2`), `NotAuthorized` (`3`), `DatabaseNotFound` (`4`),
`TransactionAborted` (`7`), and `Unavailable` (`9`). A declared shared code does not imply
that every model path emits it; the key-value write-conflict contract uses `ExecutionFailure`.

## See also

- **[Key-Value Pair](index.md)** — engine behavior.
- **[Operand types](operand-types.md)** — accepted bindings.
- **[Unsupported features](unsupported.md)** — excluded grammar and engine features.

## Sources

- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Internal/KeyValueCommandParser.cs`.
- **Engine errors** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/DESIGN.md`.
- **Wire codes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/Exceptions/ProtocolErrorCode.cs`.
- **Client categories** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/KeyValueClientErrorKind.cs`.
- **Client mapping** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/Exceptions/KeyValueClientException.cs`.
- **Read-only conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueIntrospectionTests.cs`.
