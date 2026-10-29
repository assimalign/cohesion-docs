# Expression subqueries

A parenthesized SELECT evaluates to an array of zero or more results. There is no scalar-cardinality error.

## Syntax

```syntaxsql
(SELECT projection [ , ...n ] FROM collection_name ...)
(SELECT { RAW | VALUE } expression FROM collection_name ...)
expression [ NOT ] IN (SELECT { RAW | VALUE } expression FROM collection_name ...)
```

## Remarks

Named SELECT projections produce objects with the usual omitted MISSING members. RAW and VALUE
produce values directly; a MISSING result becomes a null array element. Every SELECT requires
FROM. Queries can nest and appear in projections, WHERE, GROUP BY, HAVING, ORDER BY, scalar
arguments, constructors, CASE, comprehensions, quantified predicates and UNNEST operands.
Each query has its own grouping and binding scope. [EXISTS](exists.md) tests the resulting array.

A lone star inside a subquery retains an object root; a scalar, array or NULL root is wrapped
as `{"document": root}`. With UNNEST, star produces the binding-wrapper object. A star beside
other projections is a named `document` member. This differs from Couchbase's alias-keyed star
shape. Top-level [SELECT *](../statements/select.md) over a stored collection still returns its value unchanged.
[Derived and expression sources](../clauses/from.md) instead use alias-keyed star wrappers.

Direct IN subqueries require RAW or VALUE. They use [array membership](operators.md#in-and-between):
MISSING on the left stays MISSING, NULL stays NULL even for an empty result, a match wins over
NULL elements, and an unmatched NULL element yields NULL. With a valued left operand, an empty
array gives FALSE for IN and TRUE for NOT IN. Extra parentheses around a subquery instead make
it one array-valued item in the ordinary value-list grammar.

## Evaluation

Nested queries share the statement's fixed snapshot, earlier transaction writes, parameters,
frozen function catalog, transaction clock, cancellation and retained-memory budget. They do
not start a transaction or obtain a new snapshot. Source rows, scratch, results and arrays share
StatementMemoryBudget (64 MiB by default). Every materialized nested array, including a result
recomputed per evaluation, remains charged until statement exit. Returned JSON owns its values.

Only deterministic uncorrelated results cache, lazily by AST identity for the current statement.
NOW_STR, NOW_MILLIS and any registered developer callback disable caching, including calls in
deeper nested queries. Plans still bind once per statement and rebuild on the next execution.
An unreached CASE branch, short-circuited AND/OR operand or lazy conditional argument executes
no nested query, invokes no callback and consumes no nested work or retained array storage.
Static parameter, numeric, name, metadata and grouping validation still checks every branch.
EXPLAIN binds without executing, and describes the nested plan and caching policy.

Correlation remains deferred: references to outer FROM/UNNEST, group or comprehension bindings,
outer ORDER BY projection aliases, qualified outer stars, or an outer alias used as an inner
source are refused. Inner declarations shadow outer names by ordinal name. An unqualified
field otherwise binds to the inner source. Subqueries anywhere in INSERT,
UPSERT, UPDATE or DELETE, including RETURNING, are also deferred.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00014 | Direct IN subquery uses SELECT without RAW/VALUE | — |
| CK00001 | Correlation or DML subquery | — |
| CD05001 | Invalid expression/grouping context; known-array subquery passed to LENGTH, ABS or CONCAT | — |
| CD05015 | Serialized JSON projection exceeds 256 levels; named output reserves one level for its row object | — |
| CD05018 | Shared statement memory budget exhausted before publication | — |

## Example

Execute separately in a fresh database:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES ('a', {n:1}), ('b', {n:2}), ('c', {n:3});
SELECT (SELECT n AS x FROM items WHERE n = 1) AS objects,
       (SELECT RAW n FROM items ORDER BY n) AS numbers,
       2 IN (SELECT RAW n FROM items) AS hit FROM items LIMIT 1;
```

Returns `{"objects":[{"x":1}],"numbers":[1,2,3],"hit":true}`.

For star shape, execute separately in a new collection:

```sql
CREATE COLLECTION roots;
INSERT INTO roots (KEY, VALUE) VALUES ('a', 42), ('b', {x:1}), ('c', [1]), ('d', NULL);
SELECT RAW (SELECT * FROM roots) FROM roots LIMIT 1;
```

Returns `[{"document":42},{"x":1},{"document":[1]},{"document":null}]`.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [OqlSubqueryExpression](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-language/oql-subquery-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentSubqueryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentSubqueryBoundaryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Execution/DocumentPlanExecutor.Nested.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Planning/DocumentPlanner.Nested.cs`.
