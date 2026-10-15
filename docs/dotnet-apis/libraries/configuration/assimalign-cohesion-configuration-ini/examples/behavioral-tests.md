# Example: Ini Compatibility Tests

Exercise Ini Compatibility behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `IniCompatibilityTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Assimalign.Cohesion.Configuration.Ini.Tests;

using Assimalign.Cohesion.Configuration;

/// <summary>
/// Golden-corpus compatibility tests. Each <see cref="MemberDataAttribute"/> row pairs
/// a supported INI fragment with the exact <see cref="Path"/>/value entries the parser
/// is contractually required to produce. Changes to parser behavior must be intentional
/// and reflected here so regressions are obvious.
/// </summary>
/// <remarks>
/// These tests are the compliance suite for the documented Cohesion INI grammar (see
/// <c>docs/DESIGN.md</c>). INI has no single authoritative external spec; the corpus
/// is the spec.
/// </remarks>
public class IniCompatibilityTests
{
    public static IEnumerable<object[]> SupportedFragments => new[]
    {
        // ---- root keys ----
        new object[]
        {
            "Mode = Live",
            new (string[] Path, string Value)[]
            {
                (new[] { "Mode" }, "Live"),
            },
        },

        // ---- single section with multiple keys ----
        new object[]
        {
            """
            [Database]
            Host = db.internal
            Port = 5432
            """,
            new (string[] Path, string Value)[]
            {
                (new[] { "Database", "Host" }, "db.internal"),
                (new[] { "Database", "Port" }, "5432"),
            },
        },

        // ---- nested section via colon ----
        new object[]
        {
            """
            [Logging:Console]
            Level = Debug
            IncludeScopes = true
            """,
            new (string[] Path, string Value)[]
            {
                (new[] { "Logging", "Console", "Level" }, "Debug"),
                (new[] { "Logging", "Console", "IncludeScopes" }, "true"),
            },
        },

        // ---- comments and blank lines ----
        new object[]
        {
            """
            ; this is a comment
            # this too

            [Cache]
            ; keys begin here
            Mode = Memory

            # trailing comment
            """,
            new (string[] Path, string Value)[]
            {
                (new[] { "Cache", "Mode" }, "Memory"),
            },
        },

        // ---- duplicate keys, last wins ----
        new object[]
        {
            """
            [Cache]
            Mode = Memory
            Mode = Distributed
            """,
            new (string[] Path, string Value)[]
            {
                (new[] { "Cache", "Mode" }, "Distributed"),
            },
        },

        // ---- quoted values: literal text ----
        new object[]
        {
            """
            Banner = "Welcome to Cohesion"
            """,
            new (string[] Path, string Value)[]
            {
                (new[] { "Banner" }, "Welcome to Cohesion"),
            },
        },

        // ---- equals signs in value preserved ----
        new object[]
        {
            "ConnectionString = Server=db;User=app;Password=p@ss=word",
            new (string[] Path, string Value)[]
            {
                (new[] { "ConnectionString" }, "Server=db;User=app;Password=p@ss=word"),
            },
        },

        // ---- root keys interleaved with sections ----
        new object[]
        {
            """
            ApplicationName = Cohesion

            [Server]
            Host = localhost

            [Client]
            Timeout = 00:00:30
            """,
            new (string[] Path, string Value)[]
            {
                (new[] { "ApplicationName" }, "Cohesion"),
                (new[] { "Server", "Host" }, "localhost"),
                (new[] { "Client", "Timeout" }, "00:00:30"),
            },
        },
    };

    [Theory]
    [MemberData(nameof(SupportedFragments))]
    [Trait("Category", "Compatibility")]
    public async Task SupportedFragment_ProducesExpectedEntries(
        string content,
        (string[] Path, string Value)[] expected)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var configuration = (Configuration)new ConfigurationBuilder()
            .AddIniStream(stream, leaveOpen: true)
            .Build();

        try
        {
            foreach ((string[] segments, string value) in expected)
            {
                string key = string.Join(":", segments);
                Assert.Equal(value, configuration[key]);
            }
        }
        finally
        {
            configuration.Dispose();
        }
    }
}
```

## Walkthrough

- **Test entry point** — `SupportedFragment_ProducesExpectedEntries` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/tests/IniCompatibilityTests.cs`.
- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/tests/Assimalign.Cohesion.Configuration.Ini.Tests.csproj`.
