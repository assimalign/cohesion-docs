# SET

Changes scalar properties or node labels on incoming entity bindings.

> **Status:** Implemented.

## Syntax

```syntaxsql
incoming_clause SET set_item [ , ...n ]

<set_item> ::= variable . property = expression
             | variable = { property : expression [ , ...n ] }
             | variable += { property : expression [ , ...n ] }
             | variable IS label
             | variable : label [ : label ...n ]
```

## Remarks

Property assignment accepts a scalar value; NULL removes the key. INSERT still stores explicit
NULL properties. A literal map after = replaces every property, leaving labels unchanged;
+= merges it and preserves keys absent from the map. NULL map values remove keys.
The two characters in += must touch. Only literal whole maps are accepted; whole-map parameters
or expressions such as properties(other) are excluded.

IS and colon forms add node labels; repeated colons add several labels. Relationships accept
property changes, but their one type and endpoints remain unchanged. New labels/property keys
use INSERT's catalog discovery, ownership and resulting-property validation.

Items evaluate left to right for each row. Later rows observe earlier rows' changes, so k
occurrences of a node with SET n.x = n.x + 1 increment it k times. The statement keeps one working
copy per entity and publishes its final version once. All carried references refresh, including
entities inside lists, maps and paths; later MATCH stages see updates.

Type-and-value-identical assignments and adding an existing label are no-ops. Integer 1 and
floating 1.0 are different stored values. An entity changed and then restored still publishes
a version and counts once. OPTIONAL NULL targets do nothing and count zero. GQL has no MISSING
value; absent property reads return NULL.

Affected counts include created entities and distinct changed pre-existing entities; an entity
created then updated counts once. RETURN carries the same count in process and over the wire,
even with multiple output rows. Pure no-ops count zero. Writes remain atomic with the statement;
a mutation failure rolls back the owning logical transaction.

## Isolation and storage

Graph records use format 2 MVCC version chains; format 1 is refused before recovery scrub, with
the remedy to recreate the database and no upgrade reader. Older snapshots retain prior property,
label, index and adjacency versions. Rollback restores old stamps and removes replacements.

Snapshot writers use first-updater-wins: changing an entity committed since the writer's snapshot
aborts the stale writer with DatabaseTransactionAbortedException. ReadCommitted mutations acquire
the database writer lock, refresh the statement snapshot and replan before evaluation, so queued
increments see the preceding committed change. Auto-commit uses ReadCommitted. Explicit transactions
use the session API; GQL has no transaction statements.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| GQL0002 | Leading SET, nested target n.x.y, separated + = or property += |
| GQL0006 | Duplicate literal map key |
| COHDBL001 | Whole-map parameter/expression, literal composite property, or MERGE including ON CREATE/ON MATCH SET |
| COHDBG003 | Path/group/scalar target, relationship label update, or computed composite property value |
| COHDBG001 | ExecutePaths contains a mutation stage; unbound target or invalid direct AST |
| GQL0007 | SET composed with read-only SHOW |
| COHDBG009 | Resulting entity or index key exceeds its storage bound |

These codes have no assigned SQLSTATE. Ordinary scalar, schema, parameter and transaction failures
retain their contracts. [EXPLAIN](../statements/explain.md) describes Set/UpdateEntity targets
without executing them.

## Examples

Execute separately in a new database:

```sql
INSERT (:P {x:1, keep:9});
MATCH (n:P) SET n.x = n.x + 1, n IS Admin RETURN n.x, labels(n);
MATCH (n:P) SET n += {x:5, keep:NULL} RETURN n.x, keys(n);
MATCH (n:P) SET n = {only:7}, n:A:B RETURN keys(n), labels(n);
```

Results are (2, ['Admin','P']), then (5, ['x']), then
(['only'], ['A','Admin','B','P']). Each update affects one entity.

```sql
INSERT (:Counter {x:0});
MATCH (n:Counter) FOR i IN [1,2,3] SET n.x = n.x + 1 RETURN n.x;
```

RETURN has three rows containing 3; affected count is 1.

## See also

[REMOVE](remove.md) · [INSERT](../statements/insert.md) · [NEXT](next.md) · [Clauses](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphUpdateTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphUpdateConcurrencyTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/docs/DESIGN.md`.
