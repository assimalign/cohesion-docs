# merge_action (Cohesion SQL)

Returns the applied action as TEXT in MERGE RETURNING.

## Syntax

```syntaxsql
merge_action()
```

## Remarks

Use only within [MERGE RETURNING](../statements/merge.md), including nested scalar expressions
such as `UPPER(merge_action())`. Placement elsewhere reports CS01053; arguments, DISTINCT/ALL or
input ordering report CS01006 (42883). The expression uses scalar-call syntax but is separate
from the executable function catalog, and an application overload cannot replace its binding.
It yields `INSERT`, `UPDATE` or `DELETE` for each directly changed row. DO NOTHING produces no
RETURNING row. Target columns alongside it contain new INSERT/UPDATE values or old DELETE values;
source columns are NULL for BY SOURCE. SQL has no MISSING value.

## Example

Execute separately in a fresh database:

```sql
CREATE TABLE t (id INT);
CREATE TABLE s (id INT);
INSERT INTO t VALUES (1);
INSERT INTO s VALUES (1);
MERGE INTO t USING s ON t.id=s.id
WHEN MATCHED THEN DELETE RETURNING merge_action() AS action;
```

The result is one row with action = 'DELETE', an affected count of 1 and an empty t table.
`SELECT merge_action();` instead reports CS01053 (no assigned SQLSTATE).

## See also

[Functions](index.md) · [RETURNING](../clauses/returning.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Merge.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeExecutionTests.cs`.
