# SqlClauses

Named clause constants used with SqlLanguageProfile.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public static class SqlClauses
{
    public const string JsonValue = "JSON_VALUE";
    public const string JsonQuery = "JSON_QUERY";
    public const string JsonExists = "JSON_EXISTS";
    public const string CopyTo = "COPY TO STDOUT";
}
```

This excerpt lists the SQL/JSON and COPY constants. Each is a measured contextual clause; the words add
no lexical reservation. Reading constants and checking Supports throw no exceptions.

## Example

Checked against SqlLanguageProfileTests:

```csharp
Console.WriteLine(SqlLanguageProfile.Instance.Supports(SqlClauses.JsonValue));  // True
Console.WriteLine(SqlLanguageProfile.Instance.Supports(SqlClauses.JsonQuery));  // True
Console.WriteLine(SqlLanguageProfile.Instance.Supports(SqlClauses.JsonExists)); // True
Console.WriteLine(SqlLanguageProfile.Instance.Supports(SqlClauses.CopyTo));     // True
```

CopyTo names the [table export clause](../../../../database/sql/language/statements/copy-to.md),
which requires the dedicated stream consumer API.

## Related reference

[Assembly](index.md) · [SqlLanguageProfile](sql-language-profile.md) · [SQL/JSON functions](../../../../database/sql/language/functions/json-path.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlClauses.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlLanguageProfileTests.cs`.
