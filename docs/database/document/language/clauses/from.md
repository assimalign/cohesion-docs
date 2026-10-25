# FROM

FROM identifies one collection in the session database and optionally names its iteration variable.

## Syntax

```syntaxsql
FROM collection_name [ [ AS ] iteration_variable ]
[ { [ INNER ] UNNEST | LEFT [ OUTER ] UNNEST } expression [ [ AS ] variable ] ...n ]

<collection_name> ::= { identifier | COHESION_SCHEMA . identifier }
```

## Supported clauses

| Construct | Support | Meaning |
|---|---|---|
| One collection | Supported | Required source |
| `AS iteration_variable` or `iteration_variable` | Supported | Optional path qualifier |
| `COHESION_SCHEMA` source | Supported | Reserved current-database metadata collection |
| `JOIN` | Recognized, not supported (`CK00001`) | No multi-source query |

## Arguments

- **`identifier`** — a case-preserving collection name, optionally double quoted.
- **`iteration_variable`** — an optional identifier prefix for paths such as `p.name`.

## Remarks

An ordinary collection is unqualified. `FROM other.people` is invalid; a quoted name is one opaque
identifier. Only the reserved `COHESION_SCHEMA` namespace allows qualification. The engine resolves
the following read-only sources from the statement's catalog snapshot:

| Source | Document fields |
|---|---|
| `COHESION_SCHEMA.INDEXES` | `COLLECTION_CATALOG`, `COLLECTION_NAME`, `INDEX_NAME`, `PATH`, `PATHS`, `IS_UNIQUE` |
| `COHESION_SCHEMA.OBJECT_OWNERSHIP` | `COLLECTION_CATALOG`, `COLLECTION_NAME`, `OBJECT_TYPE`, `OBJECT_NAME`, `OWNER`, `OWNING_SCHEMA` |

`INDEXES` has one document per visible index. `OBJECT_OWNERSHIP` has one per visible collection;
index ownership follows the collection rather than an independent index owner. `PATHS` is the ordered path array, `PATH` its leading element, and `IS_UNIQUE` the actual flag.
See [composite/UNIQUE indexes](../statements/create-index.md).
`OWNER` is `Adhoc` or `Schema`; `OWNING_SCHEMA` is a schema name or null. Source names are
case-insensitive, including fully quoted names, while JSON field names remain case-sensitive.

[UNNEST and LEFT UNNEST](unnest.md) can follow the one collection source. They expand arrays
before WHERE; a missing alias defaults to the final named path step. Bindings must be unique,
including the reserved unaliased collection name (CD00010 in text, CD01023 in a direct AST).
The unaliased collection name does not become an implicit path variable.

## Examples

The parser corpus supplies the ordinary collection query; the engine design supplies introspection:

```sql
SELECT p.name FROM people AS p WHERE p.age >= $age
```

```sql
SELECT INDEX_NAME, PATH FROM COHESION_SCHEMA.INDEXES
WHERE COLLECTION_NAME = 'items' ORDER BY INDEX_NAME
```

## See also

[Clauses](index.md), [SELECT](../statements/select.md), and [WHERE](where.md).

## Sources

- **Scope and grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **System collections** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
