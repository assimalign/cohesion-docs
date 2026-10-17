# Session capability compilation tests

The former shared query pipeline was removed in #1257. Execution now owns only model-neutral
request/result contracts. The root Database test project compiles consumer snippets against the
built model assemblies to prove the session capability split (#1576, 2026-10-16).

## Behavior exercised

- Sql, Documents and Graph reject foreign requests with CS1503 naming their own request type.
- KeyValuePair accepts typed commands and rejects both foreign requests and public text execution.
- Blob and the plain root session have no execute member; the internal key-value command entry
  is inaccessible to a consumer assembly.
- Supported typed and language calls compile without errors, as do Blob container access and
  root transaction access.

Run this example in the root test project, which references the model assemblies and the
test-only centrally versioned Microsoft.CodeAnalysis.CSharp package. No compiler dependency
is added to a shipped package.

## Source example

```csharp
using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Tests;

/// <summary>Compiles consumer code against the built model assemblies, without friend access.</summary>
public sealed class SessionCompilationTests
{
    private static readonly MetadataReference[] _references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
        .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
        .Concat(Directory.GetFiles(AppContext.BaseDirectory, "Assimalign.Cohesion.*.dll"))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Select(static path => MetadataReference.CreateFromFile(path))
        .ToArray();

    [Theory(DisplayName = "Cohesion Test [Database] - ExecuteAsync: Should name the model request when a foreign request cannot compile")]
    [InlineData("sql.ExecuteAsync(keyValueRequest)", "SqlQueryRequest")]
    [InlineData("documents.ExecuteAsync(sqlRequest)", "DocumentQueryRequest")]
    [InlineData("graph.ExecuteAsync(sqlRequest)", "GraphRequest")]
    [InlineData("keyValue.ExecuteAsync(sqlRequest)", "KeyValueRequest")]
    [InlineData("keyValue.ExecuteAsync(\"GET @k\")", "KeyValueRequest")]
    public void ExecuteAsync_ForeignRequest_ShouldReportTheModelRequestType(string call, string expectedType)
    {
        // Arrange: the theory supplies the consumer call and its expected capability.

        // Act
        Diagnostic[] errors = Compile(call);

        // Assert — the target is the last type in CS1503, not merely a type mentioned elsewhere.
        Diagnostic error = errors.ShouldHaveSingleItem();
        error.Id.ShouldBe("CS1503");
        error.GetMessage(CultureInfo.InvariantCulture).ShouldEndWith(expectedType + "'", Case.Sensitive);
    }

    [Theory(DisplayName = "Cohesion Test [Database] - ExecuteAsync: Should expose no unsupported execution capability")]
    [InlineData("blob.ExecuteAsync(sqlRequest)", "BlobDatabaseSession", "ExecuteAsync")]
    [InlineData("root.ExecuteAsync(sqlRequest)", "DatabaseSession", "ExecuteAsync")]
    [InlineData("keyValue.ExecuteCommandTextAsync(\"GET @k\")", "KeyValueDatabaseSession", "ExecuteCommandTextAsync")]
    public void ExecuteAsync_UnsupportedCapability_ShouldNotCompile(string call, string sessionType, string member)
    {
        // Arrange: the theory supplies the consumer call and its expected capability.

        // Act
        Diagnostic[] errors = Compile(call);

        // Assert
        Diagnostic error = errors.ShouldHaveSingleItem();
        error.Id.ShouldBe("CS1061");
        string message = error.GetMessage(CultureInfo.InvariantCulture);
        message.ShouldContain("'" + sessionType + "'", Case.Sensitive);
        message.ShouldContain("'" + member + "'", Case.Sensitive);
    }

    [Theory(DisplayName = "Cohesion Test [Database] - ExecuteAsync: Should compile every supported model request and language call")]
    [InlineData("sql.ExecuteAsync(sqlRequest)")]
    [InlineData("sql.ExecuteAsync(\"SELECT 1\")")]
    [InlineData("documents.ExecuteAsync(documentRequest)")]
    [InlineData("documents.ExecuteAsync(\"SELECT * FROM items\")")]
    [InlineData("graph.ExecuteAsync(graphRequest)")]
    [InlineData("graph.ExecuteAsync(pathsRequest)")]
    [InlineData("graph.ExecuteAsync(\"MATCH (n) RETURN n\")")]
    [InlineData("keyValue.ExecuteAsync(keyValueRequest)")]
    [InlineData("blob.GetContainerAsync(\"items\")")]
    [InlineData("root.BeginTransactionAsync()")]
    public void ExecuteAsync_RightModel_ShouldCompile(string call)
    {
        // Arrange: the theory supplies the consumer call and its expected capability.

        // Act
        Diagnostic[] errors = Compile(call);

        // Assert
        errors.ShouldBeEmpty();
    }

    private static Diagnostic[] Compile(string call)
    {
        string source = $$"""
            using Assimalign.Cohesion.Database;
            using Assimalign.Cohesion.Database.Blob;
            using Assimalign.Cohesion.Database.Documents;
            using Assimalign.Cohesion.Database.Graph;
            using Assimalign.Cohesion.Database.KeyValuePair;
            using Assimalign.Cohesion.Database.Sql;

            public static class Consumer
            {
                public static void Run(DatabaseSession root, BlobDatabaseSession blob,
                    SqlDatabaseSession sql, DocumentDatabaseSession documents,
                    GraphDatabaseSession graph, KeyValueDatabaseSession keyValue,
                    SqlQueryRequest sqlRequest, DocumentQueryRequest documentRequest,
                    GraphQueryRequest graphRequest, GraphPathsQueryRequest pathsRequest,
                    KeyValueGetRequest keyValueRequest)
                {
                    _ = {{call}};
                }
            }
            """;
        var compilation = CSharpCompilation.Create(
            "SessionCapabilityConsumer",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))],
            _references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        return compilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/SessionCompilationTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/Assimalign.Cohesion.Database.Tests.csproj`.
