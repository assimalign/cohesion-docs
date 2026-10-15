# Example: Debug Logger Provider Tests

Exercise Debug Logger Provider behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `DebugLoggerProviderTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using Xunit;

namespace Assimalign.Cohesion.Logging.Tests;

public class DebugLoggerProviderTests
{
    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: emits to custom writer")]
    public void EmitsToWriter()
    {
        var captured = new List<string>();
        var options = new DebugLoggerOptions
        {
            Writer = captured.Add,
        };
        using var provider = new DebugLoggerProvider(options);

        provider.Create("App").LogInformation("App", "hello",
            attributes: new Dictionary<string, object?> { ["k"] = 1 });

        var line = Assert.Single(captured);
        Assert.Contains("INFO", line);
        Assert.Contains("hello", line);
        Assert.Contains("k=1", line);
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: emits exception when present")]
    public void EmitsException()
    {
        var captured = new List<string>();
        using var provider = new DebugLoggerProvider(new DebugLoggerOptions { Writer = captured.Add });

        provider.Create("App").LogError("App", "uh oh", new InvalidOperationException("boom"));

        Assert.Equal(2, captured.Count);
        Assert.Contains("uh oh", captured[0]);
        Assert.Contains("boom", captured[1]);
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: IncludeException=false suppresses exception line")]
    public void SuppressesException()
    {
        var captured = new List<string>();
        using var provider = new DebugLoggerProvider(new DebugLoggerOptions
        {
            Writer = captured.Add,
            IncludeException = false,
        });

        provider.Create("App").LogError("App", "uh oh", new InvalidOperationException("boom"));

        Assert.Single(captured);
        Assert.DoesNotContain("boom", captured[0]);
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: IncludeAttributes=false suppresses attributes")]
    public void SuppressesAttributes()
    {
        var captured = new List<string>();
        using var provider = new DebugLoggerProvider(new DebugLoggerOptions
        {
            Writer = captured.Add,
            IncludeAttributes = false,
        });

        provider.Create("App").LogInformation("App", "hello",
            attributes: new Dictionary<string, object?> { ["k"] = 1 });

        Assert.DoesNotContain("k=1", captured[0]);
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: no writer + no debugger means disabled")]
    public void NoWriterNoDebugger_Disabled()
    {
        using var provider = new DebugLoggerProvider(new DebugLoggerOptions
        {
            EmitOnlyWhenDebuggerAttached = true,
            // No Writer; in CI no debugger attached -> disabled.
        });

        var logger = provider.Create("App");
        // We cannot reliably assert IsEnabled because the test runner may or may not have a
        // debugger attached. But if it isn't, the provider must return false.
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            Assert.False(logger.IsEnabled(LogLevel.Information));
        }
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: name is Debug")]
    public void Name_IsDebug()
    {
        using var provider = new DebugLoggerProvider();
        Assert.Equal("Debug", provider.Name);
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: throws after dispose")]
    public void DisposedProvider_Throws()
    {
        var provider = new DebugLoggerProvider();
        provider.Dispose();

        Assert.Throws<ObjectDisposedException>(() => provider.Create("App"));
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: scope writes through scoped logger")]
    public void Scope_WritesThroughScope()
    {
        var captured = new List<string>();
        using var provider = new DebugLoggerProvider(new DebugLoggerOptions { Writer = captured.Add });

        var logger = provider.Create("App");
        using var scope = logger.BeginScope(new LoggerEntry(LogLevel.Information, "App", "open"));
        scope.LogInformation("App", "inside scope");

        Assert.Equal(2, captured.Count);
        Assert.Contains("open", captured[0]);
        Assert.Contains("inside scope", captured[1]);
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: rejects null options + empty category")]
    public void RejectsBadArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new DebugLoggerProvider(null!));
        using var provider = new DebugLoggerProvider();
        Assert.Throws<ArgumentException>(() => provider.Create(""));
        Assert.Throws<ArgumentNullException>(() => provider.Create(null!));
    }

    [Fact(DisplayName = "Cohesion Test [Logging.Debug] - Provider: throwing writer is isolated")]
    public void ThrowingWriter_Isolated()
    {
        using var provider = new DebugLoggerProvider(new DebugLoggerOptions
        {
            Writer = _ => throw new InvalidOperationException("bad writer"),
        });

        // Should not throw.
        provider.Create("App").LogInformation("App", "hello");
    }
}
```

## Walkthrough

- **Covered behavior** — Provider: emits to custom writer.
- **Covered behavior** — Provider: emits exception when present.
- **Covered behavior** — Provider: IncludeException=false suppresses exception line.
- **Covered behavior** — Provider: IncludeAttributes=false suppresses attributes.
- **Covered behavior** — Provider: no writer + no debugger means disabled.
- **Covered behavior** — Provider: name is Debug.
- **Covered behavior** — Provider: throws after dispose.
- **Covered behavior** — Provider: scope writes through scoped logger.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/tests/DebugLoggerProviderTests.cs`.
- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/tests/Assimalign.Cohesion.Logging.Debug.Tests.csproj`.
