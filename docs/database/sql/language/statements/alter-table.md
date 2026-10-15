# ALTER TABLE (Cohesion SQL)

Adds or removes a column or supported constraint on an existing stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
ALTER TABLE [ schema_name . ] table_name
{
    ADD [ COLUMN ] column_definition
  | DROP [ COLUMN ] column_name
  | ADD table_constraint
  | DROP CONSTRAINT constraint_name
}
[ ; ]
```

## Arguments

- **`table_name`** — The stored table being changed.
- **`column_definition`** — A column definition from [CREATE TABLE](create-table.md#syntax).
- **`table_constraint`** — A supported table constraint from [CREATE TABLE](create-table.md#syntax).
- **`column_name` or `constraint_name`** — The existing object to remove.

## Remarks

The measured subset includes literal defaults on added columns. Old rows resolve a missing trailing
field from persisted catalog metadata; their bytes and multi-version concurrency control (MVCC)
stamps are unchanged. An explicitly stored `NULL` remains `NULL`. A later update writes the full row.

| Addition | Existing rows | Later insert omitting the column |
| --- | --- | --- |
| Nullable, no default or `DEFAULT NULL` | `NULL` | `NULL` |
| Valid literal default | Default value | Default value |
| `NOT NULL`, non-null default | Default value | Default value |
| `NOT NULL`, no non-null default | Rejected if currently committed rows exist | Must supply a value if addition succeeds on an empty table |

The emptiness check uses current committed rows. Historical versions visible only to an older
snapshot can retain a missing `NULL` field after a default-free addition to a currently empty table.

Defaults must fit destination bounds. Nonliteral defaults, invalid conversions, and out-of-range
values fail before schema publication. The complete definition is published atomically; a failed
addition leaves schema, rows, and catalog unchanged. Defaults survive catalog reopen.

Already bound plans retain their immutable definition. Statements planned after the change use
the new definition even when an older row snapshot selects earlier versions. Ordinary table schemas
are not pinned to `BEGIN`; system-view catalog snapshots have their own contract.

DDL is self-committing and fails inside explicit transactions with `COHSQLT003`. Session alteration
of schema-owned tables is refused. Dropping a column rewrites rows because storage is positional.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
ALTER TABLE t ADD COLUMN extra INT DEFAULT 7;
SELECT id, name, age, extra FROM t ORDER BY id;
INSERT INTO t (id, name, age) VALUES (4, 'new', 1);
ALTER TABLE t ADD CONSTRAINT positive CHECK(extra > 0);
ALTER TABLE t DROP CONSTRAINT positive;
ALTER TABLE t DROP COLUMN extra;
```

All three original rows and the omitted-column insert read `7` for `extra` before it is dropped.
Each statement is a separate request.

## See also

[CREATE TABLE](create-table.md) · [Constraints](../constraints.md) · [System views](../system-views/index.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAddColumnExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`

