# Example: Text Line Reader Tests

Exercise Text Line Reader behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `TextLineReaderTests.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures. The using block below makes the test-framework import explicit where the original
project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.IO;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Content.Text.Tests;

public class TextLineReaderTests
{
    [Fact(DisplayName = "Cohesion Test [Content.Text] - Lines: mixed terminators are recognized and reported")]
    public void TryReadLine_MixedTerminators_ReportsEndings()
    {
        var lines = ReadAll("one\ntwo\r\nthree\rfour");

        lines.Count.ShouldBe(4);
        lines[0].Text.ShouldBe("one");
        lines[0].Ending.ShouldBe(TextLineEnding.LineFeed);
        lines[1].Text.ShouldBe("two");
        lines[1].Ending.ShouldBe(TextLineEnding.CarriageReturnLineFeed);
        lines[2].Text.ShouldBe("three");
        lines[2].Ending.ShouldBe(TextLineEnding.CarriageReturn);
        lines[3].Text.ShouldBe("four");
        lines[3].Ending.ShouldBe(TextLineEnding.None);
    }

    [Fact(DisplayName = "Cohesion Test [Content.Text] - Lines: line numbers are one-based and sequential")]
    public void TryReadLine_LineNumbers_AreSequential()
    {
        var lines = ReadAll("a\nb\nc");

        lines[0].Number.ShouldBe(1);
        lines[1].Number.ShouldBe(2);
        lines[2].Number.ShouldBe(3);
    }

    [Fact(DisplayName = "Cohesion Test [Content.Text] - Lines: empty lines are preserved")]
    public void TryReadLine_EmptyLines_ArePreserved()
    {
        var lines = ReadAll("a\n\nb");

        lines.Count.ShouldBe(3);
        lines[1].Text.ShouldBe(string.Empty);
        lines[1].Ending.ShouldBe(TextLineEnding.LineFeed);
    }

    [Fact(DisplayName = "Cohesion Test [Content.Text] - Lines: empty input yields no lines")]
    public void TryReadLine_EmptyInput_YieldsNoLines()
    {
        ReadAll(string.Empty).ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Content.Text] - Lines: a trailing terminator does not create a phantom line")]
    public void TryReadLine_TrailingTerminator_NoPhantomLine()
    {
        var lines = ReadAll("a\n");

        lines.Count.ShouldBe(1);
        lines[0].Text.ShouldBe("a");
    }

    private static List<TextLine> ReadAll(string text)
    {
        using var reader = new TextLineReader(new StringReader(text));
        var lines = new List<TextLine>();
        while (reader.TryReadLine(out var line))
        {
            lines.Add(line);
        }

        return lines;
    }
}
```

## Walkthrough

- **Covered behavior** — Lines: mixed terminators are recognized and reported.
- **Covered behavior** — Lines: line numbers are one-based and sequential.
- **Covered behavior** — Lines: empty lines are preserved.
- **Covered behavior** — Lines: empty input yields no lines.
- **Covered behavior** — Lines: a trailing terminator does not create a phantom line.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/tests/TextLineReaderTests.cs`.
- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/tests/Assimalign.Cohesion.Content.Text.Tests.csproj`.
