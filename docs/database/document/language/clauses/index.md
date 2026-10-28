# Clauses

OQL SELECT clauses bind one source, filter documents, form groups, and order the result.

## Clause order

- **Source** — [FROM](from.md) names one collection and an optional iteration variable.
- **Array expansion** — [UNNEST and LEFT UNNEST](unnest.md) bind ordered array elements before WHERE.

- **Document filter** — [WHERE](where.md) retains documents whose predicate is true.
- **Grouping** — [GROUP BY and GROUP AS](group-by.md) form groups, name keys and retain input binding wrappers.
- **Group filter** — [HAVING](having.md) filters grouped results.
- **Ordering** — [ORDER BY](order-by.md) sorts by expressions or explicit output aliases.

- **Paging** — [LIMIT and OFFSET](limit-offset.md) slice the ordered result; either may come first.

The order above is the grammar order. `FROM` is required; the other clauses are optional.

## UPDATE clauses

[SET and UNSET](../statements/update.md#set-and-unset-paths) assign original-document values or
remove named members. SET precedes UNSET when both occur, followed by an optional WHERE.

## Mutation results

[RETURNING](returning.md) follows INSERT/UPSERT VALUES or SELECT, or UPDATE/DELETE's optional WHERE.
It projects stored post-images, or DELETE pre-images, with the affected count.

## See also

[Language (OQL)](../index.md) and [SELECT](../statements/select.md).

## Sources

- **Clause grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
