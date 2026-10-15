# Example: Configuration Command Line Provider Tests

Exercise Configuration Command Line Provider behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ConfigurationCommandLineProviderTests.cs` listing from the package test
project. Keep it in that project when running it: the project supplies its package references,
generated sources, and any shared fixtures. The using block below makes the test-framework import
explicit where the original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System;
using Xunit;
using Assimalign.Cohesion.Configuration.CommandLine;
using Assimalign.Cohesion.Configuration;

namespace Assimalign.Cohesion.Configuration.CommandLine.Tests;

public class ConfigurationCommandLineProviderTests
{
    [Fact(DisplayName = "Cohesion Test [Command Line] - Builder: AddCommandLine supports basic argument forms")]
    public void CommandLine_AddCommandLine_ShouldSupportBasicArgumentForms()
    {
        string[] args =
        [
            "PlainKey=PlainValue",
            "--Section:Enabled=true",
            "/Logging:Level=Debug",
            "--Features:Cache", "enabled",
            "/Runtime:Mode", "interactive"
        ];

        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        Assert.Equal("PlainValue", configuration["PlainKey"]);
        Assert.Equal("true", configuration["Section:Enabled"]);
        Assert.Equal("Debug", configuration["Logging:Level"]);
        Assert.Equal("enabled", configuration["Features:Cache"]);
        Assert.Equal("interactive", configuration["Runtime:Mode"]);
    }

    [Fact(DisplayName = "Cohesion Test [Command Line] - Builder: Duplicate keys use the last value")]
    public void CommandLine_AddCommandLine_ShouldLetLastValueWin()
    {
        string[] args =
        [
            "--Mode=first",
            "--Mode=second"
        ];

        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        Assert.Equal("second", configuration["Mode"]);
    }

    [Fact(DisplayName = "Cohesion Test [Command Line] - Builder: Switch mappings support short and aliased switches")]
    public void CommandLine_AddCommandLine_ShouldApplySwitchMappings()
    {
        string[] args =
        [
            "-p", "8080",
            "--env=Development",
            "/name", "Cohesion"
        ];

        var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["-p"] = "Server:Port",
            ["--env"] = "Environment",
            ["--name"] = "Application:Name"
        };

        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args, mappings)
            .Build();

        Assert.Equal("8080", configuration["Server:Port"]);
        Assert.Equal("Development", configuration["Environment"]);
        Assert.Equal("Cohesion", configuration["Application:Name"]);
    }

    [Fact(DisplayName = "Cohesion Test [Command Line] - Builder: Undefined short switch with equals throws")]
    public void CommandLine_AddCommandLine_WithUndefinedShortEqualsSwitch_ShouldThrow()
    {
        string[] args = ["-p=8080"];

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build());

        Assert.IsType<FormatException>(exception.InnerException);
    }

    [Fact(DisplayName = "Cohesion Test [Command Line] - Builder: Configure callback uses options values")]
    public void CommandLine_AddCommandLine_WithConfigureOptions_ShouldUseOptions()
    {
        string[] args = ["-p", "8080"];

        var configuration = new ConfigurationBuilder()
            .AddCommandLine(options =>
            {
                options.Args = args;
                options.SwitchMappings = new Dictionary<string, string>
                {
                    ["-p"] = "Server:Port"
                };
            })
            .Build();

        Assert.Equal("8080", configuration["Server:Port"]);
    }

    [Fact(DisplayName = "Cohesion Test [Command Line] - Provider: Invalid switch mapping is rejected")]
    public void CommandLine_Provider_WithInvalidSwitchMapping_ShouldThrow()
    {
        var switchMappings = new Dictionary<string, string>
        {
            ["port"] = "Server:Port"
        };

        Assert.Throws<ArgumentException>(() =>
            new ConfigurationCommandLineProvider(["--Server:Port=8080"], switchMappings));
    }
}
```

## Walkthrough

- **Covered behavior** — Builder: AddCommandLine supports basic argument forms.
- **Covered behavior** — Builder: Duplicate keys use the last value.
- **Covered behavior** — Builder: Switch mappings support short and aliased switches.
- **Covered behavior** — Builder: Undefined short switch with equals throws.
- **Covered behavior** — Builder: Configure callback uses options values.
- **Covered behavior** — Provider: Invalid switch mapping is rejected.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/tests/ConfigurationCommandLineProviderTests.cs`.
- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/tests/Assimalign.Cohesion.Configuration.CommandLine.Tests.csproj`.
