# Example: Configuration Json Provider Tests

Exercise Configuration Json Provider behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ConfigurationJsonProviderTests.cs` listing from the package test project. Keep
it in that project when running it: the project supplies its package references, generated sources,
and any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.IO;
using System.Text;
using System;
using Xunit;

namespace Assimalign.Cohesion.Configuration.Json.Tests;

using Assimalign.Cohesion.Configuration;
using Assimalign.Cohesion.Configuration.Json;
using Assimalign.Cohesion.FileSystem;

public class ConfigurationJsonProviderTests
{
    [Fact(DisplayName = "Cohesion Test [Configuration.Json] - Builder: Json file loads nested keys")]
    public void Builder_AddJsonFile_ShouldLoadNestedKeys()
    {
        using IFileSystem fileSystem = CreateFileSystem();
        WriteFile(fileSystem, "settings.json", """
            {
              "Logging": {
                "Level": "Debug",
                "Enabled": true
              },
              "Servers": [
                "one",
                "two"
              ]
            }
            """);

        Configuration configuration = (Configuration)new ConfigurationBuilder()
            .AddJsonFile(fileSystem, "settings.json")
            .Build();

        try
        {
            Assert.Equal("Debug", configuration["Logging:Level"]);
            Assert.Equal(bool.TrueString, configuration["Logging:Enabled"]);
            Assert.Equal("one", configuration["Servers:0"]);
            Assert.Equal("two", configuration["Servers:1"]);
        }
        finally
        {
            configuration.Dispose();
        }
    }

    [Fact(DisplayName = "Cohesion Test [Configuration.Json] - Builder: Json stream loads values")]
    public void Builder_AddJsonStream_ShouldLoadValues()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("""
            {
              "Feature": {
                "Mode": "Live"
              }
            }
            """));

        Configuration configuration = (Configuration)new ConfigurationBuilder()
            .AddJsonStream(stream, leaveOpen: true)
            .Build();

        try
        {
            Assert.Equal("Live", configuration["Feature:Mode"]);
        }
        finally
        {
            configuration.Dispose();
        }
    }

    [Fact(DisplayName = "Cohesion Test [Configuration.Json] - Builder: Optional missing file is ignored")]
    public void Builder_AddJsonFile_OptionalMissingFile_ShouldBeIgnored()
    {
        using IFileSystem fileSystem = CreateFileSystem();

        Configuration configuration = (Configuration)new ConfigurationBuilder()
            .AddJsonFile(fileSystem, "missing.json", optional: true)
            .Build();

        try
        {
            Assert.Null(configuration["Feature:Mode"]);
        }
        finally
        {
            configuration.Dispose();
        }
    }

    private static IFileSystem CreateFileSystem()
    {
        return new InMemoryFileSystem(new InMemoryFileSystemOptions
        {
            RootPath = "/",
            Size = Size.FromMegabytes(8),
        });
    }

    private static void WriteFile(IFileSystem fileSystem, FileSystemPath path, string content)
    {
        if (!fileSystem.Exists(path))
        {
            fileSystem.CreateFile(path);
        }

        IFileSystemFile file = fileSystem.GetFile(path);

        using Stream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        stream.SetLength(0);

        using var writer = new StreamWriter(stream);
        writer.Write(content);
    }
}
```

## Walkthrough

- **Covered behavior** — Builder: Json file loads nested keys.
- **Covered behavior** — Builder: Json stream loads values.
- **Covered behavior** — Builder: Optional missing file is ignored.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/tests/ConfigurationJsonProviderTests.cs`.
- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/tests/Assimalign.Cohesion.Configuration.Json.Tests.csproj`.
