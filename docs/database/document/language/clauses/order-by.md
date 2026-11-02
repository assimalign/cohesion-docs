# ORDER BY

ORDER BY orders query results by one or more expressions with an optional direction for each.

## Syntax

```syntaxsql
ORDER BY expression [ ASC | DESC ] [ NULLS FIRST | NULLS LAST ] [ , ...n ]
```

## Supported clauses

| Construct | Support | Meaning |
|---|---|---|
| Source expression | Supported | Evaluates an ordering value |
| [GROUP AS and group-term aliases](group-by.md#group-as-and-aliases) | Supported | Post-group ordering expressions |
| Standalone explicit projection alias | Supported | Resolves the projected expression |
| `ASC` or `DESC` | Supported | Ascending by default, or descending |
| [LIMIT/OFFSET](limit-offset.md) | Supported | Final output slice after ordering; either clause order |

## Arguments

- **`expression`** — a source expression or a standalone alias introduced with projection `AS`.
- **`ASC` / `DESC`** — ascending or descending direction for the preceding expression.

## Remarks

A standalone explicit alias takes precedence over a source field with the same name. Grouped
ordering follows the same group-key restrictions as grouped projections. Later ORDER BY terms
break ties before the final ordinal document-identity tie break, independent of direction.
Strings use case-sensitive ordinal order.

The default direction is ASC. Placement moves the whole [MISSING](../expressions/missing.md)/NULL
block; direction controls its internal order:

| Direction | Placement | Result order |
| --- | --- | --- |
| ASC | Omitted (FIRST) | MISSING, NULL, ascending values |
| ASC | NULLS FIRST | MISSING, NULL, ascending values |
| ASC | NULLS LAST | Ascending values, MISSING, NULL |
| DESC | Omitted (LAST) | Descending values, NULL, MISSING |
| DESC | NULLS FIRST | NULL, MISSING, descending values |
| DESC | NULLS LAST | Descending values, NULL, MISSING |

Ascending values order FALSE, TRUE, numbers, ordinal strings, arrays, objects; DESC reverses it.
NULLS remains contextual, so a field named nulls needs no quoting.

With SELECT DISTINCT, every key must match a projected expression after explicit-alias rewriting;
an outside key fails with `CD01014` at that key. Top-level VALUE/RAW normalizes a MISSING projected
value to NULL before ordering by the matching expression.

### Index order and paging

An eligible scalar index supplies this order directly, forward for ASC or reverse for DESC,
and removes the result sort. ORDER BY must match consecutive index keys after any skipped
equality-fixed leading keys. Varying keys must share a direction. The index must INCLUDE MISSING
on its leading key, or a mandatory predicate must reject leading MISSING. Nondefault NULLS
placement requires proof that both NULL and MISSING cannot qualify.

UNIQUE scalar keys, suitable scalar predicates or a bounded complex-key absence probe establish
the scalar domain. Array indexes, complex values, unproven scalar domains, computed ordering,
mixed varying directions, grouping, DISTINCT, UNNEST and identity intersections keep the sort.
VALUE/RAW ordering by its projected expression also keeps the sort unless MISSING is rejected:
projection turns MISSING into NULL and changes equal-key ties. See
[EXPLAIN eligibility and reasons](../statements/explain.md#index-ordering-eligibility).

The engine completes each equal-ORDER-BY-key bucket, including trailing physical index keys,
then visits its documents in ascending ordinal identity order in either scan direction. The full
residual predicate filters that ordered stream. With fixed projection types, including SELECT *,
LIMIT/OFFSET can stop after OFFSET + LIMIT survivors; LIMIT 0 opens no ordered cursor. A boundary
entry and bounded index read-ahead may still be read. Projections needing result type inference
read all matches to establish column types, while still eliding the sort.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00013 | NULLS lacks FIRST or LAST; at the invalid option or end of input | — |
| CD01032 | Direct ordering AST has an invalid null-placement enum | — |
| CK00001 | Extra unsupported token after a complete suffix, such as NULLS FIRST LAST | — |

CD05018 also covers ordered candidate buckets, their keys and identities under StatementMemoryBudget
(no assigned SQLSTATE); exhaustion fails before result publication.

## Examples

The parser conformance corpus covers independent directions:

```sql
SELECT region, age FROM people ORDER BY region ASC, age DESC
```

Execute separately in a fresh database:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES ('m', {}), ('n', {v:NULL}), ('v', {v:1});
SELECT META().id AS id FROM items ORDER BY v DESC NULLS FIRST;
```

Returns `{"id":"n"}`, `{"id":"m"}`, `{"id":"v"}`, in that order.

With those documents, execute separately:

```sql
CREATE INDEX ix ON items (v INCLUDE MISSING);
SELECT META().id AS id FROM items ORDER BY v DESC LIMIT 1 OFFSET 1;
```

Returns `{"id":"n"}`. The scalar index supplies reverse order; the first survivor is skipped.

## See also

[Clauses](index.md), [SELECT](../statements/select.md), and [GROUP BY](group-by.md).

## Sources

- **Ordered consumption and paging** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentOrderedConsumptionTests.cs`.
- **Grammar and default direction** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
- **Ordering semantics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Placement and ties** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentOrderingTests.cs`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
