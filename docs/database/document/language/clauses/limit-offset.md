# LIMIT and OFFSET

LIMIT caps the returned rows and OFFSET skips rows before that cap is applied.

## Syntax

```syntaxsql
[ LIMIT count [ OFFSET count ] | OFFSET count [ LIMIT count ] ]
<count> ::= [ + | - ] integer_literal | parameter
```

Each clause may occur once, alone or in either order, after ORDER BY if present.
Except for LIMIT 0, paging applies after WHERE, grouping/HAVING, projection, DISTINCT and ordering.
After static validation, LIMIT 0 preserves planned column metadata and skips all source, predicate,
projection and callback evaluation, including blocking stages. This applies to inner derived
SELECTs too. Simple [derived/expression sources](from.md) page before projection and infer dynamic
types over the selected page. Paging also applies
to system collections and the SELECT source of INSERT or UPSERT.

## Arguments

Counts must be integral, nonnegative signed Int64 values (0 through 9,223,372,036,854,775,807).
`+1` and `-0` are valid. Literal `2.0` and `1e2` are invalid even though their values are integral.
Parameters use the ordinary `$name`, `@name` or `$1` binding contract. Integral decimal and JSON
numeric parameter values, including JSON `1.0`, are accepted; finite integral floating values
are accepted without rounding.

NULL/NIL and MISSING are invalid counts, including a NULL parameter. Boolean, string, fractional,
nonfinite and out-of-range values are invalid too. Expressions, parentheses and LIMIT ALL are
outside the count grammar. A limit of zero returns no rows; an offset past the end returns an
empty page. Empty pages retain column metadata and whole-document query mode.

## Remarks

Scans and seeks establish ordinal document-ID order, groups use group-key order, and system
sources use catalog order. ORDER BY ties retain that baseline order. Use an explicit ORDER BY
and unchanged data or a shared snapshot when reading successive pages. Query AffectedCount
remains -1; wire completion counts only the returned rows. EXPLAIN validates parameters and
shows resolved counts without reading documents.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `CD00007` | Invalid count, at the full operand span; parse-time literals and plan-time parameters use the same clause-specific message |
| `CD00002` | Missing operand, duplicate clause (at its second keyword), arithmetic after a count, or ORDER BY after paging |
| `CD01005` | Count parameter has no supplied value |
| `CK00001` | `LIMIT m, n` (at LIMIT, before validating m), FETCH FIRST, or paging on UPDATE, DELETE or INSERT/UPSERT VALUES |

These codes have no assigned SQLSTATE. LIMIT/OFFSET are reserved words; quote a field name or
use `d.limit`/`d.offset`. Projection labels after AS may still use them.

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES ('a', 1), ('b', 2), ('c', 3);
SELECT META().id AS id FROM items ORDER BY id OFFSET +1 LIMIT 1;
```

The query returns `{"id":"b"}`. `LIMIT 0` or `OFFSET 9223372036854775807` returns no rows.
With bare-name parameters `skip = 1` and `take = 1`, this returns the same row:

```sql
SELECT META().id AS id FROM items ORDER BY id LIMIT @take OFFSET $skip;
```

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [ORDER BY](order-by.md) · [Parameters](../expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Paging.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlPagingParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientPagingTests.cs`.
