# WITH (Cohesion SQL)

Defines named query relations for one SELECT, INSERT, UPDATE, DELETE or MERGE statement.

> **Status:** Partial. Acyclic definitions execute. Recursive definitions parse and bind, then refuse
> planning, EXPLAIN and execution with CK00001. Recursive execution is not available in this batch.

## Syntax

```syntaxsql
WITH [ RECURSIVE ] cte_name [ ( column_name [ , ...n ] ) ] AS ( query_body )
     [ , cte_name [ ( column_name [ , ...n ] ) ] AS ( query_body ) ...n ]
{ select_statement | insert_statement | update_statement | delete_statement | merge_statement }

INSERT INTO target [ ( column_name [ , ...n ] ) ]
WITH [ RECURSIVE ] cte_name AS ( query_body ) [ , ...n ] select_statement

<recursive_query_body> ::= non_recursive_term UNION [ ALL | DISTINCT ] recursive_term
```

## Arguments

- **`cte_name`** — A schema-less name, matched case-insensitively by catalog name rules.
- **`column_name`** — Optional output renames; supply exactly the body's full output width.
- **`query_body`** — A SELECT or set operation, with optional FROM at every query level.
- **`RECURSIVE`** — Gives all definitions visibility throughout the list; it does not require recursion.
  This modifier is refused before MERGE, including acyclic lists.

## Remarks

CTE bodies and the statement query may be [set operations](set-operations.md), including N-way
UNION/INTERSECT/EXCEPT with ALL/DISTINCT. Every SELECT operand may omit FROM, including
nested derived/scalar bodies; each uses one implicit row. Common types and output names
are resolved before the definition materializes.

Without RECURSIVE, definitions bind in declaration order. A body can read earlier CTEs. An
unqualified read resolves an already-visible CTE before a stored table; schema qualification bypasses CTE lookup. A
non-recursive self-reference or a later sibling's name reads a same-named stored table when one
exists. A missing later relation uses CS01026. Every definition binds, including unused ones,
so unknown columns and invalid aggregate placement fail before reads or writes.

CTE output has no schema identity: use `c.id` or an alias. `dbo.c.id` requires a stored FROM
entry or an enclosing stored relation available through correlation. A same-named CTE and stored
relation without distinct aliases conflict (CS01019). DML targets always resolve through the
catalog; a CTE cannot become an UPDATE, INSERT or DELETE target.

A FROM-less SELECT uses one implicit row and supports scalar or aggregate SELECT
clauses at every query level. A wildcard still needs a relation. Column renaming retains types, bounds, nullability, domains
and collations, including empty results. SQL has NULL and no MISSING value.

Each reachable definition materializes once per statement, reused across joins and scalar, IN
and EXISTS children. Unused definitions and dependencies that are reachable only from them never
execute. Their counts and predicates bind without evaluating counts or seek bounds. All reads
share the statement's MVCC and catalog snapshots, including the session's earlier uncommitted writes.
Before DML writes, reachable acyclic definitions materialize in binding order. Failures roll back the
whole statement while preserving an explicit transaction. INSERT retains destination defaults,
constraints and identity generation. [EXPLAIN](../statements/explain.md) describes without executing.

### WITH RECURSIVE

All names in a WITH RECURSIVE list are visible to every definition. Acyclic dependencies bind in
dependency order; independent ready definitions retain declaration-order priority. Unqualified
self-references identify recursive entries. Schema-qualified names still refer to stored relations.
Mutual recursion is refused with CK00001. An acyclic WITH RECURSIVE executes with the same
reachable-only, once-per-statement materialization as ordinary WITH.

A recursive definition must have a top-level non-recursive term (the anchor) UNION [ALL] recursive
term. The recursive term must contain exactly one self-reference. That reference cannot appear
in the anchor, inside scalar/IN/EXISTS/quantified subqueries, on the nullable side of an outer join,
under INTERSECT ALL, on either side of EXCEPT ALL, or on the right of EXCEPT DISTINCT. A derived
FROM query, preserved outer-join side, INTERSECT DISTINCT or left side of EXCEPT DISTINCT is allowed.

Aggregates at the query level containing the self-reference are refused. This includes aggregates
in a subquery that belong to the recursive level through outer-only arguments; independent
subquery aggregates are allowed. The top-level recursive UNION cannot have ORDER BY, LIMIT/FETCH
or OFFSET. SELECT row-locking tails FOR UPDATE, FOR NO KEY UPDATE, FOR SHARE and FOR KEY SHARE
are refused with CK00001 throughout the language, including CTE bodies and subqueries.

The non-recursive term fixes output names, width, types, domains, type modifiers and collations
before the recursive term binds. An optional column list must rename the full output width.
Unknown NULL/string anchor output becomes TEXT; recursive columns are conservatively nullable.
The combined UNION cannot change the anchor contract. CS02066 includes a cast hint: cast the
non-recursive output to the intended common type, and keep recursive output compatible. CS02067
includes a COLLATE hint: apply an explicit compatible COLLATE to the non-recursive output.

All definitions and the consuming statement bind before the recursion capability refusal, even
when the recursive definition is unused. A well-formed recursive query produces CK00001 during
planning, EXPLAIN and execution, before reading rows or writing. It produces no recursive results.
Plain WITH ... MERGE executes; WITH RECURSIVE ... MERGE is refused at RECURSIVE with CK00001.
Nested WITH, modifying CTE bodies, AS [NOT] MATERIALIZED and SEARCH/CYCLE remain refused.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `CS01026` | Duplicate CTE name, or unresolved reference to a later sibling; before reading rows |
| `CS05013` | Output-name list has the wrong width or a blank name; wildcard without FROM |
| `CS01015` | Unknown/ambiguous output column or invalid schema-qualified CTE column |
| `CS01019` | Conflicting relation names; give the CTE or stored relation an alias |
| `CS05017` | Invalid aggregate placement, including WHERE in an unused constant body |
| `CS00003` | Empty output-name list, malformed definition, or unsupported command kind after the list |
| `CK00001` | Recursive planning/execution, mutual recursion, prohibited recursive UNION tails, SELECT locking tails, WITH RECURSIVE ... MERGE, nested WITH, modifying CTE body, AS [NOT] MATERIALIZED, SEARCH/CYCLE |

The source assigns no SQLSTATE to CS01026. Other ordinary query diagnostics still apply.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01054 | Recursive definition lacks top-level anchor UNION [ALL] recursive term | 42P19 |
| CS01055 | Self-reference in the non-recursive term | 42P19 |
| CS01056 | Self-reference inside a scalar, IN, quantified or EXISTS subquery | 42P19 |
| CS01057 | Self-reference on the nullable side of an outer join | 42P19 |
| CS01058 | Self-reference under INTERSECT ALL | 42P19 |
| CS01059 | Self-reference in a prohibited EXCEPT operand | 42P19 |
| CS01060 | Recursive term does not have exactly one self-reference | 42P19 |
| CS01061 | Aggregate at the query level containing the self-reference | 42P19 |
| CS02065 | Anchor, recursive output or declared column-list width mismatch | 42601 |
| CS02066 | UNION changes the anchor type, domain or type modifiers; cast hint | 42804 |
| CS02067 | UNION changes or conflicts with anchor collation; COLLATE hint | 42P21 |

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

This returns `7` and `9` from stored b. Without stored b, the reference in a reports CS01026.

An acyclic list can use a later definition with RECURSIVE:

```sql
WITH RECURSIVE b AS (SELECT n FROM a), a(n) AS (SELECT 1)
SELECT n FROM b;
```

This returns one row containing `1`. The following parses and binds but reports
CK00001 when planned, explained or executed; it returns no rows:

```sql
WITH RECURSIVE r(n) AS (SELECT 1 UNION ALL SELECT n + 1 FROM r WHERE n < 3)
SELECT n FROM r;
```

## See also

[Clauses](index.md) · [FROM](from.md) · [Subqueries](../expressions/subqueries.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Execution and materialization** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.cs`.
- **Binding and constant bodies** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.Binding.cs`.
- **Syntax** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCteParserTests.cs`.
- **Recursive binding and execution refusal** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.Recursive.cs`.
