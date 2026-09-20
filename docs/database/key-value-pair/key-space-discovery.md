# Key-space discovery

KEYSPACES exposes catalog metadata for the receiving session's implicit key space.

> **Status:** Implemented.

The command returns one row, including for a database with no entries. Its typed equivalent
is `KeyValueKeySpacesRequest`, executed through the session interface. There is no named
key-space registry or database selector in the command.

## Result columns

Columns appear in this order:

| Column | Type | Meaning |
|---|---|---|
| `database_name` | `String` | The receiving session's database |
| `keyspace_id` | `Int64` | Catalog identity of the implicit key space; currently `1` |
| `entry_space_format_version` | `Int32` | Persisted entry-format marker |
| `primary_index_name` | `String` | Primary index name; currently `key` |
| `index_kind` | `String` | Catalog index kind; currently `BTree` |
| `is_unique` | `Boolean` | Whether the primary index enforces unique keys |

The key-space id is scoped by `database_name`. The same id in another database does not
identify the same key space. Physical index page locations and schema-ownership columns are
not part of the result.

## Metadata visibility

Every command captures the format marker and index registrations together. Returned rows
remain fixed; a later command captures current metadata even inside an explicit snapshot
transaction. Catalog publications are self-committing and separate from entry visibility
under multiversion concurrency control (MVCC).

Discovery does not insert metadata records into the user key space. `PUT KEYSPACES ...` and
`DELETE KEYSPACES ...` fail with `DatabaseParseException` and the stable message
`The KEYSPACES catalog surface is read-only.` A bound byte key containing `KEYSPACES`
continues to be ordinary data.

## Example

The introspection test executes this on the empty database `kv`:

```text
KEYSPACES
```

It asserts one row with `database_name=kv`, `keyspace_id=1`, `entry_space_format_version=1`,
`primary_index_name=key`, `index_kind=BTree`, and `is_unique=true`; an ordinary `SCAN`
after discovery still has no rows.

## See also

- **[Key-Value Pair](index.md)** — engine and catalog scope.
- **[KEYSPACES](commands/keyspaces.md)** — command reference.
- **[Diagnostics](diagnostics.md)** — read-only diagnostic.

## Sources

- **Contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Design** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/DESIGN.md`.
- **Conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueIntrospectionTests.cs`.
