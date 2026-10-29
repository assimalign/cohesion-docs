# SQL query performance

SQL chooses among eligible access paths using bounded probes under the statement snapshot. No
persisted statistics or standalone ANALYZE command exists. Each range count examines at most 512
physical entries and 32 page visits; invisible entries, descent pages and stopping keys consume
work. Incomplete counts are lower bounds, and unknown inputs use heuristics. Probes use the tree
read latch and validate table/index identities around the count, without transaction object locks.

Costing is lazy. Complete unique-key equality lookups avoid probes. The primary-key/base-table
total is probed only when the selected seek estimate exceeds 16 candidates. Join estimates reuse
input access counts and run only for hash choices or competing indexed directions; a complete
unique first-step probe skips pair costing, and later unique/indexed steps need no pair estimates.

Eligible equijoins between tables without countable indexes select an in-memory hash join
regardless of actual size, including empty or tiny inputs: both estimates use the 4,096-row
heuristic. Small exact indexed inputs and complete unique probes retain index nested
loops. Any ON reference to a derived String/Json/Character column prevents hash selection, even
in a residual term, preserving deferred collation/Unicode errors. Full predicates, duplicates,
NULL and LEFT semantics remain intact. See [joins](language/clauses/join.md#hash-joins-and-memory).

Configure the engine before creation:

```csharp
using Assimalign.Cohesion.Database.Sql;

var options = new SqlDatabaseEngineOptions { StatementMemoryBudget = 32L * 1024 * 1024 };
await using var engine = SqlDatabaseEngine.Create("orders", options);
```

The positive byte budget accounts for concurrently live hash builds, including nested queries and growth overlap. Exhaustion reports CS05067 (53200); there is no spill. [CTAS](language/statements/create-table.md#create-table-as) also charges retained source results and copy buffers. [Joined mutations](#joined-mutations) also charge retained targets, sources, associations and
selected writes. Other result materialization and transient planning remain outside this accounting.

FULL JOIN always hashes the written right child, including duplicate and NULL-key rows, with
match flags charged to the same budget. There is no loop fallback: conditions without a safe
mandatory hash equality, or whose full ON evaluation is not proven non-throwing, fail CS05070
(no SQLSTATE) before input reads. EXPLAIN describes FullHashJoin with `unmatched=both`.

Use [EXPLAIN](language/statements/explain.md) to inspect costs without executing the wrapped data statement. EXPLAIN ANALYZE accepts SELECT query bodies and runs expressions/application functions normally; actual_rows reports measured candidates before residuals, not final output cardinality. Estimates distinguish exact probes, lower bounds and heuristics.

Optional cost probes that observe a committed table-definition change discard their estimates
and retain the bound access path, for SELECT and plain EXPLAIN. This race does not report
CS04006. Mutation execution still checks its target definition and can report CS04006
(no SQLSTATE); roll back and retry that mutation in a fresh transaction. Plain EXPLAIN
never executes its wrapped data statement. Analysis scopes cached view bodies by relation path, so separate references report
independent counts while repeated correlated calls to the same reference accumulate.

[SQL](index.md) · [Engine options](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-engine-options.md)

## Joined mutations

[UPDATE FROM](language/statements/update.md#from-sources) and
[DELETE USING](language/statements/delete.md#using-sources) use a join with the target pinned
outer. Safe target-only predicates can seek it; mandatory equality to a source can use an
index probe or hash build on that source. EXPLAIN shows the access paths, join algorithm and
`hash_budget=no_spill` detail. Hash builds count against StatementMemoryBudget; exhaustion is
CS05067 (53200) before writes. UPDATE FROM, DELETE USING and MERGE also charge retained
target identities, non-hash input buffers, derived copies, intermediate frames, associations,
selected values, encoded replacements and joined cascade state. Exhaustion refuses before any
write and releases reservations on every exit. Shared objects are charged once; general result
materialization and transient expression values remain outside the grant.

Equality-driven steps follow written source order. If only b is connected to the target by
equality, list b first: `FROM b, a` or `USING b, a`. In
`UPDATE t SET v = b.v FROM a, b WHERE t.id = b.tid AND b.aid = a.id`, the engine visits the
t × a product before b's hash. Reversing the source list lets each equality drive its step.
Both orders are correct; general join-order search is deferred to #1629.

Every [referential closure](language/constraints.md#referential-closure-memory), including plain ON DELETE CASCADE, charges
StatementMemoryBudget. About 840 bytes per two-column row limits a default 64 MiB closure to
about 79,900 rows, less when other statement work is retained. There is no cascade depth limit.
CS05067 (53200) refuses exhaustion before writes; cleanup releases abandoned attempts.

## Window frame work

[Window stages](language/clauses/window.md) retain full partitions under the statement memory
grant (CS05079), with no spill. Safe native moving aggregates run in O(n) work using inverse
transitions or MIN/MAX queues, including exclusion. Application/order-sensitive aggregates
recompute under the shared 1,000,000-visit cap (CS05084 / 54000). Volatile or subquery inputs/FILTER
restart evaluation when the head moves; this fallback can also hit CS05084 for native aggregates.
ANALYZE reports frame_visits, transitions, inverse_transitions and boundary_visits. LIMIT cannot shorten this work.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSourceReviewTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Costing.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlHashJoinTable.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCostAndHashJoinTests.cs`.
