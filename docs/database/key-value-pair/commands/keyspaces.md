# KEYSPACES

KEYSPACES describes the current database's single implicit key space.

> **Status:** Implemented.

## Syntax

```syntaxsql
KEYSPACES
```

## Supported clauses

`KEYSPACES` accepts no clauses, parameters, or database selector.

## Arguments

There are no arguments. The result has one row with `database_name`, `keyspace_id`,
`entry_space_format_version`, `primary_index_name`, `index_kind`, and `is_unique`.
See [key-space discovery](../key-space-discovery.md) for their types and meanings.

## Remarks

The row exists even before user entries have been written. The key-space id is database-local,
not a server-wide identity. Each execution captures fresh catalog metadata, even inside an
explicit snapshot transaction; already returned rows keep their original capture.

The surface is read-only. A byte parameter whose contents spell `KEYSPACES` is still an
ordinary user key, not a catalog mutation.

## Examples

The introspection corpus executes both uppercase and lowercase forms. For the empty database
`kv`, it verifies the row `kv`, `1`, `1`, `key`, `BTree`, true.

```text
KEYSPACES
```

## See also

- **[Commands](index.md)** — grammar and lexical conventions.
- **[Key-space discovery](../key-space-discovery.md)** — metadata column reference.
- **[Diagnostics](../diagnostics.md)** — catalog mutation refusal.

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueIntrospectionTests.cs`.
