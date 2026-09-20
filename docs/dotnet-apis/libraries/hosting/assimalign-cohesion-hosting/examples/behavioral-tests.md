# Example: Host Environment Tests

Exercise Host Environment behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HostEnvironmentTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Hosting.Tests;

public sealed class HostEnvironmentTests
{
    [Theory(DisplayName = "Cohesion Test [Hosting] - Environment: Local matching is ordinal case-insensitive")]
    [InlineData(AppEnvironment.Keys.Local)]
    [InlineData("local")]
    [InlineData("LoCaL")]
    public void IsLocal_MatchingName_ShouldSelectDeveloperMachine(string name)
    {
        IHostEnvironment environment = new HostEnvironment(name);

        environment.IsLocal().ShouldBeTrue();
        environment.IsDevelopment().ShouldBeFalse();
    }

    [Theory(DisplayName = "Cohesion Test [Hosting] - Environment: Deployed names remain distinct from Local")]
    [InlineData(AppEnvironment.Keys.Development)]
    [InlineData("development")]
    [InlineData(AppEnvironment.Keys.Staging)]
    [InlineData("staging")]
    [InlineData(AppEnvironment.Keys.Production)]
    [InlineData("production")]
    public void IsLocal_DeployedName_ShouldRemainFalse(string name)
    {
        IHostEnvironment environment = new HostEnvironment(name);

        environment.IsLocal().ShouldBeFalse();
        (environment.IsDevelopment() || environment.IsStaging() || environment.IsProduction()).ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Hosting] - Environment: Named predicates share case-insensitive matching")]
    public void IsEnvironment_NamedPredicates_ShouldIgnoreCase()
    {
        new HostEnvironment("TEST").IsTest().ShouldBeTrue();
        new HostEnvironment("UAT").IsUserAcceptanceTesting().ShouldBeTrue();
        new HostEnvironment("QA").IsQualityAssurance().ShouldBeTrue();
        new HostEnvironment("custom").IsEnvironment("CUSTOM").ShouldBeTrue();
        new HostEnvironment("custom").IsEnvironment("other").ShouldBeFalse();
    }
}
```

## Walkthrough

- **Covered behavior** — Environment: Local matching is ordinal case-insensitive.
- **Covered behavior** — Environment: Deployed names remain distinct from Local.
- **Covered behavior** — Environment: Named predicates share case-insensitive matching.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/tests/HostEnvironmentTests.cs`.
- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/tests/Assimalign.Cohesion.Hosting.Tests.csproj`.
