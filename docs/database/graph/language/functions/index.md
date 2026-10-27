# Functions

GQL executes core scalar functions and six aggregates, with collect as an alias for collect_list.

- **[Scalar functions](scalar-functions.md)** — Entities/paths, strings, numbers, Cypher conversions and transaction clocks.
- **[CAST](../expressions/cast.md)** — SQL-compatible scalar conversion.
- **[Aggregate functions](aggregate-functions.md)** — count, sum, avg, min, max and collect_list/collect.

Calls are case-insensitive. CASE, COALESCE and NULLIF retain their dedicated expression forms.
Recognized unsupported calls such as ELEMENTS and statistical aggregates remain outside the profile.

## Application functions

The host can register scalars and aggregates on the engine builder before construction.
Calls use the ordinary function syntax and case-insensitive names; registrations are engine
configuration. See [GraphFunction](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-function.md)
for the .NET API and a tested example.

The default strict policy returns NULL or skips an aggregate row when any input is NULL. CalledOnNullInput passes NULL to the callback. GQL has no MISSING.
There is no determinism/volatility flag or purity guarantee. Aggregate call appearances own
independent state and argument evaluation. EXPLAIN validates without invoking callbacks.
With the host-registered developer_sum aggregate from the API example:

```sql
FOR v IN [1,2,2,NULL] RETURN developer_sum(v), developer_sum(DISTINCT v);
```

The result is one row: 5 and 3. NULL is skipped by the default policy.

External callback/result/accumulator failures use CG05021 (no SQLSTATE); CG02013 covers dynamic input shape/conversion failures.
Statement failures abort an explicit transaction; roll back before continuing. Callbacks are
trusted code and must not reenter the engine.

## See also

[Language (GQL)](../index.md) · [GROUP BY](../clauses/group-by.md) · [RETURN](../clauses/return.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlAggregateParserTests.cs`.
