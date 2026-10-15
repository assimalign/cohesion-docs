# GET

GET returns the visible entry for a single key in the session's database.

> **Status:** Implemented.

## Syntax

```syntaxsql
GET @key
```

## Supported clauses

`GET` accepts one key parameter and no additional clauses.

## Arguments

`@key` binds a nonempty `byte[]`. The command returns columns `key` (binary), `value` (binary),
and `etag` (64-bit integer), in that order.

## Remarks

An absent entry produces zero rows. A visible entry produces one row. Reads use the primary
index under the command's transaction snapshot. The etag identifies the entry's visible version
and can be passed to a later conditional `PUT` or `DELETE`.

## Examples

The command corpus binds `k` to the encoded bytes of `user:1` after writing that key:

```text
GET @k
```

## See also

- **[Commands](index.md)** — grammar and lexical conventions.
- **[PUT](put.md)** — write an entry.
- **[Operand types](../operand-types.md)** — parameter binding.

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Request** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/KeyValueGetRequest.cs`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
