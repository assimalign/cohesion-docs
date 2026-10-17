# Language (OQL)

OQL is the Documents engine's executable SQL++ subset, including collection DDL and insert-only DML.

> **Status:** Partial. The reference describes behavior implemented on the integration branch.

OQL retains its name and follows the Couchbase SQL++ NULL/MISSING and Boolean rules. A statement
uses its session database. Replacement and deletion remain collection API operations. Parameters
share SQL and GQL's case-sensitive bare-name contract.

## Reference categories

- **Conventions** — [Syntax conventions](syntax-conventions.md).
- **Statements** — [SELECT, INSERT, EXPLAIN and collection/index DDL](statements/index.md).
- **Clauses** — [FROM, WHERE, GROUP BY, HAVING and ORDER BY](clauses/index.md).
- **Expressions** — [Paths, predicates, CASE, MISSING, objects and parameters](expressions/index.md).
- **Functions** — [Aggregates](functions/aggregate-functions.md) and [META](functions/meta.md).
- **Diagnostics** — [Parser, planner and execution codes](diagnostics.md).
- **Boundaries** — [Unsupported forms](unsupported.md).

## Support matrix

| Construct | Support | Scope |
| --- | --- | --- |
| SELECT/FROM/WHERE | Supported | One source, projections and SQL++ truth conversion |
| [SELECT ALL/DISTINCT, VALUE/RAW](statements/select.md) | Supported | Duplicate removal and one bare JSON value per row; top-level MISSING becomes NULL |
| GROUP BY/HAVING/ORDER BY | Supported | Separate NULL/MISSING groups and ordering |
| MISSING and presence tests | Supported | Absent values distinct from explicit NULL |
| IN/BETWEEN/LIKE ESCAPE/CASE | Supported | SQL++ unknown rules and structural equality |
| CREATE/DROP COLLECTION | Supported | Conditional, transactional collection DDL |
| INSERT VALUES/INSERT SELECT | Supported | Explicit keys, insert-only and atomic |
| EXPLAIN | Supported | SELECT/INSERT plan rows without execution |
| CREATE/DROP INDEX | Supported | One nonunique path index in a collection |
| COUNT/SUM/AVG/MIN/MAX | Supported | NULL and MISSING skipped; COUNT(*) counts documents |
| META and [object](expressions/object-constructors.md)/[array constructors](expressions/array-constructors.md) | Supported | Stored metadata, keyed objects and ordered arrays |

## See also

[Document](../index.md) describes sessions, storage, transport and pooled clients.

## Sources

- **Dialect** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DIALECT.md`.
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/OqlExecutionConformanceTests.cs`.
