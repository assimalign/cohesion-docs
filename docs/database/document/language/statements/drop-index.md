# DROP INDEX

DROP INDEX removes a named index from an explicitly identified collection.

## Syntax

```syntaxsql
DROP INDEX index_name ON collection_name [ ; ]
```

## Supported clauses

| Construct | Support | Requirement |
|---|---|---|
| `ON collection_name` | Supported | Required; omitting `ON` is a syntax error |

## Arguments

- **`index_name`** — the identifier of the collection's index to remove.
- **`collection_name`** — the collection in the current session database.

## Remarks

The statement removes the visible index definition in its transaction. Subsequent physical plans
stop selecting it; older snapshots retain their defined visibility. The engine enforces collection
ownership under the database writer lock. Schema-owned collections reject the operation, and
reserved system collections are read-only.

The statement uses an active session transaction or an automatic statement transaction. Successful
execution returns a command `QueryResult` with `Success` and an affected count of zero.

## Examples

The parser tests use this statement:

```sql
DROP INDEX ix_postal ON people
```

## See also

[Statements](index.md) and [CREATE INDEX](create-index.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Ddl.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlIndexDdlParserTests.cs`.
