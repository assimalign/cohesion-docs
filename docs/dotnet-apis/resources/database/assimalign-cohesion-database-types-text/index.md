# Assimalign.Cohesion.Database.Types.Text

Deterministic `simple` and `english` analyzers and immutable tsvector-style values.

> **Status:** Implemented analysis/value foundation. No index or query uses it yet.

Namespace and package: `Assimalign.Cohesion.Database.Types.Text`. Assembly:
`Assimalign.Cohesion.Database.Types.Text.dll`. This independently consumable child root depends
only on Database.Types. The Database root aggregates it; it is a public reference and runtime
member of App.Database. It provides no query language, ranking, postings, text renderer or wire format.

## Reference pages

- **[TextAnalyzer](text-analyzer.md)** — Built-in analysis and exact descriptor resolution.
- **[TextAnalyzerDescriptor](text-analyzer-descriptor.md)** — Name, version and accent-folding identity.
- **[TextVector](text-vector.md)** — Sorted immutable lexemes and payload limits.
- **[TextLexeme](text-lexeme.md)** — Explicit lexeme and weighted positions.
- **[TextPosition](text-position.md)** — Positive clamped position.
- **[TextWeight](text-weight.md)** — D, C, B and A weights.
- **[TextException](text-exception.md)** — Independent child-root failure.
- **[TextErrorCode](text-error-code.md)** — Text conditions outside model diagnostic codes.
- **[Design](design.md)** — Tokenizer, PostgreSQL departures, limits and notices.

## Usage

```csharp
using Assimalign.Cohesion.Database.Types.Text;

var analyzer = TextAnalyzer.Create(TextAnalyzerDescriptor.Create("english", foldAccents: true));
var vector = analyzer.Analyze("The foxes jumped");
Console.WriteLine(vector.Lexemes[0].Text); // fox
Console.WriteLine(vector.Lexemes[0].Positions[0].Position); // 2
```

Persist all three descriptor fields with analyzed data. Changed tokenizer, Unicode folding,
stop-list or stemmer output requires a new analyzer version and refusal or rebuilding by a future
consumer. Resolution never silently upgrades an identity.

[Parent: Database](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/tests/TextAnalyzerTests.cs`.
