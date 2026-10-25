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

Eligible equijoins can use an in-memory hash build. Without a countable index, an input uses the
4,096-row heuristic: unindexed tables can select a hash join regardless of actual size, including
empty or tiny inputs. Small exact indexed inputs and complete unique probes retain index nested
loops. Any ON reference to a derived String/Json/Character column prevents hash selection, even
in a residual term, preserving deferred collation/Unicode errors. Full predicates, duplicates,
NULL and LEFT semantics remain intact. See [joins](language/clauses/join.md#hash-joins-and-memory).

Configure the engine before creation:

```csharp
using Assimalign.Cohesion.Database.Sql;

var options = new SqlDatabaseEngineOptions { StatementMemoryBudget = 32L * 1024 * 1024 };
await using var engine = SqlDatabaseEngine.Create("orders", options);
```

The positive byte budget accounts for concurrently live hash builds, including nested queries and growth overlap. Exhaustion reports CS05067 (53200); there is no spill. It does not limit total result materialization or transient planning.

Use [EXPLAIN](language/statements/explain.md) to inspect costs without executing the wrapped data statement. EXPLAIN ANALYZE accepts SELECT query bodies and runs expressions/application functions normally; actual_rows reports measured candidates before residuals, not final output cardinality. Estimates distinguish exact probes, lower bounds and heuristics.

Plain EXPLAIN can fail with `CS04006` when an index cost probe races committed DDL that changes a
bound table definition. Retry in a fresh transaction. It still never executes the wrapped data
statement. Analysis scopes cached view bodies by relation path, so separate references report
independent counts while repeated correlated calls to the same reference accumulate.

[SQL](index.md) · [Engine options](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-engine-options.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Costing.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlHashJoinTable.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCostAndHashJoinTests.cs`.
