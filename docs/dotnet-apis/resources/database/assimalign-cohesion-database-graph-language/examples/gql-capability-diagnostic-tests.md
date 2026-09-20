# Gql Capability Diagnostic Tests

This example exercises `Assimalign.Cohesion.Database.Graph.Language` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCapabilityDiagnosticTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Collection literals preserve the original capability diagnostic.

## Source example

```csharp
using System.Linq;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Graph.Language.Tests;

/// <summary>Protects capability diagnostics from cascading syntax errors in unsupported literal forms.</summary>
public sealed class GqlCapabilityDiagnosticTests
{
    /// <summary>Unsupported collection literals retain the capability diagnostic consumed by the engine.</summary>
    [Theory(DisplayName = "Cohesion Test [GQL] - Collection literals preserve the original capability diagnostic")]
    [InlineData("INSERT (:Person {tags: [1, 2]})")]
    [InlineData("INSERT (:Person {child: {name: 'Alice'}})")]
    [InlineData("INSERT (:Person)-[:KNOWS {tags: [1, 2]}]->(:Person)")]
    public void Parse_UnsupportedCollectionLiteral_DoesNotMaskCapabilityDiagnostic(string source)
    {
        var statement = new GqlQueryParser().Parse(source);

        statement.Diagnostics.Select(diagnostic => diagnostic.Code).ShouldBe(["COHDBL001"]);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCapabilityDiagnosticTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/Assimalign.Cohesion.Database.Graph.Language.Tests.csproj`.
