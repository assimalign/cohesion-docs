# Unsupported language features

OQL rejects recognized forms beyond its executable SQL++ subset with coded diagnostics.

## Language matrix

| Construct | Diagnostic | Boundary |
| --- | --- | --- |
| Nested SELECT, JOIN, WITH, set operations | CK00001 | One collection source; INSERT SELECT is supported |
| LIMIT m, n and FETCH FIRST | CK00001 | Use [LIMIT n OFFSET m](clauses/limit-offset.md); SELECT paging executes |
| Multi-binding quantifiers, ANY AND EVERY, ODMG FOR ALL/EXISTS | CK00001 | [ANY/SOME/EVERY](expressions/quantified-predicates.md) with one binding and [array IN](expressions/operators.md#in-and-between) execute |
| UNNEST in UPDATE/DELETE, NEST, WITHIN, FLATTEN calls | CK00001 | SELECT [UNNEST/LEFT UNNEST](clauses/unnest.md) executes |
| RETURNING/OPTIONS | CK00001 | INSERT, [UPSERT](statements/upsert.md), UPDATE SET/UNSET and DELETE execute; no statement TTL/version header |
| UPDATE USE KEYS, LIMIT or FOR | CK00001 | One collection, SET and/or UNSET and optional WHERE |
| DELETE USE KEYS or LIMIT | CK00001 | One collection and optional WHERE; no key-selector or delete pagination |
| IS KNOWN/IS UNKNOWN, UNDEFINED | CK00001 | MISSING is the absent value; NIL means NULL |
| FIRST/LAST/UNIQUE, ARRAY comprehensions/SET constructors | CK00001 | Reserved SQL++ forms; object and bracketed array constructors execute |
| Aggregate DISTINCT | CK00001 | `COUNT(DISTINCT x)` is outside SELECT-level deduplication |
| Computed object member names | CD00002 | Use identifier or string keys, such as `{a: 1}` |
| ARRAY_AGG, REGEXP_LIKE, REGEXP_REPLACE, REGEXP_POSITION | CK00001 | Deferred calls; [34 scalar functions](functions/scalar-functions.md) execute |
| Unknown function names | CD00008 | Not in the public executable function table |
| CREATE/DROP DATABASE, USE, ALTER | CK00001 | Database lifecycle belongs to the API |
| BEGIN/COMMIT/ROLLBACK | CK00001 | Explicit transactions use the session API; no wire transaction command |
| General database-qualified source | CD00002 | Sources stay in the session database |
| Multiple statements, invalid subscripts | CD00002 | One statement; array indices are nonnegative Int32 |
| Unsupported parameter marker ? or :name | CD00002 | Shared supported-form message |

DEFINE, ELEMENT, FLATTEN, STRUCT, LIST, BAG, LISTTOSET and TYPEOF are ordinary unquoted field
names. DEFINE is rejected statement-initially; the retired operations remain rejected in call
position. Quoted names and properties after dots retain field access, including d.end and d.case.

## Engine limits

Queries materialize input and results in managed memory. Bounded server response buffering does
not bound query execution memory or pooled-client result materialization. There is no external
sort or cost-statistics model. Strings use ordinal case-sensitive comparison. Documents protocol
server/client and hosting composition execute; replication and model-specific security remain
outside this reference's implemented surface.

## See also

[Language (OQL)](index.md) · [Diagnostics](diagnostics.md) · [Document](../index.md)

## Sources

- **Reservations and capabilities** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Vocabulary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/Internal/OqlUnsupportedVocabulary.cs`.
