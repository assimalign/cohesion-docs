# Example: Server Sent Event Formatter Tests

Exercise Server Sent Event Formatter behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ServerSentEventFormatterTests.cs` listing from the package test project. Keep
it in that project when running it: the project supplies its package references, generated sources,
and any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Text;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Http.ServerSentEvents.Tests;

public class ServerSentEventFormatterTests
{
    private static string Format(ServerSentEvent @event)
        => Encoding.UTF8.GetString(ServerSentEventFormatter.Format(@event));

    [Fact(DisplayName = "Cohesion Test [Http] - Sse: Should serialize a data-only event with the dispatch blank line")]
    public void Format_DataOnlyEvent_ShouldEmitDataFieldAndBlankLine()
    {
        string wire = Format(ServerSentEvent.Message("hello"));

        wire.ShouldBe("data: hello\n\n");
    }

    [Fact(DisplayName = "Cohesion Test [Http] - Sse: Should serialize every field in order")]
    public void Format_FullEvent_ShouldEmitAllFields()
    {
        ServerSentEvent @event = new("payload")
        {
            EventType = "update",
            Id = "42",
            Retry = TimeSpan.FromSeconds(3),
        };

        string wire = Format(@event);

        // event, id, retry, then data, then the dispatch blank line.
        wire.ShouldBe("event: update\nid: 42\nretry: 3000\ndata: payload\n\n");
    }

    [Fact(DisplayName = "Cohesion Test [Http] - Sse: Should emit one data field per line for multi-line data")]
    public void Format_MultiLineData_ShouldEmitOneDataFieldPerLine()
    {
        string wire = Format(ServerSentEvent.Message("line1\nline2\nline3"));

        wire.ShouldBe("data: line1\ndata: line2\ndata: line3\n\n");
    }

    [Fact(DisplayName = "Cohesion Test [Http] - Sse: Should split CRLF and CR line breaks in data")]
    public void Format_DataWithMixedLineBreaks_ShouldSplitOnEachBreak()
    {
        string wire = Format(ServerSentEvent.Message("a\r\nb\rc"));

        wire.ShouldBe("data: a\ndata: b\ndata: c\n\n");
    }

    [Fact(DisplayName = "Cohesion Test [Http] - Sse: Should serialize a keep-alive as a comment line")]
    public void Format_KeepAlive_ShouldEmitCommentLine()
    {
        string wire = Format(ServerSentEvent.KeepAlive());

        wire.ShouldBe(": keep-alive\n\n");
    }

    [Fact(DisplayName = "Cohesion Test [Http] - Sse: Should serialize an empty (non-null) data value as an empty data field")]
    public void Format_EmptyData_ShouldEmitEmptyDataField()
    {
        string wire = Format(ServerSentEvent.Message(string.Empty));

        wire.ShouldBe("data: \n\n");
    }

    [Fact(DisplayName = "Cohesion Test [Http] - Sse: Should carry a comment alongside data fields")]
    public void Format_CommentWithData_ShouldEmitCommentThenData()
    {
        ServerSentEvent @event = new("body") { Comment = "note" };

        string wire = Format(@event);

        wire.ShouldBe(": note\ndata: body\n\n");
    }

    [Fact(DisplayName = "Cohesion Test [Http] - Sse: MediaType should be text/event-stream")]
    public void MediaType_ShouldBeTextEventStream()
    {
        ServerSentEvent.MediaType.ShouldBe("text/event-stream");
    }
}
```

## Walkthrough

- **Covered behavior** — Sse: Should serialize a data-only event with the dispatch blank line.
- **Covered behavior** — Sse: Should serialize every field in order.
- **Covered behavior** — Sse: Should emit one data field per line for multi-line data.
- **Covered behavior** — Sse: Should split CRLF and CR line breaks in data.
- **Covered behavior** — Sse: Should serialize a keep-alive as a comment line.
- **Covered behavior** — Sse: Should serialize an empty (non-null) data value as an empty data field.
- **Covered behavior** — Sse: Should carry a comment alongside data fields.
- **Covered behavior** — Sse: MediaType should be text/event-stream.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/tests/ServerSentEventFormatterTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/tests/Assimalign.Cohesion.Http.ServerSentEvents.Tests.csproj`.
