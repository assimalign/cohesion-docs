# MATCH

MATCH binds graph variables using fixed or quantified node and relationship patterns.

> **Status:** Implemented.

## Syntax

```syntaxsql
{ [ OPTIONAL ] MATCH path [ , ...n ] [ WHERE predicate ] } ...n
    { <return_clause>
    | { INSERT | CREATE } path [ , ...n ] [ <return_clause> ]
    | [ DETACH ] DELETE variable [ , ...n ] }

<return_clause> ::= RETURN [ DISTINCT | ALL ] { * | expression [ AS alias ] [ , ...n ] }
```

## Supported clauses

| Clause | Support | Role |
|---|---|---|
| `WHERE` | Supported | Filter matched bindings |
| `RETURN` | Supported | Expressions, aliases, DISTINCT/ALL and named-binding star |
| `INSERT`, `CREATE` | Supported | Mutate from matched bindings |
| `DELETE`, `DETACH DELETE` | Supported | Delete bound elements |
| `OPTIONAL MATCH` | Supported | Preserve unmatched inputs with NULL for new variables |
| Multiple `MATCH` clauses | Supported | Source-ordered product joined on shared variables |
| `MANDATORY MATCH` | Recognized, not supported (`CK00001`) | Outside the profile |

## Arguments

- **`path`** — fixed/quantified edges and parenthesized segments; see [Patterns](../patterns.md).
- **`predicate`** — the [WHERE expression](../clauses/where.md), including logic, arithmetic and predicates.
- **`expression`** — a [RETURN value expression](../clauses/return.md), optionally with an AS alias.

## Remarks

Matching clauses must be followed by projection or mutation. Each clause has its own WHERE, which
sees variables declared through that clause, never later declarations. Several clauses form a
product joined on shared variables; unrelated patterns multiply rows. Comma-separated paths share
variable bindings within a clause.
Repeated variable names enforce element identity equality. A variable cannot represent both a node
and a relationship.

Result names must be unique ordinally; duplicate RETURN names fail CG01015 before data access,
even on empty matches. WHERE/RETURN numeric comparisons and IN use exact represented values,
agreeing with DISTINCT; arithmetic keeps its own promotion. See [Operators](../expressions/operators.md).

OPTIONAL MATCH evaluates all its paths and then its WHERE for each input row. If none survives,
it preserves exactly one input row and assigns NULL to every newly declared node, relationship
and path variable, including new group variables. Existing bindings survive. A leading OPTIONAL MATCH starts with one empty row,
so an empty graph still returns one NULL row. A mandatory MATCH with no rows empties the result;
a later optional cannot restore it. A NULL binding cannot match a later pattern. Property access
and label tests on a NULL binding yield NULL; `IS NULL` yields TRUE. Graph has no MISSING value.

DELETE ignores NULL bindings and counts only deleted entities. INSERT with a NULL endpoint fails
with `CG03003` and rolls back the statement. Path requests reject projections bound only by
optional clauses with `CG05001`, even when all data would match; a binding constrained by a
mandatory clause can be returned as a path.

Unknown labels/types in an optional clause warn with `CG01010`/`CG01011` and null-extend
that clause's input. A later unmatched mandatory clause can still empty the result.

The planner first anchors on a node bound by an earlier path or clause at any pattern position,
or on an endpoint supplied by a bound relationship. Otherwise it can use an indexed node in the
middle or at either end of a path.
Required labels come from pure pattern conjunctions and positive labeled predicates in that
clause's top-level WHERE AND chain. After bound entities, the smallest candidate estimate chooses
among property seeks, label scans and relationship-type scans, including a later node such as
`MATCH (a)->(b:Person)` or `MATCH (a)->(b) WHERE b:Person`. Pattern labels precede WHERE labels
in source order. Equal-cost ties prefer property equality seeks, then node candidates in source
order, then relationship candidates in source order. Estimates can include retained history and
pending or rolled-back publications; they affect cost, not snapshot visibility. See [WHERE](../clauses/where.md).
Label scans seek MVCC label postings and decode only matching visible nodes. A conjunction such
as `:Common&Rare` scans its cheapest required label and checks the full residual. A pure pattern
union such as `:A|B` merges those label ranges, suppressing duplicate node identities; its summed
estimate competes with other node positions. Negated, wildcard and mixed label expressions
retain a full scan unless another safe anchor narrows them. Statement-local additions and
replacements participate in the selected label ranges.

A fixed outer edge with a named type or pure union, such as `(a)-[r:T]->(b)` or
`(a)-[r:T|U]-(b)`, can start from that type's persistent postings instead of scanning nodes.
Duplicate type names are removed. All endpoint, property, binding and path-mode checks still
apply. Undirected edges produce both orientations, except a self-loop produces one. The posting
edge needs no adjacency expansion; remaining edges expand right, then left, preserving path order.
An outer fixed edge can anchor a pattern that also has quantified segments. Quantified edges,
wildcard, negated or mixed type expressions, and WHERE-only type predicates supply no type anchor.

Type estimates sum retained publications for the selected types and double for an unconstrained
direction. Maintenance reconciles counts after purge and rollback; planning uses cached counts
without scanning graph records. Old snapshots and statement-local changes keep their usual
visibility. Property seek estimates inspect at most 512 physical index entries and 32 pages;
an incomplete count falls back conservatively to the retained label population.
One posting head consumes one physical work unit, shared by both undirected orientations;
each accepted hop consumes one viable unit. LIMIT stops without visiting the next posting.

Queries without ORDER BY have no guaranteed row order; changing the anchor can change row order
while preserving the element order within each matched path. [EXPLAIN](explain.md) shows the anchor.
Each candidate still passes label, property, and variable checks. A path uses each relationship
identity at most once, while node identities may recur. Separate paths have separate edge sets.
Direction and catalog type selection precede fixed/quantified expansion, including pending INSERT
edges. Physical and viable work follow the [pattern counting rules](../patterns.md#remarks).

[Parameters](../expressions/parameters.md) bind in node and relationship maps; only runtime scalar values execute.

## Examples

Execute separately in an empty database:

```sql
INSERT ({k:1})-[:Keep {w:7}]->({k:2}), ()-[:Other]->();
MATCH (a)-[r:Keep]->(b) RETURN a.k, r.w, b.k;
EXPLAIN MATCH (a)-[r:Keep]->(b) RETURN a.k, r.w, b.k;
```

The query returns `(1, 7, 2)`. EXPLAIN includes RelationshipTypeScan for Keep at edge 0,
with estimated_rows 1 and actual_rows NULL; there is no Expand for that edge.

Execute these statements separately in an empty database:

```sql
INSERT (:Start {k:0})-[:T]->(:Keep {k:1})-[:T]->(:End {k:2}), (:Other);
MATCH (a)->(b) WHERE b:Keep RETURN a.k, b.k;
MATCH (a)->(b:Keep)->(c) RETURN a.k, b.k, c.k;
```

The queries return `(0, 1)` and `(0, 1, 2)`. Both anchor at the second node, ordinal 1.

In a separate empty database, execute each statement separately:

```sql
INSERT (:A {k:1}), (:B {k:2}), (:A:B {k:3}), (:Noise {k:4});
MATCH (n:A|B|A) RETURN n.k ORDER BY n.k;
MATCH (n:A&B) RETURN n.k;
```

The union returns `1`, `2`, `3`, each once. The conjunction returns only `3`.

The execution corpus seeds Alice, Bob, and a `KNOWS` relationship, then runs:

```sql
MATCH (a:Person {name: 'Alice'})-[r:KNOWS]->(b) RETURN b.name, r.weight
```

That fixture returns `Bob` and `2`. These are fixture-dependent results, not built-in graph data.

### Optional and repeated matching

Execute each statement separately in an empty database:

```sql
INSERT (a:Person {name: 'Alice'})-[:KNOWS]->(b:Person {name: 'Bob'}),
       (:Person {name: 'Isolated'});
MATCH (a:Person)
OPTIONAL MATCH (a)-[:KNOWS]->(b) WHERE b.name = 'Bob'
RETURN a.name, b.name;
MATCH (a:Person {name: 'Alice'}) MATCH (b:Person {name: 'Bob'})
RETURN a.name, b.name;
```

The optional query returns `(Alice, Bob)`, `(Bob, NULL)`, `(Isolated, NULL)` in unspecified
order. The repeated MATCH query returns `(Alice, Bob)`. In an empty database,
`OPTIONAL MATCH (n) RETURN n.name` returns one NULL row.

With 1,001 isolated `N` nodes, `MATCH (a:N) OPTIONAL MATCH (b)-[]->(a) RETURN a.name, b.name`
examines 2,002 candidates in the test corpus, down from 1,003,002 before bound-node anchoring.
The configured memory/work budgets apply across clauses; this is a measured fixture, not a general complexity guarantee.

The RETURN tail accepts [ORDER BY](../clauses/order-by.md) and
[OFFSET/SKIP before LIMIT](../clauses/limit-offset.md), including supported path projections.
Unordered read LIMIT can stop upstream matching. Paging never reduces an admitted mutation set.

## See also

- **[Statements](index.md)** — statement navigation.
- **[WHERE](../clauses/where.md)** — filtering.
- **[RETURN](../clauses/return.md)** — projection.
- **[Patterns](../patterns.md)** — bounds and direction.

## Path modes

Each outer comma-separated path may specify WALK, TRAIL, SIMPLE or ACYCLIC after its optional
assignment; omitted mode means TRAIL. The mode covers the complete path, including repeated
segments, and does not change OPTIONAL MATCH NULL extension. See
[identity rules and examples](../patterns.md#path-modes). Nested modes and unselected unbounded
WALK even with LIMIT report CK00001. [Selectors](../path-selectors.md) and inline WHERE prefilters
parse, bind and EXPLAIN, including selected unbounded WALK; execution reports CK00001 in this
step. Every mode retains hop/work and memory limits.

## Sources

- **Type anchors, estimates and accounting** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphRelationshipTypeAnchorTests.cs`, `GraphRelationshipTypeAnchorTests.Estimates.cs` and `GraphRelationshipTypeAnchorTests.Accounting.cs`.

- **Label postings, cost selection and unions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphLabelIndexAnchorTests.cs`.

- **Required labels and later anchors** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphWhereLabelAnchorTests.cs`.

- **Optional clauses and anchors** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlOptionalMatchTests.cs`.
- **Clause AST** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlOptionalMatchParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlProfileExecutionTests.cs`.
