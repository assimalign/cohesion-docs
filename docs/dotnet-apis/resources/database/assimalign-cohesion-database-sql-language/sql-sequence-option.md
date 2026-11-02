# SqlSequenceOption

One explicitly written CREATE/ALTER SEQUENCE option.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlSequenceOption
{
    public string Name { get; }
    public long? Value { get; }
    public string? TypeName { get; }
    public SqlTableReference? OwnerTable { get; }
    public string? OwnerColumn { get; }
}
```

## Behavior and exceptions

The parser owns construction. Name is normalized to AS, START, INCREMENT, MINVALUE, MAXVALUE,
CACHE, CYCLE, RESTART or OWNED. Value holds the integer; CYCLE/NO CYCLE use 1/0. Null Value denotes
NO MINVALUE, NO MAXVALUE or bare RESTART. TypeName holds the AS spelling when present.
Omitted options have no entry; execution supplies defaults. Accessors add no exceptions.
Invalid resolved options use DatabaseException/CS03069 (22023); malformed or duplicate syntax
produces parser diagnostics. See [sequence options](../../../../database/sql/language/statements/create-sequence.md).

The type also carries identity declaration and ALTER options. OWNED BY has Name `OWNED`;
OwnerTable and OwnerColumn identify its target, or both are null for NONE. Identity ALTER
stores generation mode separately in [SqlAlterIdentityAction.IsAlways](sql-alter-identity-action.md).
Invalid ownership reports CS03081 (55000).

## Example

Adapted from SqlSequenceParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER SEQUENCE s RESTART;");
var option = ((SqlAlterSequenceExpression)statement.SqlExpression).Options[0];
Console.WriteLine(option.Name);          // RESTART
Console.WriteLine(option.Value is null); // True
```

Execution restarts at the sequence's configured START.

## Related reference

[Assembly](index.md) · [SqlCreateSequenceExpression](sql-create-sequence-expression.md) · [SqlAlterSequenceExpression](sql-alter-sequence-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlSequenceOption.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSequenceParserTests.cs`.
