# SELECT

SELECT projects document expressions from one collection with optional filtering, grouping, and ordering.

## Syntax

```syntaxsql
SELECT projection [ , ...n ]
FROM collection_name [ [ AS ] iteration_variable ]
[ WHERE predicate ]
[ GROUP BY expression [ , ...n ] ]
[ HAVING predicate ]
[ ORDER BY expression [ ASC | DESC ] [ , ...n ] ]
[ ; ]

<projection> ::= { * | expression } [ AS output_name ]
```

## Supported clauses

| Clause | Support | Requirement |
|---|---|---|
| `FROM` | Supported | Required; exactly one source |
| `WHERE` | Supported | Optional document predicate |
| `GROUP BY` | Supported | Optional grouping expressions |
| `HAVING` | Supported | Optional group predicate |
| `ORDER BY` | Supported | Optional ordering expressions |

## Arguments

- **`projection`** — a path, literal, parameter, arithmetic expression, aggregate, or whole-document
  star. `AS` gives an explicit output name.
- **`collection_name`** — an unqualified collection or reserved `COHESION_SCHEMA` source in the
  current database.
- **`iteration_variable`** — an optional name used to qualify document paths.

## Remarks

`SELECT *` returns a `document` column containing the complete JSON value. Objects and arrays remain
`JsonElement` values; scalars become null, Boolean, decimal, or string. Mixed-type result columns use
`DatabaseType.Null` to indicate an unknown type. Duplicate output names must be resolved with aliases.

Scans and index seeks establish ordinal document-identifier order before filtering. `ORDER BY`
preserves established order for ties. Results are materialized and own their JSON values after
execution, without retaining the transaction or borrowed storage memory.

## Examples

These statements are in the parser conformance corpus:

```sql
SELECT * FROM people
```

```sql
SELECT country, COUNT(*) AS total FROM people GROUP BY country HAVING COUNT(*) > 1 ORDER BY total DESC
```

## See also

[Statements](index.md), [FROM](../clauses/from.md), [Expressions](../expressions/index.md),
and [Aggregate functions](../functions/aggregate-functions.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
- **Execution semantics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
