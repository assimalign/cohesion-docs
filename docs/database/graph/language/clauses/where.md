# WHERE

Filters MATCH bindings with a scalar Boolean expression.

## Syntax

```syntaxsql
MATCH pattern [ WHERE expression ] RETURN projection
```

## Remarks

For [selected paths](../path-selectors.md#prefilters-and-where), ordinary MATCH WHERE runs after
selection and before OPTIONAL null extension. Inline node/relationship WHERE and parenthesized
path WHERE execute inside search, with or without an explicit selector. Search prefilters cannot reference a peer-path-only
variable introduced in the same MATCH (CG01016).

[WITH WHERE](with.md) is a different stage position: it runs after projection, ordering and paging.
[FILTER](filter.md) independently filters the current bindings after any preceding stage. Both keep
only TRUE; neither changes OPTIONAL MATCH's own predicate-before-NULL-extension rule.

Expressions include AND, OR/XOR at one precedence level, NOT, arithmetic, concatenation,
comparisons, null/truth tests, bracketed IN and string predicates. A bare Boolean property is
accepted. [Operators](../expressions/operators.md) defines precedence, NULL propagation and value
errors. Only TRUE retains a binding; evaluated non-Boolean conditions raise `CG02013`.

Operands can be literals, bound variables, one-level properties or [parameters](../expressions/parameters.md).
Numeric equality, inequality, ordering and IN compare exact represented values across numeric
types, agreeing with RETURN DISTINCT. Decimal `0.1m` and Double `0.1d` compare unequal;
`1 = 1.0` is TRUE. These .NET suffixes describe parameter values. Arithmetic promotion remains
separate. Bound entities and paths compare by identity, including path traversal direction.
Property index values come from inline literal/parameter equalities or equalities in the
clause's top-level AND chain. Positive labeled predicates in that chain also supply required
node labels: `n:Person`, `n IS LABELED Person`, and `n IS LABELED Person&Tagged` can anchor like
pattern labels. All required labels participate in property index selection; the complete pattern
and WHERE still filter candidates. Bound entities take precedence, then property index seeks,
then the label scan with the smallest retained-posting estimate across required labels and pure
pattern unions at any node position. Equal estimates retain source order, pattern before WHERE.
Estimates include retained history and do not determine snapshot-visible results.

OR, XOR, NOT, truth tests, IS NOT LABELED and label expressions containing `|`, `!` or `%`
supply no required labels. A separate required conjunct can still narrow the scan.
Requirements stay local to their MATCH clause; relationship label tests test the type and
supply no node anchor. Label tests on NULL yield NULL; Graph has no MISSING value.
Label scans seek MVCC postings and decode only matching visible nodes, including pending statement
additions and replacements. The full pattern and WHERE remain residual checks. Pure pattern
unions merge selected ranges; a WHERE disjunction itself supplies no union anchor.
See [EXPLAIN](../statements/explain.md) for the selected label and ordinal.
There is no fixed comparison-count limit; recursive depth follows available stack, with CG00009
in the parser or CG05008 in engine execution.

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

On the same Person fixture:

```sql
MATCH (n) WHERE n:Person AND n.age > 20 RETURN n.name;
MATCH (n) WHERE n IS LABELED Person&Person AND n.age > 20 RETURN n.name;
```

Both return Alice and can use a Person label scan. Without ORDER BY, row order is unspecified.

## See also

[Clauses](index.md) · [Operators](../expressions/operators.md) · [MATCH](../statements/match.md)

## Sources

- **Cost selection and selected ranges** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphLabelIndexAnchorTests.cs`.

- **Label anchors and residual predicates** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphWhereLabelAnchorTests.cs`.

- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Expressions.cs`.
- **Exact comparisons** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphNumericComparisonTests.cs`.
