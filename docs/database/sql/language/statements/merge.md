# MERGE (Cohesion SQL)

Applies ordered INSERT, UPDATE or DELETE actions over a target/source association.

> **Status:** Execution, including ReadCommitted target rechecks and mixed-action final-state constraints.

## Syntax

```syntaxsql
[ WITH common_table_expression [ , ...n ] ]
MERGE INTO [ schema_name . ] table_name [ [ AS ] target_alias ]
USING source [ [ AS ] source_alias ] ON condition
<when_clause> [ ...n ]
[ RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] ] [ ; ]

<when_clause> ::=
    WHEN MATCHED [ AND condition ] THEN
        { UPDATE SET column_name = expression [ , ...n ] | DELETE | DO NOTHING }
  | WHEN NOT MATCHED [ BY TARGET ] [ AND condition ] THEN
        { INSERT [ ( column_name [ , ...n ] ) ] [ OVERRIDING { SYSTEM | USER } VALUE ]
            VALUES ( expression [ , ...n ] )
        | INSERT [ OVERRIDING { SYSTEM | USER } VALUE ] DEFAULT VALUES | DO NOTHING }
  | WHEN NOT MATCHED BY SOURCE [ AND condition ] THEN
        { UPDATE SET column_name = expression [ , ...n ] | DELETE | DO NOTHING }
```

Plain WITH can supply the USING source. For example, execute separately in a fresh database:

```sql
CREATE TABLE t (id INT);
WITH s AS (SELECT 1 AS id)
MERGE INTO t USING s ON t.id = s.id
WHEN NOT MATCHED THEN INSERT VALUES (s.id) RETURNING t.id;
```

MERGE inserts one row and returns `1`. WITH RECURSIVE before the same MERGE reports CK00001.

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

[Identity overrides](../clauses/identity.md#assignment-and-overrides) apply to MERGE INSERT.
OVERRIDING SYSTEM VALUE permits explicit ALWAYS values; USER VALUE generates after validating
ignored assignment inputs. UPDATE actions may assign DEFAULT to ALWAYS identities; other explicit
assignments refuse. Generated values update currval/lastval and can leave gaps on failure.

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
respective scopes, and action values admit existing read subqueries. RETURNING subqueries retain
CK00001. Plain [WITH](../clauses/with.md) definitions bind in declaration order; reachable definitions
materialize once before writes. WITH RECURSIVE ... MERGE is refused at RECURSIVE with CK00001,
even for acyclic lists.

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
Only direct actions count or return rows; cascades do not. RETURNING describes direct values
before referential effects, even if a cascade subsequently removes an updated or inserted row.
Output order is unspecified.

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

EXPLAIN shows a Merge node with the association kind and `actions=written_order`.
Its children describe the full association tree and ordered MergeAction
nodes. Action details include order, match kind, operation, condition presence, assignment count
and INSERT column count. Condition subqueries appear beneath their owning action. RETURNING
count/names are included. Physical slots remain target then source even after RIGHT normalization.

Plain EXPLAIN succeeds without applying actions; EXPLAIN ANALYZE remains SELECT-only.

### Atomicity, repeated targets and memory

The executor drains association cursors before selecting actions, acquiring row write locks or
writing, preventing revisits caused by its own changes. A second selected UPDATE or DELETE on
one physical target reports CS05036 (21000) before writes, including after target rechecks.
Extra matches selecting DO NOTHING do not count. Direct and referential writes to the same row
compose without the earlier conservative CS05036 refusal.

All direct actions, index changes, referential actions and RETURNING preparation share one
statement rollback bracket. Constraint, projection, encoding or cancellation failure undoes the
whole statement; earlier successful statements in an explicit transaction remain. Retryable
serialization refusals abort the transaction and require a full transaction retry after rollback.

StatementMemoryBudget covers hash builds and retained target identities, source buffers,
derived copies, intermediate frames, associations, selected values and encoded replacements,
including joined-statement cascade state. Exhaustion reports CS05067 (53200) before any write;
there is no spill. General result materialization and transient expression values are outside
this grant. Reservations release on every exit. See [memory options](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-engine-options.md#hash-build-memory).

### Concurrent target changes

Suppose another transaction updates or deletes a target chosen for a modifying action and commits
after this statement's snapshot, including while MERGE waits for its row lock. Under ReadCommitted,
MERGE locks the latest version and keeps the original source association. For an originally
MATCHED candidate, it reevaluates the full ON predicate before selecting ordered WHEN clauses
against that target. An original BY SOURCE candidate reselects clauses of that match kind.
Scalar assignments reevaluate;
independent action subqueries keep their statement cache. Only reselected modifying actions
enter repeated-target detection.

If ON is no longer true, the current target becomes NOT MATCHED BY SOURCE and the original source
becomes NOT MATCHED BY TARGET, so both action paths can run. A deleted MATCHED target leaves only
the BY TARGET path. An original BY SOURCE row never becomes MATCHED; if concurrently deleted,
it contributes no action. Competing INSERTs use ordinary uniqueness checks without rescanning
or ON CONFLICT arbitration.

Under Snapshot the changed target raises retryable DatabaseTransactionAbortedException with
CS04064 (40001). Within MERGE's row-concurrency path this code identifies target recheck or
apply-time modified-row version conflicts. Shared cascade snapshot crosschecks, referenced-parent
version checks, host-ended transactions and abandoned lock owners retain their ordinary uncoded
transaction aborts. CS04064 also retains its separate DDL/TRUNCATE uses.

### Final-state constraints and uniqueness

Foreign keys see the statement's composed final INSERT/UPDATE/DELETE rows, including CASCADE,
SET NULL and SET DEFAULT effects. A directly updated child moved away from a deleted parent
survives. Overlapping SET actions modify its final replacement; CASCADE may remove it.
NO ACTION accepts a replacement parent key supplied by the same statement; RESTRICT refuses
that substitution. A moved-then-cascaded child retains locks on its old stored parent through
commit or rollback.

An UPDATE later removed by a cascade still checks children referencing its old stored parent key
against the final rows. A surviving child left without that key refuses the entire statement
with CS03039 (23503). Cascades also cannot hide invalid direct CHECK/domain or unique proposals.
Direct-proposal uniqueness failures (CS03037, 23505) take precedence over final-state FK failures
and NOT NULL, CHECK, domain or identity failures from referential SET actions. A retained constraint
failure skips further parent-row locking, so a stale referenced parent does not mask that error.

**Difference from PostgreSQL:** Cohesion checks unique keys at statement level, accepting key
swaps that PostgreSQL's immediate uniqueness rejects. MERGE can also reuse keys freed by its
own DELETE actions. Duplicate direct proposals still fail even if a later cascade would remove
them; a competing INSERT remains subject to ordinary uniqueness enforcement.

### Wire behavior

InMemory and TCP carry the same action rows and affected count. Only direct actions contribute;
an all-DO-NOTHING MERGE RETURNING has no rows and affected count 0. Constraint or late RETURNING
failure publishes no partial rows and restores the statement's original rows, preserving earlier
explicit-transaction writes. SqlClientException.ConnectionUsable remains true for these refusals.
Wire BEGIN uses Snapshot; a concurrent UPDATE/DELETE commit produces CS04064 and requires
ROLLBACK before reuse. ReadCommitted is selected through the engine transaction API.
SQL storage format 30 and wire frames are unchanged.

[ON UPDATE actions](../constraints.md#on-update-declarations) execute CASCADE, SET NULL and
SET DEFAULT for referenced-key changes. RESTRICT is immediate; NO ACTION checks the final state.
RETURNING shows direct writes before cascades. Conflicting action origins report CS05085 (27000);
every referential closure shares StatementMemoryBudget (CS05067 / 53200).

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK00001 | Subqueries in RETURNING or qualified wildcard | — |
| CS05036 | Second modifying action on a physical target, including after rechecks | 21000 |
| CS04064 | Snapshot target conflict or failed target recheck/apply-time modified-row version check; retry the transaction | 40001 |
| CS03037 | Duplicate direct unique-key proposal or final unique-key collision; precedes final FK/SET-action failures | 23505 |
| CS03039 | Final FK failure or surviving child of an old parent key, including a cascaded UPDATE | 23503 |
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

### Overlapping direct and referential actions

In a fresh database, adapted from SqlMergeConstraintTests:

```sql
CREATE TABLE t(id INT PRIMARY KEY,
  pid INT REFERENCES t(id) ON DELETE CASCADE,v INT);
INSERT INTO t VALUES(1,NULL,10),(2,1,20),(9,NULL,90);
MERGE INTO t USING (SELECT 1 AS id UNION ALL SELECT 2) AS s ON t.id=s.id
WHEN MATCHED AND t.id=1 THEN DELETE
WHEN MATCHED THEN UPDATE SET v=t.v+1
RETURNING t.id,t.pid,t.v,merge_action();
```

Affected count is 2; RETURNING contains `(1,NULL,10,'DELETE')` and `(2,1,21,'UPDATE')` in
unspecified order. The cascade removes row 2, so only `(9,NULL,90)` remains stored. Changing
the UPDATE to `SET pid=9,v=t.v+1` moves the child away; rows 2 and 9 survive.

### Unique-key swaps

Execute separately in a fresh database:

```sql
CREATE TABLE t(id INT PRIMARY KEY,v INT);
INSERT INTO t VALUES(1,10),(2,20);
MERGE INTO t USING (SELECT 1 AS id UNION ALL SELECT 2) AS s ON t.id=s.id
WHEN MATCHED THEN UPDATE SET id=3-t.id;
SELECT id,v FROM t ORDER BY id;
```

MERGE affects 2 rows. SELECT returns `(1,20)` and `(2,10)`. PostgreSQL's immediate unique-key
checks reject this swap.

## See also

[Statements](index.md) · [UPDATE FROM](update.md#from-sources) · [RETURNING](../clauses/returning.md) · [EXPLAIN](explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Merge.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeBindingTests.cs` and `SqlMergeSourceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeExecutionTests.cs` and `SqlJoinedDmlMemoryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeConcurrencyTests.cs`, `SqlMergeConstraintTests.cs` and `SqlMergeReviewRegressionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Merge.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlMergeWireTests.cs` and `SqlMergeConcurrencyWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Merge.cs` and `docs/DIALECT.md`.
