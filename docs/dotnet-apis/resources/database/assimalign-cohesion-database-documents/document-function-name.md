# DocumentFunctionName

A generated case-insensitive unquoted function identifier.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public partial struct DocumentFunctionName
{
    public static partial bool IsValid(string value, out string? message);
    // Generated string conversion and value equality members.
}
```

## Behavior and exceptions

Names compare ordinally ignoring case and convert implicitly from string. They start with a
letter or underscore, followed by letters, digits or underscores.
IsValid returns false with a diagnostic message for invalid syntax. A syntactically valid name
can still be reserved by the builder. Default names are refused at function construction and
catalog/collection lookup boundaries with ArgumentException.

## Example

```csharp
DocumentFunctionName name = "developer_echo";
Console.WriteLine(name == (DocumentFunctionName)"DEVELOPER_ECHO"); // True
Console.WriteLine(DocumentFunctionName.IsValid("bad-name", out _)); // False
```

## Related reference

[Assembly](index.md) · [Registration](document-function-collection.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/ValueObjects/DocumentFunctionName.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
