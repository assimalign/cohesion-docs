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

Instance lists the 42 executable T1 scalar names, 16 aggregate names and CAST. Aggregates include
BOOL_AND/BOOL_OR/EVERY, STDDEV/STDDEV_SAMP/STDDEV_POP, VARIANCE/VAR_SAMP/VAR_POP and
STRING_AGG/LISTAGG. COALESCE, NULLIF,
CASE, CAST and EXTRACT remain evaluator forms; CASE is a clause rather than a function-table name. The separate read-only vocabulary lists recognized unsupported spellings for parser
diagnostics and identifier quoting. Unsupported calls report CK00001 before planning; an unknown
function outside that vocabulary reports CS01016 during planning. Recognition is not an execution
promise. SupportsJoin returns true for Inner, LeftOuter, RightOuter, FullOuter and Cross.
INNER/LEFT/RIGHT/FULL require ON or USING; CROSS accepts neither. FULL execution additionally
requires a safe mandatory hash equality and proven non-throwing ON evaluation (CS05070).
These accessors introduce no validation exceptions.

ROW_NUMBER, RANK, DENSE_RANK, PERCENT_RANK, CUME_DIST, NTILE, LAG and LEAD are enabled
with OVER. SqlClauses.Over, Partition and Window are admitted for
[window specifications](../../../../database/sql/language/clauses/window.md); explicit frames,
aggregate windows and FIRST_VALUE/LAST_VALUE/NTH_VALUE remain CK00001.

## Example

```csharp
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.Inner)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.LeftOuter)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.RightOuter)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.Cross)); // True
Console.WriteLine(SqlLanguageProfile.SupportsJoin(SqlJoinType.FullOuter)); // True
var statement = new SqlQueryParser().Parse("SELECT SQRT(id) FROM t;");
Console.WriteLine(statement.Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error)); // 0
Console.WriteLine(SqlLanguageProfile.Instance.Functions.Contains("DATE_TRUNC")); // True
```

TRIM/SUBSTRING/POSITION have standard grammar normalized onto calls. Bare CURRENT_*/LOCAL* forms
read clock values without parentheses. New T1 builtin names are reserved at all arities. Identifier
quoting includes those names and BOTH/LEADING/TRAILING (format 18), then PERCENT/TIES (format 19).
Current storage format is 30, with no upgrade path from an earlier format. The executable
profile now measures 74 of 76 declared clauses, including aggregate FILTER, CREATE TABLE AS, executable MERGE and
binding-only RECURSIVE. Recursive planning, EXPLAIN and execution remain CK00001; acyclic lists execute.
merge_action() is contextual
to its RETURNING clause and is not a standalone executable function-table entry.
SqlClauses.OnConflict is the `ON CONFLICT` capability name, enabled for VALUES, DEFAULT VALUES
and query-source inserts. SqlClauses.QuantifiedComparison is `QUANTIFIED COMPARISON`, enabling scalar ANY/SOME/ALL subqueries.
Distinct predicates, Boolean tests and LIKE ESCAPE add no named clause.
SqlClauses.Filter is the `FILTER` capability name, enabled for native and application aggregate
calls. See [FILTER](../../../../database/sql/language/clauses/filter.md) and
[aggregate types and NULL behavior](../../../../database/sql/language/functions/aggregate-functions.md#boolean-and-statistical-aggregates).
See [scalar contracts](../../../../database/sql/language/functions/builtin-functions.md).

SqlClauses.GroupingSets, Rollup and Cube enable expanded grouping. CreateSequence, AlterSequence,
DropSequence, NextValue and DiscardAll enable the sequence/session surface. nextval, currval,
setval and lastval are volatile BIGINT calls. GROUPING/GROUPING_ID are special forms.
See [sequences](../../../../database/sql/language/functions/sequences.md) and
[grouping masks](../../../../database/sql/language/functions/grouping.md).

SqlClauses.AlterIdentity, Serial, Overriding and OwnedBy enable identity integration.
SERIAL means BY DEFAULT identity; ALTER COLUMN identity, INSERT/MERGE overrides and OWNED BY
execute. See [identity rules](../../../../database/sql/language/clauses/identity.md).

## Related reference

[Assembly](index.md) · [SQL functions](../../../../database/sql/language/functions/builtin-functions.md) · [Diagnostics](../../../../database/sql/language/diagnostics.md)

SQL enables `SqlClauses.OnUpdate = "ON UPDATE"` for persisted
foreign-key declarations. Changing referenced keys under CASCADE/SET NULL/SET DEFAULT retains
CK00001 until batch 13; RESTRICT/NO ACTION keep statement-end checks.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDiagnosticTests.cs`.
