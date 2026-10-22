# COLLATE (Cohesion SQL)

Overrides the string comparison rule for a column or expression.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
column_name string_type COLLATE <collation_name>
expression COLLATE <collation_name>

<collation_name> ::= { binary | case_insensitive | case_accent_insensitive | invariant }
```

## Arguments

- **`string_type`** — A string type in a column definition.
- **`expression`** — A value whose comparison collation is overridden.
- **`collation_name`** — One of the four listed names; `invariant` is scan-only.

## Remarks

Resolution starts at the database default, then the column, then an explicit expression override.
Nested overrides resolve innermost first. Peer operands with equal precedence use the left operand.
An unconfigured database uses `binary`. Defaults and column metadata persist across reopen; the
database default is fixed for its lifetime and cannot be changed underneath existing index keys.
There is no session override.

| Name | Comparison | Index support |
| --- | --- | --- |
| `binary` | Unicode code-point order through unchanged UTF-8 bytes | Supported |
| `case_insensitive` | Invariant Unicode simple case fold, then UTF-8 | Supported |
| `case_accent_insensitive` | Canonical decomposition, mark removal, simple case fold, then UTF-8 | Supported |
| `invariant` | Compatibility linguistic comparison | Scan only; index-backed constraints rejected |

The three index-backed transforms use pinned Unicode 17.0 tables, not runtime locale data.
They preserve original spelling in stored and returned values. Effective collation governs
comparison, membership, ranges, `LIKE`, ordering, grouping, distinct results, extrema, and uniqueness.
Index seeks require a matching predicate/index collation; a differing override scans.
Foreign-key string columns must have matching effective collations; a mismatch is COHSQLE063 (42P21).

Unary signs bind above COLLATE: `-a COLLATE binary` means `(-a) COLLATE binary`.
IS [NOT] DISTINCT FROM uses the same effective collation as equality. LIKE resolves collation
from the value, pattern and ESCAPE operand, then matches literals under that rule; its wildcards
consume original Unicode scalars under every collation. See [predicates](../expressions/operators.md#like-and-escape).

Ordinary invariant LIKE prefix, suffix, contains and exact patterns are linear in input length.
Pathological invariant (linguistic) LIKE/NOT LIKE has a per-match work budget of
`64 × (input UTF-16 length + 1) × (raw pattern UTF-16 length + 1)`, computed with checked
64-bit arithmetic. Transitions and boundary probes each cost one unit; linguistic comparisons
and searches additionally charge the sum of their span lengths, at least one unit. Successful
anchors charge through the matched prefix; an independent prefix failure after boundary analysis
charges the literal extent. Each zero-initialized 32-bit visited-state word costs one unit before
allocation. Exhaustion raises located `COHSQLE040` (no SQLSTATE) for LIKE or NOT LIKE, without a
Boolean result. ExecutionFailure leaves the wire connection usable. Statement cancellation reaches
the matcher and takes precedence over exhaustion. Other collations retain O(input × pattern) search.

Unknown names parse as identifiers and fail binding with COHSQLE052 (42704), including column
and DEFAULT declarations. A missing collation in loaded persisted text instead uses COHSQLE058
(XX001). Quoted names follow ordinary delimited-identifier rules. Culture-aware rules,
user-defined collation creation, session overrides and collation-aware full-text indexes remain
outside the subset. CREATE INDEX under `invariant` uses COHSQLE013 (no assigned SQLSTATE);
index-backed constraints use COHSQLE057 (0A000).
Compatibility `invariant` fails under invariant globalization. SQL defaults do not govern the
Document, Graph, Key-Value Pair, or Blob models.

## Examples

```sql
SELECT 'a' COLLATE "nope";
```

This reports COHSQLE052 (42704) during binding. NULL remains NULL under a collation override;
SQL has no MISSING scalar.

```sql
CREATE TABLE people (name TEXT COLLATE case_insensitive UNIQUE);
SELECT name FROM people WHERE name = 'alice';
SELECT name FROM people WHERE name = 'Alice' COLLATE binary;
```

The second query inherits the column comparison; the third overrides it for that expression.

```sql
SELECT RPAD('a', 200000, 'a') COLLATE invariant LIKE '%a'
FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'dbo';
```

This returns TRUE. The [LIKE examples](../expressions/operators.md#like-and-escape) also show a
pathological pattern refused with COHSQLE040.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Types/docs/OVERVIEW.md`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCollationExecutionTests.cs`
- **Collation decisions** — `cohesion/docs/programs/COLLATION_DESIGN.md`
