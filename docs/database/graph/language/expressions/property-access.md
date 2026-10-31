# Subscripts and property access

Reads a list element, map field or stored node/relationship property from a value expression.

> **Status:** Implemented.

## Syntax

```syntaxsql
expression [ index_expression ]
expression . member_name
```

The square brackets in the first form are literal subscript delimiters.

## Remarks

List subscripts require an integral runtime index; floating 1.0 is invalid. Indexes are zero-based;
negative indexes count from the end. Out-of-range indexes yield NULL. Maps, nodes and relationships
accept string subscripts; member syntax uses a field name. Map keys compare ordinally and missing
keys/properties yield NULL. A NULL container or subscript yields NULL. GQL has no MISSING value.

Map access may chain, as in `{k:{j:1}}.k.j`. Node/relationship access also works through a value
expression: `(a).name`, `COALESCE(b,a).name` and `a['name']` read scalar stored properties and
pass the wire Execute shape check. The direct stored-property chain `a.b.c` remains CK00001;
`(a.b).c` on a scalar fails at evaluation with CG02013. Invalid container/index types also use
CG02013. Malformed syntax uses CG00002; [list slices](list-slices.md) execute with at least one bound. Lists/maps follow the
[shared limits and wire capability](list-values.md#limits-and-wire-capability).

## Unknown-property warnings

A read-only statement reports **CG01023**, severity **Warning**, for a statically named entity
property unknown throughout its statement-visible catalog. It checks node and relationship
fields in expressions and MATCH property-map keys, including reads through aliases, collections,
correlated patterns, NEXT and composite operands. EXPLAIN reports the same warnings.

A name is known when a visible committed write stored it anywhere in the database (including
unlabeled entities), DDL declared it, or an earlier write in the same transaction introduced it.
A name on another label or relationship type is sufficient, even if the current entity has no
value. Discovery follows statement snapshots: a rolled-back discovery is forgotten; removing
the value keeps its name known, including after reopen.

Map fields and dynamic subscripts never warn, including `n['key']` and `n[$key]`. Any statement
that writes skips property warnings, including its correlated reads. Missing values retain
their normal NULL behavior and the statement returns its normal result; a warning does not
abort an explicit transaction.

There is one warning per ordinal, case-sensitive property name per statement. It uses the
first occurrence's span when available; pattern-map keys and unlocated typed AST nodes have
no source span. The typed result's Diagnostics collection and negotiated wire diagnostics carry
the warning (no assigned SQLSTATE). See [diagnostic delivery](../diagnostics.md#negotiated-wire-diagnostics).
Neo4j instead warns per occurrence and keeps property tokens after transaction rollback;
Cohesion deduplicates by name and rolls discovery back with the write.

## Examples

```sql
RETURN [10,20,30][-1], [1][5], {k:1}.missing;
RETURN {k: {j: 1}}.k.j;
```

The first row is (30, NULL, NULL); the second is (1).

Execute separately in a fresh database:

```sql
INSERT (:Person {name:'Alice'}), (:Person {name:'Bob'});
MATCH (a:Person) OPTIONAL MATCH (a)->(b)
RETURN (a).name, COALESCE(b,a).name, a['name'];
```

The rows are (Alice, Alice, Alice) and (Bob, Bob, Bob), in unspecified order.

In a fresh database, execute separately:

```gql
INSERT (:Keep {name:'k'})-[:LINK {edgeKey:1}]->(:Other {otherKey:2});
MATCH (n:Keep) RETURN n.never_seen AS x, n.never_seen AS y;
MATCH (n:Keep) RETURN n.otherKey, n.edgeKey;
MATCH (n:Keep) RETURN n['never_seen'];
```

The first read returns `(NULL, NULL)` and one CG01023 at the first `n.never_seen`. The second
returns `(NULL, NULL)` without a warning because both names are known elsewhere. The final
read returns NULL without a property warning because it uses a subscript.

## See also

[Expressions](index.md) · [List values](list-values.md) · [Map values](map-values.md) · [COALESCE](coalesce.md)

## Sources

- **Warnings, discovery and snapshots** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlUnknownPropertyWarningTests.cs` and `GqlPropertyWarningReviewTests.cs` in the same directory.
- **Neo4j comparison** — local reference `neo4j/community/cypher/cypher-planner/src/main/scala/org/neo4j/cypher/internal/compiler/planner/CheckForUnresolvedTokens.scala` and `neo4j/community/kernel/src/main/java/org/neo4j/kernel/impl/core/IsolatedTransactionTokenCreator.java`.
- **Execution and results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlValueParserTests.cs`.
- **Value evaluation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphExpressionEvaluator.Values.cs`.
