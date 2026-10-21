# Subqueries (Cohesion SQL)

Embeds a correlated or uncorrelated query as a scalar, membership test, or existence test.

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

[Set operations](../clauses/set-operations.md) execute in scalar, IN/NOT IN and EXISTS/NOT EXISTS
positions, including correlated operands. `IN ((SELECT …))` is membership; a query followed by
arithmetic or a list comma remains a scalar expression. Set outputs retain common type and
collation metadata; ordinary scalar-cardinality and NULL rules below still apply.

Scalar and membership queries must return exactly one column. `EXISTS` accepts any valid select
list. A child may omit FROM and use one implicit row, including correlated children. Names bind
locally first, then in enclosing query scopes. See [SELECT](../statements/select.md#c-select-without-from).

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

Independent children materialize once per statement. Correlated children evaluate for each outer
row when their expression is reached, including repeated or NULL correlation values. They share the
outer statement's multi-version concurrency control (MVCC) snapshot and transaction context.
System-relation children use the same statement catalog snapshot. Type and collation survive
materialization, including empty and null scalar results.

These forms compose with supported SELECT projections, predicates, joins, grouping, ordering,
ordinary numeric pagination and UPDATE/DELETE WHERE. At most 32 query levels, including derived
bodies, are allowed beneath the
top-level query or insert source; level 33 reports `COHDBL001`.

### Scope and aggregate levels

Local names and aliases shadow outer names, including names two or more levels out. A local
qualifier with no matching column stops lookup; it cannot fall back to the same qualifier outside.
Unknown/hidden qualifiers and ambiguity within a scope report located `COHSQLE015`, even on empty input.
SELECT lists, WHERE, HAVING, JOIN ON, grouping expressions, ordering and UPDATE/DELETE WHERE
accept correlation. CASE, COALESCE and Boolean short-circuiting evaluate only reached children.

A child evaluated over completed outer groups may read grouping-key columns only; another outer
column reports `COHSQLE017` naming it. Children inside aggregate arguments, WHERE and ON read
source rows, including ungrouped columns of that source. Grouped references compare bound
grouping values and types, including USING merges/coercions. Aggregate levels follow PostgreSQL: the lowest variable or aggregate dependency
sets the level, including dependencies inside nested queries after adjusting for their depth.
An aggregate belonging to an outer query level reports `COHDBL001` before grouped-column checks;
relocating it to that query is excluded. A mixed aggregate with a local dependency remains local.

Quantified ANY/ALL/SOME comparisons, LATERAL syntax and subqueries in UPDATE SET, VALUES,
CHECK, DEFAULT or pagination expressions remain `COHDBL001`. [WITH](../clauses/with.md) is supported
before data statements; nested and recursive WITH remain excluded. A [derived body](../clauses/from.md#derived-tables)
may correlate to enclosing queries, but a real sibling relation reference requires unsupported
LATERAL. A typo keeps `COHSQLE015`. SQL has NULL, with no separate MISSING value.

### UPDATE and DELETE snapshots

Predicate children evaluate before any target mutation, including self-reads. Independent children
materialize once; correlated children run for each candidate row. They see the session's earlier
uncommitted changes. Rollback restores target rows and cascades together. Scalar cardinality and
NULL membership rules above apply unchanged; SET expressions remain row-local.

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

### Snapshot example

Execute each statement separately in a fresh table:

```sql
CREATE TABLE snapshot_rows (id INT, x INT);
INSERT INTO snapshot_rows VALUES (1, 1), (2, 2), (3, 9);
UPDATE snapshot_rows SET x = x + 10
WHERE id IN (SELECT id FROM snapshot_rows WHERE x < (SELECT AVG(x) FROM snapshot_rows));
DELETE FROM snapshot_rows WHERE x > (SELECT AVG(x) FROM snapshot_rows);
SELECT id, x FROM snapshot_rows;
```

UPDATE affects two rows, DELETE affects two rows and the final row is `(3, 9)`. Each predicate
uses values from before its own statement, so updates do not feed back into that statement's children.

### Correlated WHERE and JOIN ON

Using the conformance fixture:

```sql
SELECT a.id FROM t a
WHERE EXISTS (SELECT b.id FROM t b WHERE b.id = a.id AND b.age > 40)
ORDER BY a.id;
SELECT a.id, b.id FROM t a LEFT JOIN t b
ON a.id = b.id AND EXISTS (SELECT c.id FROM t c WHERE c.id = a.id AND c.age > 40)
ORDER BY a.id;
```

The WHERE query returns 2 and 3. The join returns (1, NULL), (2, 2) and (3, 3); the child sees
only that ON step's candidate pair and enclosing query scopes.

### Correlated DML snapshots

In a fresh database, execute separately:

```sql
CREATE TABLE chain (id INT);
INSERT INTO chain VALUES (1), (2), (3);
UPDATE chain SET id = id + 10
WHERE EXISTS (SELECT 1 FROM chain p WHERE p.id = chain.id - 1);
SELECT id FROM chain ORDER BY id;
```

UPDATE affects two rows and SELECT returns 1, 12 and 13. Each predicate reads the complete
pre-statement chain. On a fresh chain with 1, 2 and 3, the following DELETE affects one row and
leaves 2 and 3:

```sql
DELETE FROM chain WHERE NOT EXISTS (SELECT 1 FROM chain p WHERE p.id = chain.id - 1);
```

### Correlation and grouping

Execute separately in a fresh database, as in the correlation tests:

```sql
CREATE TABLE sales (id INT, k INT, amount INT);
CREATE TABLE labels (k INT, threshold INT);
INSERT INTO sales VALUES (1, 1, 4), (2, 1, 6), (3, 2, 8), (4, 3, 3);
INSERT INTO labels VALUES (1, 9), (2, 10);
SELECT s.k, SUM(s.amount), (SELECT l.threshold FROM labels l WHERE l.k = s.k)
FROM sales s GROUP BY s.k
HAVING SUM(s.amount) > (SELECT l.threshold FROM labels l WHERE l.k = s.k)
ORDER BY s.k;
```

The result is `(1, 10, 9)`. The child reads the completed outer grouping key k.
`SELECT (SELECT SUM(s.amount) FROM labels l) FROM sales s` reports COHDBL001:
SUM depends only on the outer query. `SUM(l.threshold + s.amount)` in that child has a local
dependency and remains at the child's level when the surrounding query is otherwise valid.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Correlation, scope, groups and DML** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.Correlation.cs`.
- **Aggregate levels** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.Regression.cs`.

- **DML snapshots** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSubqueryTests.cs`.
- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`

