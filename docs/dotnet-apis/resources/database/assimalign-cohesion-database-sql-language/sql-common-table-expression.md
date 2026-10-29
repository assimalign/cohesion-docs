# SqlCommonTableExpression

One named query definition in a statement's WITH list.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCommonTableExpression
{
    public string Name { get; }
    public bool IsRecursiveScope { get; }
    public IReadOnlyList<string> Columns { get; }
    public SqlQueryBodyExpression Body { get; }
    public Location Location { get; }
}
```

## Behavior and exceptions

Construction is internal to the parser. Name is schema-less; Columns is empty when omitted, or
contains complete positional output renames. Body sees earlier definitions in ordinary WITH;
IsRecursiveScope records WITH RECURSIVE on every definition, including acyclic entries. It makes
all list names visible and binding orders definitions by dependency. Location covers the
definition. Accessors add no validation exceptions. CommonTables on SELECT/INSERT/UPDATE/DELETE/MERGE
is an empty list when WITH is absent; its setter is internal.

All definitions bind, even unused ones. Duplicate names and unresolved future references use
CS01026 without an assigned SQLSTATE; ordinary WITH permits stored-table fallback for future/self
names. Output width or blank names use CS05013 for non-recursive definitions. Recursive form and
reference restrictions use CS01054–CS01061 (42P19); output width uses CS02065 (42601), type/domain/
modifier changes CS02066 (42804, cast hint), and collation changes CS02067 (42P21, COLLATE hint).
See [WITH diagnostics](../../../../database/sql/language/clauses/with.md#diagnostics) for each trigger.

SqlQueryRequest.FromSql rejects parser errors with DatabaseParseException; semantic errors arise
during planning as DatabaseException. Well-formed recursive entries execute on demand; retained
state uses CS05067 (53200), and an internal reader-count invariant uses CS05090 (XX000).
Mutual recursion and WITH RECURSIVE ... MERGE remain CK00001. Nested WITH, modifying bodies and
materialization hints remain parser refusals. Reachable acyclic definitions materialize once before
writes. See [WITH](../../../../database/sql/language/clauses/with.md) for snapshot and scope rules.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "WITH a (n) AS (SELECT 1 AS id) SELECT n FROM a;");
var select = (SqlSelectExpression)statement.SqlExpression;
var cte = select.CommonTables[0];
Console.WriteLine(cte.Name);       // a
Console.WriteLine(cte.Columns[0]); // n
Console.WriteLine(((SqlSelectExpression)cte.Body).From.Count); // 0
```

Execution returns one row containing Int32 1.

Adapted from SqlRecursiveCteParserTests and SqlCteExecutionTests.Recursive:

```csharp
var recursiveScope = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "WITH RECURSIVE b AS (SELECT n FROM a), a(n) AS (SELECT 1) SELECT n FROM b")).SqlExpression;
Console.WriteLine(recursiveScope.CommonTables[0].IsRecursiveScope); // True
Console.WriteLine(recursiveScope.CommonTables[1].IsRecursiveScope); // True
```

Execution returns `1`; the list is acyclic. Self-referencing bodies also execute UNION ALL/DISTINCT
fixpoints. For example, `WITH RECURSIVE r(n) AS (SELECT 1 UNION ALL SELECT n+1 FROM r)
SELECT n FROM r LIMIT 3` returns `1, 2, 3`, as checked by SqlRecursiveFixpointTests.

Body may also be [SqlSetOperationExpression](sql-set-operation-expression.md). For example,
`WITH a AS (SELECT 1 AS n UNION ALL SELECT 2 AS n) SELECT n FROM a;` has a set body and returns
1 and 2. Every SELECT operand may omit FROM and use one implicit row, including nested children.

## Related reference

[SqlSelectExpression](sql-select-expression.md) · [SqlInsertExpression](sql-insert-expression.md) · [SqlUpdateExpression](sql-update-expression.md) · [SqlDeleteExpression](sql-delete-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlCommonTableExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCteParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.Binding.cs`.
