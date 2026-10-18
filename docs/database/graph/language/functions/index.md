# Functions

GQL executes six aggregates, with collect as an alias for collect_list.

- **[Aggregate functions](aggregate-functions.md)** — count, sum, avg, min, max and collect_list/collect.

Calls are case-insensitive. CASE, COALESCE and NULLIF retain their dedicated expression forms.
Other scalar calls and statistical aggregates remain outside the executable profile.

## See also

[Language (GQL)](../index.md) · [GROUP BY](../clauses/group-by.md) · [RETURN](../clauses/return.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlAggregateParserTests.cs`.
