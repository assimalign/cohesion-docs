# SQL/JSON path functions (Cohesion SQL)

JSON_VALUE extracts a scalar, JSON_QUERY returns JSON items, and JSON_EXISTS tests for a match.

> **Status:** Implemented for the path subset below.

## Syntax

```syntaxsql
JSON_VALUE(context, path [RETURNING type]
           [value_behavior ON EMPTY] [value_behavior ON ERROR])
JSON_QUERY(context, path [RETURNING type]
           [WITHOUT [ARRAY] WRAPPER |
            WITH [CONDITIONAL | UNCONDITIONAL] [ARRAY] WRAPPER]
           [{KEEP | OMIT} QUOTES [ON SCALAR STRING]]
           [query_behavior ON EMPTY] [query_behavior ON ERROR])
JSON_EXISTS(context, path [{TRUE | FALSE | UNKNOWN | ERROR} ON ERROR])

value_behavior ::= ERROR | NULL | DEFAULT expression
query_behavior ::= ERROR | NULL | DEFAULT expression | EMPTY [ARRAY] | EMPTY OBJECT
```

ON EMPTY precedes ON ERROR; each appears at most once. Context accepts character, JSON or JSONB
input. Path must be a string constant or a parameter (`$1`, `$name`, `@name`), bound by its bare
name. Other expressions fail with CS01068. Paths are parsed before rows, including empty inputs
and skipped CASE alternatives. PASSING is refused with CS02081.

## Results and defaults

| Function | Default result and clauses | Successful result |
| --- | --- | --- |
| JSON_VALUE | TEXT; NULL ON EMPTY; NULL ON ERROR | Exactly one scalar; strings are unquoted, Booleans become `t`/`f` in TEXT |
| JSON_QUERY | JSONB; WITHOUT WRAPPER; KEEP QUOTES; NULL ON EMPTY; NULL ON ERROR | One scalar, object or array without a wrapper, or a wrapped sequence |
| JSON_EXISTS | BOOLEAN; FALSE ON ERROR | TRUE for any selected item, FALSE for an empty sequence |

RETURNING selects the conversion type for VALUE or QUERY. VALUE rejects objects, arrays and
multiple items. Numeric JSON strings use the target's input conversion: `"12.0"` cannot become
INTEGER; JSON numbers normalize first, so `1e2` becomes INTEGER 100. Character bounds and domain
checks apply. Exact JSON/JSONB VALUE targets serialize the scalar; domains over those types use
scalar input coercion.

QUERY's CONDITIONAL wrapper wraps multiple items only. WITH WRAPPER means UNCONDITIONAL and
wraps every nonempty sequence. An empty sequence still invokes ON EMPTY. OMIT QUOTES requires
WITHOUT WRAPPER. For JSON/JSONB output, an unquoted string is reparsed: `"12"` becomes JSON number
12, while `"x"` causes a conversion error. Selected JSON results use jsonb key ordering and numeric
spelling; stored JSONB bytes and arrow serialization retain their existing representation.

DEFAULT expressions may use parameters, scalar calls and operators, but no columns, subqueries,
aggregates or windows. Untyped string defaults convert during binding even when unused. A domain's
base input conversion occurs then; its CHECK runs when that behavior is selected.

## NULL and error handling

SQL NULL context or a NULL path parameter produces SQL NULL. RETURNING domain constraints still
run on NULL and their conversion errors reach ON ERROR. Context conversion happens first, even
with a NULL path. JSON `null` becomes SQL NULL for VALUE, JSON `null` for QUERY, and an existing
item (TRUE) for EXISTS.

ON ERROR handles structural path errors, result cardinality/kind errors and RETURNING conversion
or domain errors. Malformed context JSON, invalid UTF-8/Unicode and context numbers outside jsonb
input bounds escape it. ERROR ON EMPTY, errors evaluating or converting a behavior value, budgets
and cancellation also escape. Invalid path syntax is refused before execution.

## Path language

The optional `lax` (default) or `strict` prefix selects structural handling.

| Form | Selects |
| --- | --- |
| `$` | Context root |
| `.name`, `."quoted name"` | Named object member; quoted names use JSON string escapes |
| `.*` | Object members |
| `[n]`, `[n, m]` | Zero-based array indexes, in the specified sequence |
| `[n to m]` | Inclusive index range |
| `[last]`, `[last - k]` | Last index or a nonnegative offset before it; also usable in lists/ranges |
| `[*]` | Array elements |

Indexes are signed INTEGER constants. Mode, `last` and `to` ignore case; member names retain case.
Unquoted names start with a Unicode letter or underscore, followed by letters/digits/underscores.
Duplicate members resolve to the last value.

Lax member access unwraps one array level; array access treats a non-array as a singleton.
Missing members, incompatible containers and out-of-range indexes disappear; ranges clip to valid
indexes. Strict mode raises structural errors and checks the whole path, including EXISTS after
an earlier match. Final arrays are never implicitly flattened.

Every other construct is refused with CS02081, including filters, item methods, arithmetic,
comparisons, Boolean expressions, variables, `@`, regex/prefix tests, `exists`, `is unknown`,
recursive `.**` and malformed accessors.

## Limits

A path may contain at most 65,536 UTF-16 code units and 256 accessors. Path execution shares a
1,000,000-visit work cap per statement with window-frame recomputation. Transient DOM, result
references and selected-result serialization share StatementMemoryBudget. CS05094 reports these
path limits; cancellation remains cancellation.

## Examples

```sql
SELECT JSON_VALUE('{"a":"123"}', '$.a' RETURNING INT),
       JSON_QUERY('[1,2]', '$[*]' RETURNING TEXT WITH WRAPPER),
       JSON_EXISTS('null', '$');
SELECT JSON_VALUE('{}', '$.a' DEFAULT 'empty' ON EMPTY),
       JSON_VALUE('{}', 'strict $.a' DEFAULT 'error' ON ERROR);
```

The rows are `(123, '[1, 2]', TRUE)` and `('empty', 'error')`, as checked by SqlJsonPathTests.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01068 | Path is not a string constant or parameter | 42804 |
| CS01069 | Invalid context, path, DEFAULT or RETURNING signature | 42804 |
| CS02081 | Invalid/unsupported path or PASSING; refused before rows | 42601/0A000 |
| CS02082 | Strict accessor structural error | 2203A |
| CS02083 | Empty sequence with ERROR ON EMPTY | 2203F |
| CS02084 | Context number exceeds jsonb exponent, precision or scale bounds | 22003 |
| CS05093 | Result cardinality or scalar-kind violation | 22034/2203B |
| CS05094 | Path size, statement work or transient result memory limit exceeded | 54000 |

Malformed context input retains [CS02078](../expressions/json-access.md); conversion diagnostics
retain their existing codes. Clause syntax errors use CS00003.

## Compatibility

SQL catalog/record-space format **34** refuses format 33 and earlier before data recovery. There
is no in-place upgrade; compiled schema v11 and JSONB storage bytes are unchanged. See the
[export and rebuild procedure](../../index.md#sql-format-34-and-schema-v11).
Ordinary function calls named json_value, json_query or json_exists now render with quoted names.
A quoted `"json_value"(...)` remains an ordinary function call.

## See also

[Functions](index.md) · [JSON access](../expressions/json-access.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`, SQL/JSON path functions (#1722, SQ-N2).
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJsonPathTests.cs` and `SqlJsonPathFixTests.cs`.
- **Binding and error boundaries** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Evaluation/SqlExpressionEvaluator.JsonFunctions.cs`.
