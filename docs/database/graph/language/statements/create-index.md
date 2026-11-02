# CREATE INDEX

Creates a named, nonunique index on one node property for a label.

> **Status:** Implemented. Single-property node indexes only.

## Syntax

```syntaxsql
CREATE INDEX [ index_name ] [ IF NOT EXISTS ] FOR ( variable : label )
ON ( variable . property )
```

## Arguments

- **`index_name`** — Optional database-wide, case-sensitive index name.
- **`label`** — The node label to index.
- **`property`** — One property key, referenced through the same variable declared by FOR.
- **`IF NOT EXISTS`** — Skips an existing index with that name or an equivalent index on the label/property.

## Remarks

Creation discovers an unseen label and untyped property key, builds over existing nodes and
publishes the catalog and physical index atomically. The index adds no required-property or
type rule: use a constraint to require a non-NULL property. It creates no graph entity. Failure
rolls back the index and every newly discovered token. Dropping the index retains its tokens.
UNIQUE, composite and relationship-property indexes are refused with CK00001.

### Names and conditional creation

Index and constraint names share one database-wide namespace with ordinal, case-sensitive
identity. IF NOT EXISTS suppresses a same-kind name or equivalent owner/property/type rule,
even under another name. A cross-kind name collision still uses CG03008; incompatible
constraint rules use CG03009. A real create/drop affects 1; a conditional no-op affects 0.

### Generated names

Omitting the name generates `index_<sha256>` or, for a constraint, `constraint_<sha256>`,
using all 64 lowercase hexadecimal digest characters. Identity depends on object kind,
node/relationship owner kind, owner name, property and canonical type (or existence).
Variable names, aliases, whitespace and IF NOT EXISTS do not change it. Read the generated
name through [SHOW INDEXES or SHOW CONSTRAINTS](show.md) before a named drop.

### Transactions and limits

Schema DDL is standalone on the session database, with no parameter operands, query
composition or EXPLAIN. Submit each statement separately. Text and typed schema writes
run in one autocommit transaction, including discovery, validation and publication.
Cohesion refuses every schema write inside an explicit transaction with CG04025, including
conditional no-ops. The refusal aborts that transaction; ROLLBACK restores session use.
This is a Cohesion policy, also applied by the [GraphSchema API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-schema.md).

DDL observes statement work and retained-memory limits. Text requests can lower them with
GraphQueryLimits; typed writes use engine ceilings. Owner scans and backfill charge work;
cursor buffers and retained backfill entries charge memory. Exhaustion rolls back the
whole operation, including new tokens and name reservations.

## Diagnostics

These conditions have no assigned SQLSTATE:

| Code | Meaning and trigger |
| --- | --- |
| CK00001 | UNIQUE, composite or relationship-property index form |
| CG01022 | ON references a variable other than the FOR declaration |
| CG03008 | Index/constraint name is already used |
| CG03003 | Another index already targets the same label/property without a conditional no-op |
| CG04025 | Schema write inside an explicit transaction; transaction is aborted |
| CG05004 | DDL work budget exhausted |
| CG05020 | DDL retained-memory budget exhausted |
| CG08021 | Invalid GraphQueryLimits, validated inside the admitted operation |

Schema ownership protects definitions and their children. See [Diagnostics](../diagnostics.md)
for ownership, cancellation, oversized metadata, offline and commit failures.

## Example

Execute separately in a fresh database:

```sql
CREATE INDEX by_name FOR (n:Person) ON (n.name)
CREATE INDEX by_name IF NOT EXISTS FOR (n:Person) ON (n.name)
SHOW INDEXES
```

Affected counts are 1 then 0. SHOW returns one row: the session database, Person's generated
label GUID, `Person`, `by_name`, `name`, `FALSE`. No Person node is inserted.

## See also

[DROP INDEX](drop-index.md) · [CREATE CONSTRAINT](create-constraint.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaDdlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.Ddl.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DIALECT.md`.
