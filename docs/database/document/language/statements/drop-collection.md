# DROP COLLECTION

Removes a collection, its documents and its indexes in the session's database.

## Syntax

```syntaxsql
DROP COLLECTION [ IF EXISTS ] collection_name [ ; ]
```

## Remarks

Names are case-sensitive and local to the session database. Success reports affected count zero.
IF EXISTS ignores an absent collection; it does not permit dropping a schema-owned collection.
The operation uses the same core as the session API and participates in an explicit transaction
or an automatic transaction. CREATE is visible to later statements in its transaction; rollback
removes it. DROP rollback restores the collection, documents and indexes together.
Snapshot conflicts are checked before a conditional no-op. A failed collection statement aborts
an explicit transaction under the Documents failed-statement rule.

| Code | Meaning |
| --- | --- |
| `COHDOC002` | Unconditional DROP when the collection does not exist |
| `COHDOC001` | Blank or invalid name; OQL retains the name span when available |
| `COHDOC004` | System collection is read-only, including conditional forms |
| `COHDOC011` | Schema-owned DROP, even with IF EXISTS; DatabaseObjectLockedException |

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION scratch;
CREATE COLLECTION IF NOT EXISTS scratch;
DROP COLLECTION scratch;
DROP COLLECTION IF EXISTS scratch;
```

Each reports zero affected rows. The final state has no scratch collection.
An unconditional repeated DROP reports COHDOC002 when the collection does not exist.

## See also

[Statements](index.md) · [CREATE COLLECTION](create-collection.md) · [INSERT](insert.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Shared cores and transactions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`, Collection DDL.
- **Execution tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentCollectionDdlTests.cs`.
