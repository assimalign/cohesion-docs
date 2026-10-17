# JOIN (Cohesion SQL)

Combines exactly two stored tables or registered system relations through an inner join with an ON predicate.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
FROM <table_source>
[ INNER ] JOIN <table_source> ON search_condition

<table_source> ::= [ schema_name . ] table_name [ [ AS ] table_alias ]
```

## Arguments

- **`table_source`** — A stored table or an INFORMATION_SCHEMA/COHESION_SCHEMA relation.
- **`search_condition`** — The predicate evaluated for each candidate pair.

## Remarks

Only `TRUE` matches; `FALSE` and `UNKNOWN` do not. Both inputs share the statement's
multi-version concurrency control (MVCC) snapshot. Multiplicity is preserved, and an empty input
produces no joined rows. Projections, filters, distinct results, grouping, aggregates, ordering, and
pagination compose with this subset.

Unqualified names present in both inputs are ambiguous even on empty inputs. Qualify them with
table names, aliases, or schema/table names. Explicit aliases distinguish repeated table inputs.
Unqualified `*` expands the `FROM` columns then the joined columns; qualified wildcards are excluded.

A mandatory column equality matching a secondary index's leading key prefix can use index probes.
Selection prefers the longest prefix, a unique index, index name, then the right input. The full
`ON` predicate is still evaluated. Computed keys, inequalities, equalities reachable only through
disjunction, and absent usable indexes use a scan. System inputs have no index paths or object
locks; an eligible stored input can still supply index probes. Approximate numeric joins scan. The fallback buffers the right input once,
with O(L + R) stored-row reads, O(L × R) predicate evaluations, and O(R) input buffering.

| Form | Support | Boundary |
| --- | --- | --- |
| `INNER JOIN ... ON`, `JOIN ... ON` | Supported (measured) | Two stored or system relations |
| `LEFT`, `RIGHT`, `FULL` joins, with or without `OUTER` | Recognized, not supported (`COHDBL001`) | No null-extension semantics |
| `CROSS`, `NATURAL`, `USING`, absent `ON`, comma joins | Recognized, not supported (`COHDBL001`) | Use an explicit inner join predicate |
| Third or later relation | Recognized, not supported (`COHDBL001`) | Outside the two-relation subset |
| Qualified wildcard | Recognized, not supported (`COHDBL001`) | List qualified columns explicitly |

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

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinIndexSemanticsTests.cs`
