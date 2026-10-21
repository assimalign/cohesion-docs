# GQL syntax conventions

Syntax diagrams distinguish required choices, optional fragments, and literal graph punctuation.

> **Status:** Implemented.

## Diagram notation

| Notation | Meaning |
|---|---|
| `UPPERCASE` | Keyword; spelling is case-insensitive |
| `snake_case` | Placeholder for a name, value, or grammar fragment |
| `[ ... ]` | Optional fragment |
| `{ a \| b }` | Required choice |
| `\|` | Alternative |
| `[ , ...n ]` | Repetition separated by commas |
| `<label> ::=` | Named grammar block |
| Single-character double-quoted tokens | Literal punctuation in a diagram |

Graph syntax itself uses square brackets and braces. Pattern diagrams quote literal punctuation as
`"["`, `"]"`, `"{"`, and `"}"` so it is distinct from optional groups and required choices.
Those diagram quotes are not part of the query.

## Lexical rules

Names are case-sensitive and keywords are case-insensitive. Double-quoted names preserve spaces;
doubled `""` decodes to one embedded double quote: `"a""b"` names `a"b`. Variables, labels,
relationship types, property keys and aliases use this rule; rendered names escape quotes the
same way. An empty quoted name is invalid. Single-quoted strings escape a quote by doubling it.
Labels and property keys can use keyword text in their name position.

Scalar values are `NULL`, Booleans, signed 64-bit integers, finite doubles, and single-quoted strings.
The parser accepts one optional trailing semicolon. A request contains one statement.

Line comments use `--`. Block comments use `/* ... */` and may nest. The conformance corpus includes:

```sql
/* outer /* inner */ */ MATCH (a) -- comment
 RETURN a
```

It also includes quoted property names and escaped strings:

```sql
match (a:Person {"with space": 'it''s fine'}) return a."with space";
```

Parameter markers such as `$age` use the supported [parameter binding](expressions/parameters.md).
Malformed strings, quoted names, and block comments produce `GQL0003`.
This includes an escaped final quote without a closing delimiter, as in `"a""`.

Execute separately in an empty graph:

```sql
INSERT (:"L""X" {"p""q": 7});
MATCH (n:"L""X") RETURN n."p""q" AS "c""d";
```

The query returns 7 under `c"d`. Delimited names keep ordinary property NULL behavior;
GQL has no separate MISSING value.

## See also

- **[Language (GQL)](index.md)** — reference hub.
- **[Patterns](patterns.md)** — literal graph punctuation.
- **[Literals](expressions/literals.md)** — scalar value limits.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlProfileExecutionTests.cs`.

