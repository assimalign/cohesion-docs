# REMOVE

Removes properties or node labels from incoming entity bindings.

> **Status:** Implemented.

## Syntax

```syntaxsql
incoming_clause REMOVE remove_item [ , ...n ]

<remove_item> ::= variable . property
                | variable IS label
                | variable : label [ : label ...n ]
```

## Remarks

Property removal deletes the key; a later property read returns NULL and keys(entity) excludes it.
IS and colon forms remove node labels. Removing an absent key/label is a no-op. OPTIONAL NULL
targets do nothing and count zero. Relationship properties can be removed; relationship types
cannot. Path, group and scalar bindings are invalid targets. GQL has no MISSING value.

Items and rows execute in order and refresh carried aliases, lists, maps and paths.
[SET](set.md) describes shared affected-count, transaction, Snapshot first-updater-wins,
ReadCommitted snapshot-refresh and format-4 version-chain behavior. The final working version
publishes once per updated entity. An entity changed then restored still counts and publishes.
RETURN reports the mutation count alongside rows; pure no-ops count zero.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| GQL0002 | Leading REMOVE or nested property target |
| COHDBG003 | Path/group/scalar target or removal of a relationship label/type |
| COHDBG001 | Mutation through ExecutePaths, unbound target or invalid direct AST |
| GQL0007 | REMOVE composed with SHOW |

No SQLSTATE is assigned to these codes. Ordinary schema/transaction refusals retain their behavior.
[EXPLAIN](../statements/explain.md) describes Remove/UpdateEntity without mutation.

## Example

```sql
INSERT (:P:Admin {x:1, keep:2});
MATCH (n:P) REMOVE n.x, n IS Admin RETURN n.x, keys(n), labels(n);
MATCH (n:P) REMOVE n.absent, n:Admin;
```

The first update returns (NULL, ['keep'], ['P']) with affected count 1.
The second succeeds with count 0.

## See also

[SET](set.md) · [Clauses](index.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphUpdateTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
