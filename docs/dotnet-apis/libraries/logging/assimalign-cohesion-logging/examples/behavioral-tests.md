# Example: Logger Factory Behavioral Tests

Exercise Logger Factory covariant returns and forwarding registration using the original repository tests.

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


## Forwarding registration

This is the complete `LoggerFactoryBuilderForwardingTests.cs` listing from the same package test project.

```csharp
using System;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Logging.Tests;

public sealed class LoggerFactoryBuilderForwardingTests
{
    [Fact(DisplayName = "Cohesion Test [Logging] - HasForwarders: Registration is visible without creating the forwarder")]
    public void HasForwarders_WhenRegistered_ShouldReportTheRegistrationWithoutCreatingIt()
    {
        // Arrange
        ILoggerFactoryBuilder builder = new LoggerFactoryBuilder();
        int created = 0;
        builder.HasForwarders.ShouldBeFalse();

        // Act
        builder.AddForwarder(factory =>
        {
            created++;
            return new RecordingForwarder("application", factory);
        });

        // Assert
        builder.HasForwarders.ShouldBeTrue();
        created.ShouldBe(0);
        using ILoggerFactory factory = builder.Build();
        created.ShouldBe(1);
        factory.Forwarders.ShouldHaveSingleItem().Name.ShouldBe("application");
        builder.HasForwarders.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Logging] - HasForwarders: Providers and rules do not count as forwarding")]
    public void HasForwarders_WithOnlyProvidersAndRules_ShouldRemainFalse()
    {
        // Arrange
        ILoggerFactoryBuilder builder = new LoggerFactoryBuilder();

        // Act
        builder.AddProvider(new RecordingProvider());
        builder.AddRule("application", LogLevel.Debug);
        using ILoggerFactory factory = builder.Build();

        // Assert
        builder.HasForwarders.ShouldBeFalse();
        factory.Forwarders.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Logging] - HasForwarders: An existing custom builder inherits a compatible default")]
    public void HasForwarders_WithAnExistingImplementation_ShouldDefaultToFalse()
    {
        // Arrange — implements the pre-probe interface without adding the new member.
        ILoggerFactoryBuilder builder = new ExistingLoggerFactoryBuilder();

        // Act
        bool hasForwarders = builder.HasForwarders;

        // Assert
        hasForwarders.ShouldBeFalse();
    }

    private sealed class ExistingLoggerFactoryBuilder : ILoggerFactoryBuilder
    {
        public ILoggerFactoryBuilder AddProvider(ILoggerProvider provider) => throw new NotSupportedException();

        public ILoggerFactoryBuilder SetMinimumLevel(LogLevel level) => throw new NotSupportedException();

        public ILoggerFactoryBuilder AddRule(LoggerFilterRule rule) => throw new NotSupportedException();

        public ILoggerFactoryBuilder AddRule(string categoryPrefix, LogLevel minimumLevel) => throw new NotSupportedException();

        public ILoggerFactoryBuilder AddEnricher(ILoggerEnricher enricher) => throw new NotSupportedException();

        public ILoggerFactoryBuilder AddForwarder(Func<ILoggerFactory, ILoggerForwarder> create) => throw new NotSupportedException();

        public ILoggerFactory Build() => throw new NotSupportedException();
    }
}
```

### Walkthrough

- **Covered behavior** — HasForwarders: Registration is visible without creating the forwarder.

- **Covered behavior** — HasForwarders: Providers and rules do not count as forwarding.

- **Covered behavior** — HasForwarders: An existing custom builder inherits a compatible default.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/tests/LoggerFactoryCovariantReturnTests.cs`.
- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/tests/Assimalign.Cohesion.Logging.Tests.csproj`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/tests/LoggerFactoryBuilderForwardingTests.cs`.
