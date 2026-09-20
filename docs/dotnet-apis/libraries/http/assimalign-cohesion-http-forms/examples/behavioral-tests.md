# Example: Http Form Collection Tests

Exercise Http Form Collection behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpFormCollectionTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.IO;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Http.Tests;

public class HttpFormCollectionTests
{
    [Fact]
    public void Add_ValueAndFile_ShouldBeRetrievableFromCollection()
    {
        // Arrange
        HttpFormCollection form = new();
        HttpFormFile file = new("avatar", "avatar.png", () => new MemoryStream([1, 2, 3]), 3, "image/png");

        // Act
        form.Add("name", "cohesion");
        form.Add(file);
        bool foundValue = form.TryGetValue("name", out HttpQueryValue value);
        bool foundFile = form.Files.TryGetValue("avatar", out HttpFormFile uploadedFile);

        // Assert
        foundValue.ShouldBeTrue();
        value.Value.ShouldBe("cohesion");
        foundFile.ShouldBeTrue();
        uploadedFile.FileName.ShouldBe("avatar.png");
        uploadedFile.ContentType.ShouldBe("image/png");

        using Stream stream = uploadedFile.OpenReadStream();
        stream.Length.ShouldBe(3);
    }
}
```

## Walkthrough

- **Test entry point** — `Add_ValueAndFile_ShouldBeRetrievableFromCollection` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/tests/HttpFormCollectionTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/tests/Assimalign.Cohesion.Http.Forms.Tests.csproj`.
