# SCAN

SCAN returns visible entries in ascending unsigned lexicographic key order.

> **Status:** Implemented.

## Syntax

```syntaxsql
SCAN [ FROM @start ] [ TO @end ] [ PREFIX @prefix ] [ LIMIT { limit_count | @limit } ]
```

The diagrams show a conventional clause order. Accepted clauses may appear in any order,
each at most once. A non-empty `PREFIX` cannot be combined with non-empty `FROM` or `TO` bounds.

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
unbounded and an omitted limit imposes no row cap. Zero-length byte arrays also mean no bound:
`SCAN TO @empty` scans to the end of the key space. This applies to `FROM`, `TO`, and `PREFIX`.
An empty clause still counts toward the one-clause rule. Null operands remain invalid; there
is no MISSING operand value in this grammar.

The result columns are `key` (binary), `value` (binary), and `etag` (64-bit integer).

## Remarks

The primary index supplies the scan cursor under the command's snapshot. A prefix maps to
the range from the prefix, inclusive, to its byte successor, exclusive. A prefix consisting
entirely of `0xFF` bytes has no upper bound.

Index reads use bounded batches with cancellation at open, each move and while waiting for the
tree's read latch. LIMIT stops consuming after the requested rows; it does not collect the entire
selected index range. The command still builds its result, and disposal releases the cursor's
owned keys. See [range cursors](../../../dotnet-apis/resources/database/assimalign-cohesion-database-indexing/b-tree-cursor.md).

Repeated clauses fail with `CV00010`, even when the first operand is empty. A null or non-byte
operand fails with `CV02012`; a non-empty prefix combined with non-empty explicit bounds fails
request validation with `CV00004`. These are parse failures and execute nothing.

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

With `empty` bound to `Array.Empty<byte>()`, this returns all five keys in order:

```text
SCAN TO @empty
```

`SCAN PREFIX @p FROM @empty TO @empty` returns `b:1` and `b:2`. Repeating the upper clause
as `SCAN TO @empty TO @e` fails with `CV00010`.

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
- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Internal/Execution/KeyValueCommandParser.cs`.
- **Request** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/KeyValueScanRequest.cs`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
