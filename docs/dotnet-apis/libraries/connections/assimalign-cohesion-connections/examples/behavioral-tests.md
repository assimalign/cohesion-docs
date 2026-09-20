# Example: Connection Direction Tests

Exercise Connection Direction behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ConnectionDirectionTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.Tests;

public class ConnectionDirectionTests
{
    [Fact]
    public void Values_OnConnectionDirection_ShouldRemainStable()
    {
        // Arrange & Act & Assert
        ((int)ConnectionDirection.Bidirectional).ShouldBe(0);
        ((int)ConnectionDirection.ReadOnly).ShouldBe(1);
        ((int)ConnectionDirection.WriteOnly).ShouldBe(2);
    }
}
```

## Walkthrough

- **Test entry point** — `Values_OnConnectionDirection_ShouldRemainStable` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/ConnectionDirectionTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/Assimalign.Cohesion.Connections.Tests.csproj`.
