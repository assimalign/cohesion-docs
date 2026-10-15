# Statements

OQL accepts one query or one index definition statement per parse.

## Statements

- **Query documents** — [SELECT](select.md) projects and filters one collection.
- **Create an index** — [CREATE INDEX](create-index.md) builds a nonunique path index.
- **Remove an index** — [DROP INDEX](drop-index.md) removes a named collection index.

All three statements use the engine's parse, plan, and execute pipeline and the session's active
transaction or an automatic statement transaction. A trailing semicolon is optional.

## See also

[Language (OQL)](../index.md) and [Clauses](../clauses/index.md).

## Sources

- **Statement grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
