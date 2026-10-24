# DROP INDEX

DROP INDEX removes a named index from an explicitly identified collection.

## Syntax

```syntaxsql
DROP INDEX [ IF EXISTS ] index_name ON collection_name [ ; ]
```

## Supported clauses

| Construct | Support | Requirement |
|---|---|---|
| `ON collection_name` | Supported | Required; omitting `ON` is a syntax error |

## Arguments

- **`index_name`** — the identifier of the collection's index to remove.
- **`collection_name`** — the collection in the current session database.

## Remarks

IF EXISTS makes an absent index a no-op. Snapshot conflicts are checked before no-ops.
An existing schema-owned index still refuses removal; an absent conditional DROP is a no-op
even in a schema-owned collection.

The statement removes the visible index definition in its transaction. Subsequent physical plans
stop selecting it; older snapshots retain their defined visibility. The engine enforces collection
ownership under the database writer lock. Schema-owned collections reject the operation, and
reserved system collections are read-only.

The statement uses an active session transaction or an automatic statement transaction. Successful
execution returns a command `DatabaseResult` with `Success` and an affected count of zero.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD01025 | Index does not exist and IF EXISTS was omitted | — |

The engine raises DatabaseException with the code in Diagnostics and the original
DocumentCatalogException. A failure aborts an explicit transaction; roll it back before reuse.
In autocommit the next statement can run. Wire ExecutionFailure preserves the connection.

## Examples

The parser tests use this statement:

```sql
DROP INDEX IF EXISTS ix_postal ON people
```

Success has zero affected documents, whether the index existed or not.
With an existing `people` collection and no `ix_postal`, `DROP INDEX ix_postal ON people`
fails CD01025. Adding IF EXISTS returns Success and zero affected documents.

## See also

[Statements](index.md) and [CREATE INDEX](create-index.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Ddl.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlIndexDdlParserTests.cs`.
