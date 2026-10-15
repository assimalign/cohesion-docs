# Clauses

OQL SELECT clauses bind one source, filter documents, form groups, and order the result.

## Clause order

- **Source** — [FROM](from.md) names one collection and an optional iteration variable.
- **Document filter** — [WHERE](where.md) retains documents whose predicate is true.
- **Grouping** — [GROUP BY](group-by.md) forms groups from expression values.
- **Group filter** — [HAVING](having.md) filters grouped results.
- **Ordering** — [ORDER BY](order-by.md) sorts by expressions or explicit output aliases.

The order above is the grammar order. `FROM` is required; the other clauses are optional.

## See also

[Language (OQL)](../index.md) and [SELECT](../statements/select.md).

## Sources

- **Clause grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
