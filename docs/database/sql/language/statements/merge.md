# MERGE (Cohesion SQL)

Parses and binds ordered actions over a target/source association, and exposes the plan through EXPLAIN.

> **Status:** Parsing, binding and EXPLAIN only. Executing MERGE is refused with `CK00001` in
> this release step (batch 5); batch 6 adds execution.

## Syntax

```syntaxsql
MERGE INTO [ schema_name . ] table_name [ [ AS ] target_alias ]
USING source [ [ AS ] source_alias ] ON condition
<when_clause> [ ...n ]
[ RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] ] [ ; ]

<when_clause> ::=
    WHEN MATCHED [ AND condition ] THEN
        { UPDATE SET column_name = expression [ , ...n ] | DELETE | DO NOTHING }
  | WHEN NOT MATCHED [ BY TARGET ] [ AND condition ] THEN
        { INSERT [ ( column_name [ , ...n ] ) ] VALUES ( expression [ , ...n ] )
        | INSERT DEFAULT VALUES | DO NOTHING }
  | WHEN NOT MATCHED BY SOURCE [ AND condition ] THEN
        { UPDATE SET column_name = expression [ , ...n ] | DELETE | DO NOTHING }
```

## Arguments

- **`table_name`** — A stored target table in the session database; unqualified names use dbo.
- **`source`** — The read sources accepted by UPDATE FROM: stored tables, system relations, views,
  aliased derived queries (with optional complete output renames), and supported joined sources,
  including INNER, LEFT and CROSS joins. A derived SELECT may contain RIGHT/FULL joins.
  VALUES relations, table functions, LATERAL, NATURAL JOIN and aliasless derived queries retain
  UPDATE FROM's refusals. USING contains one source relation with its joins, not a comma source list.
- **`condition`** — A Boolean ON association predicate or optional WHEN action predicate.
- **`expression`** — An assignment/INSERT value or RETURNING projection. Whole-item DEFAULT is
  accepted in assignment and INSERT value positions; ordinary identity rules apply.

## Remarks

At least one WHEN is required. Clauses retain written order; the first qualifying clause of a
candidate's match kind identifies its action in the plan. NOT MATCHED defaults to BY TARGET.
An unconditional clause, including DO NOTHING, makes any later clause of the **same match kind**
unreachable (CS05071). A conditional clause is not terminal merely because its expression is TRUE.
WHEN filters select actions; they are not pushed into ON to classify matches.

### Binding scopes

| Position | Visible row columns |
| --- | --- |
| ON | Target and source |
| MATCHED AND predicate and UPDATE values | Target and source |
| NOT MATCHED [BY TARGET] AND predicate and INSERT values | Source only |
| NOT MATCHED BY SOURCE AND predicate and UPDATE values | Target only |
| RETURNING | Target and source, plus contextual merge_action() |

Aliases hide original table names. Destinations in UPDATE SET and INSERT lists always resolve
against the target. Duplicate destinations, wrong INSERT width, unknown/ambiguous names,
missing parameters, non-Boolean predicates, aggregates and incompatible assignment families fail
during binding, even for empty inputs. ON and WHEN permit existing read subqueries in their
respective scopes; action values and RETURNING subqueries retain CK00001. WITH-prefixed MERGE
is outside this step's grammar.

SQL uses three-valued predicates: NULL does not satisfy ON or a WHEN condition. There is no
MISSING value. RETURNING's source metadata includes NULL extension for absent sources; target
outputs retain their declared nullability. Bare * expands target columns then visible source
columns, including JOIN USING merges. Qualified wildcards retain CK00001.
[merge_action()](../functions/merge-action.md) is a zero-argument TEXT expression bound only in
MERGE RETURNING, including inside scalar expressions. This step produces no changed-row results.

### Association and EXPLAIN

Ignore DO NOTHING clauses when choosing which unmatched rows need preservation:

| Unmatched action kinds present | Planned association |
| --- | --- |
| Neither | Target INNER source |
| BY SOURCE only | Target LEFT source |
| BY TARGET only | Target RIGHT source, physically normalized to source LEFT target |
| Both | Target FULL source |

FULL requires a safe mandatory hash equality and a complete non-throwing ON proof, as ordinary
FULL JOIN does. Otherwise CS05070 refuses planning, including EXPLAIN, before reading inputs.
LEFT and RIGHT associations accept hashless ON predicates.

EXPLAIN shows a Merge node with the association kind, `actions=written_order` and
`execution=unsupported`; its children describe the full association tree and ordered MergeAction
nodes. Action details include order, match kind, operation, condition presence, assignment count
and INSERT column count. Condition subqueries appear beneath their owning action. RETURNING
count/names are included. Physical slots remain target then source even after RIGHT normalization.

A successfully bound MERGE is refused with CK00001 before source-index costing, source/subquery
materialization or action evaluation. Ordinary binding errors can therefore precede that refusal.
Plain EXPLAIN succeeds for valid plans; EXPLAIN ANALYZE remains SELECT-only. This step changes no
disk or wire format.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK00001 | Bound MERGE execution in this release step; subqueries in action values/RETURNING | — |
| CS05071 | WHEN follows an unconditional clause of the same match kind | — |
| CS01053 | merge_action() outside MERGE RETURNING | — |
| CS01006 | merge_action has arguments or call modifiers | 42883 |
| CS05070 | FULL association lacks safe hash equality or complete ON safety proof | — |
| CS01015 | Unknown, ambiguous or out-of-scope column/qualifier | 42703 for unknown columns |
| CS01019 | Repeated SET/INSERT destination | — |
| CS05013 | Wrong INSERT width or invalid action shape | — |
| CS02003 | Non-Boolean predicate or incompatible assignment type | 42804 |
| CS05017 | Aggregate in a MERGE action or RETURNING | — |

Other source, parameter and identity failures retain their [ordinary diagnostics](../diagnostics.md).

## Examples

Adapted from SqlMergeBindingTests; run in a fresh database:

```sql
CREATE TABLE t (id INT, v INT);
CREATE TABLE s (id INT, v INT);
EXPLAIN MERGE INTO t USING s ON t.id=s.id
WHEN MATCHED THEN UPDATE SET v=s.v
WHEN NOT MATCHED THEN INSERT (id,v) VALUES(s.id,s.v)
WHEN NOT MATCHED BY SOURCE THEN DELETE
RETURNING t.id,s.v,merge_action() AS action;
```

The result describes Merge with a FullOuter association, three MergeAction children in written
order and RETURNING columns `id`, `v`, `action`. Neither table changes. Removing EXPLAIN reports
CK00001. Replacing `ON t.id=s.id` with `ON TRUE` reports CS05070 even with EXPLAIN.

```sql
EXPLAIN MERGE INTO t USING s ON t.id=s.id
WHEN MATCHED THEN DELETE
WHEN MATCHED AND s.v>0 THEN DO NOTHING;
```

This reports CS05071 at the complete second WHEN clause.

## See also

[Statements](index.md) · [UPDATE FROM](update.md#from-sources) · [RETURNING](../clauses/returning.md) · [EXPLAIN](explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Merge.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeBindingTests.cs` and `SqlMergeSourceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Merge.cs` and `docs/DIALECT.md`.
