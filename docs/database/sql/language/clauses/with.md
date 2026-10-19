# WITH (Cohesion SQL)

Defines named, non-recursive query relations for one SELECT, INSERT, UPDATE or DELETE statement.

> **Status:** Partial. Non-recursive SELECT bodies execute; recursive and modifying bodies are excluded.

## Syntax

```syntaxsql
WITH cte_name [ ( column_name [ , ...n ] ) ] AS ( select_statement )
     [ , cte_name [ ( column_name [ , ...n ] ) ] AS ( select_statement ) ...n ]
{ select_statement | insert_statement | update_statement | delete_statement }

INSERT INTO target [ ( column_name [ , ...n ] ) ]
WITH cte_name AS ( select_statement ) [ , ...n ] select_statement
```

## Arguments

- **`cte_name`** — A schema-less name, matched case-insensitively by catalog name rules.
- **`column_name`** — Optional output renames; supply exactly the body's full output width.
- **`select_statement`** — A supported SELECT. Only a definition's SELECT root may omit FROM.

## Remarks

CTE bodies and the statement query may be [set operations](set-operations.md), including N-way
UNION/INTERSECT/EXCEPT with ALL/DISTINCT. Every top-level operand of a CTE definition may omit
FROM; nested derived/scalar bodies retain ordinary FROM rules. Common types and output names
are resolved before the definition materializes.

Definitions bind in declaration order. A body can read earlier CTEs. An unqualified read resolves
an already-visible CTE before a stored table; schema qualification bypasses CTE lookup. A
non-recursive self-reference or a later sibling's name reads a same-named stored table when one
exists. A missing later relation uses COHSQLE026. Every definition binds, including unused ones,
so unknown columns and invalid aggregate placement fail before reads or writes.

CTE output has no schema identity: use `c.id` or an alias. `dbo.c.id` requires a stored FROM
entry or an enclosing stored relation available through correlation. A same-named CTE and stored
relation without distinct aliases conflict (COHSQLE019). DML targets always resolve through the
catalog; a CTE cannot become an UPDATE, INSERT or DELETE target.

A FROM-less definition root uses one implicit row and supports scalar or aggregate SELECT
clauses. A wildcard still needs a relation. Nested expression subqueries and derived bodies
retain the ordinary FROM requirement. Column renaming retains types, bounds, nullability, domains
and collations, including empty results. SQL has NULL and no MISSING value.

Each reachable definition materializes once per statement, reused across joins and scalar, IN
and EXISTS children. Unused definitions and dependencies that are reachable only from them never
execute. Their counts and predicates bind without evaluating counts or seek bounds. All reads
share the statement's MVCC and catalog snapshots, including the session's earlier uncommitted writes.
Before DML writes, reachable definitions materialize in declaration order. Failures roll back the
whole statement while preserving an explicit transaction. INSERT retains destination defaults,
constraints and identity generation. [EXPLAIN](../statements/explain.md) describes without executing.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `COHSQLE026` | Duplicate CTE name, or unresolved reference to a later sibling; before reading rows |
| `COHSQLE013` | Output-name list has the wrong width or a blank name; wildcard without FROM |
| `COHSQLE015` | Unknown/ambiguous output column or invalid schema-qualified CTE column |
| `COHSQLE019` | Conflicting relation names; give the CTE or stored relation an alias |
| `COHSQLE017` | Invalid aggregate placement, including WHERE in an unused constant body |
| `SQL0003` | Empty output-name list, malformed definition, or unsupported command kind after the list |
| `COHDBL001` | WITH RECURSIVE, nested WITH in a subquery/derived body, modifying CTE body, AS [NOT] MATERIALIZED, SEARCH/CYCLE |

The source assigns no SQLSTATE to COHSQLE026. Other ordinary query diagnostics still apply.

## Examples

Execute separately in a fresh database:

```sql
CREATE TABLE t (id INT PRIMARY KEY);
INSERT INTO t VALUES (1), (2), (3);
WITH a AS (SELECT id FROM t WHERE id > 1 ORDER BY id LIMIT 2),
     b (n) AS (SELECT id FROM a)
SELECT b.n FROM b JOIN t ON b.n = t.id ORDER BY b.n DESC LIMIT 1;
WITH n AS (SELECT COUNT(*) AS value) SELECT value FROM n;
WITH s AS (SELECT id + 100 AS id FROM t) INSERT INTO t (id) SELECT id FROM s;
SELECT id FROM t ORDER BY id;
```

The first query returns `3`, the constant aggregate returns `1`, and INSERT affects three rows.
The final rows are `1, 2, 3, 101, 102, 103`. The INSERT source sees only pre-write rows.

Continuing with the six rows above:

```sql
WITH s AS (SELECT id FROM t WHERE id >= 100)
UPDATE t SET id = id + 100 WHERE id IN (SELECT id FROM s);
WITH s AS (SELECT id FROM t WHERE id >= 200)
DELETE FROM t WHERE id IN (SELECT id FROM s);
SELECT id FROM t ORDER BY id;
```

UPDATE and DELETE each affect three rows; the final rows are 1, 2 and 3.

For declaration-order fallback, in a separate database:

```sql
CREATE TABLE b (id INT);
INSERT INTO b VALUES (7), (9);
WITH a AS (SELECT id FROM b), b AS (SELECT 1 AS id)
SELECT id FROM a ORDER BY id;
```

This returns `7` and `9` from stored b. Without stored b, the reference in a reports COHSQLE026.

## See also

[Clauses](index.md) · [FROM](from.md) · [Subqueries](../expressions/subqueries.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Execution and materialization** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.cs`.
- **Binding and constant bodies** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.Binding.cs`.
- **Syntax** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCteParserTests.cs`.
