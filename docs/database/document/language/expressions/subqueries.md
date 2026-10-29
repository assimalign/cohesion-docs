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
StatementMemoryBudget (64 MiB by default). Cached arrays remain charged until statement exit.
Uncached arrays belong to the consuming expression: discarded predicate/iteration scratch releases
immediately; projections, group/order keys and expanded UNNEST rows own separately charged copies.
Failure and cancellation release their reservations. Returned JSON owns its values.

[StatementWorkBudget](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-database-engine-options.md#statementworkbudget)
defaults to 10,000,000 reached expression evaluations for statements containing subqueries or
expression sources. All depths and cache hits share the counter. Unreached expressions spend no
work; exhaustion reports CD05020 before the next evaluation and rolls back statement writes.

Only deterministic uncorrelated results cache, lazily by AST identity for the current statement.
NOW_STR, NOW_MILLIS and any registered developer callback disable caching, including calls in
deeper nested queries. Plans still bind once per statement and rebuild on the next execution.
An unreached CASE branch, short-circuited AND/OR operand or lazy conditional argument executes
no nested query, invokes no callback and consumes no nested work or retained array storage.
Static parameter, numeric, name, metadata and grouping validation still checks every branch.
EXPLAIN binds without executing, and describes the nested plan and caching policy.

## Correlation and scope

An inner query may capture outer FROM/UNNEST, group-term, GROUP AS and comprehension bindings.
Subqueries inside ORDER BY also see outer projection aliases. Inner declarations shadow outer
names by ordinal, case-sensitive name; an unqualified field otherwise binds to the inner source.
Stars contain only local rows; qualified stars remain refused with CK00001. Stored outer bindings
retain META id/version; expression/derived rows and scalar bindings have NULL metadata.

An outer field can supply the inner source, for example `FROM d.items AS i`; it evaluates per
outer row and follows the [expression-source rules](../clauses/from.md#derived-and-expression-rows).
Captured references are checked against **every** enclosing GROUP BY. An aggregate in an inner
query does not make a hidden outer field legal; invalid grouped captures report CD05001.

Inside a correlated subquery, reach an unaliased outer row by its collection name, such as
`items.g`. There is an open inconsistency, [cohesion#1767](https://github.com/assimalign/cohesion/issues/1767):
at top level without a subquery, `items.g` in `FROM items` reads field `items` then `g`.
Once the statement contains a subquery, that own-row path resolves to field `g` instead.
Use an explicit source alias for consistent qualified paths. Grouping validation still distinguishes
own unaliased field paths from captured collection receivers.

## Mutation subqueries

UPDATE predicates and SET, DELETE predicates, INSERT/UPSERT VALUES and SELECT sources, and
RETURNING accept subqueries. All pending writes are staged before mutation. Nested reads see
statement-start content, including earlier statements in the same transaction, and never see
this statement's writes. Derived and expression INSERT/UPSERT SELECT sources also execute.

[RETURNING](../clauses/returning.md) binds the post-image and actual reserved durable version on
INSERT/UPSERT/UPDATE, and the pre-image and old version on DELETE. Its nested collection reads
still see statement-start data. RETURNING containing subqueries stages before writes; callbacks
can run before a later storage/constraint failure, but results publish only after every write
succeeds. Failed statements can leave version gaps. Unreached RETURNING branches read nothing.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00014 | Direct IN subquery uses SELECT without RAW/VALUE | — |
| CK00001 | Qualified star or other unsupported form | — |
| CD05001 | Invalid expression/grouping context; known-array subquery passed to LENGTH, ABS or CONCAT | — |
| CD05015 | Serialized JSON projection exceeds 256 levels; named output reserves one level for its row object | — |
| CD05018 | Shared statement memory budget exhausted before publication | — |
| CD05020 | Shared reached-expression work budget exhausted | — |

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

These correlation examples need no stored collection:

```sql
SELECT RAW (SELECT RAW i FROM d.items AS i) FROM [{items:[1,2]}] AS d;
SELECT RAW (SELECT RAW n FROM [{n:2}] AS s) FROM [{n:7}] AS d;
```

Returns one bare array `[1,2]`, then one bare array `[2]`: unqualified `n` uses the inner source.

Using the items fixture above:

```sql
UPDATE items d SET n = d.n + 10
RETURNING n, (SELECT RAW s.n FROM items s ORDER BY s.n) AS old;
```

Returns `{"n":11,"old":[1,2,3]}`, `{"n":12,"old":[1,2,3]}` and
`{"n":13,"old":[1,2,3]}`. The nested reads all precede this statement's writes.

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

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentCorrelatedSubqueryTests.cs`, `DocumentDmlSubqueryTests.cs` and `DocumentSourceScopingTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentSubqueryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentSubqueryBoundaryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Execution/DocumentPlanExecutor.Nested.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Planning/DocumentPlanner.Nested.cs`.
