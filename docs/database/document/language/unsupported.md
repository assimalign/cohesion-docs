# Unsupported language features

OQL rejects recognized forms beyond its executable SQL++ subset with coded diagnostics.

## Language matrix

| Construct | Diagnostic | Boundary |
| --- | --- | --- |
| Nested SELECT, JOIN, WITH, set operations | COHDBL001 | One collection source; INSERT SELECT is supported |
| DISTINCT/ALL, LIMIT/OFFSET | COHDBL001 | No deduplication or pagination clauses |
| IN array form, EXISTS and quantified predicates | COHDBL001 | Use a nonempty parenthesized IN value list |
| UPDATE/DELETE/UPSERT/RETURNING | COHDBL001 | INSERT is supported; use collection API for replacement/deletion |
| IS KNOWN/IS UNKNOWN, UNDEFINED | COHDBL001 | MISSING is the absent value; NIL means NULL |
| FIRST/LAST/UNIQUE, ARRAY/SET constructors | COHDBL001 | Reserved SQL++ forms; object constructors execute |
| Unknown scalar calls, including ABS | COHDBL001 | Executable function profile contains aggregates and META |
| CREATE/DROP DATABASE, USE, ALTER | COHDBL001 | Database lifecycle belongs to the API |
| BEGIN/COMMIT/ROLLBACK | COHDBL001 | Explicit transactions use the session API; no wire transaction command |
| General database-qualified source | OQL0002 | Sources stay in the session database |
| Multiple statements, invalid subscripts | OQL0002 | One statement; array indices are nonnegative Int32 |
| Unsupported parameter marker ? or :name | OQL0002 | Shared supported-form message |

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
