# Statements

Graph statements match data, mutate bound graphs, or inspect the session database catalog.

> **Status:** Implemented.

## Queries and mutations

- **[MATCH and OPTIONAL MATCH](match.md)** — bind nodes and relationships before projection or mutation.
- **[RETURN](return.md)** — query composition and result projection.
- **[Linear composition](../clauses/next.md)** — LET, FILTER, FOR/UNWIND, RETURN … NEXT and Cypher WITH.
- **[Inline CALL](../clauses/next.md#inline-call-subqueries)** — read-only returning subqueries and OPTIONAL CALL; writing/unit bodies remain deferred.
- **[INSERT](insert.md)** — insert nodes and relationships.
- **[MERGE](merge.md)** — match a whole path or create every unbound part, with ON CREATE/ON MATCH SET.
- **[CREATE](create.md)** — compatibility spelling for insertion.
- **[DELETE](delete.md)** — restricted deletion of matched elements.
- **[DETACH DELETE](detach-delete.md)** — deletion with incident-relationship cascading.
- **[SHOW](show.md)** — read-only catalog metadata.

A query reading graph elements binds variables through MATCH; LET and FOR/UNWIND add value or
proven entity bindings. Insertion, RETURN and value pipelines may stand alone. `SHOW` is a separate
statement and cannot be combined with graph matching, projection, or mutation.

- **[EXPLAIN](explain.md)** — Return a chosen plan without executing it.

[SET](../clauses/set.md) and [REMOVE](../clauses/remove.md) execute as source-ordered stages after
an incoming clause, with optional RETURN. They are not standalone leading statements.

## See also

- **[Language (GQL)](../index.md)** — reference hub.

## Schema statements

- **[CREATE INDEX](create-index.md)** and **[DROP INDEX](drop-index.md)** — named exact and trigram node-property indexes.
- **[CREATE CONSTRAINT](create-constraint.md)** and **[DROP CONSTRAINT](drop-constraint.md)** — type/existence rules for node and relationship properties.
- **[CREATE LABEL](create-label.md)** and **[DROP LABEL](drop-label.md)** — Cohesion label definitions.
- **[CREATE RELATIONSHIP TYPE](create-relationship-type.md)** and **[DROP RELATIONSHIP TYPE](drop-relationship-type.md)** — Cohesion relationship-type definitions.
- **[ALTER CURRENT GRAPH TYPE](#current-graph-type)** — set, add, alter or drop open node and edge declarations.

All schema writes require autocommit. An admitted write inside an explicit transaction,
including a conditional no-op, reports CG04025 and aborts it. SHOW remains read-only.

## Current graph type

```syntaxsql
ALTER CURRENT GRAPH TYPE { SET | ADD | ALTER | DROP } { declarations }
```

The braces are literal syntax.

Node declarations use `(:Label => :Implied & Other {p :: STRING NOT NULL})`; implied labels
and properties may be empty. Edges use `(:Source)-[:TYPE => {p :: INT64}]->(:Target)`.
`IMPLIES` aliases `=>`; `TYPED` or an omitted separator aliases `::`. Optional node variables
can reference declarations in the same body, including forward references. Each endpoint has
an optional variable and one optional identifying label from the resulting type.

SET replaces the complete type; ADD requires new names; ALTER replaces complete existing
declarations and preserves unmentioned types. DROP selects names with `(:Label =>)` or
`()-[:TYPE =>]->()`, with optional aliases. Partial/full declarations in DROP refuse CK00001.
Empty ADD/ALTER/DROP preserves state; SET {} sets an empty type. Success reports affected count 1.
Only typed DropGraphTypeAsync() restores absence. [SHOW](show.md#current-graph-type) reads the type.

```sql
ALTER CURRENT GRAPH TYPE SET {
  (:Person => :Entity {name :: STRING NOT NULL}),
  (:Place =>),
  (:Person)-[:VISITS => {count :: INT64}]->(:Place)
}
```

Node writes enforce implied labels (CG03012) and property rules (CG03003). Declared relationship
creates enforce endpoint identifying labels (CG03013) and properties (CG03003). Matching MERGE
validates only its writes; ON CREATE can complete labels/properties before final validation.
Label additions do no edge work. Label removals check affected incident declared edges; without
a current type, no edge-validation work is added. Rules apply per SET/REMOVE stage: removing an
endpoint label before deleting its edge can refuse; deleting the edge first permits removal.
Extra labels, types and properties remain writable.

Proposed SET/ADD/ALTER validates new or strengthened members through owner postings under
CG05004/CG05020. CG03010 leaves the previous type in place. Independent constraints survive
and must agree with scalar types (CG03009). Canonical DDL quotes identifiers and orders
elements, implied labels and properties ordinally; DEFINITION omits independent constraints.

| Refusal | Condition |
| --- | --- |
| CG03011 | Invalid declaration/mutation names, dangling endpoints, non-node-type endpoint labels or identifying/implied label overlap |
| CG00002 | Missing/numeric/punctuation type token, empty delimited type name, or `=>`/`IMPLIES` at an endpoint |
| CK00001 | Empty endpoints outside DROP, unsupported type identifiers, nonempty quoted types, unions, `!` nullability, inline KEY/UNIQUE, element IS/REQUIRE or graph-type constraint specifications |
| CK00001 | CLOSED/OPEN mode clauses and ISO CREATE [CLOSED] GRAPH TYPE; closed mode awaits a later run |
| CG04025 | Schema write in an explicit transaction, including a no-op |

Property types are unquoted BOOLEAN/BOOL, INT8, INT16, INT32, INT64/INTEGER/INT, FLOAT32,
FLOAT64/FLOAT, DECIMAL and STRING. Catalog v5 marks enforced edge declarations; v4 and earlier
refuse CG06019 before any write, clean or after an abrupt stop. Recreate every old Graph database.

## Transaction statements

[BEGIN/START TRANSACTION, COMMIT and ROLLBACK](transactions.md) execute as standalone controls on Execute.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
