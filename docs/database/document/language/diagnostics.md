# Diagnostics

OQL reports coded parser, planner and execution failures with available source locations.

## Parser codes

| Code | Meaning |
| --- | --- |
| `COHDBL001` | Recognized form outside the executable SQL++ subset |
| `OQL0001` | Empty/comment-only statement |
| `OQL0002` | Malformed syntax, identifier, path, parameter, literal LIKE escape or multiple statements |
| `OQL0003` | Unterminated string, quoted identifier or block comment |
| `OQL0004` | Malformed numeric literal syntax |
| `OQL0005` | More than 128 recursive expression parse levels |
| `OQL0006` | Invalid aggregate arity or star operand |

Numeric tokens retain their digits in a JsonElement. Valid syntax that is out of decimal range,
underflows, or loses nonzero fractional digits fails during planning/evaluation as COHDOC007,
rather than being rounded by the parser.

## Engine codes

| Code | Meaning and trigger | SQLSTATE where supplied |
| --- | --- | --- |
| `COHDOC001` | Invalid statement, index definition, expression, metadata or grouping context | — |
| `COHDOC002` | Collection does not exist | — |
| `COHDOC003` | Duplicate projected name; supply distinct aliases | — |
| `COHDOC004` | System collection is read-only | — |
| `COHDOC005` | Referenced parameter has no value | — |
| `COHDOC006` | Division or remainder by zero | 22012 |
| `COHDOC007` | Numeric value cannot fit decimal exactly | 22003 |
| `COHDOC008` | Invalid operand/parameter type, document key or JSON value | — |
| `COHDOC009` | Caller keys collide after one leading parameter sigil is stripped | — |
| `COHDOC010` | CREATE COLLECTION finds an existing collection without IF NOT EXISTS | 42P07 |
| `COHDOC011` | DROP COLLECTION targets a schema-owned collection, including IF EXISTS | — |
| `COHDOC012` | INSERT finds an existing key, or INSERT/UPSERT repeats a key in one statement | — |
| `COHDOC013` | Evaluated LIKE escape is invalid or pattern ends in its escape character | — |
| `COHDOC014` | SELECT DISTINCT ORDER BY key is absent from the projection after alias rewriting; reported at that key | — |
| `COHDOC015` | SELECT object/array constructor result exceeds 256 levels, including nested values and a named projection wrapper; reported at the constructor | — |
| `COHDOC016` | Invalid UPDATE target, root replacement, UNSET ending in an index, incompatible traversal, missing array or out-of-range index | — |
| `COHDOC017` | Duplicate or prefix-overlapping UPDATE targets after alias normalization, including SET/UNSET overlap | — |
| `COHDOC018` | Patched UPDATE JSON is invalid or exceeds storage depth 128 | — |

For example, repeating `INSERT INTO items (KEY, VALUE) VALUES ('a', 1)` on the same visible
collection reports COHDOC012. A malformed static escape such as ESCAPE 'ab' is OQL0002 instead
of COHDOC013. Ordinary NULL and MISSING results are values, not diagnostics.

COHDOC016–018 have no assigned SQLSTATE. [UPDATE](statements/update.md) reports target locations
where available and validates all patched documents before the first write.

INSERT/UPSERT repeated-key failures use `Document key 'a' appears more than once in one statement.`
(COHDOC012, no SQLSTATE). Other shared key/value, binding and JSON validation messages name the
written verb, including UPSERT. See [UPSERT diagnostics](statements/upsert.md#diagnostics).

## Locations and wire behavior

DatabaseException.Diagnostics is never null and orders located entries by source start before
unlocated entries. Offsets are zero-based UTF-16 with exclusive ends; lines are one-based.
Planner/binder failures retain available AST spans. Query requests reject all parser errors
before a recovered AST reaches planning. In-process messages are code-free where the carrier
supplies a separate diagnostic; the server composes CODE: message from the first Error entry.

The Documents model sends terminal Failure (72), carrying the wire error category and coded
diagnostics with spans. This remains its structured diagnostic path on protocol 2.1; it does not
negotiate the core Diagnostics frame. Statement refusal preserves the connection; malformed
UTF-8/payloads and message order are protocol violations. The client exception retains diagnostics.

## See also

[Language (OQL)](index.md) · [Parameters](expressions/parameters.md) · [MISSING](expressions/missing.md) · [Unsupported](unsupported.md)

## Sources

- **Carriers** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/OqlDiagnosticTests.cs` and `OqlDiagnosticsFixPassTests.cs` in the same directory.
- **Numbers and duplicate keys** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
- **Failures** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Protocol/DocumentProtocolFailureMessage.cs`.
