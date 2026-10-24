# SQL query performance

SQL chooses among eligible access paths using bounded probes under the statement snapshot. No persisted statistics or standalone ANALYZE command exists. Each range count examines at most 512 physical entries and 32 page visits; invisible entries, descent pages and stopping keys consume work. Incomplete counts are lower bounds, and unknown inputs use heuristics. Complete unique-key equality lookups avoid probes. Temporary metadata intent locks protect a probe and are released afterward; a refused intent gives an unknown estimate without waiting.

Eligible broad equijoins can use an in-memory hash build. Small exact indexed inputs and complete unique probes retain index nested loops. Full predicates, duplicates, NULL and LEFT semantics remain intact. See [joins](language/clauses/join.md#hash-joins-and-memory).

Configure the engine before creation:

```csharp
var options = new SqlDatabaseEngineOptions { StatementMemoryBudget = 32L * 1024 * 1024 };
await using var engine = SqlDatabaseEngine.Create("orders", options);
```

The positive byte budget accounts for concurrently live hash builds, including nested queries and growth overlap. Exhaustion reports CS05067 (53200); there is no spill. It does not limit total result materialization or transient planning.

Use [EXPLAIN](language/statements/explain.md) to inspect costs without executing the wrapped data statement. EXPLAIN ANALYZE accepts SELECT query bodies and runs expressions/application functions normally; actual_rows reports measured candidates before residuals, not final output cardinality. Estimates distinguish exact probes, lower bounds and heuristics.

[SQL](index.md) · [Engine options](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-engine-options.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Costing.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlHashJoinTable.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCostAndHashJoinTests.cs`.
