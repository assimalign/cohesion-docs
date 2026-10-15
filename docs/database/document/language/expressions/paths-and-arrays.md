# Paths and arrays

OQL paths traverse case-sensitive document properties and zero-based array elements.

## Syntax

```syntaxsql
<document_path> ::= identifier [ path_segment ...n ]
<path_segment> ::= { . identifier | left_bracket array_index right_bracket
                  | left_bracket string_literal right_bracket }
```

`left_bracket` and `right_bracket` mean the literal `[` and `]` characters. Repeated path segments
have no separator beyond their own dot or brackets.

## Arguments

- **`identifier`** — a property name, optionally double quoted; the first segment may be the query's
  iteration variable. Keywords are allowed as property names after a dot.
- **`array_index`** — a nonnegative 32-bit integer, starting at zero.
- **`string_literal`** — a single-quoted property name, including names containing punctuation.

## Remarks

An absent field, incompatible intermediate shape, or out-of-range position evaluates to null.
Bracket strings access properties and integer subscripts access array elements. Neither form
expands an array into rows. `FLATTEN` is recognized but unsupported.

Index definitions use this path grammar relative to each document, without an iteration alias.
The parser retains each property or index as an `OqlPathSegment` in an `OqlPathExpression`.

## Examples

Both examples occur in the parser corpus:

```sql
SELECT p.address.city, p.orders[0].total FROM people p
```

```sql
SELECT p['unusual-field'][2] FROM people p
```

## See also

[Expressions](index.md), [FROM](../clauses/from.md), and [CREATE INDEX](../statements/create-index.md).

## Sources

- **Path parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Missing-value behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
