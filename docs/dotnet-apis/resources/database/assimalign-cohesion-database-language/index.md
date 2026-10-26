# Assimalign.Cohesion.Database.Language

The shared query-language substrate for database models.

> **Status:** Partial.

`Diagnostic`, `DiagnosticLocation` and `DiagnosticSeverity` live in `Database.Types`
(#1577, 2026-10-16). Language references Types and produces these shared values.

## Reference pages

- **[QueryLanguageProfile](query-language-profile.md)** — Immutable clause and lexer configuration.

- **[TokenLexerOptions](token-lexer-options.md)** — Vocabulary and opt-in left-arrow recognition.

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The shared parser blocks without busy-spinning for registered analyzers, propagating their faults
and cancellation within one timeout. No analyzers means no analyzer wait or timeout work.
See [signatures, exceptions and example](design.md#analyzer-completion-and-diagnostics).
The following public declarations are taken from the C# source.

| Type | Source responsibility |
|---|---|
| `QueryDiagnostics` | Creates diagnostics shared by model-specific query languages. |
| `Token` | Represents a single lexical token scanned from a query statement. |
| `TokenLexer` | A zero-allocation lexer that tokenizes query statements for SQL, OQL, and GQL. `Supports` iteration via the foreach pattern. var lexer = new `TokenLexer`(sql, options); foreach (var token in lexer) { /* ... */ } |
| `TokenLexerOptions` | Configuration that tells `TokenLexer` which words are keywords or built-in functions for a particular query language (SQL, OQL, GQL). |
| `TokenType` | Classifies lexical tokens produced by `TokenLexer`. Covers operators and punctuation used across SQL, OQL, and GQL. |
| `QueryAnalyzer` | An abstract analyzer for implementing custom analysis on parsed queries. |
| `QueryAnalyzerContext` | Public class declared in `QueryAnalyzerContext.cs`. |
| `QueryLanguageProfile` | The grammar surface one model's query language exposes: its lexical vocabulary and the set of clauses its parser accepts. A clause outside the profile is reported as unsupported by that model rather than failing as a generic syntax error. |
| `QueryParser` | Provides the common lexer, parsing, and analyzer flow for model-specific query parsers. |
| `QueryParserOptions` | Public class declared in `QueryParserOptions.cs`. |
| `Location` | Public class declared in `Location.cs`. |
| `QueryExpression` | Base for model-specific expressions with optional source text and location. |
| `QueryStatement` | Public class declared in `QueryStatement.cs`. |

[Parent: Database](../index.md)

## Analyzer context

[QueryAnalyzerContext](query-analyzer-context.md) is a sealed parser-owned context.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Assimalign.Cohesion.Database.Language.csproj`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Types/src/Diagnostics/Diagnostic.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Types/src/Diagnostics/DiagnosticLocation.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Types/src/Diagnostics/DiagnosticSeverity.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Diagnostics/QueryDiagnostics.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Lexer/Token.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Lexer/TokenLexer.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Lexer/TokenLexerOptions.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Lexer/TokenType.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/QueryAnalyzer.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/QueryAnalyzerContext.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/QueryLanguageProfile.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/QueryParser.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/QueryParserOptions.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/SyntaxTree/Location.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/SyntaxTree/QueryExpression.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/SyntaxTree/QueryStatement.cs`.
