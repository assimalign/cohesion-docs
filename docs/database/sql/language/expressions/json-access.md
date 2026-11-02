# JSON access and IS JSON (Cohesion SQL)

Extracts JSON members and array elements, or tests whether a value is JSON of a given kind.

> **Status:** Partial. Extraction, kind tests and scalar casts execute. SQL/JSON paths remain deferred.

## Syntax

```syntaxsql
json_expression -> selector
json_expression ->> selector
expression IS [ NOT ] JSON [ VALUE | SCALAR | ARRAY | OBJECT ]
```

## Extraction

The left operand must have SQL type JSON or JSONB; cast text first. A text selector names an
object member, compared ordinally. SMALLINT and INTEGER selectors select zero-based array
elements. Negative indexes count from the end; an out-of-range index returns SQL NULL.
An untyped string literal such as `'0'` is a text key, not an array index. BIGINT and TINYINT
selectors, including typed NULLs, require an explicit INTEGER cast.

`->` retains the input JSON/JSONB type and the selected value's JSON spelling. `->>` returns
TEXT, unquoting strings and decoding their escapes. Arrays and objects returned by `->>` keep
their JSON serialization. A text selector supplies the output collation; otherwise TEXT uses
the database default.

| Input condition | `->` | `->>` |
| --- | --- | --- |
| Missing member, out-of-range index or wrong container kind | SQL NULL | SQL NULL |
| Member containing JSON `null` | JSON `null` | SQL NULL |
| SQL NULL input or selector, with a valid signature | SQL NULL | SQL NULL |

Every non-array root, including a JSONB scalar at index 0 or -1, returns NULL for an index
selector. Duplicate keys resolve to the last member, including escaped-equivalent names.
Arrows chain left to right. They share the generic precedence level with concatenation and
bitwise operators; [postfix `::`](cast.md#postfix-casts) binds tighter.

CASE/COALESCE alternatives in a JSON operand or selector resolve to one SQL type before
choosing member lookup or indexing. Untyped strings and NULL adopt a declared alternative;
an all-untyped set resolves to TEXT. Incompatible declared types, such as INTEGER and TEXT,
or JSON and JSONB, fail with CS02080 before rows are read. Even skipped untyped alternatives
undergo input conversion; invalid input retains CS02022 (22P02).

```sql
SELECT '{"a":{"b":"hello"},"dup":1,"dup":2}'::JSON -> 'a' ->> 'b',
       '[10,20,30]'::JSONB ->> -1,
       '{"dup":1,"dup":2}'::JSON ->> 'dup',
       '{"a":null}'::JSON -> 'a', '{"a":null}'::JSON ->> 'a';
SELECT '[10,20]'::JSON ->> CASE WHEN FALSE THEN 1 ELSE '1' END,
       '[10,20]'::JSON ->> '1';
```

The rows are `('hello', '30', '2', JSON null, SQL NULL)` and `('20', NULL)`.
`SELECT '[]'::JSON -> 1::BIGINT;` fails with CS01067; cast the selector to INTEGER.

## IS JSON

The predicate accepts character, JSON and JSONB input. VALUE is the default and accepts any
JSON value. SCALAR accepts strings, numbers, Booleans and JSON `null`; ARRAY and OBJECT require
their respective containers. Malformed text returns FALSE, or TRUE with NOT. SQL NULL returns
SQL NULL under either spelling. This tests syntax rather than decoding Unicode escapes.
`WITH UNIQUE KEYS` and `WITHOUT UNIQUE KEYS` are refused with CK00001.

```sql
SELECT 'null' IS JSON SCALAR, '[1,2]' IS JSON ARRAY,
       '{bad}' IS JSON, '{bad}' IS NOT JSON, NULL IS JSON;
```

Returns `(TRUE, TRUE, FALSE, TRUE, NULL)`; JSON `null` is a scalar.

## Validation and limits

Access and JSON/JSONB column assignment validate one complete value with strict UTF-8 and a
maximum nesting depth of 128. Invalid input fails with CS02078. Extraction also refuses invalid
Unicode escapes and `\u0000` anywhere in the document, even when the requested member is
missing. IS JSON instead returns FALSE for malformed syntax, invalid encoding or excess depth.
JSONB retains original UTF-8 text, whitespace, key order and duplicates; it has no normalization.

[Scalar casts](cast.md#json-scalar-casts) convert JSON Booleans/numbers and JSON `null` to SQL
scalars. Casting to TEXT keeps JSON serialization, including quotes around a JSON string.

## Partial indexes

A partial-index predicate using extraction or IS JSON can serve a query that repeats its
conjunct exactly. Different selector/source types, keys, arrow forms, kinds or negations do
not establish that proof. The complete query predicate still filters rows.

Execute these statements separately:

```sql
CREATE TABLE json_rows (id INT, doc JSON);
INSERT INTO json_rows VALUES (1,'{"k":"x"}'),(2,'{"k":"y"}');
CREATE INDEX ix_json_x ON json_rows(id) WHERE doc ->> 'k' = 'x';
SELECT id FROM json_rows WHERE id = 1 AND doc ->> 'k' = 'x';
```

SELECT returns `1`; the repeated conjunct permits the partial index.

## Diagnostics and deferred forms

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01067 | Invalid extraction or IS JSON signature, including an unresolved NULL left operand or BIGINT selector | 42883 |
| CS02078 | Malformed access/assignment document, invalid encoding or Unicode conversion, or depth over 128 | 22032 |
| CS02079 | Wrong JSON kind for a numeric or Boolean scalar cast | 22023 |
| CS02080 | Incompatible declared CASE/COALESCE alternatives in a JSON operand or selector | 42804 |
| CK00001 | WITH/WITHOUT UNIQUE KEYS is unsupported | — |

JSON_VALUE, JSON_QUERY, JSON_EXISTS and SQL/JSON paths are planned for SQ-N2. JSON_TABLE,
question-mark operators and JSONB normalization remain deferred beyond this subset.

## See also

[Expressions](index.md) · [Operators](operators.md) · [CAST](cast.md) · [CREATE INDEX](../statements/create-index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJsonAccessTests.cs` and `SqlJsonReviewFixTests.cs` in the same directory.
- **Validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Evaluation/SqlJsonAccess.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJsonParserTests.cs`.
