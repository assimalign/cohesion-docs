# SqlTypeNames

Resolves contextual SQL built-in names and modifiers to shared Database identities.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public static bool TryResolve(string name, int? length, int? precision, int? scale,
    out DatabaseTypeInfo typeInfo);
public static bool TryResolve(string name, out DatabaseType type);
```

## Behavior and exceptions

Names compare case-insensitively and exclude written arguments. DDL and CAST use one parser grammar
for multi-word/national character names, DOUBLE PRECISION and temporal names, without reserving new
words. FLOAT(p) selects Float32 for 1..24 bits and Float64 for 25..53 or no precision. Fixed
national forms resolve as Character; VARYING forms as String. SQL Server spellings are lexical
aliases, not emulated range/rounding/storage behavior. TIME WITH TIME ZONE has no shared storage
type and returns false; the parser reports CS00005. Unknown names return false for engine domain
resolution. Null names throw ArgumentNullException. Invalid modifiers return false.

## Example

```csharp
bool found = SqlTypeNames.TryResolve("NATIONAL CHARACTER VARYING", 20, null, null, out var type);
Console.WriteLine(found);     // True
Console.WriteLine(type.Type); // String
SqlTypeNames.TryResolve("FLOAT", null, 24, null, out var real);
Console.WriteLine(real.Type); // Float32
```

## Related reference

[SQL type names](../../../../database/sql/language/data-types/index.md) · [Column definitions](sql-column-definition.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlTypeNames.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.
