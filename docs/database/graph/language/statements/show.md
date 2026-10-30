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
| `SHOW DATABASES` | Recognized, not supported (`CK00001`) |
| Database qualification, filtering, projection | Not in the language |
| Mutation composed with `SHOW` | Not in the language; produces `CG00007` |

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
`DatabaseType` name or null for an unconstrained key. PROPERTY KEYS reports effective type and
requiredness from declarations plus named type/existence constraints. Current node-property
indexes are nonunique.

Ownership is `Adhoc` or `Schema`, with nullable `OWNING_SCHEMA`. Property and index rows report
their parent definition's authority. Named constraints add OBJECT_TYPE `CONSTRAINT` rows whose
OBJECT_NAME is the constraint name, with the same inherited authority. Label definitions precede relationship types, with ordinal
ordering for definition and child names.

Constraints and database-wide schema names are managed through the typed
[GraphSchema API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-schema.md).
There is no text DDL or SHOW CONSTRAINTS in this run. Typed GetPropertyKeysAsync returns declared
metadata only; SHOW PROPERTY KEYS includes named-rule effects.

`GraphDatabaseServer` accepts these catalog statements alongside its scalar queries, mutations
and path requests. Mutation composition with SHOW is rejected before writer locks or data mutation, with
`CG00007`; direct abstract syntax tree requests receive the same protection.

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

After the [GraphSchema constraint example](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-schema.md#example)
creates p_type and p_exists, before dropping either rule:

```sql
SHOW PROPERTY KEYS
```

The N.p row reports DATA_TYPE `Int64` and IS_REQUIRED `TRUE`. SHOW OBJECT OWNERSHIP also
includes CONSTRAINT rows named p_type and p_exists with their label's ownership. After dropping
p_exists, N.p remains Int64 and IS_REQUIRED becomes FALSE, unless an independent declaration
still requires it.

## See also

- **[Statements](index.md)** — statement navigation.
- **[Diagnostics](../diagnostics.md)** — catalog read-only error.
- **[Graph](../../index.md)** — engine delivery status.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCatalogParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.

