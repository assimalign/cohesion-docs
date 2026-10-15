# Example: Glob Matcher Options Tests

Exercise Glob Matcher Options behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `GlobMatcherOptionsTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Globalization;
using System;
using Xunit;

namespace Assimalign.Cohesion.FileSystem.Globbing.Tests;

public class GlobMatcherOptionsTests
{
    [Fact]
    public void Constructor_DefaultIgnoreCase_IsTrue()
    {
        var options = new GlobMatcherOptions();

        Assert.True(options.IgnoreCase);
    }

    [Fact]
    public void Constructor_DefaultCultureInfo_IsInvariantCulture()
    {
        var options = new GlobMatcherOptions();

        Assert.Equal(CultureInfo.InvariantCulture, options.CultureInfo);
    }

    [Fact]
    public void Constructor_DefaultExcludeDirectories_IsFalse()
    {
        var options = new GlobMatcherOptions();

        Assert.False(options.ExcludeDirectories);
    }

    [Fact]
    public void IgnoreCase_CanBeSet()
    {
        var options = new GlobMatcherOptions
        {
            IgnoreCase = false
        };

        Assert.False(options.IgnoreCase);
    }

    [Fact]
    public void CultureInfo_CanBeSet()
    {
        var culture = new CultureInfo("en-US");
        var options = new GlobMatcherOptions
        {
            CultureInfo = culture
        };

        Assert.Equal(culture, options.CultureInfo);
    }

    [Fact]
    public void ExcludeDirectories_CanBeSet()
    {
        var options = new GlobMatcherOptions
        {
            ExcludeDirectories = true
        };

        Assert.True(options.ExcludeDirectories);
    }
}
```

## Walkthrough

- **Test entry point** — `Constructor_DefaultIgnoreCase_IsTrue` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Constructor_DefaultCultureInfo_IsInvariantCulture` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Constructor_DefaultExcludeDirectories_IsFalse` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `IgnoreCase_CanBeSet` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CultureInfo_CanBeSet` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `ExcludeDirectories_CanBeSet` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/tests/GlobMatcherOptionsTests.cs`.
- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/tests/Assimalign.Cohesion.FileSystem.Globbing.Tests.csproj`.
