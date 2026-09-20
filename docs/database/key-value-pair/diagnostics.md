# Diagnostics

Key-Value Pair distinguishes invalid commands, conditional misses, execution failures, and protocol failures.

> **Status:** Implemented.

The command parser raises `DatabaseParseException`; it does not assign an OQL-style or
GQL-style diagnostic identifier. The wire uses the shared `ProtocolErrorCode` values.
The typed client preserves the code in `KeyValueClientException.Code` and supplies a
model-specific category in `KeyValueClientException.Kind`.

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
The KEYSPACES catalog surface is read-only.
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
- **Wire codes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolErrorCode.cs`.
- **Client categories** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/KeyValueClientErrorKind.cs`.
- **Client mapping** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/Exceptions/KeyValueClientException.cs`.
- **Read-only conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueIntrospectionTests.cs`.
