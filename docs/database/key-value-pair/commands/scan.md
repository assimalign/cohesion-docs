# SCAN

SCAN returns visible entries in ascending unsigned lexicographic key order.

> **Status:** Implemented.

## Syntax

```syntaxsql
SCAN [ FROM @start ] [ TO @end ] [ LIMIT { limit_count | @limit } ]
SCAN PREFIX @prefix [ LIMIT { limit_count | @limit } ]
```

The diagrams show a conventional clause order. Accepted clauses may appear in any order,
each at most once. `PREFIX` cannot be combined with `FROM` or `TO`.

## Supported clauses

| Clause | Support | Meaning |
|---|---|---|
| `FROM @start` | Supported | Inclusive lower bound |
| `TO @end` | Supported | Exclusive upper bound |
| `PREFIX @prefix` | Supported | Keys beginning with the bound byte prefix |
| `LIMIT limit_count` | Supported | Maximum rows as a nonnegative integer literal |
| `LIMIT @limit` | Supported | Maximum rows as an integer parameter |

## Arguments

`@start`, `@end`, and `@prefix` bind `byte[]` values. The limit must fit between `0` and
`int.MaxValue`; a parameter can be an `int` or `long` within that range. Omitted bounds are
unbounded and an omitted limit imposes no row cap.

The result columns are `key` (binary), `value` (binary), and `etag` (64-bit integer).

## Remarks

The primary index supplies the scan cursor under the command's snapshot. A prefix maps to
the range from the prefix, inclusive, to its byte successor, exclusive. A prefix consisting
entirely of `0xFF` bytes has no upper bound.

Duplicate clauses, negative limits, missing operands, and a prefix combined with explicit
bounds fail during parsing rather than execution.

## Examples

The command corpus inserts `a:1`, `a:2`, `b:1`, `b:2`, and `c:1` as encoded keys.
With `s` bound to `a:2` and `e` to `b:2`, this returns `a:2` and `b:1`:

```text
SCAN FROM @s TO @e
```

With `p` bound to the bytes of `b:`, this returns the two `b:` entries:

```text
SCAN PREFIX @p
```

These separate corpus commands cap the scan at two rows and at the bound count, respectively:

```text
SCAN LIMIT 2
SCAN LIMIT @n
```

## See also

- **[Commands](index.md)** — grammar and lexical conventions.
- **[Operand types](../operand-types.md)** — accepted parameter values.
- **[GET](get.md)** — point lookup.

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Design** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/DESIGN.md`.
- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Internal/KeyValueCommandParser.cs`.
- **Request** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/KeyValueScanRequest.cs`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
