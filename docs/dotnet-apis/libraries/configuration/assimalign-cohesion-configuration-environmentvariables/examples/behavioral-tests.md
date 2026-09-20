# Example: Configuration Environment Variables Provider Tests

Exercise Configuration Environment Variables Provider behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ConfigurationEnvironmentVariablesProviderTests.cs` listing from the package
test project. Keep it in that project when running it: the project supplies its package references,
generated sources, and any shared fixtures. The using block below makes the test-framework import
explicit where the original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System;
using Xunit;

namespace Assimalign.Cohesion.Configuration.Tests;

public class ConfigurationEnvironmentVariablesProviderTests
{
    [Fact(DisplayName = "Cohesion Test [Environment Variables] - Builder: AddEnvironmentVariables loads prefixed values")]
    public void EnvironmentVariables_AddEnvironmentVariables_ShouldLoadPrefixedValues()
    {
        string prefix = $"COHESION_ENV_{CreateToken()}_";

        using var variables = new EnvironmentVariablesScope(
            ($"{prefix}Logging__Level", "Debug"),
            ($"{prefix}Features__UseCache", "true"));

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix)
            .Build();

        Assert.Equal("Debug", configuration["Logging:Level"]);
        Assert.Equal("true", configuration["Features:UseCache"]);
    }

    [Fact(DisplayName = "Cohesion Test [Environment Variables] - Builder: Prefix is trimmed before key normalization")]
    public void EnvironmentVariables_AddEnvironmentVariables_ShouldTrimPrefixBeforeNormalizingKey()
    {
        string prefix = $"COHESION__{CreateToken()}__";

        using var variables = new EnvironmentVariablesScope(
            ($"{prefix}Nested__Value", "42"));

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix)
            .Build();

        Assert.Equal("42", configuration["Nested:Value"]);
    }

    [Fact(DisplayName = "Cohesion Test [Environment Variables] - Builder: Configure callback uses options prefix")]
    public void EnvironmentVariables_AddEnvironmentVariables_WithConfigureOptions_ShouldUsePrefix()
    {
        string prefix = $"COHESION_ENV_{CreateToken()}_";

        using var variables = new EnvironmentVariablesScope(
            ($"{prefix}Service__Url", "https://example.test"));

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(options => options.Prefix = prefix)
            .Build();

        Assert.Equal("https://example.test", configuration["Service:Url"]);
    }

    [Fact(DisplayName = "Cohesion Test [Environment Variables] - Provider: Connection string prefixes are mapped")]
    public void EnvironmentVariables_Provider_ShouldMapConnectionStringPrefixes()
    {
        string token = CreateToken();

        using var variables = new EnvironmentVariablesScope(
            ($"MYSQLCONNSTR_{token}", "Server=db;Uid=user;Pwd=pass;"));

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        Assert.Equal("Server=db;Uid=user;Pwd=pass;", configuration[$"ConnectionStrings:{token}"]);
        Assert.Equal("MySql.Data.MySqlClient", configuration[$"ConnectionStrings:{token}_ProviderName"]);
    }

    private static string CreateToken()
    {
        return Guid.NewGuid().ToString("N").ToUpperInvariant();
    }

    private sealed class EnvironmentVariablesScope : IDisposable
    {
        private readonly List<(string Key, string? PreviousValue)> _values;

        public EnvironmentVariablesScope(params (string Key, string Value)[] values)
        {
            _values = [];

            foreach ((string key, string value) in values)
            {
                _values.Add((key, Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process)));
                Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
            }
        }

        public void Dispose()
        {
            foreach ((string key, string? previousValue) in _values)
            {
                Environment.SetEnvironmentVariable(key, previousValue, EnvironmentVariableTarget.Process);
            }
        }
    }
}
```

## Walkthrough

- **Covered behavior** — Builder: AddEnvironmentVariables loads prefixed values.
- **Covered behavior** — Builder: Prefix is trimmed before key normalization.
- **Covered behavior** — Builder: Configure callback uses options prefix.
- **Covered behavior** — Provider: Connection string prefixes are mapped.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/tests/ConfigurationEnvironmentVariablesProviderTests.cs`.
- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/tests/Assimalign.Cohesion.Configuration.EnvironmentVariables.Tests.csproj`.
