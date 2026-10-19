# Statements

OQL accepts one SQL++ query, data mutation, collection definition or index statement per parse.

## Statements

- **Create a collection** — [CREATE COLLECTION](create-collection.md).
- **Remove a collection** — [DROP COLLECTION](drop-collection.md).
- **Insert documents** — [INSERT](insert.md), from VALUES or SELECT.
- **Insert or replace documents** — [UPSERT](upsert.md), replacing the whole value by key.
- **Update document paths** — [UPDATE](update.md), with SET and/or UNSET and an optional predicate.
- **Delete documents** — [DELETE](delete.md), with an optional predicate and affected count.
- **Inspect a plan** — [EXPLAIN](explain.md), without execution.
- **Query documents** — [SELECT](select.md) projects and filters one collection.
- **Create an index** — [CREATE INDEX](create-index.md) builds a nonunique path index.
- **Remove an index** — [DROP INDEX](drop-index.md) removes a named collection index.

These statements use the engine's parse, plan, and execute pipeline and the session's active
transaction or an automatic statement transaction. A trailing semicolon is optional.

## See also

[Language (OQL)](../index.md) and [Clauses](../clauses/index.md).

## Sources

- **Statement grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
