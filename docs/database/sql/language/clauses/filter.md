# FILTER (Cohesion SQL)

Selects input rows for one aggregate call.

## Syntax

```syntaxsql
aggregate_call FILTER ( WHERE search_condition ) [ OVER window_specification ]
LISTAGG ( [ DISTINCT | ALL ] value [ , delimiter ] )
    WITHIN GROUP ( ORDER BY key [ , ...n ] ) FILTER ( WHERE search_condition )
```

FILTER follows the complete call, including LISTAGG's WITHIN GROUP ordering.

## Remarks

Every catalog aggregate, including application aggregates, accepts FILTER. It composes with
omitted/ALL/DISTINCT quantifiers, COUNT(*), input ORDER BY, GROUP BY, HAVING and query ORDER BY.
Only TRUE admits an input row; FALSE and NULL skip it. SQL has no MISSING value.

The filter evaluates before any argument, argument coercion, input ordering key, DISTINCT tuple
or aggregate transition. A rejected row therefore cannot trigger division by zero or side effects
in those expressions. This also applies to correlated and independent argument/ordering subqueries.
Independent children inside a filtered call run on first demand and cache their result for the
statement. Syntax, names and types still validate before input reads; FILTER does not hide invalid
expressions from binding.

The condition binds over source rows and can reference enclosing columns or contain supported
subqueries. An aggregate local to a child query in the condition is allowed. An aggregate at the
same query level inside the condition is refused. Existing outer-only aggregate ownership
restrictions retain CK00001. A filtered call remains an aggregate: it is excluded from its own
query level's WHERE, JOIN ON and GROUP BY. FILTER also works on aggregate windows, before OVER;
windows inside a window aggregate condition use CS05077 (42P20). FILTER on ranking/navigation/value
functions is an invalid call modifier (CS00011, 42601).

Calls with different filters have independent states. Filtered calls with volatile arguments,
ordering keys or conditions also retain independent states; child queries conservatively prevent
sharing. EXPLAIN includes `filtered_aggregates=N` on a grouping node when N is nonzero.
FILTER adds one named clause; the profile now measures 80 of 82 declared clauses.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS05072 | FILTER attached to a scalar/non-aggregate call | 42809 |
| CS05073 | Same-scope aggregate inside the filter condition | 42803 |
| CS02003 | Non-Boolean condition; untyped NULL is allowed | 42804 |
| CS00003 | Malformed suffix, including missing WHERE/condition or a repeated FILTER | — |
| CS05077 | Window function nested in a window aggregate filter | 42P20 |
| CS00011 | FILTER on a ranking, navigation or frame value function | 42601 |
| CK00001 | Existing unsupported aggregate query-level ownership | — |

## Examples

Execute separately in a fresh database:

```sql
CREATE TABLE filtered (v INT, flag BOOLEAN);
INSERT INTO filtered VALUES (0,FALSE),(2,TRUE),(4,TRUE),(4,NULL);
SELECT COUNT(*) FILTER (WHERE flag),
       COUNT(DISTINCT v) FILTER (WHERE flag),
       SUM(ALL 8/v) FILTER (WHERE flag),
       SUM((SELECT 1/0)) FILTER (WHERE FALSE)
FROM filtered;
```

Returns `(2, 2, 6, NULL)`; SUM returns Decimal 6. The excluded zero never divides, and the
independent argument subquery is never demanded.

`SELECT UPPER('a') FILTER (WHERE TRUE);` fails CS05072.
`SELECT SUM(v) FILTER (WHERE COUNT(*) > 0) FROM filtered;` fails CS05073.
`SELECT SUM(v) FILTER (WHERE 1) FROM filtered;` fails CS02003.

On the same table, a filtered running window is:

```sql
SELECT v, SUM(v) FILTER(WHERE flag) OVER(ORDER BY v)
FROM filtered ORDER BY v;
```

Returns `(0,NULL)`, `(2,2)`, `(4,6)`, `(4,6)`. The NULL flag contributes no input.

## See also

[Clauses](index.md) · [Aggregate functions](../functions/aggregate-functions.md) · [GROUP BY](group-by.md) · [HAVING](having.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Evaluation, subqueries and placement** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.FilterStatistics.cs`.
- **Grammar and AST** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAggregateFilterParserTests.cs`.
