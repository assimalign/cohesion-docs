# OqlExplainExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlExplainExpression : OqlExpression
{
    public OqlExpression Statement { get; }
}
```

## Behavior and exceptions

Parser-created wrapper around SELECT, INSERT, UPSERT, UPDATE or DELETE. It has no public constructor. Statement retains the
wrapped AST; planning validates it and returns DatabasePlanDescription without data execution. DDL,
nested EXPLAIN and ANALYZE report CK00001. Parsing returns diagnostics; executing a malformed
request throws DatabaseParseException before planning.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "EXPLAIN SELECT name FROM items");
var explain = (OqlExplainExpression)statement.OqlExpression;
Console.WriteLine(explain.Statement is OqlSelectExpression); // True
```

## Related reference

[Assembly](index.md) · [OqlUpdateExpression](oql-update-expression.md) · [EXPLAIN](../../../../database/document/language/statements/explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlExplainExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentExplainTests.cs`.
