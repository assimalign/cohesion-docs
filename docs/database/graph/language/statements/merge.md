# MERGE

Matches a complete fixed pattern for each incoming row, or creates every unbound part.

> **Status:** Implemented, including ON CREATE SET and ON MATCH SET.

## Syntax

```syntaxsql
MERGE path
[ ON { CREATE | MATCH } SET set_item [ , ...n ] ...n ]
[ RETURN expression [ AS alias ] [ , ...n ] ]
```

## Arguments

- **`path`** — One fixed path of nodes and typed relationships with scalar property equalities.
- **`set_item`** — An ordinary [SET](../clauses/set.md) property, literal-map or node-label assignment.
  Either ON branch may come first; branches may repeat.

## Remarks

MERGE matches the whole path. Every complete match receives ON MATCH SET. If there is no complete
match, MERGE creates every unbound node and relationship, then applies ON CREATE SET once. A partial
match does not supply unbound parts. To reuse existing node identities, bind them with an earlier
MATCH and use bare variables in MERGE. A bound node is never modified just to make the pattern fit.

Each MERGE takes one path; use successive MERGE clauses for separate match/create decisions.
Repeated occurrences of an unbound node variable reuse the first node and can form a self-loop;
only its first occurrence may declare labels or properties. Already-bound nodes must also be bare
variables. Relationships must be newly declared, with distinct variables within the path.

Nodes accept creation-compatible label conjunctions, such as `:A&B` or `:A:B`; disjunction,
negation and wildcard labels cannot be created. Each relationship needs exactly one type.
Fixed undirected relationships (`-[r:R]-` or `<-[r:R]->`) match either stored orientation.
When creating, they run from the left node to the right node. An incoming `<-[r:R]-` creates
from right to left; an outgoing `-[r:R]->` creates from left to right. Variable-length and
quantified update paths, named paths, selectors, path modes and inline predicates are refused.

Property equalities evaluate once per incoming row against its existing scope. Newly introduced
pattern variables are unavailable to these expressions. Values must be finite, non-NULL storage
scalars. GQL has no MISSING value; an absent property read returns NULL and is invalid as a MERGE
equality. A NULL or absent bound node is also invalid. Both branches and all equality expressions
are validated before execution, including under EXPLAIN.

### ON CREATE and ON MATCH

Only the selected branch executes. Repeated clauses of that kind append their SET items in source
order, and items run left to right using ordinary SET behavior. NULL SET values remove properties;
literal maps can replace or merge properties, and node labels can be added. Earlier input rows'
creations and changes are visible to later rows and stages. Returned entity aliases refresh to
their final statement values, including aliases nested in lists or maps.

ON CREATE can supply schema-required properties. Required-property validation runs after the
finished ON CREATE branch, before the created row proceeds to any later stage. A later SET cannot
supply a property missing at that boundary: the statement already fails with CG03003 and rolls
back. Supplied property types are validated during creation; ordinary SET also validates changed
entities at its branch boundary.

Created entities count once; changed pre-existing entities count once per statement. Pure matches
without changes count zero. Empty input produces no decision or creation. RETURN, WITH and NEXT
compose with MERGE in the ordinary stage order. Wire Execute can return scalar projections;
ExecutePaths refuses MERGE before mutation. EXPLAIN shows Merge, its probe and OnCreate/OnMatch
children without executing them.

### Concurrency and access

MERGE serializes decisions through the database writer lock and a transaction-owned pattern lock.
Snapshot compares its decision with the latest committed pattern. A changed/disappeared match or
bound identity, or a planned creation that now has a committed match, reports retryable CG04024.
ReadCommitted refreshes and re-matches after serialization. A failed statement aborts its explicit
transaction; roll back before retrying.

An ended statement or session reports CK04002 while the database remains live, including writer,
parent and pattern lock waits and late grants. Database disposal takes precedence with CG09019.

Eligible row-dependent equalities use a property index with each row's evaluated value. These seeks
take precedence over label/type scans; a dropped index falls back to scanning. Writer-overlay
property changes are found by value, including earlier creations, replacements, relabeling and
deletions, so later rows find current values. Work and retained state use the existing statement
budgets. This batch changes no disk or wire format.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00002 | Multiple paths in one MERGE, malformed ON branch, or variable-length/quantified update path | — |
| CK00001 | Bare list/map property literal or another excluded form | — |
| CG03003 | Created entity still lacks a Required property after ON CREATE, or an ordinary SET/schema failure | — |
| CG03004 | Proven/evaluated NULL, non-finite or non-scalar equality | — |
| CG03005 | Bound node is NULL, absent or incompatible with its node pattern | — |
| CG03006 | Labels or properties on a repeated or already-bound node | — |
| CG03007 | Relationship variable was declared before MERGE | — |
| CG05001 | Repeated relationship variable in the path, missing/multiple relationship types, non-creatable label expression, new-variable equality reference, or ExecutePaths mutation | — |
| CG04024 | Stale Snapshot pattern decision; retry after rollback | — |
| CG05023 | Invalid bound pattern-lock identity | — |
| CK04002 | Statement/session ended while the database is live | — |
| CG09019 | Database disposed during admitted execution | — |

Bare list/map property literals fail parsing with CK00001; other proven or evaluated non-scalar
equalities use CG03004. Supplied parameters retain ordinary validation: missing values use CG05001,
unsupported/non-finite CLR values CG03003. Other [expression, budget and transaction diagnostics](../diagnostics.md)
still apply. No SQLSTATE is assigned to the codes above.

## Examples

Execute separately in a fresh database, following GraphMergeExecutionTests:

```sql
INSERT (:P {id:1}), (:P {id:2});
MERGE (a:P {id:1})-[r:R]-(b:P {id:2})
ON CREATE SET r.hits=1 ON MATCH SET r.hits=r.hits+1 RETURN r.hits;
MERGE (a:P {id:1})-[r:R]-(b:P {id:2})
ON CREATE SET r.hits=1 ON MATCH SET r.hits=r.hits+1 RETURN r.hits;
MATCH (n:P) RETURN count(*);
```

The first MERGE creates two new nodes and their relationship, affects three entities and returns
`1`. The second matches the complete path, changes one relationship and returns `2`. The final
count is `4`: the initially inserted nodes were only a partial match.

For bound-node reuse, in a separate fresh database:

```sql
INSERT (:P {id:1}), (:P {id:2});
MATCH (a:P {id:1}), (b:P {id:2}) FOR x IN [1,1,1]
MERGE (a)-[r:R {id:x}]-(b)
ON CREATE SET r.hits=1 ON MATCH SET r.hits=r.hits+1 RETURN r.hits;
```

This affects one relationship and returns three rows containing `3`. It keeps the two existing
nodes; later rows match the relationship created by the first row.

## See also

[Statements](index.md) · [INSERT](insert.md) · [SET](../clauses/set.md) · [Patterns](../patterns.md) · [EXPLAIN](explain.md)

## Sources

- **Whole-pattern behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphMergeExecutionTests.cs`.
- **Required properties, redeclarations and repeated branches** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphMergeFixTests.cs`.
- **Concurrency and lifetime** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphMergeExecutionConcurrencyTests.cs` and `GraphMergeThirdPassLifetimeTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Match.cs` and `GqlQueryParser.Patterns.cs`.
