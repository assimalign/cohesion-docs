# JOIN (Cohesion SQL)

Combines stored tables and registered system relations with INNER, LEFT, CROSS, and comma joins.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
FROM <table_source> [ <join> ...n ] [ , <table_source> [ <join> ...n ] ...n ]

<table_source> ::= [ schema_name . ] table_name [ [ AS ] table_alias ]
<join> ::= { [ INNER ] JOIN | LEFT [ OUTER ] JOIN } <table_source>
           { ON search_condition | USING ( column_name [ , ...n ] ) }
         | CROSS JOIN <table_source>
```

## Arguments

- **`table_source`** — A stored table or an INFORMATION_SCHEMA/COHESION_SCHEMA relation.
- **`search_condition`** — The predicate evaluated for each candidate pair.
- **`column_name`** — A name occurring exactly once in each input to that USING step.

## Remarks

Only `TRUE` matches ON or USING's implicit equalities; `FALSE` and `UNKNOWN` do not.
LEFT emits one row with NULL in every right column when no candidate matches. A predicate in ON
can preserve unmatched left rows; the same predicate in WHERE can remove them. `COUNT(*)` counts
the extended row, while `COUNT(right.column)` ignores its NULL. SQL has no MISSING value.
CROSS and comma joins produce Cartesian products; an empty input makes the product
empty while retaining column metadata.

All inputs share the statement's multi-version concurrency control (MVCC) snapshot. Multiplicity
is preserved. Grouping/HAVING, DISTINCT, ordering, pagination and subqueries compose with the joins.
Row order requires ORDER BY. System relations share one captured catalog image.

JOIN binds more tightly than comma. In `FROM a, b JOIN c ON ...`, ON sees b and c, but cannot
see a or a later input. Joins associate to the left within each comma item. Explicit aliases hide
base names and distinguish repeated tables. Unqualified names shared by inputs remain ambiguous
unless USING merges them. These checks apply before rows are read, even on empty inputs.

### USING columns

USING compares each named pair with ordinary equality and collation precedence; NULL never
matches NULL. It exposes one merged column per name for unqualified lookup and `*`. Wildcard
order is the USING list, remaining left columns, then remaining right columns. Later joins apply
the rule to the accumulated left output; comma items concatenate their visible columns.

Merged columns use PostgreSQL's common-type selection through implicit casts within a type
category: INT/DECIMAL becomes DECIMAL and INT/BIGINT becomes BIGINT in either input order.
Arithmetic and function overloads see that common type. LEFT uses the coerced left value and its
nullability; INNER prefers an unchanged input of the common type when available. Identical domains
retain identity; mixed domain/base or differing domains resolve through base types. Length,
precision and scale survive only when both types and modifiers agree. CHAR(3)/VARCHAR(10), for
example, clears the merged length. Qualified `left.id` and `right.id` retain their original types
and modifiers; the right column becomes nullable under LEFT.

[ORDER BY](order-by.md) aliases after `*` and numeric ordinals count the visible merged columns.
Qualified wildcards such as `t.*` remain excluded.

### Access paths

Execution follows written order. Only the first INNER pair may swap inputs for an index; later
steps may seek into the added input, and LEFT always drives from its preserved left side.
Mandatory equalities can probe an index's leading key prefix under the existing type/collation
rules; the full predicate still filters candidates. Otherwise the added input buffers once.
System inputs have no index paths or object locks. Computed merged keys do not gain index probes.
EXPLAIN describes each join step and its scan or seek.

| Form | Support | Boundary |
| --- | --- | --- |
| INNER or LEFT [OUTER] JOIN | Supported (measured) | Exactly one ON or USING per step; any number of steps |
| CROSS JOIN and comma sources | Supported (measured) | Products; CROSS accepts neither ON nor USING |
| RIGHT, FULL, NATURAL, LATERAL | Recognized, not supported (`COHDBL001`) | Outside this join subset |
| Qualified wildcard | Recognized, not supported (`COHDBL001`) | List qualified columns explicitly |

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `COHSQLE015` | Unknown/hidden/out-of-scope qualifier, ambiguous column, missing/repeated/ambiguous USING name, or no common USING type; rejected during planning, including empty inputs |
| `COHSQLE019` | Duplicate table binding or alias |
| `COHDBL001` | Unsupported join/wildcard, or INNER/LEFT without ON or USING |
| `SQL0003` | Malformed join/USING list, both ON and USING, or CROSS with either condition |

These join diagnostics have no SQLSTATE assigned by the source.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
CREATE TABLE profiles (user_id INT REFERENCES t(id), email TEXT);
INSERT INTO profiles VALUES (1, 'ada@example.test'), (3, 'alan@example.test');
SELECT t.name, p.email
FROM t INNER JOIN profiles p ON t.id = p.user_id
ORDER BY t.id;
```

The result contains Ada's and Alan's matching email addresses.

```sql
SELECT s.TABLE_NAME, c.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLES s
JOIN INFORMATION_SCHEMA.COLUMNS c ON s.TABLE_NAME = c.TABLE_NAME
WHERE s.TABLE_NAME = 'profiles' ORDER BY c.ORDINAL_POSITION;
```

In this fixture the result is `(profiles, user_id)`, `(profiles, email)`.

### LEFT, USING and computed output

Execute each statement separately:

```sql
CREATE TABLE a (id INT NOT NULL);
CREATE TABLE b (id DECIMAL NOT NULL);
CREATE TABLE c (id INT NOT NULL, enabled BOOLEAN);
INSERT INTO a VALUES (1), (2), (3);
INSERT INTO b VALUES (1), (2), (3);
INSERT INTO c VALUES (1, TRUE), (2, FALSE);
SELECT a.id, c.id
FROM a JOIN b ON a.id = b.id
LEFT OUTER JOIN c ON b.id = c.id AND c.enabled = TRUE
ORDER BY a.id;
SELECT *, id / 2 AS ratio FROM a LEFT JOIN b USING (id) ORDER BY ratio;
SELECT COUNT(*) FROM a CROSS JOIN b;
SELECT COUNT(*) FROM a, b;
```

The first query returns `(1, 1)`, `(2, NULL)`, `(3, NULL)`. Moving `c.enabled = TRUE`
to WHERE leaves only `(1, 1)`. The USING query has columns `id`, `ratio`, both Decimal,
and returns `(1, 0.5)`, `(2, 1)`, `(3, 1.5)`. Each product count is `9`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinIndexSemanticsTests.cs`
- **N-way, LEFT, USING and output types** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlNWayJoinTests.cs`
- **Syntax boundaries** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJoinShapeTests.cs`
