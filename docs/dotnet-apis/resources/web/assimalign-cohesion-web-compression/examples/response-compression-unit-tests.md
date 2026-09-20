# Response Compression Unit Tests

This example exercises `Assimalign.Cohesion.Web.Compression` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.Compression/tests/ResponseCompressionUnitTests.cs`.
It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — BREACH: an https response is not compressed by default.
- **Case 2** — BREACH: an https response is compressed when explicitly enabled.
- **Case 3** — BREACH: an http response is compressed with the default HTTPS-off policy.
- **Case 4** — Matcher: media types are matched exactly, by subtype wildcard, and by full wildcard.

## Source example

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Compression.Internal;
using Assimalign.Cohesion.Web.Compression.Tests.TestObjects;

namespace Assimalign.Cohesion.Web.Compression.Tests;

/// <summary>
/// Unit coverage for behaviors the in-memory HTTP/1.1 factory cannot express directly — chiefly the
/// BREACH gate, which requires an <c>https</c> request the plaintext in-memory transport never
/// produces — plus the media-type matcher. These drive the internal middleware over the
/// <see cref="TestHttpContext"/> double.
/// </summary>
public class ResponseCompressionUnitTests
{
    [Fact(DisplayName = "Cohesion Test [Web.Compression] - BREACH: an https response is not compressed by default")]
    public async Task InvokeAsync_HttpsWithoutOptIn_DoesNotCompress()
    {
        // Arrange
        ResponseCompressionMiddleware middleware = new(new ResponseCompressionOptions());
        TestHttpContext context = NewContext(HttpScheme.Https, "gzip");
        MemoryStream body = (MemoryStream)context.Response.Body;

        // Act
        await middleware.InvokeAsync(context, WriteLargeJsonAsync);

        // Assert
        context.Response.Headers.ContainsKey(HttpHeaderKey.ContentEncoding).ShouldBeFalse();
        CompressionPayloads.Utf8(body.ToArray()).ShouldBe(CompressionPayloads.LargeJson);
    }

    [Fact(DisplayName = "Cohesion Test [Web.Compression] - BREACH: an https response is compressed when explicitly enabled")]
    public async Task InvokeAsync_HttpsWithOptIn_Compresses()
    {
        // Arrange
        ResponseCompressionMiddleware middleware = new(new ResponseCompressionOptions { EnableForHttps = true });
        TestHttpContext context = NewContext(HttpScheme.Https, "gzip");
        MemoryStream body = (MemoryStream)context.Response.Body;

        // Act
        await middleware.InvokeAsync(context, WriteLargeJsonAsync);

        // Assert
        context.Response.Headers[HttpHeaderKey.ContentEncoding].Value.ShouldBe("gzip");
        CompressionPayloads.Utf8(CompressionPayloads.GzipDecompress(body.ToArray())).ShouldBe(CompressionPayloads.LargeJson);
    }

    [Fact(DisplayName = "Cohesion Test [Web.Compression] - BREACH: an http response is compressed with the default HTTPS-off policy")]
    public async Task InvokeAsync_Http_Compresses()
    {
        // Arrange
        ResponseCompressionMiddleware middleware = new(new ResponseCompressionOptions());
        TestHttpContext context = NewContext(HttpScheme.Http, "gzip");
        MemoryStream body = (MemoryStream)context.Response.Body;

        // Act
        await middleware.InvokeAsync(context, WriteLargeJsonAsync);

        // Assert
        context.Response.Headers[HttpHeaderKey.ContentEncoding].Value.ShouldBe("gzip");
        CompressionPayloads.Utf8(CompressionPayloads.GzipDecompress(body.ToArray())).ShouldBe(CompressionPayloads.LargeJson);
    }

    [Theory(DisplayName = "Cohesion Test [Web.Compression] - Matcher: media types are matched exactly, by subtype wildcard, and by full wildcard")]
    [InlineData(new[] { "application/json" }, "application/json; charset=utf-8", true)]
    [InlineData(new[] { "application/json" }, "text/html", false)]
    [InlineData(new[] { "text/*" }, "text/html", true)]
    [InlineData(new[] { "text/*" }, "application/json", false)]
    [InlineData(new[] { "*/*" }, "image/png", true)]
    [InlineData(new[] { "application/json" }, null, false)]
    public void CompressibleMimeMatcher_MatchesExpected(string[] patterns, string? contentType, bool expected)
    {
        // Arrange
        CompressibleMimeMatcher matcher = CompressibleMimeMatcher.Create(patterns);

        // Act
        bool result = matcher.IsMatch(contentType);

        // Assert
        result.ShouldBe(expected);
    }

    private static TestHttpContext NewContext(HttpScheme scheme, string acceptEncoding)
    {
        TestHttpContext context = new();
        context.Request.Scheme = scheme;
        context.Request.Headers[HttpHeaderKey.AcceptEncoding] = acceptEncoding;
        return context;
    }

    private static async Task WriteLargeJsonAsync(IHttpContext context)
    {
        context.Response.Headers[HttpHeaderKey.ContentType] = "application/json";
        await context.Response.Body.WriteAsync(CompressionPayloads.Utf8(CompressionPayloads.LargeJson).AsMemory());
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Compression/tests/ResponseCompressionUnitTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Compression/tests/Assimalign.Cohesion.Web.Compression.Tests.csproj`.
