# Example: Logger Factory Covariant Return Tests

Exercise Logger Factory Covariant Return behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `LoggerFactoryCovariantReturnTests.cs` listing from the package test project.
Keep it in that project when running it: the project supplies its package references, generated
sources, and any shared fixtures. The using block below makes the test-framework import explicit
where the original project supplies it globally.

## Code

```csharp
using Xunit;
using Assimalign.Cohesion.Logging;

namespace Assimalign.Cohesion.Logging.Tests;

public class LoggerFactoryCovariantReturnTests
{
    [Fact(DisplayName = "Cohesion Test [Logging] - LoggerFactory: Create returns Logger covariantly")]
    public void Create_ReturnsLogger()
    {
        var concreteFactory = (LoggerFactory)new LoggerFactoryBuilder()
            .AddProvider(new RecordingProvider())
            .Build();

        // Strongly typed call returns Logger.
        Logger typed = concreteFactory.Create("Cat");

        Assert.NotNull(typed);
        Assert.Equal("Cat", typed.Category);

        // Calling through ILoggerFactory returns ILogger but is the same instance.
        ILoggerFactory asInterface = concreteFactory;
        ILogger interfaceLogger = asInterface.Create("Cat");
        Assert.Same(typed, interfaceLogger);

        concreteFactory.Dispose();
    }
}
```

## Walkthrough

- **Covered behavior** — LoggerFactory: Create returns Logger covariantly.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/tests/LoggerFactoryCovariantReturnTests.cs`.
- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/tests/Assimalign.Cohesion.Logging.Tests.csproj`.
