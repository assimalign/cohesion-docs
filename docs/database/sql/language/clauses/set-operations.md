# UNION, INTERSECT and EXCEPT (Cohesion SQL)

Combine query rows by position, with DISTINCT or bag multiplicities.

> **Status:** Implemented. Applies in every query position.

## Syntax

```syntaxsql
query_body { UNION | INTERSECT | EXCEPT } [ ALL | DISTINCT ] query_body
[ ORDER BY { output_name | ordinal } [ ASC | DESC ] [ NULLS FIRST | NULLS LAST ] [ , ...n ] ]
[ LIMIT count_expression ] [ OFFSET count_expression [ ROW | ROWS ] ]
-- OFFSET/FETCH is also accepted. Parentheses group query bodies and their own tails.
```

## Rows, duplicates and NULL

DISTINCT is the default. Rows compare as complete tuples after conversion to common output types.
NULL cells compare equal for duplicate and membership checks. SQL has no MISSING value.

| Operator | DISTINCT | ALL, for m left copies and n right copies |
| --- | --- | --- |
| UNION | Each row from either input once | m + n copies |
| INTERSECT | Each common row once | min(m, n) copies |
| EXCEPT | Each left row absent from the right once | max(m − n, 0) copies |

INTERSECT binds more tightly than UNION/EXCEPT; UNION and EXCEPT associate left to right.
Parentheses override precedence. Without ORDER BY, result order is not promised. The spelling of
a surviving collation-equivalent string is unspecified.

## Types and names

Inputs must have equal column counts. The engine applies PostgreSQL common-type rules before
reading rows: compatible categories use implicit registered casts; the left candidate wins when
conversion works both ways. INTEGER with BIGINT produces Int64; DECIMAL with DOUBLE produces Double.
Untyped NULL and unadorned string literals are unknown and take the other input's type. Two unknown
inputs resolve to TEXT. An explicit CAST to TEXT stays typed and cannot combine with INTEGER.
Invalid unknown-literal text can fail conversion even when the source is empty.

Length, precision and scale survive only when both types and modifiers agree. Domains survive
only for identical domains and modifiers. TEXT with CHAR converts CHAR through its text cast,
trimming padding; CHAR first with differing types/modifiers becomes unconstrained CHAR and retains
full values, including longer TEXT. No implicit CHAR(1) constraint is introduced. DATE with TIMESTAMP
promotes calendar values. Output names come from the first input; nullability combines both inputs.

Explicit COLLATE outranks column collation, which outranks the database default; the left input
wins equal priorities. That common collation governs both set comparison and IN membership.

## Query positions and paging

Set bodies execute at the top level, in derived tables, CTE definitions, INSERT sources, scalar
subqueries, IN/NOT IN and EXISTS/NOT EXISTS, including correlated operands. Every SELECT
operand may omit FROM and use one implicit row. All operands share
the statement snapshot/catalog capture and see the transaction's own writes. Scalar cardinality
and INSERT atomicity rules remain in force. `IN ((SELECT …))` is membership, not a scalar value list.

An unparenthesized ORDER BY/LIMIT/OFFSET/FETCH tail belongs to the combined result. Combined
ORDER BY accepts only first-input output names or positive one-based ordinals, including implicit
output names. Parenthesize an operand to give it its own tail. TOP belongs to its SELECT operand.
Compatible UNION chains with the same quantifier execute as an N-way materialization; all set
results materialize in memory, with no spilling or streaming guarantee.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS05013 | Different column counts; names both counts | — |
| CS02003 | No common type; names one-based output position and both types | 42804 |
| CS02022 | Invalid unknown-literal conversion text | 22P02 |
| CS02018 | Combined ordering uses an expression, qualified/missing name or invalid ordinal; also invalid paging counts | — |
| CS01019 | Ambiguous combined output name | — |
| CS00003 | Operand tail appears before another operator without parentheses | — |
| CK00001 | CORRESPONDING, including BY forms | — |

Nested/recursive WITH and LATERAL retain their existing exclusions. See [Diagnostics](../diagnostics.md).

## Examples

```sql
CREATE TABLE a (id INT);
CREATE TABLE b (id INT);
INSERT INTO a VALUES (1), (1), (2), (NULL);
INSERT INTO b VALUES (1), (3), (NULL);
SELECT id FROM a UNION SELECT id FROM b ORDER BY 1;
```

Result: `NULL, 1, 2, 3` in ascending order.
`SELECT id FROM a INTERSECT ALL SELECT id FROM b ORDER BY 1;` returns `NULL, 1`.
`SELECT id FROM a EXCEPT ALL SELECT id FROM b ORDER BY 1;` returns `1, 2`.

```sql
WITH valueset AS (SELECT 1 AS n UNION ALL SELECT 2 AS n)
SELECT n FROM valueset ORDER BY n OFFSET 1 ROW FETCH NEXT 1 ROW ONLY;
```

Result: `2`. `SELECT id FROM a WHERE id IN ((SELECT id FROM b)) ORDER BY id;` returns `1, 1`.

## See also

[SELECT](../statements/select.md) · [WITH](with.md) · [Subqueries](../expressions/subqueries.md) · [ORDER BY](order-by.md) · [Clauses](index.md) · [Query body .NET API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-language/sql-query-body-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.SetOperations.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.SetOperations.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSetOperationExecutionTests.cs`.
