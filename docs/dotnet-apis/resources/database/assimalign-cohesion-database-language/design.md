# Assimalign.Cohesion.Database.Language design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Language`.

> **Status:** Partial.

The shared query-language substrate for database models. It owns model-neutral lexical tokens,
syntax-tree primitives, diagnostic production, and parser orchestration; SQL, OQL, GQL, and future model
languages own their vocabulary and grammar.

`Diagnostic`, `DiagnosticSeverity` and `DiagnosticLocation` live in `Database.Types`,
unchanged after #1577 (2026-10-16). Language references Types; Execution needs no language
dependency to expose diagnostics.

## Design intent

The lexer retains `"a""b"` as one QuotedIdentifier token with its raw text and source offsets.
A doubled delimiter is part of the name; a lone delimiter closes it. SQL, OQL and GQL decode the
identifier value to `a"b` and reject an unterminated token at their parser boundary, including a
token ending in an escaped quote. Single-quoted strings use their separate lexical rule.

Each model declares one `QueryLanguageProfile`. The profile is the complete boundary between the
shared parser machinery and a model language: it supplies the lexer's keyword and function tables,
names the language used in diagnostics, and lists the clauses that model's parser accepts. A parser
therefore cannot silently inherit a flat, database-wide grammar.

## Why-this-not-that decisions

- **Language-specific clause names, not a shared enum.** SQL `JOIN`, OQL `FLATTEN`, and GQL `MATCH`
  are different vocabularies. Model packages publish constants for their own clause names, while
  the shared profile compares strings using the same case policy as its lexical tables.
- **One parser declaration, not parallel options.** `QueryParser.Profile` replaces the former
  lexer-options member. Lexer construction projects that profile through `ToLexerOptions()`, so
  lexical behavior and parser capabilities cannot be configured independently by accident.
- **Defensive profile snapshots.** Constructor arrays are copied and clauses are exposed through a
  read-only view. Registration code can safely reuse its source arrays without mutating a running
  parser's grammar surface.
- **One unsupported-clause diagnostic.** `QueryDiagnostics.UnsupportedClause` emits stable code
  `COHDBL001` and a shared message naming the rejected clause and model language. Conformance
  corpora can distinguish an intentionally unsupported capability from malformed syntax.
- **Parser-level gating.** Model parsers call `RequireClause` at the recognized clause boundary.
  The helper records the shared diagnostic and returns whether parsing may continue; the shared
  package does not attempt to understand any model's grammar.

## Analyzer completion and diagnostics

The changed parser and diagnostic members retain these signatures:

```csharp
// QueryParser
public virtual QueryStatement Parse(ReadOnlySpan<char> query);
// QueryAnalyzer
public abstract Task AnalyzeAsync(QueryAnalyzerContext context,
    CancellationToken cancellationToken = default);
// QueryParserOptions
public List<QueryAnalyzer> Analyzers { get; }
public TimeSpan AnalyzerTimeout { get; set; } // default: five seconds
// QueryStatement
public virtual void AddDiagnostic(Diagnostic diagnostic);
public IEnumerable<Diagnostic> Diagnostics { get; }
```

Configure Analyzers before parsing. Parse invokes them in registration order and blocks without
busy-spinning until all tasks complete, using one shared timeout. A timeout ends the wait with
OperationCanceledException even if an analyzer ignores cancellation; a later task fault is observed.
Analyzer faults propagate to the caller. Analyzers must honor cancellation and avoid depending on
the caller's synchronization context. With no analyzers, Parse ignores AnalyzerTimeout, even zero,
and creates no analyzer context, cancellation source, timer or task list.

AddDiagnostic supports concurrent analyzer reporting and throws ArgumentNullException for null
without changing the collection. Enumeration orders by severity and then source end position.
Location.Create retains the model parser's supplied positions without normalization.

This adapts the asynchronous completion case in QueryAnalyzerTests:

```csharp
using System.Threading;
using System.Threading.Tasks;
using Assimalign.Cohesion.Database.Language;
using Assimalign.Cohesion.Database.Types;

sealed class ExampleAnalyzer : QueryAnalyzer
{
    public override async Task AnalyzeAsync(QueryAnalyzerContext context,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken).ConfigureAwait(false);
        context.Statement.AddDiagnostic(new Diagnostic("TEST", "analyzed", 1, 2,
            DiagnosticSeverity.Warning));
    }
}
```

Register it with `options.Analyzers.Add(new ExampleAnalyzer())` before constructing the model
parser with those QueryParserOptions. When Parse returns, the statement includes the warning;
faulted/canceled analysis throws instead. The example uses Language and Types namespaces.

## Non-goals

- **No shared clause enum** — or universal grammar.
- **No parser implementation for** — a particular database model.
- **No reflection-based discovery or** — runtime registration of language profiles.

## AOT posture

Profiles use copied arrays, a comparer-backed set, and direct construction. Lexer projection and
diagnostic creation require no reflection, dynamic code generation, or dependency-injection stack.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Analyzer completion, timeout and null validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/tests/QueryAnalyzerTests.cs` and `QueryAnalyzerTimeoutTests.cs` in the same directory.
- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Assimalign.Cohesion.Database.Language.csproj`.
