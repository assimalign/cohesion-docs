# GROUP BY

GROUP BY partitions documents by one or more expression values for grouped projection and aggregation.

## Syntax

```syntaxsql
GROUP BY expression [ [ AS ] alias ] [ , ...n ] [ GROUP AS group_name ]
```

## Supported clauses

| Construct | Support | Meaning |
|---|---|---|
| One or more grouping expressions | Supported | Structural grouping keys |
| Group-term aliases | Supported | Name computed keys, with or without AS |
| `GROUP AS` | Supported | Retains each group's input bindings as an array of objects |
| Subsequent `HAVING` | Supported | Filters groups |
| Subsequent `ORDER BY` | Supported | Orders grouped results |

## Arguments

- **`expression`** — a grouping key expression; multiple expressions are comma separated.
- **`alias`** — an explicit name for a grouping term.
- **`group_name`** — the post-group binding for the array of member wrappers.

## Remarks

MISSING and NULL form separate groups. Group values compare structurally. Ascending ordering uses
MISSING, NULL, FALSE, TRUE, number, ordinal string, array, object. Arrays compare element by
element; objects compare ordinal property names and their values.

Outside aggregate calls, grouped projection, `HAVING`, and `ORDER BY` must use group keys or
expressions built from group keys and constants. Binding compares expression structure, preserves
literal scalar types, and treats qualified and unqualified paths for the same iteration variable
as equivalent. Aggregate queries without explicit grouping form one group even for an empty source.

### GROUP AS and aliases

Each GROUP AS array element is an object containing the FROM binding (its alias, or the collection
name when unaliased), every UNNEST binding, and every explicit group-term alias. Unnamed group
terms add no member. The source value can be an object, array, scalar or NULL. MISSING members
are omitted; explicit NULL remains, including NULL UNNEST elements. An absent LEFT UNNEST
element therefore omits its binding. Empty input produces zero groups. No order is promised
for group-array elements.

Group terms evaluate against the input row independently of other group aliases. Their aliases
bind computed keys in aggregate row inputs and representative keys in SELECT, HAVING and ORDER BY.
GROUP AS is visible only after grouping in SELECT, HAVING and ORDER BY, including paths such as
`g[0].d.name`, constructors, scalar calls, ARRAY/FIRST comprehensions and ANY/SOME/EVERY predicates.
It cannot supply an aggregate's row input: `ARRAY_AGG(g)` reports CD01031. Local comprehension
and quantified variables may shadow group names within their lexical scope.

WHERE and GROUP BY still look up input fields, without seeing the new bindings. GROUP AS does
not permit ungrouped input fields outside aggregate calls. Standalone projection aliases retain
precedence in ORDER BY. Names are ordinal and case-sensitive and must be nonempty and distinct
from other group, FROM and UNNEST bindings. LETTING remains refused with CK00001.

Wrappers and arrays share [StatementMemoryBudget](../functions/aggregate-functions.md#statement-memory)
with source rows, keys, aggregates and output. EXPLAIN reports group_as and group_aliases without
evaluating inputs. Returned JSON owns its values independently of the transaction.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00012 | GROUP AS without preceding GROUP BY terms; at GROUP | — |
| CD01030 | Empty/duplicate group name, collision with FROM/UNNEST, or invalid direct-AST alias-list shape; at the statement | — |
| CD01031 | GROUP AS binding referenced in an aggregate's row input; at the path | — |
| CD05001 | Ungrouped input expression outside an aggregate | — |
| CD05018 | Accounted retained statement memory exceeds the budget | — |
| CK00001 | LETTING or another excluded construct | — |

## Examples

The parser corpus uses a country key and document count:

```sql
SELECT country, COUNT(*) AS total FROM people GROUP BY country HAVING COUNT(*) > 1 ORDER BY total DESC
```

Execute separately in a fresh database:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES ('a', {}), ('b', {k: NULL, tags: [NULL]});
SELECT key, g FROM items d LEFT UNNEST d.tags t
GROUP BY d.k AS key GROUP AS g ORDER BY key;
```

The two rows are `{"g":[{"d":{}}]}` and
`{"key":null,"g":[{"d":{"k":null,"tags":[null]},"t":null,"key":null}]}`.
JSON member order is immaterial. With no FROM alias, `SELECT VALUE g FROM items GROUP BY TRUE
GROUP AS g` wraps each source under `items`.

## See also

[Clauses](index.md), [HAVING](having.md), and [Aggregate functions](../functions/aggregate-functions.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
- **Grouping semantics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
- **Bindings, NULL/MISSING and refusals** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Grouping.cs`.
