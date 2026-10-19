# CREATE COLLECTION

Creates a named collection in the session's database.

## Syntax

```syntaxsql
CREATE COLLECTION [ IF NOT EXISTS ] collection_name [ ; ]
```

## Remarks

Names are case-sensitive and local to the session database. Success reports affected count zero. IF
NOT EXISTS preserves an existing collection and its identity and ownership. The operation uses the
same core as the session API and participates in an explicit transaction or an automatic
transaction. CREATE is visible to later statements in its transaction; rollback removes it. DROP
rollback restores the collection, documents and indexes together. Snapshot conflicts are checked
before a conditional no-op. A failed collection statement aborts an explicit transaction under the
Documents failed-statement rule.

| Code | Meaning |
| --- | --- |
| `COHDOC010` (SQLSTATE 42P07) | Unconditional CREATE when the collection already exists |
| `COHDOC001` | Blank or invalid name; OQL retains the name span when available |
| `COHDOC004` | System collection is read-only, including conditional forms |

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION scratch;
CREATE COLLECTION IF NOT EXISTS scratch;
DROP COLLECTION scratch;
DROP COLLECTION IF EXISTS scratch;
```

Each reports zero affected rows. The final state has no scratch collection. An unconditional
repeated CREATE reports COHDOC010 when the collection already exists.

## See also

[Statements](index.md) · [DROP COLLECTION](drop-collection.md) · [INSERT](insert.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Shared cores and transactions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`, Collection DDL.
- **Execution tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentCollectionDdlTests.cs`.
