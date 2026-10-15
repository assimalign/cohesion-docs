# SHOW

SHOW returns typed catalog metadata from the session database snapshot.

> **Status:** Implemented.

## Syntax

```syntaxsql
SHOW { LABELS | RELATIONSHIP TYPES | PROPERTY KEYS | INDEXES | OBJECT OWNERSHIP }
```

## Supported clauses

| Construct | Support |
|---|---|
| The five catalog subjects in the diagram | Supported |
| `SHOW DATABASES` | Recognized, not supported (`COHDBL001`) |
| Database qualification, filtering, projection | Not in the language |
| Mutation composed with `SHOW` | Not in the language; produces `GQL0007` |

## Arguments

- **`LABELS`** — node-label definitions.
- **`RELATIONSHIP TYPES`** — relationship-type definitions.
- **`PROPERTY KEYS`** — property metadata attached to either definition kind.
- **`INDEXES`** — named node-property index metadata.
- **`OBJECT OWNERSHIP`** — definition and child metadata ownership.

## Remarks

`SHOW` is a Cohesion extension. It returns catalog definitions rather than fabricated graph
elements. Results use the existing pinned transaction snapshot, including the session's own
uncommitted definitions. Definitions can remain visible after the final corresponding data
element has been deleted.

Each subject has fixed columns in this order:

| Statement | Ordered columns |
|---|---|
| `SHOW LABELS` | `DATABASE_NAME`, `LABEL_ID`, `LABEL_NAME` |
| `SHOW RELATIONSHIP TYPES` | `DATABASE_NAME`, `RELATIONSHIP_TYPE_ID`, `RELATIONSHIP_TYPE_NAME` |
| `SHOW PROPERTY KEYS` | `DATABASE_NAME`, `DEFINITION_TYPE`, `DEFINITION_ID`, `DEFINITION_NAME`, `PROPERTY_KEY`, `DATA_TYPE`, `IS_REQUIRED` |
| `SHOW INDEXES` | `DATABASE_NAME`, `LABEL_ID`, `LABEL_NAME`, `INDEX_NAME`, `PROPERTY_KEY`, `IS_UNIQUE` |
| `SHOW OBJECT OWNERSHIP` | `DATABASE_NAME`, `DEFINITION_TYPE`, `DEFINITION_ID`, `DEFINITION_NAME`, `OBJECT_TYPE`, `OBJECT_NAME`, `OWNER`, `OWNING_SCHEMA` |

Name columns are strings, identity columns are GUIDs, and `IS_REQUIRED` and `IS_UNIQUE` are
Booleans. `DEFINITION_TYPE` is `LABEL` or `RELATIONSHIP TYPE`. `DATA_TYPE` is a declared
`DatabaseType` name or null for an unconstrained key. Current node-property indexes are nonunique.

Ownership is `Adhoc` or `Schema`, with nullable `OWNING_SCHEMA`. Property and index rows report
their parent definition's authority. Label definitions precede relationship types, with ordinal
ordering for definition and child names.

`GraphDatabaseServer` accepts these catalog statements. General GQL queries are not supported by
that wire server. Mutation composition is rejected before writer locks or data mutation, with
`GQL0007`; direct abstract syntax tree requests receive the same protection.

## Examples

The catalog parser corpus includes:

```sql
SHOW LABELS
```

```sql
SHOW /* metadata */ PROPERTY KEYS
```

```sql
SHOW OBJECT OWNERSHIP
```

Each block is a separate request.

## See also

- **[Statements](index.md)** — statement navigation.
- **[Diagnostics](../diagnostics.md)** — catalog read-only error.
- **[Graph](../../index.md)** — engine delivery status.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCatalogParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.

