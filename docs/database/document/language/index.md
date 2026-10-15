# Language (OQL)

Object Query Language (OQL) queries one document collection and manages its secondary indexes.

> **Status:** Partial. The documented subset is executable; the full Object Data Management Group language is not implemented.

The parser produces typed statements for the Documents engine. A statement addresses the session's
database and cannot switch databases or servers. Document replacement and deletion remain collection
API operations. The grammar adds explicit index data definition language (DDL) to OQL.

## Reference categories

- **Conventions** — [Syntax conventions](syntax-conventions.md) explains diagrams and lexical rules.
- **Statements** — [SELECT, CREATE INDEX, and DROP INDEX](statements/index.md).
- **Clauses** — [FROM, WHERE, GROUP BY, HAVING, and ORDER BY](clauses/index.md).
- **Expressions** — [Paths, operators, literals, and parameters](expressions/index.md).
- **Functions** — [Aggregate functions](functions/index.md).
- **Diagnostics** — [Parser errors and locations](diagnostics.md).
- **Boundaries** — [Unsupported language features](unsupported.md).

## Support matrix

| Construct | Support | Scope |
|---|---|---|
| `SELECT`, `FROM`, `WHERE` | Supported | One source, projections, scalar predicates |
| `GROUP BY`, `HAVING`, `ORDER BY` | Supported | Grouping, aggregate filtering, stable ordering |
| `CREATE INDEX`, `DROP INDEX` | Supported | One nonunique path index in one collection |
| `COUNT`, `SUM`, `AVG`, `MIN`, `MAX` | Supported | One argument; `COUNT(*)` also accepts a star |
| `DEFINE`, `ELEMENT`, `FLATTEN`, nested `SELECT` | Recognized, not supported (`COHDBL001`) | Reserved constructs |
| Document data-mutation statements | Recognized, not supported (`COHDBL001`) | Use the collection API |

## See also

[Document](../index.md) describes collections, transaction ownership, storage, and engine limits.

## Sources

- **Language overview** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/OVERVIEW.md`.
- **Language contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
