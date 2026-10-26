# Operators and predicates (Cohesion SQL)

Combines scalar values and evaluates SQL search conditions.

> **Status:** Partial. This page describes the executable subset and its boundaries.

## Syntax

```syntaxsql
left_expression { + | - | * | / | % | || } right_expression
- expression
left_expression { = | <> | < | <= | > | >= } right_expression
expression IS [ NOT ] NULL
left_expression IS [ NOT ] DISTINCT FROM right_expression
expression IS [ NOT ] { TRUE | FALSE | UNKNOWN }
expression [ NOT ] BETWEEN lower_expression AND upper_expression
expression [ NOT ] IN ( expression [ , ...n ] )
expression [ NOT ] LIKE pattern_expression [ ESCAPE escape_expression ]
NOT search_condition
left_condition { AND | OR } right_condition
```

## Arguments

- **`expression`** — An executable scalar value.
- **`search_condition`** — A Boolean predicate using SQL three-valued logic.
- **`pattern_expression`** — A `LIKE` pattern; percent matches any sequence and underscore
  matches one character.

## Remarks

Precedence follows PostgreSQL, from low to high: `OR`, `AND`, logical `NOT`, `IS` tests and
distinct predicates, comparisons, `[NOT] BETWEEN/IN/LIKE`, `ESCAPE`, concatenation (`||`),
addition/subtraction, multiplication/division/remainder, `COLLATE`, unary signs, then primary
expressions. Thus `1 = 1 IS TRUE` tests `(1 = 1)`, `'x' || 1 + 2` returns `'x3'`, and
`-a COLLATE binary` groups as `(-a) COLLATE binary`. NOT keeps its ordinary precedence before
EXISTS: `NOT EXISTS (query) IS TRUE` means `NOT ((EXISTS (query)) IS TRUE)`.

Non-associative chains such as `a < b < c`, `a LIKE b LIKE c`, and
`a IS DISTINCT FROM b IS NULL` report one `CS00003` at the second operator. Parentheses can
group an intermediate result explicitly. A completed IN/NOT IN list or subquery may feed another
predicate: `1 IN (1) IN (TRUE)` and `1 IN (SELECT id FROM t WHERE id = 1) IN (TRUE)` test the
first Boolean result. Completed postfix tests may also continue: `NULL IS NULL = TRUE` is TRUE.
The lower BETWEEN bound accepts restricted comparison/distinct expressions.
Prefix/infix `~`, binary `&`, `|`, `<<`, `>>`, and `::` casts report `CK00001`.

```sql
SELECT 1 = 1 IS TRUE, TRUE = 'abc' LIKE 'a%', 1 IN (1) IN (TRUE), 'x' || 1 + 2
FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'dbo';
```

The result is `(TRUE, TRUE, TRUE, 'x3')`.

Comparison with a null operand yields `UNKNOWN`; `IS NULL` and `IS NOT NULL` test nullness.
`BETWEEN` includes its lower and upper bounds and returns unknown with a null operand or bound.
Only true conditions retain rows in `WHERE`, `ON`, and `HAVING`.

| Inputs | `AND` | `OR` |
| --- | --- | --- |
| `TRUE`, `UNKNOWN` | `UNKNOWN` | `TRUE` |
| `FALSE`, `UNKNOWN` | `FALSE` | `UNKNOWN` |
| `UNKNOWN`, `UNKNOWN` | `UNKNOWN` | `UNKNOWN` |

`NOT UNKNOWN` remains unknown. Membership and its empty-result rules are detailed in
[Subqueries](subqueries.md); a null-containing candidate list can also produce unknown.
AND, OR, NOT and truth-test operands require BOOLEAN under the
[search-condition rules](../clauses/where.md#boolean-search-conditions). Known non-Boolean operands
fail CS02003 (42804), even in a short-circuited branch. Untyped NULL remains UNKNOWN;
valid Boolean input string literals coerce in these positions.

SQL has NULL, with no MISSING scalar. The predicates below also work in projections, JOIN ON,
HAVING, CASE, grouping, ordering and correlated subqueries.

Predicates, ordering, distinct results, grouping, and extrema share one non-null comparator.
Binary values compare unsigned bytes lexicographically without padding; a proper prefix sorts first.
Signed integers and decimals compare exactly. Mixed exact/approximate numerics compare represented
values without rounding either operand; for example, a bound binary floating-point `0.1` is
greater than exact decimal `0.1`. Compatible floating types retain their represented precision.

All floating-point not-a-number (NaN) values compare equal and below other non-null numerics.
The order is NaN, negative infinity, finite values, positive infinity. Signed zeros compare equal.
NaN is not null. Bind these values as parameters; SQL has no NaN/infinity literals. These comparison
rules do not broaden CAST, arithmetic, aggregate accumulation, or physical index-key semantics.
Floating range and signed-zero equality predicates scan or remain residual filters rather than
supplying unsafe index bounds.

Strings use effective collation; Boolean false sorts before true. Incompatible type families
raise a query error.

## IS DISTINCT FROM

`IS DISTINCT FROM` is a NULL-safe inequality test: exactly one NULL gives TRUE, two NULLs give
FALSE, and two non-NULL values use the negation of ordinary equality. `IS NOT DISTINCT FROM`
is its complement. Both always return a non-NULL BOOLEAN. Non-NULL comparisons use the same
type, CHAR coercion, NaN equality and effective collation rules as `=`. These predicates scan
rather than supplying index seek comparisons.

```sql
SELECT NULL IS DISTINCT FROM 1, NULL IS NOT DISTINCT FROM NULL,
       'a' COLLATE case_insensitive IS NOT DISTINCT FROM 'A'
FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'dbo';
```

The one row is `(TRUE, TRUE, TRUE)`.

## Boolean tests

Truth tests accept BOOLEAN or NULL and always return a non-NULL BOOLEAN.

| Input | IS TRUE | IS NOT TRUE | IS FALSE | IS NOT FALSE | IS UNKNOWN | IS NOT UNKNOWN |
| --- | --- | --- | --- | --- | --- | --- |
| TRUE | TRUE | FALSE | FALSE | TRUE | FALSE | TRUE |
| FALSE | FALSE | TRUE | TRUE | FALSE | FALSE | TRUE |
| NULL | FALSE | TRUE | FALSE | TRUE | TRUE | FALSE |

A known non-Boolean operand, including a supplied parameter, fails binding with `CS02003`
(42804), even on an empty table. `CHECK (flag IS TRUE)` rejects NULL, whereas `CHECK (flag)`
accepts it as UNKNOWN.

```sql
SELECT NULL IS TRUE, NULL IS NOT FALSE, NULL IS UNKNOWN
FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'dbo';
```

The result is `(FALSE, TRUE, TRUE)`. Postfix ISNULL/NOTNULL, ILIKE/NOT ILIKE, SIMILAR TO/NOT
SIMILAR TO, and IS JSON/IS OF/IS NORMALIZED report `CK00001`.

## LIKE and ESCAPE

`%` matches any run of original Unicode scalars and `_` matches one. The default escape is
backslash; `ESCAPE ''` disables escaping. A nonempty escape must be exactly one Unicode scalar.
It quotes the next scalar literally, including ordinary letters, `%`, `_`, or itself. The escape
is recognized before wildcards, even when it is `%` or `_`. Patterns and escapes may be literals,
parameters, columns or concatenated expressions.

For valid patterns, NULL in any operand gives UNKNOWN; NOT LIKE preserves UNKNOWN. A longer
escape or a pattern ending in its escape raises `CS02034` (22025). Constant and supplied
parameter patterns/escapes, including concatenations, validate during binding and compile once,
so invalid fixed patterns fail even on empty input or with a NULL matched value. This eager
validation intentionally differs from PostgreSQL's match-time trailing-escape check. Dynamic
operands validate when evaluated; dynamic NULL operands still yield NULL. A NULL escape is
handled before escape-length validation.

The value, pattern and escape all participate in effective collation resolution. Raw escapes
and wildcards are recognized before literal normalization. Wildcards consume original characters
under all four collations, including combining marks and characters whose normalized spelling
expands; normalization cannot erase an escape or turn it into a wildcard. CHAR values keep
stored padding, while CHAR patterns use text conversion. Matching is iterative and observes
statement cancellation.

Ordinary fixed prefix, suffix, contains and exact patterns use work linear in input length,
including under `invariant`. Pathological invariant (linguistic) matching has a per-match budget:

```text
64 × (input UTF-16 length + 1) × (raw pattern UTF-16 length + 1)
```

The budget uses checked 64-bit arithmetic. Each search transition and scalar-boundary probe
costs one unit. Each linguistic comparison/search additionally costs the sum of its span lengths,
with a minimum of one unit. Successful anchors charge only through the matched prefix; an
independent prefix failure after boundary analysis charges the literal extent. Each zero-initialized
32-bit visited-state word costs one unit, charged before allocation. Exceeding the budget raises
located `CS05040` (no assigned SQLSTATE). LIKE and NOT LIKE both fail; neither returns an
estimated Boolean. Over the wire this is ExecutionFailure and leaves the connection usable.
Cancellation reaches row-time matching, including cached views and stored CHECKs, and takes
precedence over a budget refusal. Other collations retain O(input × pattern) search.

```sql
SELECT '50%' LIKE '50!%' ESCAPE '!', 'a%' LIKE 'a\%',
       'a\z' LIKE 'a\%' ESCAPE '', '😀' LIKE '_',
       'A%' LIKE 'aX%' ESCAPE 'X' COLLATE case_insensitive
FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'dbo';
```

All five outputs are TRUE. The default backslash and empty escape follow PostgreSQL; ISO SQL
has no implicit escape and refuses an empty one. Malformed predicates, missing FROM in a
distinct test, or a missing ESCAPE operand use `CS00003`.

These examples need only the built-in dbo schema:

```sql
SELECT RPAD('a', 200000, 'a') COLLATE invariant LIKE '%a',
       ('abc' || RPAD('z', 199997, 'z')) COLLATE invariant LIKE 'abc%'
FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'dbo';
SELECT RPAD('เก', 1024, 'เก') COLLATE invariant
       LIKE RPAD('%เก', 96, '%เก') || 'ข'
FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'dbo';
```

The first returns `(TRUE, TRUE)`. The second fails with located `CS05040`; replacing LIKE
with NOT LIKE also fails. A subsequent query on that session still executes.

Distinct predicates, truth tests and LIKE ESCAPE persist in CHECK, parenthesized DEFAULT,
domain checks, SQL function bodies and views. Storage format 26 refuses earlier files without
an upgrade reader. These expression forms add no named clause.

## Temporal arithmetic

Day-time [INTERVAL literals](literals.md) and stored/parameter durations use TimeSpan precision.
The following pairs execute; operands keep their written order.

| Operands | Result |
| --- | --- |
| DATE + or - signed integer | DATE, adding/subtracting days |
| DATE - DATE | INTEGER day count |
| DATE + or - INTERVAL | TIMESTAMP at midnight plus/minus the duration |
| TIMESTAMP + or - INTERVAL | TIMESTAMP |
| TIMESTAMPTZ + or - INTERVAL | TIMESTAMPTZ; UTC instant arithmetic |
| TIMESTAMP/TIMESTAMPTZ - either timestamp type | INTERVAL; zone-less timestamp is interpreted at +00:00 |
| INTERVAL + or - INTERVAL, unary - INTERVAL | INTERVAL |

NULL operands propagate NULL. Other temporal arithmetic pairs fail planning with CS02003
(42804), including empty inputs: TIME + INTERVAL, TIMESTAMP - DATE, INTERVAL + integer and
multiplication/division/remainder over temporal values are excluded. Overflow or results outside
years 1..9999 use CS02002 (22003). TIMESTAMPTZ keeps its written offset when its resulting
local clock fits; otherwise it uses +00:00.

DATE compares with TIMESTAMP/TIMESTAMPTZ at midnight. A zone-less TIMESTAMP reads as UTC when
compared with an instant. Equal calendar values share hashes in DISTINCT/GROUP BY. CASE and
COALESCE promote calendar alternatives to TIMESTAMP, or TIMESTAMPTZ when an instant occurs.
[Clock functions](../functions/builtin-functions.md#clock-temporal-and-uuid-functions) use one
transaction-start instant for seek bounds and residual predicates.

```sql
SELECT DATE '2024-03-04' + 2,
       DATE '2024-03-04' - DATE '2024-03-01',
       DATE '2024-03-04' + INTERVAL '1' HOUR,
       INTERVAL '1' DAY + INTERVAL '2' HOUR
FROM t WHERE id = 1;
```

On the conformance fixture, this returns DATE 2024-03-06, INTEGER 3, TIMESTAMP 2024-03-04 01:00:00
and a 26-hour TimeSpan. SELECTing `-INTERVAL '1' DAY` from the same row returns minus one day.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT id FROM t WHERE age > 40 AND name LIKE 'G%' ORDER BY id;
SELECT id, age + 1 AS next_age FROM t ORDER BY id;
```

These conformance queries exercise predicates and row arithmetic.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Less-than followed by minus

In SQL, `a<-1` and `a <- 1` both mean `a < -1`, following PostgreSQL's lexical rule.
`a<--1` starts a `--1` line comment after `<`; it leaves the comparison without a right operand
and reports CS00003. Only GQL recognizes `<-` as a relationship arrow. NULL comparison
behavior is unchanged.

For a row with a = -2, `SELECT a FROM t WHERE a<-1` returns -2. The corresponding
`WHERE a<--1` form reports CS00003.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Predicates and results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPredicateExecutionTests.cs`.
- **Budget refusal and usable sessions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlPredicateWireTests.cs`.
- **Precedence and refusals** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlPredicateParserTests.cs`.
- **Temporal execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlT1TemporalTests.cs`.
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Evaluation/SqlExpressionEvaluator.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Expressions.cs`
