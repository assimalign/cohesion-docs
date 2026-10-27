# FROM (Cohesion SQL)

Identifies stored tables, system relations and derived queries, with a join tree for each comma item.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
FROM <table_source> [ <join> ...n ] [ , <table_source> [ <join> ...n ] ...n ]

<table_source> ::= [ schema_name . ] table_name [ [ AS ] table_alias ]
                 | ( select_statement ) [ AS ] table_alias [ ( column_name [ , ...n ] ) ]
                 | ( <table_source> <join> [ <join> ...n ] )
                   [ [ AS ] table_alias [ ( column_name [ , ...n ] ) ] ]
```

## Arguments

Derived query bodies may be [set operations](set-operations.md); their common output types and
first-input names become the derived relation's columns. Existing alias and outer-ordering rules apply.

- **`table_name`** — A stored table, a [persisted view](../statements/create-view.md), a visible [CTE](with.md), or one of the named virtual system relations.
- **`schema_name`** — A SQL namespace inside the session's database.
- **`table_alias`** — An optional name used to qualify column references.

## Remarks

[SELECT without FROM](../statements/select.md#c-select-without-from) uses one implicit row,
including nested queries, CTEs and views. A wildcard still requires a relation. A session cannot select another database through
qualification or switch databases through SQL. System relations require the `INFORMATION_SCHEMA`
or `COHESION_SCHEMA` qualifier.

Each item can contain any number of [INNER, LEFT, RIGHT, FULL or CROSS joins](join.md). Comma-separated
items produce a Cartesian product. JOIN binds more tightly
than comma: in `FROM a, b JOIN c ON ...`, ON sees only b and c. It cannot see later inputs.
An explicit alias hides the base name in SELECT,
UPDATE and DELETE: `SELECT t.id FROM t AS x` reports located `CS01015`; use `x.id`.
Unknown or ambiguous columns fail during planning, including on empty inputs.
Invalid qualifiers/columns use `CS01015`; duplicate table bindings use `CS01019`.

Parenthesized join groups preserve association, including groups starting with an aliased derived
SELECT. Without an alias, component names remain visible. A group alias hides them and behaves as
a derived SELECT *, with the same complete column-alias-list rule. RIGHT preserves its written
right side while making the left subtree nullable; visible column order stays as written.
FULL preserves unmatched rows from both subtrees and requires a safe mandatory hash equality.

### Derived tables

A parenthesized SELECT is a read-only relation in FROM, JOIN or a comma list. Its alias is required
(`CS00003` if absent) and hides every underlying table name. The optional column list renames every
output by position: it must have exactly the output width, unlike a partial PostgreSQL list.
Shorter/longer lists and empty or whitespace-only output names fail planning with `CS05013`,
including empty inputs. Duplicate output names remain ambiguous when referenced (`CS01015`).

An independent body runs once in the enclosing statement context; a correlated body runs again
for each enclosing evaluation frame. Its WHERE, GROUP BY/HAVING, ORDER BY and
LIMIT/OFFSET apply before the enclosing query reads its rows. Derived, stored and system inputs
can join each other, including LEFT's NULL extension. Types, length/precision/scale, nullability,
domains and collation provenance follow the projected expression and survive empty materialization.
Qualified USING outputs keep their physical metadata; unqualified USING outputs use merged metadata.
An IN/NOT IN projection is Boolean, regardless of the membership element type. SQL has no MISSING.

Each body has its own FROM scope. A real sibling reference fails `CK00001` with
`A derived table cannot reference sibling relations without LATERAL.` followed by the binding
detail at the offending reference. A typo or nonexistent/hidden qualifier keeps `CS01015`.
An enclosing-query reference from a body nested in a subquery follows the ordinary
[correlation scope chain](../expressions/subqueries.md) and executes for each outer row. Explicit LATERAL syntax remains unsupported. Derived bodies and
expression subqueries share the 32-level nesting limit (`CK00001` at level 33).

Outer [ORDER BY](order-by.md) accepts derived outputs, including aliases and ordinals, with
per-key NULLS FIRST/LAST. Ordering inside nested derived bodies follows the same rules.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT u.name FROM dbo.t AS u WHERE id = 1;
```

This conformance query returns `Ada`.

```sql
SELECT COUNT(*) FROM t AS a, t AS b CROSS JOIN t AS c;
```

This returns `27`: each of the three inputs contains three rows. An empty input makes a product
empty. See [JOIN](join.md) for LEFT's NULL extension and USING's merged columns.

```sql
SELECT d.a, d.b
FROM (SELECT id, name FROM t WHERE id = 1) AS d (a, b);
SELECT d.id FROM t AS base
JOIN (SELECT id FROM t) AS d ON base.id = d.id WHERE base.id = 2;
SELECT d.id FROM t AS base, (SELECT id FROM t) AS d
WHERE base.id = d.id AND d.id = 2;
SELECT d.id FROM (SELECT id FROM t ORDER BY id DESC LIMIT 1) AS d;
```

Results are `(1, Ada)`, `2`, `2` and `3`. `SELECT d.id FROM (SELECT id FROM t) d ORDER BY d.id`
returns 1, 2, 3.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Mutation sources

[MERGE USING](../statements/merge.md) binds the same supported read-source forms as UPDATE FROM,
including views, system relations, aliased derived queries and INNER/LEFT/CROSS joins. MERGE
execution remains CK00001; EXPLAIN describes its association.

UPDATE ... FROM and DELETE ... USING execute with this source-list/join grammar. Their target
remains the outer join input; target-only predicates can seek it, while mandatory equality to
a source can drive a source index probe or budgeted hash join. Equality-driven steps follow
written source order: list the source connected to the target first. General join-order search
is deferred to #1629. See [UPDATE](../statements/update.md#from-sources) and
[DELETE](../statements/delete.md#using-sources) for cardinality, snapshots and locking.

SET values, WHERE and RETURNING can read source columns; SET destinations remain target-only.
Bare RETURNING * expands target columns then visible sources in FROM/USING order, retaining
JOIN USING merges. Source JOIN ON sees only its comma item's namespace, excluding the target.

## Sources

- **Derived execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDerivedTableExecutionTests.cs`.
- **Derived syntax** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlDerivedTableParserTests.cs`.
- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
