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
Foreign-key string columns must have matching effective collations.

Unknown names, culture-aware rules, user-defined collations, session overrides, and collation-aware
full-text indexes are outside the subset and produce `COHDBL001` or an explicit execution error.
Compatibility `invariant` fails under invariant globalization. SQL defaults do not govern the
Document, Graph, Key-Value Pair, or Blob models.

## Examples

```sql
CREATE TABLE people (name TEXT COLLATE case_insensitive UNIQUE);
SELECT name FROM people WHERE name = 'alice';
SELECT name FROM people WHERE name = 'Alice' COLLATE binary;
```

The second query inherits the column comparison; the third overrides it for that expression.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Types/docs/OVERVIEW.md`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCollationExecutionTests.cs`
- **Collation decisions** — `cohesion/docs/programs/COLLATION_DESIGN.md`
