# Query Language Profile Tests

This example exercises `Assimalign.Cohesion.Database.Language` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Language/tests/QueryLanguageProfileTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Supports_WhenProfileIsCaseInsensitive_IgnoresClauseCase.
- **Case 2** — Supports_WhenProfileIsCaseSensitive_RequiresClauseCase.
- **Case 3** — ToLexerOptions_RoundTripsLexicalTablesAndCasePolicy.
- **Case 4** — Supports_WhenClauseSetIsEmpty_SupportsNothing.

## Source example

```csharp
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Language.Tests;

public sealed class QueryLanguageProfileTests
{
    [Fact]
    public void Supports_WhenProfileIsCaseInsensitive_IgnoresClauseCase()
    {
        var profile = new QueryLanguageProfile("TEST", [], [], ["GROUP BY"]);

        profile.Supports("group by").ShouldBeTrue();
    }

    [Fact]
    public void Supports_WhenProfileIsCaseSensitive_RequiresClauseCase()
    {
        var profile = new QueryLanguageProfile("TEST", [], [], ["GROUP BY"], isCaseSensitive: true);

        profile.Supports("GROUP BY").ShouldBeTrue();
        profile.Supports("group by").ShouldBeFalse();
    }

    [Fact]
    public void ToLexerOptions_RoundTripsLexicalTablesAndCasePolicy()
    {
        string[] keywords = ["SELECT", "FROM"];
        string[] functions = ["COUNT", "SUM"];
        var profile = new QueryLanguageProfile(
            "TEST",
            keywords,
            functions,
            ["SELECT"],
            isCaseSensitive: true);

        var options = profile.ToLexerOptions();

        options.Keywords.ToArray().ShouldBe(keywords);
        options.Functions.ToArray().ShouldBe(functions);
        options.IsCaseSensitive.ShouldBeTrue();
    }

    [Fact]
    public void Supports_WhenClauseSetIsEmpty_SupportsNothing()
    {
        var profile = new QueryLanguageProfile("TEST", [], [], []);

        profile.Clauses.ShouldBeEmpty();
        profile.Supports("SELECT").ShouldBeFalse();
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/tests/QueryLanguageProfileTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/tests/Assimalign.Cohesion.Database.Language.Tests.csproj`.
