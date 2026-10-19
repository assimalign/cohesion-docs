# Assimalign.Cohesion.Database.Documents.Language design

OQL parses the Documents engine's executable SQL++ subset into typed statements and expressions.

## Parser and AST

The shared lexer/parser infrastructure owns tokenization, diagnostics and source locations. OQL
partial parsers own dispatch, SELECT, DDL, DML and expressions. OqlQueryStatement wraps one SELECT,
INSERT, UPSERT, UPDATE, DELETE, EXPLAIN, collection DDL or index DDL root. Parser-produced lists are read-only snapshots;
public constructors retain child references and semantic validation belongs to the engine.

Offsets use zero-based UTF-16 with exclusive ends and one-based lines. Parsing returns diagnostics;
DocumentQueryRequest and session execution reject all parser errors before planning. Expressions
have a 128-level recursive parse bound. [OqlFunctions](oql-functions.md) is the immutable table of
META, five aggregates and 34 scalars, shared by parser, profile and engine. [OqlFunction](oql-function.md)
records arity, JSON kinds and result type; [OqlCallExpression](oql-call-expression.md) retains calls.
Unknown calls report OQL0008, invalid arity/star OQL0006 and deferred calls COHDBL001.

## SQL++ values

MISSING has its own OqlMissingExpression. An absent path or navigation through NULL produces MISSING;
reading a null member produces NULL. Result/object members omit MISSING and retain NULL. GROUP BY
keeps the two separate and ascending order places MISSING first. Aggregates skip both unknown states.
WHERE/HAVING/AND/OR/NOT/searched WHEN share SQL++ truth conversion: nonzero numbers and nonempty
strings/arrays/objects are true. See the [truth tables](../../../../database/document/language/expressions/missing.md).

OqlLiteralExpression.Value holds parsed numbers as JsonElement tokens, preserving every digit until
engine validation. Lossy decimal conversion is COHDOC007 (22003), malformed syntax OQL0004.
OqlInExpression, OqlBetweenExpression, OqlLikeExpression and OqlCaseExpression retain their operands,
negation, optional escape or ordered branches. LIKE is ordinal, Unicode-scalar based and cancellable.
Object constructors retain keyed members; META binds stored-document id/version.

[OqlSelectExpression](oql-select-expression.md) carries IsDistinct and IsValue constructor flags.
ALL is the default; VALUE and RAW share IsValue. Bare MISSING becomes NULL before DISTINCT,
ordering by that projection and counting. [OqlArrayExpression](oql-array-expression.md) retains
ordered elements, converting MISSING to JSON null. OqlClauses adds Distinct (`DISTINCT`), SelectValue
(`SELECT VALUE`), ObjectConstructor (`OBJECT CONSTRUCTOR`) and ArrayConstructor (`ARRAY CONSTRUCTOR`)
capability names. Constructors validate and group recursively; text parsing keeps the 128-level bound.

## Result paging

OqlClauses.Limit (`LIMIT`) and Offset (`OFFSET`) enable final SELECT paging.
[OqlSelectExpression](oql-select-expression.md) retains optional Limit and Offset operands, after
IsValue in its constructor. Both survive alias rewriting. Counts are nonnegative Int64 integer
literals or parameters; NULL is invalid. Invalid values use located OQL0007 during parsing or
planning; missing values use COHDOC005. LIMIT m, n is COHDBL001; use LIMIT n OFFSET m.
Either clause may come first. INSERT/UPSERT SELECT consumes the final page; DML VALUES, UPDATE
and DELETE paging remain unsupported.

## Whole-document UPSERT

[OqlUpsertExpression](oql-upsert-expression.md) uses INSERT's KEY/VALUE shapes with a distinct
OqlClauses.Upsert capability. VALUES or materialized SELECT input replaces whole documents.
Repeated keys use DocumentDuplicateKeyException/COHDOC012 with a statement-specific message.
EXPLAIN includes Upsert/UpsertSelect. OPTIONS and RETURNING remain excluded with COHDBL001.

## Partial updates

[OqlUpdateExpression](oql-update-expression.md) retains Collection, Alias, Assignments, Unset and
Predicate. [OqlUpdateAssignment](oql-update-assignment.md) retains a Path and Value. OqlClauses.Update
and OqlClauses.Unset enable UPDATE and UNSET. SET precedes optional UNSET; UNSET alone is valid.
Every value reads the original document. Invalid targets/shapes use COHDOC016, normalized overlaps
COHDOC017 and invalid/deep patched JSON COHDOC018; none has an assigned SQLSTATE. Missing arrays
and out-of-range indices fail for removals too. SET-to-MISSING removes named members or writes null
to existing array elements. USE KEYS, RETURNING, LIMIT, FOR and nested queries remain COHDBL001.

## Names and reservations

Collection, property, alias and parameter names preserve case; keywords are case-insensitive.
The shared parameter contract accepts $name/@name/$n with ASCII numeric names, case-sensitive
bare-name binding and one stripped key sigil. Collisions are COHDOC009; unbound values COHDOC005.

DEFINE, ELEMENT, FLATTEN, STRUCT, LIST, BAG, LISTTOSET and TYPEOF are ordinary field names. Their
retired command/call positions remain rejected. MISSING/VALUED and CASE/WHEN/THEN/ELSE/END/ESCAPE
are reserved; quoting or dotted access preserves fields such as d.end. OqlClauses.Define/Element/
Flatten were removed without compatibility aliases. UNDEFINED is reserved and never means MISSING.

## Statements and validation

[OqlDeleteExpression](oql-delete-expression.md) retains Collection, Alias, Predicate and Location;
the public constructor stores arguments and a null predicate selects all documents. OqlClauses.Delete
is `DELETE`. The engine applies SELECT's parameter/metadata/filter validation without aggregates,
chooses the existing scan/seek, collects snapshot identities before writes and deletes atomically.
EXPLAIN DELETE describes a Delete root and its access child without data execution.

CREATE/DROP COLLECTION retain conditional flags and use the same transactional core as session
methods. INSERT retains VALUES pairs or SELECT and output names for keys/values. The planner
validates every branch and parameter even when CASE/logic would skip it at runtime. EXPLAIN wraps
SELECT/INSERT/UPSERT/UPDATE/DELETE, invokes normal planning/admission, and returns DatabasePlanDescription without writes.

[Language reference](../../../../database/document/language/index.md) documents all executable syntax.
[Diagnostics](../../../../database/document/language/diagnostics.md) lists parser and COHDOC001–022
codes with source SQLSTATE categories. Each public node is linked from the [assembly index](index.md).

## Scalar evaluation and metadata

[Scalar functions](../../../../database/document/language/functions/scalar-functions.md) propagate
MISSING before NULL, except TYPE and lazy conditionals. Known incompatible literals fail planning;
wrong-kind stored values return NULL. SQRT/POWER domain failures and fractional ROUND/TRUNC scales
use COHDOC019 (no assigned SQLSTATE); fractional REPLACE counts use COHDOC008.
COUNT/LENGTH/POSITION/ARRAY_LENGTH and epoch milliseconds expose Int64 metadata and long values.
NOW_STR/NOW_MILLIS share the transaction-start UTC instant; output strings end in Z.

## Composite and UNIQUE indexes

[OqlCreateIndexExpression](oql-create-index-expression.md) snapshots ordered Paths and retains
IsUnique/IfNotExists; Path is its leading member. [OqlDropIndexExpression](oql-drop-index-expression.md)
adds IfExists while preserving its preceding constructor. The parser caps keys at 32 with OQL0009
at the 33rd path; direct AST planning uses COHDOC022. Duplicate paths are OQL0002 in text.
The catalog stores all index definitions and registrations as v2 and refuses v1 before recovery
writes, with a recreate remedy and no upgrade. UNIQUE failures are COHDOC020 (23505) or
COHDOC021 (non-scalar component), carried by the catalog-owned constraint exception.

## Array bindings and predicates

[UNNEST/LEFT UNNEST](../../../../database/document/language/clauses/unnest.md) expands arrays before
WHERE and preserves document-ID/array order. LEFT synthesizes MISSING for empty/non-array inputs;
star constructs owned objects keyed by binding names. Quantifiers use inner lexical scopes and
true/not-true element results; collection MISSING propagates and NULL/non-array gives NULL.
Array IN retains the separate NULL-left rule, including NULL IN [] = NULL.

FROM/UNNEST names are unique, reserving the unaliased source name (OQL0010 in text, COHDOC023
for direct SELECT AST duplicates/empty names). Quantifiers may shadow outer names. META on
an array/quantifier variable is COHDOC001. EXPLAIN adds Unnest/LeftUnnest and SystemCollectionScan
without changing the shared eight columns. Catalog, storage and wire layouts are unchanged;
the profile executes 31 of 32 declared clauses.

## Array clause constants

```csharp
public static class OqlClauses
{
    public const string Unnest = "UNNEST";
    public const string LeftUnnest = "LEFT UNNEST";
    public const string Any = "ANY ... SATISFIES";
    public const string Every = "EVERY ... SATISFIES";
}
```

These constant reads have no exceptions. SOME uses Any. OqlLanguageProfile.Instance advertises
all four; `OqlLanguageProfile.Instance.Supports(OqlClauses.LeftUnnest)` returns true. IN's existing
capability now also admits an array operand. Malformed text still returns parser diagnostics;
duplicate bindings use OQL0010 and direct SELECT AST duplicates/empty names use COHDOC023.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlPredicateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/OqlExecutionConformanceTests.cs`.
