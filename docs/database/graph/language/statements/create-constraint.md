# CREATE CONSTRAINT

Creates an independently named property-existence or property-type rule for nodes or relationships.

> **Status:** Implemented. One property per constraint.

## Syntax

```syntaxsql
CREATE CONSTRAINT [ constraint_name ] [ IF NOT EXISTS ]
FOR <schema_pattern> REQUIRE variable . property IS NOT NULL

CREATE CONSTRAINT [ constraint_name ] [ IF NOT EXISTS ]
FOR <schema_pattern> REQUIRE variable . property IS :: type

<schema_pattern> ::= ( variable : label )
                  | () - "[" variable : relationship_type "]" - ()
                  | () - "[" variable : relationship_type "]" -> ()
                  | () <- "[" variable : relationship_type "]" - ()
                  | () <- "[" variable : relationship_type "]" -> ()
```

## Arguments

- **`constraint_name`** — Optional database-wide, case-sensitive name; omission generates a stable name.
- **`variable`** — FOR's variable, which must match the property reference.
- **`property`** — One scalar property key on the owner label or relationship type.
- **`type`** — A supported stored scalar type from the table below.

## Remarks

An existence rule requires a present, non-NULL property. A type rule permits absent/NULL
values and requires each present, non-NULL value to have the exact stored type, without
coercion. Existing data is validated before publication. INSERT/CREATE, SET, REMOVE and
MERGE enforce the effective rules; DELETE may remove constrained entities. Relationship
rules apply to their type regardless of arrow direction; canonical DDL uses the undirected pattern.

| Type spelling | Stored type | Aliases |
| --- | --- | --- |
| BOOLEAN | Boolean | BOOL |
| INT8 | Int8 | — |
| INT16 | Int16 | — |
| INT32 | Int32 | — |
| INT64 | Int64 | INTEGER, INT |
| FLOAT32 | Float32 | — |
| FLOAT64 | Float64 | FLOAT |
| DECIMAL | Decimal | — |
| STRING | String | — |

INTEGER/INT mean Int64 here; [CAST](../expressions/cast.md) retains its Int32 meaning.
Type and existence rules have independent names and drops. A named rule may repeat an
identical unnamed property declaration; dropping it preserves that declaration. Duplicate
same-kind rules or incompatible types are refused, except an equivalent conditional create
is a no-op. UNIQUE, KEY, composite constraints, closed graph types and unsupported type
forms use CK00001.

Creation discovers unseen label/type/property tokens in the same transaction. Discovered
properties are untyped metadata; dropping the last rule retains the known property. Failure
rolls back all new tokens. No graph entity is inserted.

[Name identity, IF NOT EXISTS and generated names](create-index.md#names-and-conditional-creation)
follow the shared schema rules. The [autocommit policy and DDL limits](create-index.md#transactions-and-limits)
apply: an admitted explicit-transaction write aborts it with CG04025, including a no-op.

## Diagnostics

These conditions have no assigned SQLSTATE:

| Code | Meaning and trigger |
| --- | --- |
| CK00001 | Unsupported constraint kind, composite, closed graph type or property type form |
| CG01022 | REQUIRE references a variable other than the FOR declaration |
| CG03008 | Database-wide index/constraint name is already used |
| CG03009 | Duplicate constraint kind or incompatible property type rule |
| CG03010 | Existing data violates the new rule; creation rolls back |
| CG03003 | A later entity write violates its effective schema |
| CG04025 | Schema DDL inside an explicit transaction; transaction is aborted |
| CG05004 / CG05020 | DDL work / retained-memory budget exhausted |
| CG08021 | Invalid GraphQueryLimits |

See [Diagnostics](../diagnostics.md) for the remaining engine conditions.

## Example

Execute separately in a fresh database:

```sql
CREATE CONSTRAINT typed FOR (n:Fresh) REQUIRE n.p IS :: INTEGER
CREATE CONSTRAINT required FOR (n:Fresh) REQUIRE n.p IS NOT NULL
INSERT (:Fresh {p:1})
MATCH (n:Fresh) RETURN n.p
SHOW CONSTRAINTS
DROP CONSTRAINT required
INSERT (:Fresh)
```

MATCH returns `1`. SHOW has `required` (DATA_TYPE NULL, IS_REQUIRED TRUE) and `typed`
(DATA_TYPE `Int64`, IS_REQUIRED FALSE). The final insert succeeds because the remaining
type rule permits an absent p. Before the drop it would fail with CG03003.

Relationship rules use the same semantics, for example:

```sql
CREATE CONSTRAINT edge_required FOR ()-[r:KNOWS]->() REQUIRE r.since IS NOT NULL
```

On a fresh database this affects 1 and discovers KNOWS.since without inserting an edge.

## See also

[DROP CONSTRAINT](drop-constraint.md) · [SHOW](show.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaDdlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlSchemaDdlParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DIALECT.md`.
