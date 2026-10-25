# SqlParameterCollection

The mutable named bindings owned by SqlCommand.

Namespace: `Assimalign.Cohesion.Database.Sql.Client`.

## Signature

```csharp
public sealed class SqlParameterCollection
{
    public int Count { get; }
    public object? this[string name] { get; set; }
    public SqlParameterCollection Add(string name, object? value);
    public void Clear();
}
```

## Behavior and exceptions

Add and the indexer strip one leading @/$ and use ordinal, case-sensitive bare names. Both replace
an existing bare-name value; they do not create a colliding raw request map. Null binds SQL NULL.
Numeric names such as 01 remain names, distinct from 1. Unused bindings are ignored by the server.
Null/blank input names throw ArgumentException; reading an absent name throws KeyNotFoundException.

For raw parameter dictionaries, use the SqlConnection string overloads or the session API. The
server rejects normalized collisions with CS01023, including unused keys, and unbound references
with CS01020 before reading or writing rows.

## Example

```csharp
var command = new SqlCommand("SELECT id FROM t WHERE id = $id;");
command.Parameters.Add("@id", 1).Add("id", 2);
Console.WriteLine(command.Parameters.Count); // 1
Console.WriteLine(command.Parameters["$id"]); // 2
var rows = await connection.QueryAsync(command); // Uses id = 2
```

## Related reference

[Assembly](index.md) · [SQL parameters](../../../../database/sql/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlParameterCollection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlParameterWireTests.cs`.
