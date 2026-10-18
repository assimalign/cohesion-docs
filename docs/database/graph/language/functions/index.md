# Functions

GQL executes core scalar functions and six aggregates, with collect as an alias for collect_list.

- **[Scalar functions](scalar-functions.md)** — Entities/paths, strings, numbers, Cypher conversions and transaction clocks.
- **[CAST](../expressions/cast.md)** — SQL-compatible scalar conversion.
- **[Aggregate functions](aggregate-functions.md)** — count, sum, avg, min, max and collect_list/collect.

Calls are case-insensitive. CASE, COALESCE and NULLIF retain their dedicated expression forms.
Recognized unsupported calls such as ELEMENTS and statistical aggregates remain outside the profile.

## See also

[Language (GQL)](../index.md) · [GROUP BY](../clauses/group-by.md) · [RETURN](../clauses/return.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlAggregateParserTests.cs`.
