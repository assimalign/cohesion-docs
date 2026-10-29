# Language (OQL)

OQL is the Documents engine's executable SQL++ subset, including collection DDL, INSERT, UPSERT, UPDATE and DELETE.

> **Status:** Partial. Queries, mutations and DDL execute; text transaction control is available in process and through the Documents client.

OQL retains its name and follows the Couchbase SQL++ NULL/MISSING and Boolean rules. A statement
uses its session database. [UPDATE](statements/update.md) changes selected paths and
[DELETE](statements/delete.md) removes matching documents. Parameters
share SQL and GQL's case-sensitive bare-name contract.

## Reference categories

- **Conventions** — [Syntax conventions](syntax-conventions.md).
- **Statements** — [SELECT, INSERT, UPSERT, UPDATE, DELETE, EXPLAIN and collection/index DDL](statements/index.md).
- **Clauses** — [FROM, WHERE, GROUP BY, HAVING, ORDER BY and LIMIT/OFFSET](clauses/index.md).
- **Expressions** — [Paths, predicates, CASE, MISSING, constructors, subqueries and EXISTS](expressions/index.md).
- **Functions** — [34 SQL++ scalars](functions/scalar-functions.md), [aggregates](functions/aggregate-functions.md) and [META](functions/meta.md).
- **Diagnostics** — [Parser, planner and execution codes](diagnostics.md).
- **Boundaries** — [Unsupported forms](unsupported.md).

## Support matrix

| Construct | Support | Scope |
| --- | --- | --- |
| SELECT/FROM/WHERE | Supported | One collection, ordered UNNEST bindings, projections and SQL++ truth conversion |
| [SELECT ALL/DISTINCT, VALUE/RAW](statements/select.md) | Supported | Duplicate removal and one bare JSON value per row; top-level MISSING becomes NULL |
| GROUP BY/HAVING/ORDER BY | Supported | Separate NULL/MISSING groups and ordering |
| [LIMIT/OFFSET](clauses/limit-offset.md) | Supported | Final paging by literal or parameter count; NULL is invalid |
| MISSING and presence tests | Supported | Absent values distinct from explicit NULL |
| IN/BETWEEN/LIKE ESCAPE/CASE | Supported | SQL++ unknown rules and structural equality |
| [Expression subqueries](expressions/subqueries.md), [EXISTS](expressions/exists.md) | Supported subset | Uncorrelated SELECT arrays, including RAW/VALUE and direct IN; derived FROM and DML subqueries remain deferred |
| CREATE/DROP COLLECTION | Supported | Conditional DDL in an automatic statement transaction |
| INSERT VALUES/INSERT SELECT | Supported | Explicit keys, insert-only and atomic |
| [RETURNING](clauses/returning.md) | Supported | INSERT/UPSERT/UPDATE post-images, DELETE pre-images; named, RAW/VALUE or lone star projection with affected count |
| [DELETE](statements/delete.md) | Supported | Snapshot filtering, index maintenance and affected count; retains collection/index definitions |
| [UPDATE SET/UNSET](statements/update.md) | Supported | Snapshot values, nested objects and existing array positions; matched count |
| [UPSERT VALUES/SELECT](statements/upsert.md) | Supported | Whole-document replacement by explicit key; atomic index maintenance |
| [BEGIN/START, COMMIT, ROLLBACK](statements/transactions.md) | Supported | In process and over the wire; Snapshot by default, explicit READ COMMITTED; CD07002 only without negotiated status |
| EXPLAIN | Supported | SELECT/INSERT/UPSERT/UPDATE/DELETE plan rows without execution |
| CREATE/DROP INDEX | Supported | One to 32 ordered scalar/array keys, scalar-only UNIQUE and conditional DDL |
| [Eleven native aggregate names](functions/aggregate-functions.md) | Supported | ALL/DISTINCT; ARRAY_AGG retains NULL; Boolean/statistics aggregates skip wrong kinds; COUNT(*) counts input rows |
| [34 scalar functions](functions/scalar-functions.md) | Supported | Strings, exact numerics, transaction-start UTC dates, types, arrays and lazy conditionals |
| META and [object](expressions/object-constructors.md)/[array constructors](expressions/array-constructors.md) | Supported | Stored metadata, keyed objects and ordered arrays |
| [UNNEST/LEFT UNNEST](clauses/unnest.md) | Supported | Array rows; LEFT preserves empty/non-array inputs with MISSING |
| [ANY/SOME/EVERY](expressions/quantified-predicates.md) and array IN | Supported | Scoped SQL++ predicates and structural membership |
| [ARRAY/FIRST comprehensions](expressions/comprehensions.md) | Supported | Ordered mapping, optional WHEN and nested scopes; RETURNING included |

## See also

[Document](../index.md) describes sessions, storage, transport and pooled clients.

## Sources

- **Dialect** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DIALECT.md`.
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/OqlExecutionConformanceTests.cs`.
