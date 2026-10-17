# SqlLanguageProfile

Declares executable SQL functions/clauses separately from recognized unsupported vocabulary.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public static class SqlLanguageProfile
{
    public static QueryLanguageProfile Instance { get; }
    public static IReadOnlyList<string> RecognizedUnsupportedVocabulary { get; }
    public static bool SupportsJoin(SqlJoinType joinType);
}
```

## Behavior and exceptions

Instance lists the five aggregates, COALESCE, CAST, UPPER, LOWER, LENGTH and ABS as executable
functions/forms. The separate read-only vocabulary lists recognized unsupported spellings for parser
diagnostics and identifier quoting. Unsupported calls report COHDBL001 before planning; an unknown
function outside that vocabulary reports COHSQLE016 during planning. Recognition is not an execution
promise. SupportsJoin returns true for Inner, LeftOuter and Cross over N-way stored/system
sources. INNER/LEFT require ON or USING; CROSS accepts neither. RightOuter and FullOuter
return false. These accessors introduce no validation exceptions.

## Example

```csharp
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.Inner)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.LeftOuter)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.Cross)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.FullOuter)); // False
var statement = new SqlQueryParser().Parse("SELECT SQRT(id) FROM t;");
Console.WriteLine(statement.Diagnostics[0].Code); // COHDBL001
```

## Related reference

[Assembly](index.md) · [SQL functions](../../../../database/sql/language/functions/builtin-functions.md) · [Diagnostics](../../../../database/sql/language/diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDiagnosticTests.cs`.
