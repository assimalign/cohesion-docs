# Diagnostics

OQL parsing returns stable error codes with absolute source locations instead of accepting malformed statements.

## Codes

| Code | Meaning | Corpus trigger |
|---|---|---|
| `COHDBL001` | Recognized construct outside the executable profile | `SELECT DISTINCT name FROM people` |
| `OQL0001` | Empty or comment-only statement | `-- comment` |
| `OQL0002` | Invalid syntax, identifier, path, or multiple statements | `SELECT a[-1] FROM people` |
| `OQL0003` | Unterminated string, quoted identifier, or block comment | `SELECT * FROM people /* unfinished` |
| `OQL0004` | Invalid or out-of-range decimal literal | `SELECT 1e100 FROM people` |
| `OQL0005` | More than 128 recursive expression parse levels | Excessively nested parentheses |
| `OQL0006` | Invalid aggregate arity or star operand | `SELECT SUM(*) FROM people` |

## Locations and processing

`OqlQueryStatement` retains diagnostics even for malformed input. Callers must reject statements
with errors before planning. The engine checks diagnostics for text and directly constructed
requests. Every error carries zero-based UTF-16 offsets, an exclusive end, and a one-based line
number. End-of-input errors have both offsets equal to the source length.

Capability checks precede syntax parsing, so reserved unsupported constructs receive `COHDBL001`
instead of incidental syntax errors. Quoted text, comments, and property names after dots do not
become clause capabilities. A nested `SELECT` is reported as `SUBQUERY` at the inner keyword.

Malformed index definitions use `OQL0002`. Semantic errors such as invalid aggregate placement,
ownership restrictions, and read-only system collections belong to planning or execution rather
than these parser codes.

## See also

[Language (OQL)](index.md), [Unsupported](unsupported.md), and [Syntax conventions](syntax-conventions.md).

## Sources

- **Diagnostic contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Diagnostic construction** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.cs`.
- **Conformance corpus** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
- **Engine validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
