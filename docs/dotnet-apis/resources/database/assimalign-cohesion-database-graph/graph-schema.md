# GraphSchema

The `GraphSchema` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

Creates session-bound graph metadata access, including database-wide named indexes and constraints.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

```csharp
public sealed class GraphSchema
{
    public static GraphSchema Open(GraphDatabase database, GraphDatabaseSession session);
    public ValueTask<IReadOnlyList<GraphLabelMetadata>> GetLabelsAsync(CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<GraphRelationshipTypeMetadata>> GetRelationshipTypesAsync(CancellationToken cancellationToken = default);
    public ValueTask SaveLabelAsync(GraphLabelMetadata definition, CancellationToken cancellationToken = default);
    public ValueTask SaveRelationshipTypeAsync(GraphRelationshipTypeMetadata definition, CancellationToken cancellationToken = default);
    public ValueTask SavePropertyKeyAsync(GraphPropertyKeyMetadata definition, CancellationToken cancellationToken = default);
    public ValueTask CreateIndexAsync(string label, string name, string propertyKey, CancellationToken cancellationToken = default);
    public ValueTask DropLabelAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask DropRelationshipTypeAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<GraphSchemaResult<GraphIndexMetadata>> GetIndexesAsync(string label, CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<GraphPropertyKeyMetadata>> GetPropertyKeysAsync(
        Guid definitionId, CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<GraphIndexMetadata>> GetIndexesAsync(CancellationToken cancellationToken = default);
    public ValueTask<GraphIndexMetadata?> FindIndexAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<GraphConstraintMetadata>> GetConstraintsAsync(CancellationToken cancellationToken = default);
    public ValueTask<GraphConstraintMetadata?> FindConstraintAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask CreateConstraintAsync(GraphConstraintMetadata constraint, CancellationToken cancellationToken = default);
    public ValueTask DropConstraintAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask DropIndexAsync(string name, CancellationToken cancellationToken = default);
}
```

- **`GraphSchema.Open(database, session)`** — returns `GraphSchema` for label/type discovery, definition
  and property changes, node-property indexes and independently named constraints. It rejects foreign sessions.

Index and constraint names share one database-wide ordinal, case-sensitive namespace. A name
cannot be reused on another label or relationship type, or across those two object kinds.
The database-wide GetIndexesAsync and GetConstraintsAsync return visible definitions in ordinal
name order; Find returns null when absent. The existing GetIndexesAsync(string label, ...)
overload remains scoped to a label.

[GraphConstraintMetadata](graph-constraint-metadata.md) identifies a label or relationship type,
a property and an optional DatabaseType. A type rule allows absent/null values and requires
present non-NULL values to have that type. Type = null denotes an existence rule requiring a
non-NULL value. CreateConstraintAsync validates existing entities before publishing. CREATE/INSERT,
SET, REMOVE and MERGE enforce the resulting rules; DELETE may remove constrained entities.

Type and existence rules have independent names and drops. Equivalent named rules of the same
kind or incompatible type rules are refused. A named rule may repeat an identical unnamed
property declaration; dropping it preserves that declaration and all other constraints. A newly
discovered property key remains known after the last constraint is dropped.

GetPropertyKeysAsync returns declared metadata only, excluding named-rule effects. Use
[SHOW PROPERTY KEYS](../../../../database/graph/language/statements/show.md) for effective type
and requiredness, or the constraint APIs for individual rules. DropIndexAsync removes both the
catalog description and physical index. Reads follow the session transaction. All schema writes
now own an autocommit transaction; they report CG04025 and abort an explicit transaction,
including requests that would do no work. Roll back before reusing that session.
[Text DDL and SHOW CONSTRAINTS](../../../../database/graph/language/statements/index.md#schema-statements)
use the same rules.

CreateIndexAsync discovers missing label and untyped property tokens atomically with index
publication. Text constraints also discover missing labels/types; typed CreateConstraintAsync
requires an existing owner GUID. Failure rolls all new tokens back; successful rule drops
retain them. SavePropertyKeyAsync and constraint creation validate existing owner postings;
DropLabelAsync/DropRelationshipTypeAsync refuse an in-use definition and remove an empty
definition's children. Index backfill uses only the owning label's postings.

All typed writes use engine statement work and retained-memory limits. Scans/backfill charge
work; cursor buffers and retained backfill entries charge memory. CG05004/CG05020 exhaustion
rolls back the entire operation and releases its memory. Text requests can lower limits with
GraphQueryLimits, validated inside admission with CG08021.

## Exceptions

FindIndexAsync/FindConstraintAsync and label-scoped GetIndexesAsync reject null names with
ArgumentNullException. CreateIndexAsync rejects blank label/name/property strings with
ArgumentException. SaveLabelAsync/SaveRelationshipTypeAsync/SavePropertyKeyAsync validate
metadata, and SavePropertyKeyAsync checks supported types, before admission. Drop methods
reject a null, empty or whitespace name with ArgumentException. CreateConstraintAsync rejects
blank names/property keys or an empty owner GUID with ArgumentException before statement
admission. Unsupported types use DatabaseException/CG03003 before admission; those argument
failures leave an explicit transaction usable.

| Code in DatabaseException.Diagnostics | Condition |
| --- | --- |
| CG04025 | Any schema write inside an explicit transaction; aborts it |
| CG05004 | Schema work budget exhausted; operation rolls back |
| CG05020 | Schema retained-memory budget exhausted; operation rolls back |
| CG01002 | Label/type requested for DROP is absent |
| CG01021 | Named index or constraint requested for DROP is absent |
| CG03008 | An index or constraint already uses the database-wide name |
| CG03009 | Duplicate constraint kind or incompatible property type rule |
| CG03010 | Existing data violates the new constraint; creation rolls back |
| CG03003 | Missing owner, unsupported property type, invalid existing data for a property save, or a nonempty definition requested for DROP |

These conditions have no assigned SQLSTATE. Mutating a schema-owned definition or its child
throws DatabaseObjectLockedException. Cancellation uses OperationCanceledException; an offline
database uses DatabaseOfflineException (CG06012), and an unconfirmed autocommit uses
DatabaseTransactionCommitUnconfirmedException.

Null Open arguments throw ArgumentNullException; a disposed database throws ObjectDisposedException.
A foreign session raises DatabaseException with CG07005. Reads follow the bound session's
transaction; schema writes require autocommit. An aborted transaction refuses work with CG04007 until rollback.

Catalog snapshot changes raise DatabaseTransactionAbortedException with structured CG04007.
Other deterministic identity/owner/index-target errors raise DatabaseException with CG03003,
and malformed catalog records use CG06006. Unsupported catalog versions use CG06019 at open;
only v2 records are admitted, with no upgrade reader. Metadata records over 8,092 encoded bytes, including
names and owning schema, use CG02009. These engine exceptions retain ordinary unprefixed
messages and their original cause. Disposal during an admitted operation uses CG09019;
the direct Open/lifetime guard retains ObjectDisposedException. See [diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
await using var engine = GraphDatabaseEngine.Create("people-engine", new());
var graph = await engine.CreateDatabaseAsync("people");
await using var session = await graph.CreateSessionAsync();
await session.ExecuteAsync("INSERT (:Person {name:'Ada'})");
await GraphSchema.Open(graph, session).CreateIndexAsync("Person", "by_name", "name");
```

The index is created over Person.name in that database, as in the engine overview example.

Named type and existence rules, following GraphSchemaConstraintTests in a fresh database:

```csharp
using Assimalign.Cohesion.Database.Graph;
using Assimalign.Cohesion.Database.Graph.Catalog;
using Assimalign.Cohesion.Database.Types;

await using var engine = GraphDatabaseEngine.Create("constraints", new());
var database = await engine.CreateDatabaseAsync("graph");
await using var session = await database.CreateSessionAsync();
var schema = GraphSchema.Open(database, session);
var id = Guid.NewGuid();
await schema.SaveLabelAsync(new(id, "N"));
await session.ExecuteAsync("INSERT (:N {p:1})");
await schema.CreateConstraintAsync(new(id, "p_type", "p", DatabaseType.Int64));
await schema.CreateConstraintAsync(new(id, "p_exists", "p"));
Console.WriteLine((await schema.GetConstraintsAsync()).Count); // 2
await schema.DropConstraintAsync("p_exists");
await session.ExecuteAsync("INSERT (:N)"); // Type rule permits an absent p.
Console.WriteLine((await schema.FindConstraintAsync("p_type"))?.PropertyKey); // p
```

Before dropping p_exists, INSERT (:N) would fail with CG03003; creation over an already missing
p would instead fail with CG03010. Dropping p_exists preserves p_type.

The explicit-transaction refusal is covered by GraphSchemaDdlTests:

```csharp
await using var transaction = await session.BeginTransactionAsync();
try
{
    await schema.CreateIndexAsync("Fresh", "ix", "p");
}
catch (Assimalign.Cohesion.Database.DatabaseException error)
{
    Console.WriteLine(error.Diagnostics.Single().Code); // CG04025
    Console.WriteLine(transaction.State); // Faulted
    await transaction.RollbackAsync();
}
```

The failed operation discovers no Fresh label, property or index. Earlier transaction writes
are rolled back when the caller rolls back. Typed argument failures before admission leave
an explicit transaction active.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Autocommit, discovery and budgets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaDdlTests.cs`, `src/GraphSchema.cs` and `src/GraphSchema.Ddl.cs` in the same project.

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.cs`.
- **Named rules** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaConstraintTests.cs` and `GraphSchemaConstraintRegressionTests.cs` in the same directory.
