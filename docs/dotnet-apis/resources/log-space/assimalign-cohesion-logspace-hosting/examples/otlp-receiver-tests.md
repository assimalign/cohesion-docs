# Otlp Receiver Tests

This example exercises `Assimalign.Cohesion.LogSpace.Hosting` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/tests/OtlpReceiverTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
`Use` it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — Segments: persisted restart, filters and cursor retain ordering.
- **Case 2** — JSON reader refuses service impersonation and malformed values.

## Source example

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.LogSpace.Hosting.Internal;

namespace Assimalign.Cohesion.LogSpace.Hosting.Tests;

public sealed class OtlpReceiverTests
{
    [Fact(DisplayName = "Cohesion Test [LogSpace.Hosting] - Segments: persisted restart, filters and cursor retain ordering")]
    public void Store_Restart_ShouldAppendAndPage()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "seg-" + Guid.NewGuid().ToString("N"));
        try
        {
            var store = new LogSegmentStore(directory);
            for (int index = 0; index < 3; index++)
            { store.TryEnqueue(new StoredLog((1700000000000000000L + index).ToString(), 9, "INFO", "web", "test", "record" + index, new Dictionary<string, JsonElement>())).ShouldBeTrue(); }
            store.Flush(stopping: true);
            var next = new LogSegmentStore(directory);
            next.TryEnqueue(new StoredLog("1700000001000000000", 10, "EVENT", "other", "test", "restart", new Dictionary<string, JsonElement>())).ShouldBeTrue();
            next.Flush(stopping: true);
            Directory.EnumerateFiles(directory, "*.ndjson").Count().ShouldBe(2);
            Directory.EnumerateFiles(directory, "*.index").Count().ShouldBe(2);
            var first = next.Query("web", null, 1, null);
            first.Body.ShouldContain("record0", Case.Sensitive); first.Cursor.ShouldNotBeNull();
            var second = next.Query("web", null, 1, first.Cursor);
            second.Body.ShouldContain("record1", Case.Sensitive);
            Should.Throw<ArgumentException>(() => next.Query("other", null, 1, first.Cursor));
            next.Query(null, DateTimeOffset.FromUnixTimeSeconds(1700000001), 100, null).Body.ShouldContain("restart", Case.Sensitive);
            next.Query("missing", null, 100, null).Body.ShouldBeEmpty();
        }
        finally { if (Directory.Exists(directory)) { Directory.Delete(directory, recursive: true); } }
    }

    [Fact(DisplayName = "Cohesion Test [LogSpace.Hosting] - JSON reader refuses service impersonation and malformed values")]
    public void Read_ResourceIdentity_ShouldMatchAuthenticatedEmitter()
    {
        using var document = JsonDocument.Parse("""{"resourceLogs":[{"resource":{"attributes":[{"key":"service.name","value":{"stringValue":"web"}}]},"scopeLogs":[{"scope":{"name":"category"},"logRecords":[{"timeUnixNano":"1","body":{"stringValue":"message"}}]}]}]}""");
        OtlpLogReader.Read(document.RootElement, "web").Single().Body.ShouldBe("message");
        Should.Throw<UnauthorizedAccessException>(() => OtlpLogReader.Read(document.RootElement, "other"));
        using var empty = JsonDocument.Parse("{}");
        OtlpLogReader.Read(empty.RootElement, "web").ShouldBeEmpty();
        using var invalid = JsonDocument.Parse("{\"resourceLogs\":false}");
        Should.Throw<InvalidOperationException>(() => OtlpLogReader.Read(invalid.RootElement, "web"));
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/tests/OtlpReceiverTests.cs`.
- **Source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/tests/Assimalign.Cohesion.LogSpace.Hosting.Tests.csproj`.
