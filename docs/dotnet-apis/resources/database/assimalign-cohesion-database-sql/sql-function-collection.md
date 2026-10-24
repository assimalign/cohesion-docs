# SqlFunctionCollection

Registers functions while an SQL engine is being built, with the standard library already present.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

```csharp
public sealed class SqlFunctionCollection : IReadOnlyCollection<SqlFunction>
{
    public int Count { get; }
    public SqlFunctionCollection Add(SqlFunction function);
    public bool Contains(string name);
    public IEnumerator<SqlFunction> GetEnumerator();
}
// Obtained from SqlDatabaseEngine.CreateBuilder(name).Functions; no public constructor.
```

## Behavior and exceptions

Names match ignoring case in one engine-wide namespace. Count counts overloads; enumeration follows
registration order, standard library first. Add returns this collection. Build freezes registrations
into the engine's SqlFunctionCatalog; there is no removal. The builder collection is not thread-safe;
each registered function instance serves all sessions and must be stateless or thread-safe.

The catalog now executes the [42 T1 scalar names](../../../../database/sql/language/functions/builtin-functions.md).
COALESCE, NULLIF, CASE, CAST and EXTRACT remain reserved evaluator forms. Newly executable T1
names such as TRIM, ROUND, NOW and DATE_TRUNC are reserved at every arity. UPPER/LOWER/LENGTH/ABS
can retain application overloads at arities the standard library does not accept; a two-argument
upper(TEXT, BIGINT) remains valid. Standard-library calls cannot be replaced or made ambiguous.

Null function/name throws ArgumentNullException. An invalid identifier, special-form/keyword/type
name or reserved T1 name throws ArgumentException. Duplicate signatures, mixed scalar/aggregate
names, overlap with a builtin's accepted calls, or mutation after build begins throw
InvalidOperationException. A valid registration does not execute its function. Planning rejects
unmatched signatures with CS01006 (42883) or ambiguous calls with CS01008 (42725).

## Example

Adapted from SqlFunctionExtensibilityTests:

```csharp
var builder = SqlDatabaseEngine.CreateBuilder("sql-functions-catalog");
builder.Functions.Add(SqlScalarFunction.Create("slugify",
    static (string text) => text.ToLowerInvariant().Replace(' ', '-'),
    SqlFunctionVolatility.Immutable));
Console.WriteLine(builder.Functions.Contains("SLUGIFY")); // True
Console.WriteLine(builder.Functions.Contains("DATE_TRUNC")); // True
await using var engine = await builder.BuildAsync();
```

An SQL call `slugify('A B')` returns `a-b`. Trying to register an application function named
TRIM at any arity raises ArgumentException; adding a function after BuildAsync starts raises
InvalidOperationException.

## Related reference

[Assembly](index.md) · [Design](design.md) · [SqlFunctionContext](sql-function-context.md) · [Scalar contracts](../../../../database/sql/language/functions/builtin-functions.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Functions/SqlFunctionCollection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlFunctionExtensibilityTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlFunctionExtensibilityTests.Contracts.cs`.
