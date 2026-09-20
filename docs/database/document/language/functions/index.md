# Functions

OQL implements five aggregate functions over document expressions.

## Function reference

- **Aggregates** — [COUNT, SUM, AVG, MIN, and MAX](aggregate-functions.md).

Each function takes one argument, and only `COUNT` accepts `*`. Function names are
case-insensitive. The Documents planner and executor validate placement, grouping, and values.
Unknown functions and reserved unimplemented calls such as `ABS` produce `COHDBL001`.

## See also

[Language (OQL)](../index.md), [GROUP BY](../clauses/group-by.md), and [Unsupported](../unsupported.md).

## Sources

- **Function contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
