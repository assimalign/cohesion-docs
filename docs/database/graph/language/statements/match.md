# MATCH

MATCH binds graph variables using finite node and relationship patterns.

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
| `MANDATORY MATCH` | Recognized, not supported (`COHDBL001`) | Outside the profile |

## Arguments

- **`path`** — a node followed by zero or more relationship/node pairs; see [Patterns](../patterns.md).
- **`predicate`** — the [WHERE expression](../clauses/where.md), including logic, arithmetic and predicates.
- **`expression`** — a [RETURN value expression](../clauses/return.md), optionally with an AS alias.

## Remarks

Matching clauses must be followed by projection or mutation. Each clause has its own WHERE, which
sees variables declared through that clause, never later declarations. Several clauses form a
product joined on shared variables; unrelated patterns multiply rows. Comma-separated paths share
variable bindings within a clause.
Repeated variable names enforce element identity equality. A variable cannot represent both a node
and a relationship.

Result names must be unique ordinally; duplicate RETURN names fail COHDBG015 before data access,
even on empty matches. WHERE/RETURN numeric comparisons and IN use exact represented values,
agreeing with DISTINCT; arithmetic keeps its own promotion. See [Operators](../expressions/operators.md).

OPTIONAL MATCH evaluates all its paths and then its WHERE for each input row. If none survives,
it preserves exactly one input row and assigns NULL to every newly declared node, relationship
and path variable. Existing bindings survive. A leading OPTIONAL MATCH starts with one empty row,
so an empty graph still returns one NULL row. A mandatory MATCH with no rows empties the result;
a later optional cannot restore it. A NULL binding cannot match a later pattern. Property access
and label tests on a NULL binding yield NULL; `IS NULL` yields TRUE. Graph has no MISSING value.

DELETE ignores NULL bindings and counts only deleted entities. INSERT with a NULL endpoint fails
with `COHDBG003` and rolls back the statement. Path requests reject projections bound only by
optional clauses with `COHDBG001`, even when all data would match; a binding constrained by a
mandatory clause can be returned as a path.

Unknown labels/types in an optional clause warn with `COHDBG010`/`COHDBG011` and null-extend
that clause's input. A later unmatched mandatory clause can still empty the result.

The planner first anchors on a node bound by an earlier path or clause at any pattern position,
or on an endpoint supplied by a bound relationship. Otherwise it can use an indexed node in the
middle or at either end of a path.
Each candidate still passes label, property, and variable checks. A path uses each relationship
identity at most once, while node identities may recur. Separate paths have separate edge sets.

[Parameters](../expressions/parameters.md) bind in node and relationship maps; only runtime scalar values execute.

## Examples

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
The 100,000 intermediate-binding and 1,000,000 statement-candidate limits still apply across clauses
(`COHDBG004`); this is a measured fixture, not a general complexity guarantee.

The RETURN tail accepts [ORDER BY](../clauses/order-by.md) and
[OFFSET/SKIP before LIMIT](../clauses/limit-offset.md), including supported path projections.
Paging does not reduce matching bounds.

## See also

- **[Statements](index.md)** — statement navigation.
- **[WHERE](../clauses/where.md)** — filtering.
- **[RETURN](../clauses/return.md)** — projection.
- **[Patterns](../patterns.md)** — bounds and direction.

## Sources

- **Optional clauses and anchors** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlOptionalMatchTests.cs`.
- **Clause AST** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlOptionalMatchParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlProfileExecutionTests.cs`.
