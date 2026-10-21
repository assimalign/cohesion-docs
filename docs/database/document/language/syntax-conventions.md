# Syntax conventions

OQL syntax diagrams distinguish literal keywords from placeholders and optional grammar elements.

## Diagram notation

| Notation | Meaning |
|---|---|
| `SELECT` | Literal keyword; keywords and function names are case-insensitive |
| `collection_name` | Placeholder to replace with an identifier or value |
| `[ item ]` | Optional item |
| `{ first \| second }` | Required choice |
| `[ , ...n ]` | Repeat the preceding item, separated by commas |
| `<label> ::=` | Definition of a named grammar block |

Square brackets inside document paths are actual OQL punctuation. The path page explicitly names
these literal bracket tokens to distinguish them from optional diagram notation. Diagrams describe
accepted forms; rejected constructs belong to the [unsupported matrix](unsupported.md).

## Lexical rules

- **Identifiers** — double quotes delimit identifiers. Collection and property names preserve case;
  doubled `""` decodes to one embedded quote, so `"a""b"` names `a"b`. This applies to collection,
  property and alias names. Empty identifiers are invalid; a missing closing delimiter, including
  a final escaped quote, reports `OQL0003`. A property after a dot may use a reserved word.
  VALUE/RAW and LIMIT/OFFSET are reserved; use quoted identifiers or qualified paths for those fields.
  After AS, a SELECT projection label may use a reserved word. DISTINCT/ALL are quantifiers
  only immediately after SELECT; misplaced quantifiers report `COHDBL001`.
- **Strings** — single quotes delimit strings, and a doubled single quote represents one quote.
- **Parameters** — `$name`, `$1`, and `@name` are accepted; the parameter node omits the prefix.
  An empty name or a `?` marker is invalid.
- **Comments** — `--` introduces a line comment; `/* ... */` introduces a block comment, including
  nested block comments.
- **Numbers** — decimal and scientific notation retain digits during parsing; execution requires
  an exact decimal value (`COHDOC007` for range or lost precision).
- **Statement boundary** — one trailing semicolon is optional; multiple statements are rejected.

Parentheses group expressions. Array positions are zero-based nonnegative 32-bit integers.
Expression nesting is limited to 128 recursive parse levels.

## Example

The lexer/parser conformance corpus includes this nested comment and trailing semicolon:

```sql
/* outer /* nested */ comment */ SELECT name -- trailing comment
FROM people;
```

## Delimited identifier example

With a collection named `t"x` already created, execute each statement separately:

```sql
INSERT INTO "t""x" (KEY, VALUE) VALUES ('k', {"a""b": 7});
SELECT "a""b" AS "c""d" FROM "t""x";
```

The query returns decimal 7 in a column named `c"d`. Quoting names preserves normal
[NULL and MISSING](expressions/missing.md) behavior.

## See also

[Language (OQL)](index.md), [Paths and arrays](expressions/paths-and-arrays.md),
and [Diagnostics](diagnostics.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/OqlExecutionConformanceTests.cs`.
