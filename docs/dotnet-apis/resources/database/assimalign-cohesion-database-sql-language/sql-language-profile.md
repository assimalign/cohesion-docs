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

Instance lists the 42 executable T1 scalar names, the seven aggregates (including STRING_AGG/LISTAGG) and CAST. COALESCE, NULLIF,
CASE, CAST and EXTRACT remain evaluator forms; CASE is a clause rather than a function-table name. The separate read-only vocabulary lists recognized unsupported spellings for parser
diagnostics and identifier quoting. Unsupported calls report CK00001 before planning; an unknown
function outside that vocabulary reports CS01016 during planning. Recognition is not an execution
promise. SupportsJoin returns true for Inner, LeftOuter and Cross over N-way stored/system
sources. INNER/LEFT require ON or USING; CROSS accepts neither. RightOuter and FullOuter
return false. These accessors introduce no validation exceptions.

## Example

```csharp
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.Inner)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.LeftOuter)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.RightOuter)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.Cross)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.FullOuter)); // False
var statement = new SqlQueryParser().Parse("SELECT SQRT(id) FROM t;");
Console.WriteLine(statement.Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error)); // 0
Console.WriteLine(SqlLanguageProfile.Instance.Functions.Contains("DATE_TRUNC")); // True
```

TRIM/SUBSTRING/POSITION have standard grammar normalized onto calls. Bare CURRENT_*/LOCAL* forms
read clock values without parentheses. New T1 builtin names are reserved at all arities. Identifier
quoting includes those names and BOTH/LEADING/TRAILING (format 18), then PERCENT/TIES (format 19).
Current storage format is 26, with no upgrade path from an earlier format. The executable
profile now measures 55 of 61 declared clauses, including FETCH, lexical TOP and all five delete actions.
SqlClauses.OnConflict is the `ON CONFLICT` capability name, enabled for VALUES, DEFAULT VALUES
and query-source inserts. SqlClauses.QuantifiedComparison is `QUANTIFIED COMPARISON`, enabling scalar ANY/SOME/ALL subqueries.
Distinct predicates, Boolean tests and LIKE ESCAPE add no named clause.
See [scalar contracts](../../../../database/sql/language/functions/builtin-functions.md).

## Related reference

[Assembly](index.md) · [SQL functions](../../../../database/sql/language/functions/builtin-functions.md) · [Diagnostics](../../../../database/sql/language/diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDiagnosticTests.cs`.
