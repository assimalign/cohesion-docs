# Syntax conventions

Explains the notation used in Cohesion SQL reference syntax diagrams.

> **Status:** Implemented.

## Reading a diagram

Syntax diagrams use the `syntaxsql` fence. They describe the accepted subset; excluded forms
appear in support matrices and [Unsupported syntax](unsupported.md), never as executable alternatives.

| Convention | Meaning |
| --- | --- |
| `UPPERCASE` | SQL keywords |
| `lower_case_name` | User-supplied placeholder written in snake case |
| `\|` | Alternatives inside brackets or braces; choose one |
| `[ ]` | Optional item; do not type the brackets |
| `{ }` | Required choice of one alternative; do not type the braces |
| `[ , ...n ]` | Repeat the preceding item up to n times, comma-separated |
| `[ ...n ]` | Repeat the preceding item up to n times, separated by spaces |
| `;` | Optional statement terminator; omission reports informational `SQL0100` |
| `<label> ::=` | Definition of a syntax block referenced elsewhere as `<label>` |

Parentheses in diagrams are literal SQL punctuation. Angle brackets name reusable grammar blocks;
they are not typed in a statement. Lowercase collation names in the COLLATE alternatives are the
literal declared names, not arbitrary placeholders.

## Example

```syntaxsql
SELECT [ DISTINCT ] <select_list>
FROM [ schema_name . ] table_name
[ ; ]

<select_list> ::= { * | expression [ , ...n ] }
```

The diagram permits `SELECT * FROM t;` and `SELECT id, name FROM dbo.t;`.
It is an abbreviated illustration; the [SELECT reference](statements/select.md) gives the full
supported statement shape.

## Statement and example scope

Submit one statement per SQL execute request. When a code block contains multiple statements,
execute them as successive requests on the same session. Transaction examples require the same
client connection for their entire sequence. A missing semicolon is informational; it does not
remove the requirement for a valid executable statement.

Examples using `t` refer to the three-row
[conformance fixture](statements/select.md#a-create-the-conformance-fixture), unless a page supplies
its own setup. Tests measure results and intended errors, not merely the ability to construct a
parser tree.

## Support vocabulary

| Label | Meaning |
| --- | --- |
| Supported | The stated feature is in the declared executable contract |
| Supported (measured) | The bounded feature has live-engine execution evidence |
| Recognized, not supported (`COHDBL001`) | A known clause or form is capability-gated |
| Not in the dialect (`SQL0002`) | An unknown leading command |

The current dialect contract defines support through measured live execution, including correct
results, mutations, and intended semantic errors. These labels never imply complete ISO SQL support.

## See also

[Language (SQL)](index.md) · [Diagnostics](diagnostics.md) · [Unsupported syntax](unsupported.md)

## Sources

- **Executable grammar and terminators** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Parser profile** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLanguageProfile.cs`
- **Example fixture** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

