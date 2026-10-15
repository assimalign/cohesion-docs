# Assimalign.Cohesion.Database.Language

The shared query-language substrate for database models.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

No project overview is present. The following public declarations are taken from the C# source.

| Type | Source responsibility |
|---|---|
| `Diagnostic` | Public class declared in `Diagnostic.cs`. |
| `DiagnosticLocation` | Public enum declared in `DiagnosticLocation.cs`. |
| `DiagnosticSeverity` | Public enum declared in `DiagnosticSeverity.cs`. |
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
| `QueryExpression` | This represents a base node for all query expressions in the syntax tree. It serves as a common ancestor for various types of query expressions, such as select statements, insert statements, etc. Each specific query expression will inherit from this base class and provide |
| `QueryStatement` | Public class declared in `QueryStatement.cs`. |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Assimalign.Cohesion.Database.Language.csproj`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Diagnostics/Diagnostic.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Diagnostics/DiagnosticLocation.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Diagnostics/DiagnosticSeverity.cs`.
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
