# EXPLAIN

Returns the chosen GQL plan as rows without executing the wrapped data statement.

## Syntax

```syntaxsql
EXPLAIN { match_statement | return_statement | insert_statement | create_statement | delete_statement } [ ; ]
```

## Remarks

DELETE statements include their required preceding MATCH. SHOW is not admitted under EXPLAIN.
Normal parameter checks, binding, planning and provisioning admission apply. Mutation plans change
neither nodes nor relationships, including if an explicit transaction subsequently commits.
Nested EXPLAIN and EXPLAIN ANALYZE report COHDBL001. Plans use the
[shared eight-column shape](../../../sql/language/statements/explain.md): node_id, parent_id,
operator, target, index, detail, estimated_rows, actual_rows. Nodes are depth-first pre-order;
estimated and actual counts are NULL. GQL uses ordinary row-result frames, including for an
explained statement whose normal projection is a graph entity.

Operators include LabelScan, IndexSeek and one Expand per traversed relationship. An IndexSeek
reports the chosen index and property bound. A test guard requires every plan record's mapping.
Standalone RETURN uses a Return root; projected expressions have Project nodes, with a Distinct
parent when requested. Duplicate final names still fail planning with COHDBG015 before execution.

Repeated matching clauses have source-ordered Match/OptionalMatch nodes. OptionalMatch detail
reports whether a filter is present, whether the clause matches nothing, and new variables that
would receive NULL. EXPLAIN itself does not perform matching or null extension.

## Examples

For a path Ada—KNOWS→Bob—KNOWS→Cy and an index by_name on Person.name:

```sql
EXPLAIN MATCH (a:Person {name:'Ada'})-[:KNOWS]->(b)-[:KNOWS]->(c) RETURN a;
EXPLAIN INSERT (:Person {name:'Dee'});
```

The first returns IndexSeek with target Person, index by_name, detail `node=0; property=name;
value='Ada'` and two Expand nodes. Without that index it uses LabelScan. The second describes
insertion and creates no Dee node.

```sql
EXPLAIN RETURN DISTINCT 1 + 1 AS two;
```

This returns plan rows with a Return root, one Distinct node and a Project whose target is `two`.

## See also

[Statements](index.md) · [MATCH](match.md) · [DatabasePlanDescription](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-execution/database-plan-description.md)

## Sources

- **Plans and no execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphExplainTests.cs`.
- **Wire** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphExplainWireTests.cs`.
- **RETURN plans** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
