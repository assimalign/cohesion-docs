# Query Redirect Tests

This example exercises `Assimalign.Cohesion.Web.Query` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.Query/tests/QueryRedirectTests.cs`. It retains the
test class and assertions so the setup, operation, and expected outcome stay together. `Use` it in the
source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — Redirect: `RedirectQuery` emits 307 with the `Location` field.
- **Case 2** — Redirect: `RedirectQuery` permanent emits 308 with the `Location` field.
- **Case 3** — Redirect: `RedirectQueryToGet` emits 303 with the `Location` field.
- **Case 4** — Redirect: an empty location is rejected.
- **Case 5** — Redirect: a redirected QUERY lands as a QUERY with its content (RFC 10008 §2.5).
- **Case 6** — Redirect: the 303 hand-off lands as a bodiless GET (RFC 10008 §2.5.3).

## Source example

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CohesionHttpMethod = Assimalign.Cohesion.Http.HttpMethod;
using NetHttpMethod = System.Net.Http.HttpMethod;
using NetHttpStatusCode = System.Net.HttpStatusCode;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Testing;

namespace Assimalign.Cohesion.Web.Query.Tests;

/// <summary>
/// RFC 10008 &#167; 2.5 tests for the QUERY redirect helpers: the raw response shaping
/// (307/308/303 + <c>Location</c>, unit level) and the end-to-end flow over the in-memory
/// transport — a redirected QUERY is re-issued as a QUERY with its content, and the
/// <c>303 See Other</c> hand-off is re-issued as a bodiless GET.
/// </summary>
public class QueryRedirectTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(30);
    private static readonly NetHttpMethod QueryMethod = new("QUERY");

    // ============================================================================
    // Response shaping (unit level — the raw 3xx the helper writes)
    // ============================================================================

    [Fact(DisplayName = "Cohesion Test [Web.Query] - Redirect: RedirectQuery emits 307 with the Location field")]
    public void RedirectQuery_Temporary_ShouldEmit307WithLocation()
    {
        // Arrange
        var context = new TestHttpContext("/search", CohesionHttpMethod.Query);

        // Act
        context.Response.RedirectQuery("/search-v2");

        // Assert — 307 preserves method + content by definition (RFC 9110 §15.4.8).
        context.Response.StatusCode.ShouldBe(HttpStatusCode.RedirectKeepVerb);
        context.Response.Headers.GetValue(HttpHeaderKey.Location).ShouldBe("/search-v2");
    }

    [Fact(DisplayName = "Cohesion Test [Web.Query] - Redirect: RedirectQuery permanent emits 308 with the Location field")]
    public void RedirectQuery_Permanent_ShouldEmit308WithLocation()
    {
        // Arrange
        var context = new TestHttpContext("/search", CohesionHttpMethod.Query);

        // Act
        context.Response.RedirectQuery("https://origin.example/search-v2", permanent: true);

        // Assert — 308 is the permanent method-preserving redirect (RFC 9110 §15.4.9).
        context.Response.StatusCode.ShouldBe(HttpStatusCode.PermanentRedirect);
        context.Response.Headers.GetValue(HttpHeaderKey.Location).ShouldBe("https://origin.example/search-v2");
    }

    [Fact(DisplayName = "Cohesion Test [Web.Query] - Redirect: RedirectQueryToGet emits 303 with the Location field")]
    public void RedirectQueryToGet_ShouldEmit303WithLocation()
    {
        // Arrange
        var context = new TestHttpContext("/search", CohesionHttpMethod.Query);

        // Act
        context.Response.RedirectQueryToGet("/results/42");

        // Assert — the one sanctioned method switch (RFC 10008 §2.5.3).
        context.Response.StatusCode.ShouldBe(HttpStatusCode.SeeOther);
        context.Response.Headers.GetValue(HttpHeaderKey.Location).ShouldBe("/results/42");
    }

    [Fact(DisplayName = "Cohesion Test [Web.Query] - Redirect: an empty location is rejected")]
    public void RedirectQuery_EmptyLocation_ShouldThrow()
    {
        var context = new TestHttpContext("/search", CohesionHttpMethod.Query);

        Should.Throw<ArgumentException>(() => context.Response.RedirectQuery(string.Empty));
        Should.Throw<ArgumentException>(() => context.Response.RedirectQueryToGet(string.Empty));
    }

    // ============================================================================
    // End to end — the redirected QUERY is re-issued with method + content preserved
    // ============================================================================

    private static async Task<(WebApplicationTestFactory Factory, HttpClient Client)> CreateRedirectAppAsync(
        bool permanent,
        bool toGet,
        CancellationToken cancellationToken)
    {
        var factory = new WebApplicationTestFactory();

        factory.Application.Use(async (context, next) =>
        {
            if (context.Request.Path.ToString() == "/search")
            {
                // The resource moved: shape the redirect with the QUERY-preserving helper.
                if (toGet)
                {
                    context.Response.RedirectQueryToGet("/search-v2");
                }
                else
                {
                    context.Response.RedirectQuery("/search-v2", permanent);
                }
                return;
            }

            await next.Invoke(context);
        });
        factory.Application.Use(async (context, next) =>
        {
            // Terminal echo at the redirect target: reveals the re-issued method and content.
            string body;
            using (var reader = new System.IO.StreamReader(context.Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync(context.RequestCancelled);
            }

            context.Response.StatusCode = HttpStatusCode.Ok;
            byte[] payload = Encoding.UTF8.GetBytes($"{context.Request.Method.Value}:{body}");
            await context.Response.Body.WriteAsync(payload, context.RequestCancelled);
        });

        await factory.StartAsync(cancellationToken);
        return (factory, factory.CreateClient());
    }

    [Theory(DisplayName = "Cohesion Test [Web.Query] - Redirect: a redirected QUERY lands as a QUERY with its content (RFC 10008 §2.5)")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RedirectQuery_EndToEnd_ShouldReissueQueryWithContent(bool permanent)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TestTimeout);
        (WebApplicationTestFactory factory, HttpClient client) = await CreateRedirectAppAsync(
            permanent, toGet: false, cancellation.Token);
        await using WebApplicationTestFactory ownedFactory = factory;
        using HttpClient ownedClient = client;

        using var request = new HttpRequestMessage(QueryMethod, "/search")
        {
            Content = new StringContent("{\"q\":\"cohesion\"}", Encoding.UTF8, "application/json"),
        };

        // Act — the BCL client auto-follows the 307/308; both preserve method + content.
        using HttpResponseMessage response = await client.SendAsync(request, cancellation.Token);

        // Assert — the target observed a QUERY carrying the original content: no GET downgrade.
        response.StatusCode.ShouldBe(NetHttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(cancellation.Token)).ShouldBe("QUERY:{\"q\":\"cohesion\"}");
    }

    [Fact(DisplayName = "Cohesion Test [Web.Query] - Redirect: the 303 hand-off lands as a bodiless GET (RFC 10008 §2.5.3)")]
    public async Task RedirectQueryToGet_EndToEnd_ShouldReissueAsGetWithoutContent()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TestTimeout);
        (WebApplicationTestFactory factory, HttpClient client) = await CreateRedirectAppAsync(
            permanent: false, toGet: true, cancellation.Token);
        await using WebApplicationTestFactory ownedFactory = factory;
        using HttpClient ownedClient = client;

        using var request = new HttpRequestMessage(QueryMethod, "/search")
        {
            Content = new StringContent("{\"q\":\"cohesion\"}", Encoding.UTF8, "application/json"),
        };

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, cancellation.Token);

        // Assert — the one intended method switch: GET on the Location URI, content dropped.
        response.StatusCode.ShouldBe(NetHttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(cancellation.Token)).ShouldBe("GET:");
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Query/tests/QueryRedirectTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Query/tests/Assimalign.Cohesion.Web.Query.Tests.csproj`.
