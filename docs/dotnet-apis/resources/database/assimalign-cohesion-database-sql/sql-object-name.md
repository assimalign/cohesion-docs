# SqlObjectName

A decoded schema or object name compared ordinally ignoring case.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

Selected generated members and validation:

```csharp
public readonly partial struct SqlObjectName
{
    public SqlObjectName(string value);
    public string Value { get; }
    public static partial bool IsValid(string value, out string? message);
    public static bool TryCreate(string? value, out SqlObjectName result);
    public static implicit operator SqlObjectName(string item);
    public static implicit operator string(SqlObjectName item);
    public override string ToString();
}
```

## Behavior and exceptions

Supply one identifier component without SQL delimiters. Spaces, punctuation, embedded quotes and
dots are accepted; a dot is part of the name, never a qualifier. Validation requires at least one
non-whitespace character and does not trim the name. Equality, ordering and hashing use
OrdinalIgnoreCase; Value and ToString preserve the supplied text.

Construction and implicit conversion from string throw ArgumentException for null, empty or
whitespace-only values. IsValid returns false with a refusal message for invalid input, and true
with a null message for valid input. TryCreate returns false and default for invalid input.
A default struct has a null Value and ToString returns an empty string;
[RewriteTableAsync](sql-database.md) rejects default components with ArgumentException before admission.
This type has no query diagnostic code or SQLSTATE of its own.

## Example

The validation and public rewrite tests cover these names:

```csharp
SqlObjectName schema = "DBO";
SqlObjectName table = "a.b";
Console.WriteLine(table.ToString());                   // a.b
Console.WriteLine(SqlObjectName.IsValid("  ", out _)); // False
Console.WriteLine(SqlObjectName.IsValid(" a ", out _)); // True
await database.RewriteTableAsync(schema, table, cancellationToken);
```

Given `CREATE TABLE "a.b" (...)` in dbo, the call rewrites that single table. Names such as
`a space` and `a"quote` also resolve through the public API when supplied without SQL delimiters.

## Related reference

[Assembly](index.md) · [SqlDatabase](sql-database.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/ValueObjects/SqlObjectName.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
- `cohesion/build/Tasks/Tasks/CodeGeneration/CodeGenerationCreateValueTypeTask.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTableRewriteTests.Lifecycle.cs`.
