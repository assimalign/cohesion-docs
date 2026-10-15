# PUT

PUT inserts or replaces one entry and can require absence or a matching etag.

> **Status:** Implemented.

## Syntax

```syntaxsql
PUT @key @value [ IF { ABSENT | @etag } ]
```

## Supported clauses

| Form | Support | Condition |
|---|---|---|
| No `IF` | Supported | Unconditional upsert |
| `IF ABSENT` | Supported | No visible entry exists |
| `IF @etag` | Supported | The visible entry's etag matches |

## Arguments

`@key` binds a nonempty `byte[]`; `@value` binds a `byte[]`. `@etag` binds a `long` or `int`.
The result is one row with `applied` (Boolean) and `etag` (nullable 64-bit integer).
The affected count is `1` when applied and `0` otherwise.

## Remarks

An applied write returns the new etag. A condition miss returns `applied=false` and the current
etag, or null if no visible entry exists. It makes no mutation and is not an exception.
Unconditional writes always report `applied=true` when they complete successfully.

Etags are writer transaction sequences. A concurrently committed change is a transaction
conflict rather than a conditional miss; see [diagnostics](../diagnostics.md).

## Examples

These forms appear in the command tests. The tests bind `k` and `v` to byte arrays; the
conditional replacement binds `etag` to the etag returned by the first insertion.
Each line is a separate execution.

```text
PUT @k @v
PUT @k @v IF ABSENT
PUT @k @v IF @etag
```

## See also

- **[Commands](index.md)** — grammar and lexical conventions.
- **[GET](get.md)** — read an entry and its etag.
- **[DELETE](delete.md)** — conditional removal.

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Request** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/KeyValuePutRequest.cs`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
