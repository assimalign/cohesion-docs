# SqlOnConflictClause

Unique-key target and action attached to SqlInsertExpression.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlOnConflictClause
{
    public SqlOnConflictTargetKind TargetKind { get; }
    public IReadOnlyList<string> Columns { get; }
    public string? ConstraintName { get; }
    public SqlOnConflictAction Action { get; }
    public IReadOnlyList<SqlAssignment> Assignments { get; }
    public SqlExpression? TargetWhere { get; }
    public SqlExpression? Where { get; }
    public SqlRelationReference Excluded { get; }
}
```

## Behavior and exceptions

The constructor is internal; parse [SqlInsertExpression.OnConflict](sql-insert-expression.md) to
obtain a node. [TargetKind](sql-on-conflict-target-kind.md) distinguishes all-unique-index selection,
column-set inference, and ON CONSTRAINT. Columns is empty without a column target; ConstraintName
is set for a named target. [Action](sql-on-conflict-action.md) selects DoNothing or DoUpdate.
Assignments is empty for DO NOTHING. TargetWhere is the optional column-set inference predicate;
Where is the separate DO UPDATE filter. Each is null when omitted. Excluded is the parser-supplied
proposed-row relation, with alias EXCLUDED, for binding alongside the original target row.

These properties add no validation exceptions. Parse returns diagnostics; request/session factories
reject parser errors with DatabaseParseException. Unsupported inference modifiers, subqueries in
DO UPDATE SET/WHERE, DO SELECT and MySQL/SQLite alternate spellings use CK00001. Execution raises
DatabaseException with CS03035 (42P10) for an absent/nonunique arbiter or untargeted DO UPDATE,
and CS05036 (21000) for affecting a row twice. Duplicate SET targets use CS01019; ambiguous
target/EXCLUDED columns use CS01015 and aggregates use CS05017.
Constraint failures use [SqlConstraintViolationException](../assimalign-cohesion-database-sql/sql-constraint-violation-exception.md)
with one Error diagnostic: CS03037 (23505) for uniqueness, CS03038 (23514) for CHECK/domain
checks, CS03039 (23503) for foreign keys, or CS03024 (23502) for NOT NULL.

TargetWhere can prove a partial unique arbiter; it never filters proposed rows. Non-row-local
inference predicates use CS05092 (no assigned SQLSTATE); a valid but unproven target uses
CS03035 (42P10). Parameters and non-immutable calls provide no proof.

EXCLUDED holds values after coercion/default/identity allocation. SET expressions all read the
original target plus proposal. A false/UNKNOWN filter locks but skips the row. RETURNING exposes
target values only and counts inserted plus updated rows. Snapshot conflicts and deadlocks require
whole-transaction rollback/retry; ReadCommitted uses the newest committed conflict. See the complete
[language contract](../../../../database/sql/language/clauses/on-conflict.md).

DO NOTHING takes no conflicting-row lock. DO UPDATE acquires the row before evaluating Where;
contention releases new arbitration grants, waits, keeps the row grant and restarts the whole
unwritten batch. Preparation failure releases this statement's new grants. Successful waits keep
their rows locked until transaction end. Opposite multi-row key/row orders can still produce a
retryable deadlock victim.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "INSERT INTO t VALUES (1, 20) ON CONFLICT (id) DO UPDATE SET age = EXCLUDED.age " +
    "WHERE t.age < EXCLUDED.age RETURNING id;");
var clause = ((SqlInsertExpression)statement.SqlExpression).OnConflict!;
Console.WriteLine(clause.TargetKind); // Columns
Console.WriteLine(clause.Columns[0]); // id
Console.WriteLine(clause.Action); // DoUpdate
Console.WriteLine(clause.Assignments.Count); // 1
Console.WriteLine(clause.Where is not null); // True
Console.WriteLine(clause.Excluded.Alias); // EXCLUDED
```

With t(id PRIMARY KEY, age) initially containing `(1, 10)`, execution updates it to `(1, 20)`,
returns id 1 and reports count 1.

A target predicate is retained independently, as in SqlInsertParserTests:

```csharp
var parsed = (SqlQueryStatement)new SqlQueryParser().Parse(
    "INSERT INTO t VALUES (1, TRUE) ON CONFLICT (id) WHERE active DO NOTHING;");
var inferred = ((SqlInsertExpression)parsed.SqlExpression).OnConflict!;
Console.WriteLine(inferred.TargetWhere is not null); // True
Console.WriteLine(inferred.Where is null); // True
```

## Related reference

[Assembly](index.md) · [SqlInsertExpression](sql-insert-expression.md) · [SqlOnConflictTargetKind](sql-on-conflict-target-kind.md) · [SqlOnConflictAction](sql-on-conflict-action.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlOnConflictClause.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlInsertParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertArbitrationConformanceTests.cs`.
