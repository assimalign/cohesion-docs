# JOIN (Cohesion SQL)

Combines exactly two stored tables through an inner join with an ON predicate.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
FROM <table_source>
[ INNER ] JOIN <table_source> ON search_condition

<table_source> ::= [ schema_name . ] table_name [ [ AS ] table_alias ]
```

## Arguments

- **`table_source`** — One of the two stored-table inputs.
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
disjunction, and absent usable indexes use a scan. Approximate numeric, `DateTime`, and
`DateTimeOffset` join comparisons conservatively scan. The fallback buffers the right input once,
with O(L + R) stored-row reads, O(L × R) predicate evaluations, and O(R) input buffering.

| Form | Support | Boundary |
| --- | --- | --- |
| `INNER JOIN ... ON`, `JOIN ... ON` | Supported (measured) | Two stored tables |
| `LEFT`, `RIGHT`, `FULL` joins, with or without `OUTER` | Recognized, not supported (`COHDBL001`) | No null-extension semantics |
| `CROSS`, `NATURAL`, `USING`, absent `ON`, comma joins | Recognized, not supported (`COHDBL001`) | Use an explicit inner join predicate |
| Third or later table, system-relation input | Recognized, not supported (`COHDBL001`) | Outside the two stored-table subset |
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

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinIndexSemanticsTests.cs`

