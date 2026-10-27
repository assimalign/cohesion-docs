# MERGE (Cohesion SQL)

Applies ordered INSERT, UPDATE or DELETE actions over a target/source association.

> **Status:** Execution, run 2 of 3. ReadCommitted target rechecks and complete mixed-action
> final-state constraint validation remain deferred.

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
candidate's match kind selects its action. DO NOTHING stops action selection for that candidate
and contributes neither an affected row nor a RETURNING row. No qualifying WHEN also does nothing.
NOT MATCHED defaults to BY TARGET.
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
respective scopes, and action values admit existing read subqueries. RETURNING subqueries retain CK00001. WITH-prefixed MERGE
is outside this step's grammar.

Conditions run in written order only for candidates of that match kind. Values and their
subqueries run only for the selected action after its condition holds. Scalar branches and
subqueries are lazy; binding and EXPLAIN do not invoke action callbacks, even immutable ones.

SQL uses three-valued predicates: NULL does not satisfy ON or a WHEN condition. There is no
MISSING value. RETURNING's source metadata includes NULL extension for absent sources; target
outputs retain their declared nullability. Bare * expands target columns then visible source
columns, including JOIN USING merges. Qualified wildcards retain CK00001.
[merge_action()](../functions/merge-action.md) is a zero-argument TEXT expression bound only in
MERGE RETURNING, including inside scalar expressions. It returns `INSERT`, `UPDATE` or `DELETE`.
Target columns contain new values for INSERT/UPDATE and old values for DELETE. BY SOURCE
null-extends source values. UPDATE assignments all read the original association, so swapping
two target columns uses their old values. Ordinary coercion, defaults and identity rules apply.
Only direct actions count or return rows; cascades do not. Output order is unspecified.

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

EXPLAIN shows a Merge node with the association kind and `actions=written_order`; the branch
still emits the stale detail label `execution=unsupported` even though MERGE executes.
Its children describe the full association tree and ordered MergeAction
nodes. Action details include order, match kind, operation, condition presence, assignment count
and INSERT column count. Condition subqueries appear beneath their owning action. RETURNING
count/names are included. Physical slots remain target then source even after RIGHT normalization.

Plain EXPLAIN succeeds without applying actions; EXPLAIN ANALYZE remains SELECT-only.

### Atomicity, repeated targets and memory

The executor drains association cursors before selecting actions, acquiring row write locks or
writing, preventing revisits caused by its own changes. A second selected UPDATE or DELETE on
one physical target reports CS05036 (21000) before writes. Extra matches selecting DO NOTHING
do not count. Competing INSERT keys use ordinary uniqueness enforcement.

All direct actions, index changes, referential actions and RETURNING preparation share one
statement rollback bracket. Constraint, projection, encoding or cancellation failure undoes the
whole statement; earlier successful statements in an explicit transaction remain. CHECK/domain,
FK and referential actions reuse existing DML checks. Overlap between a direct UPDATE and a
referential DELETE/UPDATE is conservatively refused with CS05036.

StatementMemoryBudget covers hash builds and retained target identities, source buffers,
derived copies, intermediate frames, associations, selected values and encoded replacements,
including joined-statement cascade state. Exhaustion reports CS05067 (53200) before any write;
there is no spill. General result materialization and transient expression values are outside
this grant. Reservations release on every exit. See [memory options](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-engine-options.md#hash-build-memory).

ReadCommitted currently refuses stale targets through existing first-updater-wins version checks,
including after a lock wait. PostgreSQL-style target rechecks and complete mixed INSERT/UPDATE/
DELETE final-state constraint composition arrive in run 3. No disk or wire format changes.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK00001 | Subqueries in RETURNING or qualified wildcard | — |
| CS05036 | Second modifying action on a physical target, or overlapping direct UPDATE/referential write | 21000 |
| CS05067 | Retained joined-DML state/hash builds exceed StatementMemoryBudget, before writes | 53200 |
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

Adapted from SqlMergeExecutionTests; execute separately in a fresh database:

```sql
CREATE TABLE t (id INT, v INT);
CREATE TABLE s (id INT, v INT);
INSERT INTO t VALUES (1,10),(2,20);
INSERT INTO s VALUES (1,11),(3,33);
MERGE INTO t USING s ON t.id=s.id
WHEN MATCHED THEN UPDATE SET v=s.v
WHEN NOT MATCHED THEN INSERT (id,v) VALUES(s.id,s.v)
WHEN NOT MATCHED BY SOURCE THEN DELETE
RETURNING t.id,t.v,s.v AS source_v,merge_action() AS action;
```

The affected count is 3. In unspecified order, RETURNING contains `(1,11,11,'UPDATE')`,
`(2,20,NULL,'DELETE')` and `(3,33,33,'INSERT')`. The stored rows are `(1,11)` and `(3,33)`.
Prefixing MERGE with EXPLAIN describes a FullOuter association and ordered MergeAction children
without writes. Replacing `ON t.id=s.id` with `ON TRUE` reports CS05070 even with EXPLAIN.

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
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeExecutionTests.cs` and `SqlJoinedDmlMemoryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Merge.cs` and `docs/DIALECT.md`.
