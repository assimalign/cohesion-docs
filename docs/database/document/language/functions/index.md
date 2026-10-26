# Functions

OQL implements 34 SQL++ scalar functions, five aggregates and META over document expressions.

## Function reference

- **Aggregates** — [COUNT, SUM, AVG, MIN, and MAX](aggregate-functions.md).
- **Scalars** — [String, numeric, date, type, array and conditional functions](scalar-functions.md).

Each aggregate takes one argument, and only `COUNT` accepts `*`. Function names are
case-insensitive. The Documents planner and executor validate placement, grouping, and values.
Unknown functions produce `CD00008`; deferred calls such as REGEXP_LIKE produce `CK00001`.

## Application functions

The host can register scalars and aggregates on the engine builder before construction.
Calls use the ordinary function syntax and case-insensitive names; registrations are engine
configuration. See [DocumentFunction](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-function.md)
for the .NET API and a tested example.

Default scalars propagate MISSING before NULL without calling the core; aggregates skip both. CalledOnNullOrMissingInput passes both through; DocumentValue distinguishes them. Incompatible valued stored inputs yield scalar NULL or skipped typed aggregate rows.
There is no determinism/volatility flag or purity guarantee. Aggregate call appearances own
independent state and argument evaluation. EXPLAIN validates without invoking callbacks.
For a host-registered slug(string) that lowercases its input, and an items document
{"name":"Alpha","amount":2}, this returns one row with name = "alpha":

```sql
SELECT slug(name) AS name FROM items WHERE amount = 2;
```

External callback/result/accumulator failures use CD05017 (no SQLSTATE); CD02008 covers fractional integer conversion; CD02007 (22003) covers integer range overflow.
Statement failures abort an explicit transaction; roll back before continuing. Callbacks are
trusted code and must not reenter the engine.

## See also

[Language (OQL)](../index.md), [GROUP BY](../clauses/group-by.md), and [Unsupported](../unsupported.md).

[META](meta.md) exposes stored-document keys and versions, including in INSERT SELECT.

## Sources

- **Function contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
