# Assimalign.Cohesion.Database.Documents.Language design

OQL parses the Documents engine's executable SQL++ subset into typed statements and expressions.

## Parser and AST

The shared lexer/parser infrastructure owns tokenization, diagnostics and source locations. OQL
partial parsers own dispatch, SELECT, DDL, DML and expressions. OqlQueryStatement wraps one SELECT,
INSERT, UPDATE, DELETE, EXPLAIN, collection DDL or index DDL root. Parser-produced lists are read-only snapshots;
public constructors retain child references and semantic validation belongs to the engine.

Offsets use zero-based UTF-16 with exclusive ends and one-based lines. Parsing returns diagnostics;
DocumentQueryRequest and session execution reject all parser errors before planning. Expressions
have a 128-level recursive parse bound. The executable function profile contains aggregates and
META; recognized unsupported vocabulary reports COHDBL001 separately.

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
SELECT/INSERT/UPDATE/DELETE, invokes normal planning/admission, and returns DatabasePlanDescription without writes.

[Language reference](../../../../database/document/language/index.md) documents all executable syntax.
[Diagnostics](../../../../database/document/language/diagnostics.md) lists parser and COHDOC001–018
codes with source SQLSTATE categories. Each public node is linked from the [assembly index](index.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlPredicateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/OqlExecutionConformanceTests.cs`.
