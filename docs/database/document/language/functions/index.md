# Functions

OQL implements 34 SQL++ scalar functions, five aggregates and META over document expressions.

## Function reference

- **Aggregates** — [COUNT, SUM, AVG, MIN, and MAX](aggregate-functions.md).
- **Scalars** — [String, numeric, date, type, array and conditional functions](scalar-functions.md).

Each aggregate takes one argument, and only `COUNT` accepts `*`. Function names are
case-insensitive. The Documents planner and executor validate placement, grouping, and values.
Unknown functions produce `CD00008`; deferred calls such as REGEXP_LIKE produce `CK00001`.

## See also

[Language (OQL)](../index.md), [GROUP BY](../clauses/group-by.md), and [Unsupported](../unsupported.md).

[META](meta.md) exposes stored-document keys and versions, including in INSERT SELECT.

## Sources

- **Function contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
