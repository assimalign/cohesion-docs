# EXPLAIN (Cohesion SQL)

Describes the chosen plan; ANALYZE executes a SELECT to collect candidate-work counts.

> **Status:** Implemented. Supports data statements.

## Syntax

```syntaxsql
EXPLAIN [ ANALYZE ] { select_statement | insert_statement | update_statement | delete_statement | merge_statement } [ ; ]
EXPLAIN { create_sequence_statement | alter_sequence_statement | drop_sequence_statement } [ ; ]
```

## Remarks

[WITH RECURSIVE](../clauses/with.md#with-recursive) shows RecursiveUnion and WorkTableScan. Plain
EXPLAIN does not produce recursive rows. EXPLAIN ANALYZE executes the consumer on demand and adds
iterations (including the terminal empty pass), accepted rows and candidates before DISTINCT.

### MERGE plans

Plain EXPLAIN supports executable [MERGE](merge.md#association-and-explain). It shows Merge, the complete association tree, ordered MergeAction children and
RETURNING names. Merge detail includes the association kind and `actions=written_order`. Action
details identify match kind, operation, optional condition and column counts. Action-condition
subqueries appear under their action. EXPLAIN performs scope, reachability and FULL hash-safety
checks. ANALYZE remains SELECT-only.

### RIGHT and grouped joins

RIGHT [OUTER] JOIN and parenthesized joins execute. EXPLAIN identifies `normalized RIGHT to LEFT`
and the chosen hash, index or buffered path. JoinStep nodes nest by physical association. Both plain
EXPLAIN and EXPLAIN ANALYZE emit the complete physical outer subtree before the correlated inner
IndexSeek, including derived outer inputs. Visible query columns still follow written order.
For the [RIGHT example](../clauses/join.md#right-using-and-groups), execute separately:

```sql
CREATE INDEX ix_a ON a(id);
EXPLAIN SELECT * FROM a RIGHT JOIN b ON a.id = b.id;
```

The description includes the dbo.b outer TableScan before the dbo.a IndexSeek on ix_a, beneath
the normalized JoinStep. Plain EXPLAIN reads no data rows and changes no table contents.

FULL retains its written children and reports `FullHashJoin; unmatched=both` in JoinStep detail.
It always builds the written right child. EXPLAIN still performs FULL's hash-safety validation;
an unsupported condition reports CS05070 before reading inputs. For example, on the
[FULL fixture](../clauses/join.md#full-using-and-groups),
`EXPLAIN SELECT * FROM a FULL JOIN b USING (id)` describes FullHashJoin without executing it.
ANALYZE's actual_rows counts candidate pairs, excluding the separately emitted unmatched rows.

### Window frames

WindowAgg detail retains each function's frame and sort_shared for compatible partition/order
specifications. Different frames can share the sort. EXPLAIN ANALYZE reports frame_visits
(distinct safe native inputs or fallback candidates before FILTER), transitions, inverse_transitions
and boundary_visits. It executes full partitions even with LIMIT. Application/order-sensitive
aggregates and restart-sensitive inputs retain [CS05084](../clauses/window.md#recomputation-limit).
Plain EXPLAIN does not evaluate frame offsets or aggregate inputs.

### Planning and results

[Grouping sets](../clauses/group-by.md#grouping-sets-rollup-and-cube) add the expanded set count
and `set_keys` listing to Group detail, including duplicate sets and zero-based grouping-key
ordinals. `CUBE(a,b)` reports `grouping_sets=4; set_keys=[(),(0),(1),(0,1)]`. Detail also identifies
mask bit order and statement-budget/no-spill behavior. Plain GROUP BY keeps its original plan path.

Plain EXPLAIN performs the statement's normal parameter validation, name binding, planning and
provisioning admission. It waits for the same reserved tables as that statement. It never calls its
data executor, even inside a transaction that later commits. Sequence DDL is also described without
execution. Other DDL, transaction control and nested EXPLAIN report `CK00001`.
[Sequence calls](../functions/sequences.md) allocate nothing under plain EXPLAIN; EXPLAIN ANALYZE
SELECT executes their effects. EXPLAIN ANALYZE accepts SELECT query bodies, including read-only WITH
and set operations. Data changes, including WITH data changes, normally report `CS05057` (0A000)
before execution; the earlier system-relation read-only check can retain `CS04021`. Standalone
ANALYZE remains unsupported. Ordinary planning errors retain their codes.

Optional cost probes that observe a committed table-definition change discard their estimates
and retain the bound access path, for SELECT and plain EXPLAIN. This race does not report
CS04006. Mutation execution still checks its target definition and can report CS04006
(no SQLSTATE); roll back and retry that mutation in a fresh transaction. Plain EXPLAIN
never executes its wrapped data statement.

All three query languages use the [DatabasePlanDescription](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-execution/database-plan-description.md)
shape, in depth-first pre-order:

| Column | Type | Nullable |
| --- | --- | --- |
| node_id | Int32 | No |
| parent_id | Int32 | Yes; root has NULL |
| operator | String | No |
| target | String | Yes |
| index | String | Yes |
| detail | String | Yes |
| estimated_rows | Int64 | Yes; probe count or explicitly heuristic estimate when available |
| actual_rows | Int64 | Yes; NULL for plain EXPLAIN, measured candidates for ANALYZE |

SQL operators include TableScan, IndexSeek, SystemView, NestedLoopJoin, HashJoin, JoinStep, Group, Insert, InsertSelect,
Update and Delete. IndexSeek includes the chosen index and bounds. These are model descriptions, not
an execution API. SQL carries the rows through ordinary result frames. A source test guard requires
every concrete plan record to have a description mapping.

Update and Delete have a chosen IndexSeek or TableScan child, using the same
[predicate eligibility as SELECT](select.md#index-access). Leading equality prefixes and supported
range comparisons/BETWEEN supply bounds; NULL/scalar-subquery comparands and computed key expressions do not.
Aliases and qualified target columns are supported. The full WHERE remains a residual check.
EXPLAIN executes neither target collection nor mutation; the actual statement materializes all
targets before writes, and falls back to a scan if its bound index was dropped or replaced.
N-way joins expose one JoinStep child per join with its kind and scan/seek access, including
steps whose inputs will be empty. Plan output uses the same eight columns.
The first child describes the chosen driving input, which can now be an IndexSeek with constant
bounds from a selective predicate. Probe costs can choose either first INNER input, including when the cheaper input is written second;
complete unique-key paths retain their cheap lookup. JoinStep children describe the other inputs'
constant or correlated seeks, or scans. Only [proven non-throwing restrictions](../clauses/join.md#predicate-safety)
can move into an input. EXPLAIN describes the planned access; execution falls back to a scan if a
planned input or inner-probe index was dropped or replaced. The eight-column shape is unchanged; estimated_rows and detail carry available cost observations.
Derived inputs add a DerivedTable node with their child plan. UPDATE/DELETE predicate subqueries
are described beneath Subqueries or CorrelatedSubqueries; EXPLAIN does not materialize their data or mutate the target.

[WITH](../clauses/with.md) descriptions add With, Materialize and CteScan operators. Constant
marks a FROM-less CTE body. `EXPLAIN WITH a AS (SELECT 1 AS id) SELECT * FROM a` yields
With → Materialize → Constant, followed by Select → CteScan, without evaluating the body.
CorrelatedSubqueries retains the complete child plans evaluated for each outer row.

[Set-operation](../clauses/set-operations.md) bodies use Union, Intersect and Except nodes with
operand plans beneath them. Compatible same-quantifier UNION chains may have N children.
Combined ordering/paging and DISTINCT state appear in detail. For example, after creating
`a(id INT)` and `b(id INT)`, `EXPLAIN SELECT id FROM a UNION ALL SELECT id FROM b;`
returns a Union node with two Select/TableScan operand plans, without reading their rows.

ORDER BY detail records effective placement for every key, for example
`order_by=[ASC NULLS FIRST, DESC NULLS LAST]`. DML with [RETURNING](../clauses/returning.md)
adds the expanded count and output names, such as `returning=2; columns=id, name`, without
executing projections.

## Examples

On the [fixture](select.md#a-create-the-conformance-fixture):

```sql
EXPLAIN DELETE FROM t WHERE id = 1;
SELECT id FROM t ORDER BY id;
```

EXPLAIN returns eight-column plan rows including a Delete operator. The SELECT still returns `1`,
`2`, `3`. An indexed `EXPLAIN SELECT id FROM t WHERE age >= 20 AND age < 40` reports IndexSeek with
that bound when an eligible age index exists.

On an empty database, execute the definition and each EXPLAIN separately:

```sql
CREATE TABLE t (id INT PRIMARY KEY, v INT);
EXPLAIN UPDATE dbo.t AS x SET v = 3 WHERE x.id = 2;
EXPLAIN DELETE FROM dbo.t AS x WHERE x.id = 2;
```

The first returns Update → IndexSeek and the second Delete → IndexSeek. The seek target is dbo.t,
index names the primary-key index, and detail is `id = 2`. actual_rows remains NULL; estimated_rows can contain a probe observation.
No rows are changed. SqlExplainTests checks the same shape with `@key` bound to 2.

With the [three-input JOIN fixture](../clauses/join.md#left-using-and-computed-output):

```sql
EXPLAIN SELECT * FROM a JOIN b ON a.id = b.id LEFT JOIN c ON b.id = c.id;
```

The description contains two JoinStep children, one Inner and one LeftOuter, with table scans
under the hash joins because this fixture defines no indexes. Unknown counts use the 4,096-row
heuristic, so the tiny actual inputs do not prevent hash selection. It returns plan rows without
reading or changing data rows.

### A selective driving input

On an empty database, execute each statement separately:

```sql
CREATE TABLE orders (id INT PRIMARY KEY);
CREATE TABLE lines (order_id INT, line INT);
INSERT INTO orders VALUES (1), (2);
INSERT INTO lines VALUES (1, 10), (1, 11), (2, 20);
CREATE INDEX ix_lines ON lines(order_id);
EXPLAIN SELECT o.id, l.line FROM lines l JOIN orders o ON l.order_id = o.id
WHERE o.id = 1 ORDER BY l.line;
SELECT o.id, l.line FROM lines l JOIN orders o ON l.order_id = o.id
WHERE o.id = 1 ORDER BY l.line;
```

The plan's driving child is IndexSeek on `dbo.orders`, with detail `id = 1` and its primary-key
index name. The JoinStep has an IndexSeek on `dbo.lines` using `ix_lines` with a correlated key.
The SELECT returns `(1, 10)` and `(1, 11)`. actual_rows stays NULL; available estimates are reported with probe qualifications.

## See also

[Statements](index.md) · [SELECT](select.md) · [Diagnostics](../diagnostics.md)

## Estimates and analysis

Planning uses bounded snapshot-visible range-count probes, at most 512 physical entries and 32 page visits per probe. Invisible entries and the stopping key consume work. Detail distinguishes exact_probe, lower_bound and heuristic estimates; an unfinished probe is not an exact cardinality. No persisted statistics or standalone ANALYZE command is introduced. Plain EXPLAIN can read indexes for costing but never invokes the wrapped data executor.

Probes use tree read latches and catalog identity validation without transaction object locks.
The primary-key/base-table total is probed only when a selected seek estimate exceeds 16.
Join estimates are lazy and reuse input counts; complete unique probes skip pair costing.

EXPLAIN ANALYZE executes and drains its SELECT under ordinary snapshot, cancellation, timeout and hash-memory rules. Scalar expressions and application functions run normally. Instrumented actual_rows counts candidates before residual predicates rather than final output rows. Repeated correlated invocations accumulate on child nodes; parent counts exclude descendant work. Unmeasured nodes remain NULL.

Separate references to a cached view body have independent relation-path counters, including
nested views. Repeated correlated invocations at one reference still accumulate. A SELECT with
one derived input counts its input candidates without incrementing join-pair totals.

On the fixture above, execute separately:

```sql
EXPLAIN ANALYZE SELECT id FROM t WHERE id >= 1;
```

The returned rows describe the executed SELECT, including measured work where available. See [query performance](../../performance.md) and [hash joins](../clauses/join.md#hash-joins-and-memory).

## Source DML

EXPLAIN UPDATE ... FROM and EXPLAIN DELETE ... USING describe the executable association join
without running source callbacks, SET/RETURNING expressions or writes. The target stays outer.
Target-only predicates may show an IndexSeek on the target; mandatory target/source equality
may show an index-probe step on the source or HashJoin; JoinStep detail includes `hash_budget=no_spill`. Hash builds
and retained joined-DML targets, sources, associations and selected writes are charged to
StatementMemoryBudget during execution (CS05067 / 53200 before any write). EXPLAIN also shows the target-first,
visible-source RETURNING * expansion.

For stored t(id INT PRIMARY KEY,v INT) and s(id INT PRIMARY KEY,v INT):

```sql
EXPLAIN UPDATE t SET v = s.v FROM s WHERE t.id = s.id AND t.id = 1 RETURNING *;
```

The plan includes target/source IndexSeek paths and a four-column RETURNING description
(id, v, id, v). General join-order search is deferred; equality-driven source steps follow
written FROM/USING order. See [performance](../../performance.md#joined-mutations).
EXPLAIN ANALYZE remains SELECT-only.

## Sources

- **Plans and no execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlExplainTests.cs`.
- **Shape** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/src/DatabasePlanDescription.cs`.
- **Join steps** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlNWayJoinTests.cs`.
- **Selective join plans** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinExecutionTests.cs`.
- **Replaced-index fallback** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinIndexSemanticsTests.cs`.
