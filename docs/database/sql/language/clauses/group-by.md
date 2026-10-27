# GROUP BY (Cohesion SQL)

Forms groups from one or more scalar expressions over the filtered input.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
GROUP BY expression [ , ...n ]
```

## Arguments

- **`expression`** — A scalar grouping key bound to the input source.

## Remarks

Rows surviving `WHERE` form groups by the tuple of grouping values. `NULL` keys group
together. Strings use matching equality and hashing under the effective collation; a folded group
preserves a member's original spelling.

A projected, ordered, or `HAVING` column outside an aggregate must be a grouping expression or
derived from grouped columns. Invalid ungrouped columns fail planning even on empty input.
Aggregate arguments cannot contain aggregates, and aggregates cannot be grouping keys.

Explicit grouping over empty input returns no groups. An ungrouped aggregate instead has one
implicit group. Grouping works over one stored table, one system relation, or the supported inner
join. `HAVING` filters groups before ordering and pagination.

Projection aliases are outside grouping's alias-binding scope. Standalone numeric keys
(`GROUP BY 1`) report `CK00001`; `GROUP BY 1 + 1` is an ordinary constant expression.
Empty grouping sets, `GROUPING SETS`, `ROLLUP`, `CUBE`, and grouping functions are excluded.

[Aggregate calls](../functions/aggregate-functions.md) accept DISTINCT/ALL and input ORDER BY.
STRING_AGG and LISTAGG concatenate ordered text within each group.

## Repeated subquery keys

A scalar subquery, EXISTS, quantified comparison or IN expression repeated in the select list,
HAVING or ORDER BY reuses its GROUP BY value when the complete expression matches structurally
in the same scope, including identical correlations. This follows PostgreSQL's whole-expression
grouping rule. Operators, quantifiers (including ANY versus SOME), negation, bound scalar operands
and query bodies remain significant. Different or unproven expressions do not share a group slot;
an ungrouped reference still reports CS05017 (no assigned SQLSTATE).

For `candidates(id)` containing 1, 2, 3 and NULL:

```sql
SELECT c.id = ANY (SELECT 2), COUNT(*)
FROM candidates c
GROUP BY c.id = ANY (SELECT 2)
HAVING c.id = ANY (SELECT 2)
ORDER BY c.id = ANY (SELECT 2);
```

The result is `(TRUE, 1)`. See [quantified comparisons](../expressions/quantified-comparisons.md).

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT age > 40, COUNT(*), SUM(age)
FROM t
GROUP BY age > 40
ORDER BY age > 40;
```

The rows are `(FALSE, 1, 36)` and `(TRUE, 2, 86)`; sums have shared type `Decimal`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.Grouping.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.cs`

