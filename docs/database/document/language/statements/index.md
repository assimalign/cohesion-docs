# Statements

OQL accepts one SQL++ query, data mutation, collection definition or index statement per parse.

## Statements

- **Control a session transaction** — [BEGIN/START, COMMIT and ROLLBACK](transactions.md) in process or through the Documents client.
- **Create a collection** — [CREATE COLLECTION](create-collection.md).
- **Remove a collection** — [DROP COLLECTION](drop-collection.md).
- **Insert documents** — [INSERT](insert.md), from VALUES or SELECT.
- **Insert or replace documents** — [UPSERT](upsert.md), replacing the whole value by key.
- **Update document paths** — [UPDATE](update.md), with SET and/or UNSET and an optional predicate.
- **Delete documents** — [DELETE](delete.md), with an optional predicate and affected count.
- **Inspect a plan** — [EXPLAIN](explain.md), without execution.
- **Plan a merge** — [MERGE](#merge-planning) parses/binds/explains; execution refuses CD05022.
- **Query documents** — [SELECT](select.md) projects and filters sources with ordered key and array bindings.
- **Create an index** — [CREATE INDEX](create-index.md) builds scalar/UNIQUE or DISTINCT array indexes.
- **Remove an index** — [DROP INDEX](drop-index.md) removes a named collection index.

These statements use the engine's parse, plan, and execute pipeline and the session's active
transaction or an automatic statement transaction. A trailing semicolon is optional.

## MERGE planning

MERGE parses, renders, binds, plans and explains. Execution is not available yet: a valid executed
statement refuses one located CD05022 before any source read or write.
It follows ordinary statement failure/explicit-transaction abort rules. Supports("MERGE") on
OqlLanguageProfile.Instance remains false; the profile advertises executable capability.

```syntaxsql
MERGE INTO target [ [ AS ] t ] USING source [ AS s ]
  ON [ PRIMARY ] KEY key_expression
  [ WHEN MATCHED THEN UPDATE { SET assignments [ UNSET paths ] | UNSET paths } [ WHERE predicate ] ]
  [ WHEN MATCHED THEN DELETE [ WHERE predicate ] ]
  [ WHEN NOT MATCHED THEN INSERT value_expression [ WHERE predicate ] ]
  [ RETURNING projection ]

MERGE INTO target [ [ AS ] t ] USING source [ AS s ]
  ON META(t).id = key_expression
  [ WHEN MATCHED THEN UPDATE { SET assignments [ UNSET paths ] | UNSET paths } [ WHERE predicate ] ]
  [ WHEN MATCHED THEN DELETE [ WHERE predicate ] ]
  [ WHEN NOT MATCHED THEN INSERT (KEY key_expression, VALUE value_expression) [ WHERE predicate ] ]
  [ RETURNING projection ]
```

At least one action is required, each at most once in UPDATE, DELETE, INSERT order. Missing,
repeated/out-of-order actions use CD00002; invalid direct AST source/form/action uses CD01039.
Sources can be collections, aliased parenthesized queries (including compounds), or expressions.
A query needs an alias (CD00015 in text). Generic expression aliases require AS; a path can
infer its final named member. Collections infer their name, or the final system-source segment.
Aliases are ordinal and distinct (CD00010 in text, CD01023 in a direct AST).

ON KEY sees only the source. ANSI ON sees both bindings but accepts only the written
META(target).id = expression shape, with a key computable before lookup. UPDATE values/filter
and DELETE filter see both; INSERT key/value/filter see the source and MISSING target, including
MISSING target metadata. RETURNING retains target-image scope. UPDATE destinations use the
ordinary target-relative SET/UNSET rules (CD02016/CD01017). Unavailable ON KEY target references,
unnamed AST roots and invalid META receivers use CD01040. System targets use CD04004.

The following conservatively retain CK00001: reversed ANSI equality/general ON predicates;
target-dependent keys, including a subquery capturing target; source references in RETURNING;
omitted ANSI INSERT KEY/VALUE semantics; unqualified value paths outside nested SELECT/RETURNING;
META without an explicit visible binding outside RETURNING; LIMIT, OFFSET, OPTIONS, USE INDEX,
USE KEYS, join hints and source joins. Lookup INSERT inherits ON KEY. A nested MERGE in an
expression also remains CK00001. Bare aliases cannot be keys, options, limit, offset, use or
JOIN/NEST/UNNEST starters (including LEFT/INNER); use AS and a quoted name.

Canonical OqlMergeExpression.ToString text reparses with the same tree shape. Complete-tree and
canonical parse costs stop at 128 levels (CD00005). EXPLAIN emits Merge, its source plan,
KeyLookup/KeyJoin, ordered MergeUpdate/MergeDelete/MergeInsert and RETURNING, with action filters
after matching. It evaluates no source/action callbacks, probes no target and reads/writes no
documents. Counts, duplicate-write rules and RETURNING image/snapshot execution arrive with MERGE execution.

```sql
EXPLAIN MERGE INTO targets t USING sources s ON KEY s.key
  WHEN MATCHED THEN DELETE WHERE t.deleted;
```

With both collections present, this describes the plan; omitting EXPLAIN refuses CD05022.

## See also

[Language (OQL)](../index.md) and [Clauses](../clauses/index.md).

## Sources

- **MERGE planning/refusal** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DIALECT.md`, `tests/OqlMergeParserTests.cs` in that project, and `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentMergePlanningTests.cs` at `2dd935103`.
- **Statement grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
