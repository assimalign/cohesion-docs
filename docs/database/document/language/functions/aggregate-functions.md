# Aggregate functions

OQL aggregate functions count documents or summarize nonnull expression values within a group.

## Syntax

```syntaxsql
COUNT ( { * | expression } )
SUM ( expression )
AVG ( expression )
MIN ( expression )
MAX ( expression )
```

## Supported forms

| Function | Support | Semantics |
|---|---|---|
| `COUNT(*)` | Supported | Counts documents |
| `COUNT(expression)` | Supported | Counts nonnull values |
| `SUM(expression)` | Supported | Sums numeric nonnull inputs |
| `AVG(expression)` | Supported | Averages numeric nonnull inputs |
| `MIN(expression)`, `MAX(expression)` | Supported | Minimum or maximum nonnull value |
| `DISTINCT` aggregate arguments | Recognized, not supported (`COHDBL001`) | No distinct aggregate modifier |

## Arguments

- **`expression`** — the aggregate's single argument, evaluated over its group.
- **`*`** — counts documents; accepted only by `COUNT`.

## Remarks

With no nonnull inputs, `SUM`, `AVG`, `MIN`, and `MAX` return null, while `COUNT` returns zero.
An ungrouped aggregate query yields one group even for an empty source. `SUM` and `AVG` require
numeric nonnull values.

The parser reports `OQL0006` for an invalid argument count or a star argument to another aggregate.
The planner validates aggregate placement and grouped expressions; parser acceptance alone does
not establish semantic validity.

## Examples

The parser conformance corpus contains these calls:

```sql
SELECT COUNT(*) FROM people
```

```sql
SELECT SUM(score), AVG(score), MIN(score), MAX(score), COUNT(score) FROM people
```

## See also

[Functions](index.md), [GROUP BY](../clauses/group-by.md), and [HAVING](../clauses/having.md).

## Sources

- **Parsing and arity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Aggregate execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
