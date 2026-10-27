# Graph patterns

Graph patterns describe fixed or quantified paths of labeled nodes and typed relationships with scalar expression properties.

> **Status:** Fixed/quantified matching executes; selectors and inline WHERE support binding/EXPLAIN only.

## Syntax

```syntaxsql
<paths> ::= path [ , ...n ]
<path> ::= [ variable "=" ] [ selector ] [ WALK | TRAIL | SIMPLE | ACYCLIC ] [ PATH | PATHS ] [ GROUP | GROUPS ] path_body
<path_body> ::= path_primary [ { relationship [ quantifier ] path_primary | parenthesized | node } ...n ]
<path_primary> ::= node | parenthesized
<parenthesized> ::= "(" path_body [ WHERE condition ] ")" [ quantifier ]
<quantifier> ::= "{" m [ "," [ n ] ] "}" | "{," [ n ] "}" | "+" | "*"

<node> ::= "(" [ variable ] [ ":" label_name ...n ] [ property_map ] [ WHERE condition ] ")"

<relationship> ::=
    { "-" "[" relationship_body "]" "->"
    | "<-" "[" relationship_body "]" "-"
    | "-" "[" relationship_body "]" "-"
    | "<-" "[" relationship_body "]" "->"
    | "->" | "<-" | "-" | "<->" }

<relationship_body> ::= [ variable ] [ ":" type_name ] [ cypher_length ] [ property_map ] [ WHERE condition ]
<cypher_length> ::= "*" [ m ] [ ".." [ n ] ]

<property_map> ::= "{" [ property_name ":" scalar_expression [ , ...n ] ] "}"
```

Double-quoted punctuation in this diagram denotes the literal character or arrow; diagram quotes
are not entered in the query. Optional square brackets in the diagram differ from the quoted
square brackets that surround relationship syntax.

## Supported forms

| Construct | Support | Constraint |
|---|---|---|
| Named and anonymous nodes | Supported | Node parentheses are required |
| Incoming, outgoing, either-direction matching | Supported | Full bracketed or ISO abbreviated arrows |
| Repeated colon labels | Supported | Cohesion convenience; one label is the portable form |
| Scalar expression property maps | Supported | Unique keys and scalar values evaluated per incoming row |
| Quantified edges/subpaths and Cypher bracketed lengths | Supported | Outer WALK/TRAIL/SIMPLE/ACYCLIC mode; group bindings are lists; maximum 64 hops |
| Collection-valued properties in text | Recognized, not supported (`CK00001`) | No list or nested map literal |

## Arguments

- **`variable`** — an optional case-sensitive node or relationship binding.
- **`label_name`** — a node label; repeated labels each have their own colon.
- **`type_name`** — an optional matching type, required for relationship insertion.
- **`property_map`** — an optional map of scalar expressions with ordinal, unique keys.

## Remarks

MATCH inline maps are predicates on the complete bound clause, including references to later
comma-separated pattern parts. INSERT/CREATE maps see only incoming stage bindings; references
to variables being created in that stage use CG05001. Only literal/parameter equalities provide
index seek bounds. List/map property syntax remains CK00001; evaluated composite values use
CG03003. See [LET](clauses/let.md) and [FOR/UNWIND](clauses/for.md) for row-supplied values.

For example, on P nodes with k values 1 and 2,
`LET value = 2 MATCH (n:P {k:value}) RETURN n.k;` returns `2`.

A path always contains one more node than relationship. Anonymous elements have no variable.
Incoming arrows reverse endpoints relative to consecutive pattern nodes. Matching permits an
undirected relationship, but insertion requires a direction and a type.

`->` matches an outgoing stored edge and `<-` an incoming one. `-` and `<->` match either
orientation, as do `-[]-` and `<-[]->`; a self-loop yields one orientation. Abbreviated arrows
carry no variable, type or property map, so typed/named edges use brackets. Tilde edge forms are
unsupported (`CK00001`). Cypher `(a)-->(b)`/`(a)--(b)` after a pattern element report `CG00008`
because `--` starts a comment; use `(a)->(b)`/`(a)-(b)`. Cypher `<--`/`<-->` report `CG00002`.

A matched path contains at most 64 relationships. Written repetition-bound sums are checked
before execution: fixed edges contribute one, finite repetitions contribute upper bound times
body edge count, and unbounded repetitions contribute their lower bound times body edge count. The omitted mode is TRAIL: repeated nodes are permitted but a relationship identity
cannot repeat within one matched path. Comma-separated paths share variables but keep separate
identity-reuse scopes. Conflicting node/relationship variable kinds fail planning.

Fixed and quantified hops seek direction and catalog relationship-type IDs before candidate
expansion. Boolean type expressions select the visible catalog types they accept; wildcard/untyped
hops select all types. Expansion left of an anchor reverses the seek direction. The same selection
applies to edges inserted earlier in the statement. Either-direction reads return a self-loop once.

Read execution streams within retained-memory and physical-work budgets. The
physical counter charges visited anchor candidates and selected visible relationship candidates as
they are consumed, including heads needed to initialize the identity-ordered stream merge and
dead ends. Incident-type discovery and its first stream head share one charge; returning a primed
head does not charge it again. Unrelated directions/types
add no candidates. The viable counter counts hops passing trail uniqueness, binding, relationship
and safe endpoint checks before copying path state; deferred predicates retain final checks.
Physical work above the effective configured ceiling fails with CG05004 and actual physical/viable totals, without
silent truncation. These counters are not public telemetry. Selected ranges stream in bounded,
cancellable index batches, retaining one decoded head per type/direction range. Type discovery
uses one-entry probes and charges before head decoding, so initialization also obeys the budget.
Budget refusal, cancellation and errors close every active parent stream. Result materialization
and the 64-hop trail limit still apply. An unordered read LIMIT can stop before visiting a branch. See [EXPLAIN](statements/explain.md)
for the counting description; LIMIT does not reduce matching work.

[Parameters](expressions/parameters.md) bind in node and relationship maps; only runtime scalar values execute.

## Path modes

[Path selectors](path-selectors.md) document the selector prefix, count/group ordering and inline
WHERE prefilters. Those forms bind and EXPLAIN, but execution reports CK00001 in this step.

Write the mode after any path assignment: `MATCH p = WALK PATHS (a)->{1,3}(b) RETURN p`.
PATH and PATHS are optional synonyms. A mode applies to the complete outer path, including all
fixed edges and parenthesized/quantified segments, regardless of the chosen index anchor.

| Mode | Identity reuse within one complete path |
| --- | --- |
| WALK | Nodes and relationships may repeat; every quantifier must have a finite upper bound |
| TRAIL | Relationship identities cannot repeat; node identities may repeat; the default when omitted |
| SIMPLE | Node identities cannot repeat except that the final node may close onto the first |
| ACYCLIC | Node identities cannot repeat, including the first at the end |

SIMPLE permits a self-loop and an out-and-back over the same relationship; traversal stops at
closure. It does not add TRAIL's relationship-uniqueness requirement. All modes allow zero hops.
Each comma-separated path has its own mode and may reuse another path's identities.

A mode inside a parenthesized or quantified subpath reports CK00001. Unselected unbounded WALK reports
CK00001 even with LIMIT: this includes +, *, open ISO upper bounds and open Cypher lengths.
Selectors permit unbounded WALK during planning; their execution remains CK00001. Mode words remain contextual
identifiers. INSERT/CREATE modes report CG00002; invalid direct AST mode values or insertion
modes report CG05001. Work, retained-memory, cancellation and the 64-hop limit apply to every
mode (CG05004 for hop/work limits, CG05020 for memory). EXPLAIN shows the resolved mode.

On a fresh graph, execute separately:

```sql
INSERT (a:Pair {k:0})-[:P]->(b:Pair {k:1});
MATCH SIMPLE (a:Pair {k:0})-[:P]->(b)<-[:P]-(c) RETURN count(*);
MATCH TRAIL (a:Pair {k:0})-[:P]->(b)<-[:P]-(c) RETURN count(*);
MATCH ACYCLIC (a:Pair {k:0})-[:P]->(b)<-[:P]-(c) RETURN count(*);
```

The counts are 1, 0 and 0. SIMPLE allows closure to a over the same edge; TRAIL rejects the
repeated relationship and ACYCLIC rejects the repeated node. WALK also returns 1 for this fixed path.

## Quantified paths

| Form | Inclusive repetitions |
| --- | --- |
| `->{m}`, `->{m,n}`, `->{m,}`, `->{,n}`, `->{,}` | m..m, m..n, m..unbounded, 0..n, 0..unbounded |
| `->+`, `->*` | 1..unbounded, 0..unbounded |
| `-[r:T*]->`, `-[*..n]->` | 1..unbounded, 1..n |
| `-[*m]->`, `-[*m..n]->`, `-[*m..]->` | m..m, m..n, m..unbounded |

A quantifier follows an ISO edge or a parenthesized subpath. Cypher lengths sit inside brackets,
after the type and before the property map. ISO {0}/{0,0} are invalid; Cypher [*0] is valid.
Zero repetitions test both outer endpoints on the same node, produce a one-node path and empty
group lists. OPTIONAL MATCH with no match binds new variables to NULL; GQL has no MISSING value.

In `(a) ((x)-[r:T]->(y)){2} (b)`, each y joins the next x. Every repeated map is tested at
each occurrence. Variables declared inside a quantified segment bind source-ordered lists of
nodes or relationships. RETURN * and WITH * include them in declaration order. FOR/UNWIND
preserves their entity kinds, including across WITH/NEXT. Indexing a group can select an entity;
direct `r.w`, deleting r, a singleton/group name conflict or a repeated map referencing a group
uses CG03003.

With the default TRAIL mode, unbounded quantifiers stop when no unused relationship qualifies. A qualifying 65th edge raises
CG05004 before testing its endpoint if visited; endpoint filters cannot suppress a visited
65th hop. An unordered read LIMIT can stop upstream before that branch.
Bare unbounded quantifiers use implicit trail semantics. Relationship uniqueness is per path,
so separate comma paths may reuse an edge; this differs from Neo4j's clause-wide default.

In-process rows may contain entity lists. Wire Execute requires graph.composite-values for
possible composites (CG07016 without it), then refuses possibly nonempty entity lists with
CG05001. A provably empty Max=0 group can return [] when negotiated. Expand groups with
FOR/UNWIND and project scalar properties, or use QueryPathsAsync for one named full path.
No new wire encoding is required.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| CG00002 | Reversed bounds, ISO {0}/{0,0}, node quantifier or INSERT/CREATE quantifier |
| CG00004 | Bound outside Int32 |
| CG00005 | Text repetition-bound sum above 64 |
| CG00008 | -- directly touches a subpath closing parenthesis, quantifier or bracketed Cypher length and would hide an arrow |
| CG00009 | Parser stack guard while nesting subpaths; engine translates to CG05008 (54001) |
| CK00001 | Questioned path ?, nested quantifiers, subpath assignment, nested modes/selectors, unselected unbounded WALK, counted ANY or selector/inline WHERE execution |
| CG00011 | SHORTEST count is missing or is not a positive Int64 literal |
| CG01016 | Selected prefilter references a peer-path-only binding in the same MATCH |
| CG05001 | Invalid direct AST bounds/shape, nested repetitions, cycles/reused path instances, depth above 128, or quantified/parenthesized insertion AST |
| CG03003 | Invalid use of group variables as described above |
| CG05004 | Runtime 64-hop path or configured physical-work limit |

No SQLSTATE is assigned here except CG05008's 54001.

## Examples

The parser corpus exercises incoming and undirected relationships in one chain:

```sql
MATCH (a:Person {name: 'Alice'})<-[r:KNOWS {weight: 2}]-(b)-[s]-(c) RETURN a, r, b.name AS friend
```

It also accepts anonymous nodes:

```sql
MATCH ()-[r:KNOWS]->() RETURN r
```

### Repeated segments and groups

Execute separately:

```sql
INSERT (a:V {name:'A'})-[:T {w:1}]->(b:V {name:'B'})-[:T {w:1}]->(c:V {name:'C'})-[:T {w:2}]->(:V {name:'D'});
MATCH (a:V {name:'A'})-[:T]->{1,3}(b) RETURN b.name;
MATCH (a:V {name:'A'})-[:T*1..3]->(b) RETURN b.name;
MATCH (a:V {name:'A'}) ((x)-[r:T]->(y)){2} (b) RETURN b.name;
MATCH (a:V {name:'A'})-[r:T]->{2}(b) WITH * UNWIND r AS e RETURN e.w;
MATCH (a:V {name:'A'})-[*0]->(b) RETURN b.name;
```

The first two queries return B, C, D; the parenthesized query returns C; expansion returns 1, 1;
the zero-length query returns A. On the same chain, `->+` returns B, C, D; `->*` or `->{,}`
returns A, B, C, D; `->{2,}` returns C, D; and `->{,2}` returns A, B, C. For example:

```sql
MATCH (a:V {name:'A'})-[:T]->*(b) RETURN b.name;
```

A path request using
`MATCH p = (a:V {name:'A'})-[:T]->{1,3}(b) RETURN p` returns trails of lengths 1, 2 and 3,
with every intermediate node in source order.

## See also

- **[Language (GQL)](index.md)** — reference hub.
- **[Syntax conventions](syntax-conventions.md)** — diagram notation.
- **[MATCH](statements/match.md)** — pattern matching.
- **[INSERT](statements/insert.md)** — insertion rules.
- **[Diagnostics](diagnostics.md)** — bound and binding failures.

## Sources

- **Filtered adjacency and budgets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphAdjacencyFilterTests.cs`.
- **Quantified execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
- **Bounds** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.

- **Direction forms** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlEdgeDirectionParserTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLabelDirectionExecutionTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Patterns.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Planning/GraphPlanner.cs`.
