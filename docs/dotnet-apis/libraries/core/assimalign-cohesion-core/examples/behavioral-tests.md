# Example: Tests File Name

Exercise Tests File Name behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `Tests.FileName.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures. The using block below makes the test-framework import explicit where the original
project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Xunit;

namespace System.IO.Tests;

public class FileNameTests
{
    [Theory]
    [InlineData("test.txt", "///test.txt")]
    public void FormatTest(string expected, string value)
    {
        FileName name = value;

        Assert.Equal(expected, name);
    }

    [Theory]
    [InlineData("test/")]
    [InlineData("test?t")]
    public void BadFormatTest(string value)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            FileName name = value;
        });
    }
}
```

## Walkthrough

- **Test entry point** — `FormatTest` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `BadFormatTest` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/tests/System/IO/Tests.FileName.cs`.
- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/tests/Assimalign.Cohesion.Core.Tests.csproj`.
