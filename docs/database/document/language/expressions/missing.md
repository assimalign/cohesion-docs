# MISSING and presence tests

MISSING represents an absent value and is distinct from explicit JSON NULL in OQL's SQL++ subset.

## Syntax

```syntaxsql
MISSING
expression IS [ NOT ] { NULL | MISSING | VALUED }
```

## Remarks

Absent fields, incompatible shapes, navigation through NULL and out-of-range array indexes
produce MISSING. Reading a NULL field itself returns NULL. Dotted access does not map across arrays.
Projections and [object constructors](object-constructors.md) omit MISSING members and retain NULL.
Documents rows support `DatabaseRow.SupportsMissing` and `IsMissing(ordinal)`; GetValue returns
null for either unknown state, while IsNull returns true only for explicit NULL.

For top-level [SELECT VALUE/RAW](../statements/select.md), MISSING becomes NULL before DISTINCT,
ordering by that projection and result counting on every transport. Its row has IsMissing false
and IsNull true. [Array constructors](array-constructors.md) replace MISSING elements with JSON
null while retaining their positions. Named projections and object members still omit MISSING.

| Test | Present non-null value | NULL | MISSING |
| --- | --- | --- | --- |
| IS NULL | FALSE | TRUE | MISSING |
| IS NOT NULL | TRUE | FALSE | MISSING |
| IS MISSING | FALSE | FALSE | TRUE |
| IS NOT MISSING | TRUE | TRUE | FALSE |
| IS VALUED | TRUE | FALSE | FALSE |
| IS NOT VALUED | FALSE | TRUE | TRUE |

Comparisons and arithmetic propagate MISSING before NULL. Ascending order is MISSING, NULL, FALSE,
TRUE, number, ordinal string, array, object; descending reverses it. GROUP BY keeps MISSING and NULL
separate. COUNT(expr), SUM, AVG, MIN and MAX skip both; COUNT(*) counts input rows, including expanded UNNEST rows.

## Boolean contexts

WHERE, HAVING, AND, OR, NOT and searched CASE use one truth conversion. Nonzero numbers and nonempty
strings, arrays and objects are true; zero and empty values are false. NULL and MISSING retain their
states. The tables below use T/F/N/M for TRUE/FALSE/NULL/MISSING.

| AND | T | F | N | M |
| --- | --- | --- | --- | --- |
| T | T | F | N | M |
| F | F | F | F | F |
| N | N | F | N | M |
| M | M | F | M | M |

| OR | T | F | N | M |
| --- | --- | --- | --- | --- |
| T | T | T | T | T |
| F | T | F | N | M |
| N | T | N | N | N |
| M | T | M | N | M |

NOT maps T/F/N/M to F/T/N/M. WHERE and HAVING retain only converted TRUE. NIL is a NULL synonym;
UNDEFINED is reserved and reports `COHDBL001`, as do deferred IS KNOWN and IS UNKNOWN forms.

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION presence;
INSERT INTO presence (KEY, VALUE) VALUES ('a', {}), ('b', {'v': NULL}), ('c', {'v': 1});
SELECT META().id AS id, v FROM presence ORDER BY META().id;
SELECT META().id AS id FROM presence WHERE v IS MISSING;
SELECT COUNT(v) AS present, COUNT(*) AS total FROM presence WHERE 1;
```

Projected JSON objects are `{"id":"a"}`, `{"id":"b","v":null}` and `{"id":"c","v":1}`. The presence
filter returns a; the aggregate row is `(1, 3)`. The wire carries projected results as Document
frames, preserving the distinction rather than replacing missing cells with NULL.

[UPDATE SET/UNSET](../statements/update.md#set-and-unset-paths) uses MISSING as removal for named
members and JSON null for existing array elements. Removing an absent member is a no-op, but a
missing array or out-of-range index fails for both SET and UNSET.

## See also

[Operators](operators.md) · [WHERE](../clauses/where.md) · [DatabaseRow](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-execution/database-row.md)

## Sources

- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentMissingTests.cs`.
- **Truth conversion** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/DocumentExpressionEvaluator.Values.cs`.
- **Wire** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentMissingWireTests.cs`.
