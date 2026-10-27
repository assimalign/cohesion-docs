# KeyValueFunctionName

A generated case-insensitive unquoted function identifier.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`.

## Signature

```csharp
public readonly partial struct KeyValueFunctionName
{
    public static partial bool IsValid(string value, out string? message);
    // Generated string conversion and value equality members.
}
```

## Behavior and exceptions

Names compare ordinally ignoring case and convert implicitly from string. They start with a
ASCII letter or underscore, followed by ASCII letters, digits or underscores.
IsValid returns false with a diagnostic message for invalid syntax. A syntactically valid name
can still be reserved by the builder. Default names are refused at function construction and
catalog/collection lookup boundaries with ArgumentException.

## Example

```csharp
KeyValueFunctionName name = "developer_echo";
Console.WriteLine(name == (KeyValueFunctionName)"DEVELOPER_ECHO"); // True
Console.WriteLine(KeyValueFunctionName.IsValid("bad-name", out _)); // False
```

## Related reference

[Assembly](index.md) · [Registration](../assimalign-cohesion-database-keyvaluepair/key-value-function-collection.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/src/ValueObjects/KeyValueFunctionName.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueFunctionTests.cs`.
