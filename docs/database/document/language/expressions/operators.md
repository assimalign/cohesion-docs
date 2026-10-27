# Operators and predicates

OQL evaluates arithmetic, comparisons, Boolean operators and predicates under SQL++ NULL and MISSING rules.

## Syntax

```syntaxsql
expression { OR | AND | = | != | <> | < | <= | > | >= | + | - | * | / | % } expression
{ NOT | + | - } expression
expression IS [ NOT ] { NULL | MISSING | VALUED }
expression [ NOT ] IN ( expression [ , ...n ] )
expression [ NOT ] IN array_expression
expression [ NOT ] BETWEEN lower AND upper
expression [ NOT ] LIKE pattern [ ESCAPE escape_expression ]
```

## Precedence

Lowest first: OR, AND, comparisons/presence tests/IN/BETWEEN/LIKE, addition/subtraction,
multiplication/division/remainder, unary signs. NOT wraps comparisons: NOT age < 18 means
NOT (age < 18). Parentheses override grouping. CASE is a primary expression.

## Comparison, arithmetic and logic

Arithmetic requires decimal operands. Division/remainder by zero is `CD02006` (22012),
numeric range failure is `CD02007` (22003), and invalid operand types are `CD02008`.
Ordinary comparisons and arithmetic propagate MISSING before NULL. String comparison is ordinal
and case-sensitive; range comparison requires matching scalar kinds. SQL COLLATE does not apply.

Equality compares arrays and objects structurally, independent of object property order.
A definite structural mismatch is FALSE; otherwise equal structures containing NULL compare as
NULL. WHERE, HAVING and logical operators use the [shared SQL++ truth rule](missing.md#boolean-contexts),
so WHERE 1 retains rows. NOT preserves NULL and MISSING.

## IN and BETWEEN

IN accepts a nonempty parenthesized value list or an unparenthesized array operand. Parentheses
always mean a list, even for a single array-valued item. It uses
structural equality. A MISSING left operand yields MISSING; NULL yields NULL. For a present left
operand, a match wins; without a match a NULL list item wins over a MISSING item, otherwise FALSE.
NOT IN negates only Boolean results. For array IN, MISSING on either side gives MISSING;
a NULL/non-array right operand gives NULL. With a valued left operand, a matching element wins;
without a match, a NULL element gives NULL, otherwise FALSE. NULL IN [] remains NULL on this
branch, while a valued operand IN [] is FALSE. Nested SELECT remains CK00001.

[ANY/SOME/EVERY](quantified-predicates.md) use scoped SATISFIES predicates with different element
truth handling: NULL/MISSING predicate results are not true.

BETWEEN tests inclusive same-kind scalar bounds. Any MISSING operand/bound yields MISSING before
NULL is considered, even when the other bound would exclude the value. NOT BETWEEN preserves
unknown states. Constant inclusive bounds on an indexed path can supply an index seek.

## LIKE and ESCAPE

LIKE is ordinal and case-sensitive. % matches zero or more Unicode scalars; _ matches one,
including a supplementary character or newline. Backslash is the default escape; string literals
do not consume it. ESCAPE accepts one Unicode scalar; an empty string disables escaping. An escape
quotes the following scalar, including itself, % or _.

MISSING takes precedence; non-string or NULL operands, patterns or escape values give NULL.
A literal escape longer than one scalar is `CD00002`. An evaluated invalid escape or a trailing
pattern escape is `CD02013`. LIKE checks the statement cancellation token while compiling and
matching; cancellation raises OperationCanceledException. NOT LIKE preserves NULL/MISSING.

## Examples

For a collection items containing `{"name":"beta","amount":6}`:

```sql
SELECT amount IN (6, NULL) AS member, amount BETWEEN 3 AND 6 AS bounded,
       name LIKE 'b_ta' ESCAPE '!' AS matched FROM items;
SELECT MISSING IN (1) AS m, 0 BETWEEN 1 AND MISSING AS b,
       NULL LIKE MISSING AS l FROM items;
```

The first row is `(TRUE, TRUE, TRUE)`. All three columns in the second row are MISSING, so its
projected JSON object is `{}`. `name LIKE 'b\%'` matches the literal text b%, not beta.

### Array membership

With one document in items:

```sql
SELECT 2 IN [1, 2, NULL] AS hit, 3 IN [1, NULL] AS unknown,
       3 IN [] AS empty, NULL IN [] AS null_left,
       [1, 2] IN ([1, 2]) AS whole_array FROM items;
```

The row is {"hit":true,"unknown":null,"empty":false,"null_left":null,"whole_array":true}.
The parenthesized form compares the whole array as one list item.

## See also

[Expressions](index.md) · [MISSING](missing.md) · [CASE](case.md) · [Diagnostics](../diagnostics.md)

## Less-than followed by minus

In OQL, `a<-1` and `a <- 1` both mean `a < -1`, following PostgreSQL's lexical rule.
`a<--1` starts a `--1` line comment after `<`; it leaves the comparison without a right operand
and reports CD00002. Only GQL recognizes `<-` as a relationship arrow. NULL comparison
behavior is unchanged.

For a row with a = -2, `SELECT a FROM t WHERE a<-1` returns -2. The corresponding
`WHERE a<--1` form reports CD00002.

## Sources

- **Predicates** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Evaluation/DocumentExpressionEvaluator.Predicates.cs`.
- **Structural equality** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Evaluation/DocumentExpressionEvaluator.Values.cs`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
- **Cancellation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentLikeTests.cs`.
