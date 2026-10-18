# WHERE

Filters MATCH bindings with a scalar Boolean expression.

## Syntax

```syntaxsql
MATCH pattern [ WHERE expression ] RETURN projection
```

## Remarks

[WITH WHERE](with.md) is a different stage position: it runs after projection, ordering and paging.
[FILTER](filter.md) independently filters the current bindings after any preceding stage. Both keep
only TRUE; neither changes OPTIONAL MATCH's own predicate-before-NULL-extension rule.

Expressions include AND, OR/XOR at one precedence level, NOT, arithmetic, concatenation,
comparisons, null/truth tests, bracketed IN and string predicates. A bare Boolean property is
accepted. [Operators](../expressions/operators.md) defines precedence, NULL propagation and value
errors. Only TRUE retains a binding; evaluated non-Boolean conditions raise `COHDBG013`.

Operands can be literals, bound variables, one-level properties or [parameters](../expressions/parameters.md).
Numeric equality, inequality, ordering and IN compare exact represented values across numeric
types, agreeing with RETURN DISTINCT. Decimal `0.1m` and Double `0.1d` compare unequal;
`1 = 1.0` is TRUE. These .NET suffixes describe parameter values. Arithmetic promotion remains
separate. Bound entities and paths compare by identity, including path traversal direction.
Index anchors come only from mandatory AND equalities, never from an optional OR/XOR arm or NOT.
There is no fixed comparison-count limit; recursive depth follows available stack, with GQL0009
in the parser or COHDBG008 in engine execution.

## Examples

Each WHERE belongs to the immediately preceding MATCH or OPTIONAL MATCH and sees only variables
declared through that clause. An optional clause filters its candidates before NULL extension;
a predicate that fails for all candidates preserves the input with NULL for new bindings. It does
not filter away that input row. See [MATCH](../statements/match.md#optional-and-repeated-matching).

For nodes Alice age 42 and Bob age 17:

```sql
MATCH (a:Person) WHERE a.age / 5 = 8 AND a.absent IS NULL RETURN a.name;
MATCH (a:Person) WHERE a.name CONTAINS 'lic' OR a.age = 17 RETURN a.name;
```

The first returns Alice. The second returns both names; no ordering is promised.

## See also

[Clauses](index.md) · [Operators](../expressions/operators.md) · [MATCH](../statements/match.md)

## Sources

- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Expressions.cs`.
- **Exact comparisons** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphNumericComparisonTests.cs`.
