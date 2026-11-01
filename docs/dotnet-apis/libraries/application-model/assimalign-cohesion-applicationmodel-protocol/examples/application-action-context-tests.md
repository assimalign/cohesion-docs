# Application Action Context Tests

This example exercises `Assimalign.Cohesion.ApplicationModel.Protocol` through its co-located test source.

> **Status:** Implemented.

The example reproduces
`cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Protocol/tests/ApplicationActionContextTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
Use it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — An invocation requests exactly one operation.
- **Case 2** — An invocation requires a content serializer.
- **Case 3** — Input is read with the invocation's serializer.
- **Case 4** — Reading absent input is refused content.

## Source example

```csharp
using System;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.ApplicationModel.Protocol.Tests;

public class ApplicationActionContextTests
{
    private const string DisplayPrefix = "Cohesion Test [ApplicationModel] - ApplicationActionContext: ";

    [Theory(DisplayName = DisplayPrefix + "An invocation requests exactly one operation")]
    [InlineData((ApplicationActionOperation)0)]
    [InlineData((ApplicationActionOperation)3)]
    [InlineData((ApplicationActionOperation)4)]
    public void Create_WithoutExactlyOneOperation_ShouldThrow(ApplicationActionOperation operation)
    {
        // Arrange, Act & Assert
        Should.Throw<ArgumentException>(() => ApplicationActionContext.Create(operation, TestSerializer.Json));
    }

    [Fact(DisplayName = DisplayPrefix + "An invocation requires a content serializer")]
    public void Create_WithoutSerializer_ShouldThrow()
    {
        // Arrange, Act & Assert
        Should.Throw<ArgumentNullException>(() => ApplicationActionContext.Create(ApplicationActionOperation.Read, null!));
    }

    [Fact(DisplayName = DisplayPrefix + "Input is read with the invocation's serializer")]
    public void ReadInput_WithMatchingContent_ShouldReturnValue()
    {
        // Arrange
        ApplicationActionContext context = ApplicationActionContext.Create(ApplicationActionOperation.Write, TestSerializer.Json, """{"Level":"Debug"}"""u8.ToArray());

        // Act
        LogLevelSetting? setting = context.ReadInput<LogLevelSetting>();

        // Assert
        context.Serializer.ShouldBeSameAs(TestSerializer.Json);
        setting.ShouldBe(new LogLevelSetting("Debug"));
    }

    [Fact(DisplayName = DisplayPrefix + "Reading absent input is refused content")]
    public void ReadInput_WithNoInput_ShouldThrowResourceContentException()
    {
        // Arrange
        ApplicationActionContext context = ApplicationActionContext.Create(ApplicationActionOperation.Write, TestSerializer.Json);

        // Act & Assert
        Should.Throw<ApplicationContentException>(() => context.ReadInput<LogLevelSetting>());
    }
}
```

[Examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Protocol/tests/ApplicationActionContextTests.cs`.
- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Protocol/tests/Assimalign.Cohesion.ApplicationModel.Protocol.Tests.csproj`.
