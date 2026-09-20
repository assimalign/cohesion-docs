# Subqueries (Cohesion SQL)

Embeds an uncorrelated query as a scalar, membership test, or existence test.

> **Status:** Partial. This page describes the executable subset and its boundaries.

## Syntax

```syntaxsql
( select_statement )
expression [ NOT ] IN ( select_statement )
[ NOT ] EXISTS ( select_statement )
```

## Arguments

- **`select_statement`** — A supported `SELECT` with its own local `FROM` scope.
- **`expression`** — The value tested against a one-column membership result.

## Remarks

Scalar and membership queries must return exactly one column. `EXISTS` accepts any valid select
list. Every child query requires `FROM`; its references must resolve within its own query scope.

| Form | Result |
| --- | --- |
| Scalar, one row | That row's value |
| Scalar, no rows | Typed `NULL` |
| Scalar, more than one row | Cardinality error; never an arbitrary first row |
| `IN`, equal non-null member | `TRUE` |
| `IN`, no match, null member | `UNKNOWN` |
| `IN`, null operand and nonempty result | `UNKNOWN` |
| `IN`, empty result | `FALSE`, including for a null operand |
| `NOT IN` | Negates `TRUE`/`FALSE`; preserves `UNKNOWN`; empty result is `TRUE` |
| `EXISTS` / `NOT EXISTS` | Test whether rows exist; a projected null still counts as a row |

Child results are materialized before the containing relational plan executes. They share the
outer statement's multi-version concurrency control (MVCC) snapshot and transaction context.
System-relation children use the same statement catalog snapshot. Type and collation survive
materialization, including empty and null scalar results.

These forms compose with supported SELECT projections, predicates, inner joins, grouping, ordering,
and ordinary numeric pagination. At most 32 expression-subquery levels are allowed beneath the
top-level query or insert source; level 33 reports `COHDBL001`.

Correlation, quantified `ANY`/`ALL`/`SOME` comparisons, derived tables, common table
expressions, lateral joins, and subqueries in `UPDATE`, `DELETE`, `VALUES`, `CHECK`,
`DEFAULT`, or pagination expressions are excluded with `COHDBL001`. Local aliases may shadow
outer names. Explicit outer qualifiers are diagnosed by parsing; unresolved local names are rejected
during catalog binding.

[INSERT ... SELECT](../statements/insert.md) materializes its complete source before writes and
uses destination coercion, defaults, and constraints atomically for the inserted rows.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT id, (SELECT MAX(age) FROM t) AS oldest
FROM t
WHERE id IN (SELECT id FROM t WHERE age > 40)
  AND EXISTS (SELECT id FROM t WHERE age = 36)
ORDER BY id;
```

The fixture returns `(2, 45)` and `(3, 45)`.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`

