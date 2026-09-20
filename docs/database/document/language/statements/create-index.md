# CREATE INDEX

CREATE INDEX builds one named nonunique index over a document path in a collection.

## Syntax

```syntaxsql
CREATE INDEX index_name ON collection_name ( document_path ) [ ; ]
```

## Supported clauses

| Construct | Support | Requirement |
|---|---|---|
| `ON collection_name` | Supported | Required collection target |
| `( document_path )` | Supported | Exactly one path |
| `UNIQUE` | Recognized, not supported (`COHDBL001`) | Indexes are nonunique |

## Arguments

- **`index_name`** — the index identifier, using ordinary identifier quoting and case preservation.
- **`collection_name`** — the target collection in the session's database.
- **`document_path`** — a path relative to each document, including nested properties, integer
  array subscripts, or bracket-string properties; no iteration alias is introduced.

## Remarks

Index data definition language (DDL) follows the query pipeline. Creation populates existing
documents in the statement's transaction, and later document writes maintain the index atomically.
The engine takes the database's exclusive writer lock and enforces collection ownership.
Schema-owned collections reject this operation. Reserved system collections are read-only.

The catalog retains a canonical path without losing property-name punctuation or case. The physical
planner can choose indexes for equality or range predicates and still reapplies the full predicate.
A successful statement returns a command `QueryResult` with `Success` and zero affected documents.

## Examples

The index parser tests cover nested array and quoted-property paths:

```sql
CREATE INDEX ix_postal ON people (addresses[0]['postal-code']);
```

```sql
create index "ix city" on "person collection" ("home address".city);
```

## See also

[Statements](index.md), [DROP INDEX](drop-index.md),
and [Paths and arrays](../expressions/paths-and-arrays.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Ddl.cs`.
- **Index behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlIndexDdlParserTests.cs`.
