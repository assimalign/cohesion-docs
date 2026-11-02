# Operators

GQL WHERE and RETURN evaluate Boolean, comparison, arithmetic, concatenation and string predicates.

## Syntax and precedence

Operators are listed from weakest to strongest; binary runs associate left.

| Level | Forms |
| --- | --- |
| 1 | OR, XOR at one level |
| 2 | AND |
| 3 | NOT |
| 4 | =, <>, !=, <, <=, >, >=; IN list_expression; STARTS WITH, ENDS WITH, CONTAINS; IS [NOT] NULL/TRUE/FALSE/UNKNOWN |
| 5 | `\|\|` |
| 6 | Binary +, -; string + |
| 7 | *, /, % |
| 8 | Unary +, - |
| 9 | Literals, parameters, bound variables, CASE/COALESCE/NULLIF, list/map constructors, parentheses and postfix subscripts/member access |

## NULL and truth

WHERE retains only TRUE. Missing graph properties read NULL; GQL has no Documents MISSING value.
NOT/AND/OR/XOR use three-valued logic. FALSE AND unknown is FALSE; TRUE OR unknown is TRUE;
XOR with unknown is unknown; NOT unknown is unknown. UNKNOWN is a NULL Boolean literal.
IS [NOT] NULL returns Boolean. Truth tests accept Boolean/NULL and return Boolean; IS UNKNOWN
is true for NULL. Evaluated non-Boolean logical/WHERE values raise `CG02013`.

Ordinary comparisons involving NULL yield unknown. Incomparable non-null values are unequal;
incomparable ordering yields unknown. Strings compare ordinally and case-sensitively.

All numeric comparisons (`=`, `<>`, `!=`, `<`, `<=`, `>`, `>=`), property matching and IN use
exact represented values across runtime numeric types, agreeing with RETURN DISTINCT. Integer
`1` equals Double `1.0`; Decimal `0.1m` does not equal Double `0.1d`. Large integers above 2^53
retain every bit and subnormals remain greater than zero. Signed zeros compare equal. These
examples use .NET type suffixes to describe parameter values; GQL has no `m`/`d` literal suffix.
Non-finite referenced parameters or programmatic AST literals fail planning with CG03003;
numeric text outside the finite Double range fails parsing with CG00004. DISTINCT groups NULLs, while
ordinary comparisons with NULL remain unknown. Bound nodes/relationships compare by identity;
paths compare their ordered node/relationship identities and traversal direction.

## Arithmetic and concatenation

Any floating operand promotes to Double; otherwise decimal/ulong promotes to Decimal; other
integer operands use checked Int64. Integer division truncates toward zero. Remainder by -1 is
zero, including Int64.MinValue; Int64.MinValue / -1 overflows. Zero divisors, overflow, non-finite
results and invalid arithmetic types are `CG02013`. Arithmetic propagates NULL.
Concatenation `||` and string + require two strings and propagate NULL.
This promotion applies to arithmetic; comparisons, IN and DISTINCT use exact value comparison.

[Braced EXISTS and NOT EXISTS](exists.md) execute as Boolean expressions with lexical correlation.

## Membership and string predicates

IN accepts a list-valued expression, including brackets, parameters or parenthesized lists. Every
item evaluates before membership is tested, so a later value error still raises CG02013 after an
earlier match. Empty-list IN is FALSE even for NULL. Otherwise a match wins; then an unknown element
comparison yields unknown; otherwise FALSE. A NULL right operand yields NULL, a non-list CG02013.
[Lists and maps](list-values.md#comparison) compare structurally and preserve three-valued equality.
STARTS WITH, ENDS WITH and CONTAINS use ordinal case-sensitive text and yield unknown unless both
operands are strings. IN and these string predicates are Cohesion extensions.

LIKE, BETWEEN, regular-expression match and quantified comparisons remain
`CK00001`. [CASE](case.md), [COALESCE](coalesce.md) and [NULLIF](nullif.md) execute as special
expression forms. [Scalar calls](../functions/scalar-functions.md) and [CAST](cast.md) execute.
[Subscripts/member access](property-access.md) compose with lists/maps and entities.
IN (1, 2) remains malformed; use IN [1, 2], IN ([1, 2]) or a list-valued parameter.
[RETURN](../clauses/return.md) accepts these scalar expressions, bound variables and optional aliases.

## Examples

Execute separately:

```sql
INSERT (:Person {name:'Alice', age:42}), (:Person {name:'Bob', age:17});
MATCH (a:Person) WHERE NOT (a.age < 18) AND a.name STARTS WITH 'Al' RETURN a.name;
MATCH (a:Person) WHERE TRUE OR TRUE XOR TRUE RETURN a.name;
MATCH (a:Person) WHERE a.age IN [42, NULL] RETURN a.name;
```

Results are Alice, no rows, and Alice, respectively. The second WHERE groups as
(TRUE OR TRUE) XOR TRUE, so OR and XOR's shared precedence matters.

Bind `decimal` to .NET `0.1m` and `double` to .NET `0.1d`:

```sql
RETURN $decimal = $double AS equal, $decimal <> $double AS different,
       $decimal < $double AS less, $decimal IN [$double] AS member;
RETURN 1 = 1.0 AS same;
```

Results are `(FALSE, TRUE, TRUE, FALSE)` and `(TRUE)`. Inserting both parameter values as a
property and returning DISTINCT preserves two rows, one Decimal and one Double.

## See also

[Expressions](index.md) · [WHERE](../clauses/where.md) · [Parameters](parameters.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Truth, promotion and failures** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
- **Grammar and precedence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DIALECT.md`.
- **Exact comparison and DISTINCT** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphNumericComparisonTests.cs`.
