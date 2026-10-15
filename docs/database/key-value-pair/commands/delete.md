# DELETE

DELETE removes a visible entry, optionally requiring a matching etag.

> **Status:** Implemented.

## Syntax

```syntaxsql
DELETE @key [ IF @etag ]
```

## Supported clauses

| Clause | Support | Meaning |
|---|---|---|
| `IF @etag` | Supported | Delete only the visible version with the supplied etag |

## Arguments

`@key` binds a nonempty `byte[]`. `@etag` binds a `long` or `int`.
The plain result contains an affected count, without a result set.

## Remarks

The affected count is `1` for a deletion, or `0` when the key has no visible entry or the
condition misses. A condition miss is not an error. Concurrent committed changes can still
produce a retryable execution conflict.

## Examples

The command tests first use a stale etag and observe affected count `0`, then the current etag
and observe `1`. A subsequent unconditional deletion of the missing key returns `0`.
Each line below is a separate command from that corpus.

```text
DELETE @k IF @etag
DELETE @k
```

## See also

- **[Commands](index.md)** — grammar and lexical conventions.
- **[PUT](put.md)** — conditional writes.
- **[Diagnostics](../diagnostics.md)** — misses and conflicts.

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Request** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/KeyValueDeleteRequest.cs`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
