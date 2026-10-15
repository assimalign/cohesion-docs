# Sql Token Lexer Test

This example exercises `Assimalign.Cohesion.Database.Sql.Language` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlTokenLexerTest.cs`.
It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Lexer: tokenizes keywords, functions, and qualified names.

## Source example

```csharp
using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Database.Language;

namespace Assimalign.Cohesion.Database.Sql.Language.Tests;

/// <summary>
/// Lexer smoke tests over the declared SQL token tables.
/// </summary>
public class SqlTokenLexerTest
{
    [Fact(DisplayName = "Cohesion Test [Sql.Language] - Lexer: tokenizes keywords, functions, and qualified names")]
    public void MoveNext_SimpleQuery_ShouldClassifyTokens()
    {
        // Arrange
        var lexer = new TokenLexer(
            "SELECT Count(*) FROM dbo.Users",
            SqlLanguageProfile.Instance.ToLexerOptions());
        var tokens = new List<(string Value, TokenType Type)>();

        // Act
        while (lexer.MoveNext())
        {
            tokens.Add((lexer.Current.Value.ToString(), lexer.Current.Type));
        }

        // Assert
        tokens.ShouldBe(new[]
        {
            ("SELECT", TokenType.Keyword),
            ("Count", TokenType.Function),
            ("(", TokenType.LeftParen),
            ("*", TokenType.Asterisk),
            (")", TokenType.RightParen),
            ("FROM", TokenType.Keyword),
            ("dbo", TokenType.Identifier),
            (".", TokenType.Dot),
            ("Users", TokenType.Identifier),
        });
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlTokenLexerTest.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/Assimalign.Cohesion.Database.Sql.Language.Tests.csproj`.
