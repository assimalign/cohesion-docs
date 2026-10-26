# CREATE COLLECTION

Creates a named collection in the session's database.

## Syntax

```syntaxsql
CREATE COLLECTION [ IF NOT EXISTS ] collection_name [ ; ]
```

## Remarks

Names are case-sensitive and local to the session database. Success reports affected count zero.
IF NOT EXISTS preserves an existing collection and its identity and ownership.
OQL collection DDL runs in an automatic statement transaction. Inside an explicit transaction it
reports CD04105 before mutation and faults that transaction, including conditional no-ops.
Use [ROLLBACK](transactions.md) to recover. Typed C# collection operations retain their existing
transaction contract. Snapshot conflicts are checked before an automatic conditional no-op.

| Code | Meaning |
| --- | --- |
| `CD03010` (SQLSTATE 42P07) | Unconditional CREATE when the collection already exists |
| `CD05001` | Blank or invalid name; OQL retains the name span when available |
| `CD04004` | System collection is read-only, including conditional forms |

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION scratch;
CREATE COLLECTION IF NOT EXISTS scratch;
DROP COLLECTION scratch;
DROP COLLECTION IF EXISTS scratch;
```

Each reports zero affected rows. The final state has no scratch collection. An unconditional
repeated CREATE reports CD03010 when the collection already exists.

## See also

[Statements](index.md) · [DROP COLLECTION](drop-collection.md) · [INSERT](insert.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Shared cores and transactions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`, Collection DDL.
- **Execution tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentCollectionDdlTests.cs`.
