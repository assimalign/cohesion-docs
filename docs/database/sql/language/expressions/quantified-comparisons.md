# Quantified comparisons (Cohesion SQL)

Compares one scalar value with the rows of a single-column subquery.

> **Status:** Implemented for scalar subquery operands.

## Syntax

```syntaxsql
expression { = | <> | != | < | <= | > | >= } { ANY | SOME | ALL } ( query )
```

## Remarks

ANY and SOME are synonyms: return TRUE if any comparison is TRUE. ALL returns TRUE if every
comparison is TRUE. The subquery must return exactly one column, even when it returns no rows
(CS05013). It may return many rows and may contain set operations, derived sources or references
to an enclosing WITH. SQL has NULL, with no separate MISSING value.

| Comparison results from the subquery | ANY / SOME | ALL |
| --- | --- | --- |
| No rows | FALSE | TRUE |
| Only TRUE | TRUE | TRUE |
| Only FALSE | FALSE | FALSE |
| Only UNKNOWN | UNKNOWN | UNKNOWN |
| TRUE and UNKNOWN | TRUE | UNKNOWN |
| FALSE and UNKNOWN | UNKNOWN | FALSE |
| TRUE and FALSE, with or without UNKNOWN | TRUE | FALSE |

A NULL operand produces UNKNOWN against a nonempty result. A NULL candidate contributes UNKNOWN
unless a TRUE decides ANY/SOME or a FALSE decides ALL. Empty ANY/SOME is FALSE and empty ALL is
TRUE even for a NULL operand. Projection returns UNKNOWN as a NULL Boolean cell; WHERE, HAVING
and ON retain only TRUE.

These comparisons execute in SELECT expressions, WHERE, JOIN ON, GROUP BY, HAVING, ORDER BY and
UPDATE/DELETE WHERE. [Correlation](subqueries.md#scope-and-aggregate-levels) uses ordinary lexical
scopes. Independent children materialize once per statement; correlated children execute for each
reached outer frame. All read the enclosing statement snapshot. The scalar operand evaluates
once for a nonempty set and is skipped for an empty set. Comparison folding stops at a deciding
result, but child projection evaluation is not guaranteed to stop early.

Comparison uses the ordinary numeric, temporal, binary, CHAR and collation rules. Explicit left
COLLATE overrides the child column's collation; otherwise the child contributes its declared
collation, as for IN. Incompatible compared non-NULL values report CS02055 (42804).

Subqueries remain excluded in UPDATE SET, INSERT VALUES, RETURNING, CHECK, DEFAULT and pagination
expressions (CK00001). Array operands, including parameter arrays or value lists, report CK00001;
row-value comparisons report CS00003. The ordinary 32-level subquery and expression-depth limits
apply. See [subqueries](subqueries.md) and [repeated grouping keys](../clauses/group-by.md#repeated-subquery-keys).

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS05013 | Subquery output has other than one column, including an empty result | — |
| CS01015 | Unknown, hidden or ambiguous column/qualifier | 42703 for unknown columns |
| CS02055 | Incompatible non-NULL values when compared | 42804 |
| CS05017 | Invalid aggregate placement or ungrouped reference | — |
| CS00003 | Row-value syntax or malformed quantified comparison | — |
| CK00001 | Array form, excluded subquery position or more than 32 subquery levels | — |
| CS00006 | Whole-expression nesting exceeds its configured limit | — |

## Examples

Execute each statement separately:

```sql
SELECT 2 = ANY (SELECT 1 UNION ALL SELECT 2),
       2 = SOME (SELECT 1 UNION ALL SELECT CAST(NULL AS INT)),
       2 = ALL (SELECT 2 UNION ALL SELECT CAST(NULL AS INT));
SELECT CAST(NULL AS INT) = ANY (SELECT 1 WHERE FALSE),
       CAST(NULL AS INT) = ALL (SELECT 1 WHERE FALSE);
```

The rows are `(TRUE, NULL, NULL)` and `(FALSE, TRUE)`.

For correlation, execute separately in a new database:

```sql
CREATE TABLE candidates (id INT);
CREATE TABLE choices (id INT);
INSERT INTO candidates VALUES (1), (2), (3), (NULL);
INSERT INTO choices VALUES (2), (NULL);
SELECT c.id, c.id = SOME (SELECT d.id FROM choices d WHERE d.id = c.id)
FROM candidates c WHERE c.id IS NOT NULL ORDER BY c.id;
```

The rows are `(1, FALSE)`, `(2, TRUE)` and `(3, FALSE)`.

## See also

[Expressions](index.md) · [Operators](operators.md) · [Subqueries](subqueries.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.Quantified.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlQuantifiedComparisonWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlQuantifiedComparisonParserTests.cs`.
