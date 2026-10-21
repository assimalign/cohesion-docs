# WHERE (Cohesion SQL)

Filters input rows using a Boolean search condition.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
WHERE search_condition
```

## Arguments

- **`search_condition`** — A supported predicate over source columns, parameters, literals, or
  scalar expressions.

## Remarks

Only `TRUE` retains a row. `FALSE` and `UNKNOWN` are discarded; comparison with `NULL`
does not substitute for `IS NULL`. Filtering occurs before grouping and aggregation.
Aggregates cannot occur in `WHERE`.

`SELECT`, `UPDATE`, and `DELETE` accept `WHERE`. Supported correlated and uncorrelated subqueries can
appear in all three predicates. UPDATE/DELETE children finish for all candidate rows before target writes against
the pre-statement snapshot; see [Subqueries](../expressions/subqueries.md#update-and-delete-snapshots). String comparison and
`LIKE` use the effective [collation](collate.md).

## Boolean search conditions

WHERE, [HAVING](having.md), [JOIN ON](join.md), [CHECK](../constraints.md), domain CHECK,
[searched CASE WHEN](../expressions/case.md) and [ON CONFLICT DO UPDATE WHERE](on-conflict.md)
require BOOLEAN. AND, OR, NOT and IS TRUE/FALSE/UNKNOWN operands follow the same rule.
Known non-Boolean types fail during planning, before rows are read, including empty tables.
The diagnostic is COHSQLE003 (SQLSTATE 42804), naming the construct with its available source span.
Runtime validation applies the same rule when a type could not be established earlier.

Untyped NULL and parameters whose type is unknown are accepted as UNKNOWN. Supplied Boolean
parameters retain their value; a supplied NULL filters out the row. A supplied non-Boolean value
is rejected. A typed non-Boolean NULL such as CAST(NULL AS INT) is also rejected. SQL has no MISSING.

Unadorned string literals accept these Boolean inputs, case-insensitively:

| TRUE inputs | FALSE inputs |
| --- | --- |
| 'true', 't', 'yes', 'y', 'on', '1' | 'false', 'f', 'no', 'n', 'off', '0' |

Other strings fail COHSQLE003. Text columns, text parameters and explicit TEXT casts retain
their types and receive no literal coercion. Every CASE result and COALESCE alternative used
in a Boolean position must qualify, even an unreachable alternative. Boolean short-circuiting
still skips later runtime evaluation once the result is decided; it does not skip type validation.
Only TRUE selects a row or searched CASE arm. CHECK instead accepts TRUE or UNKNOWN.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT id FROM t WHERE age > 40 AND name LIKE 'G%' ORDER BY id;
```

The result is the single identifier `2`.

These queries need no table:

```sql
SELECT 1 WHERE 'on';
SELECT 1 WHERE NULL;
SELECT 1 WHERE COALESCE(NULL, TRUE);
```

They return `1`, no rows, and `1`, respectively. `SELECT 1 WHERE 1` and
`SELECT 1 WHERE CASE WHEN TRUE THEN TRUE ELSE 1 END` each fail with COHSQLE003 (42804).

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Boolean conditions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBooleanPredicateTests.cs`.


- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

