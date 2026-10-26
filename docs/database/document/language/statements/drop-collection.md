# DROP COLLECTION

Removes a collection, its documents and its indexes in the session's database.

## Syntax

```syntaxsql
DROP COLLECTION [ IF EXISTS ] collection_name [ ; ]
```

## Remarks

Use [DELETE](delete.md) to remove documents while retaining the collection and its index definitions.

Names are case-sensitive and local to the session database. Success reports affected count zero.
IF EXISTS ignores an absent collection; it does not permit dropping a schema-owned collection.
OQL collection DDL runs in an automatic statement transaction. Inside an explicit transaction it
reports CD04105 before mutation and faults that transaction, including conditional no-ops.
Use [ROLLBACK](transactions.md) to recover. Typed C# collection operations retain their existing
transaction contract. Snapshot conflicts are checked before an automatic conditional no-op.

| Code | Meaning |
| --- | --- |
| `CD01002` | Unconditional DROP when the collection does not exist |
| `CD05001` | Blank or invalid name; OQL retains the name span when available |
| `CD04004` | System collection is read-only, including conditional forms |
| `CD03011` | Schema-owned DROP, even with IF EXISTS; DatabaseObjectLockedException |

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION scratch;
CREATE COLLECTION IF NOT EXISTS scratch;
DROP COLLECTION scratch;
DROP COLLECTION IF EXISTS scratch;
```

Each reports zero affected rows. The final state has no scratch collection. An unconditional
repeated DROP reports CD01002 when the collection does not exist.

## See also

[Statements](index.md) · [CREATE COLLECTION](create-collection.md) · [INSERT](insert.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Shared cores and transactions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`, Collection DDL.
- **Execution tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentCollectionDdlTests.cs`.
