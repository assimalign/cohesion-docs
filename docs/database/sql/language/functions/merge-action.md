# merge_action (Cohesion SQL)

Binds a contextual TEXT action slot in MERGE RETURNING.

> **Status:** Binding and EXPLAIN only. MERGE execution reports CK00001 in batch 5.

## Syntax

```syntaxsql
merge_action()
```

## Remarks

Use only within [MERGE RETURNING](../statements/merge.md), including nested scalar expressions
such as `UPPER(merge_action())`. Placement elsewhere reports CS01053; arguments, DISTINCT/ALL or
input ordering report CS01006 (42883). The expression uses scalar-call syntax but is separate
from the executable function catalog, and an application overload cannot replace its binding.
It has TEXT type; this step refuses execution before any action value or NULL result is produced.
SQL has no MISSING value.

## Example

On the [MERGE fixture](../statements/merge.md#examples):

```sql
EXPLAIN MERGE INTO t USING s ON t.id=s.id
WHEN MATCHED THEN DELETE RETURNING merge_action() AS action;
```

The plan includes a MergeAction for DELETE and RETURNING name `action`. Removing EXPLAIN
reports CK00001. `SELECT merge_action();` instead reports CS01053.

## See also

[Functions](index.md) · [RETURNING](../clauses/returning.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Merge.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeBindingTests.cs`.
