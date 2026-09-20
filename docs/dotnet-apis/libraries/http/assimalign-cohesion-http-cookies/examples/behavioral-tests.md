# Example: Http Response Cookie Extensions Tests

Exercise Http Response Cookie Extensions behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpResponseCookieExtensionsTests.cs` listing from the package test project.
Keep it in that project when running it: the project supplies its package references, generated
sources, and any shared fixtures. The using block below makes the test-framework import explicit
where the original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Http.Cookies.Tests;

public class HttpResponseCookieExtensionsTests
{
    [Fact]
    public void Cookies_FirstRead_ShouldReturnEmptyMutableCollection()
    {
        IHttpContext context = new BareHttpContext();

        IHttpCookieCollection cookies = context.Response.Cookies;

        cookies.ShouldNotBeNull();
        cookies.Count.ShouldBe(0);
        cookies.IsReadOnly.ShouldBeFalse();
    }

    [Fact]
    public void Cookies_FirstRead_ShouldInstallFeature()
    {
        IHttpContext context = new BareHttpContext();

        _ = context.Response.Cookies;

        context.Features.Get<IHttpResponseCookieFeature>().ShouldNotBeNull();
    }

    [Fact]
    public void Cookies_RepeatedReads_ShouldReturnSameInstance()
    {
        IHttpContext context = new BareHttpContext();

        IHttpCookieCollection first = context.Response.Cookies;
        IHttpCookieCollection second = context.Response.Cookies;

        second.ShouldBeSameAs(first);
    }

    [Fact]
    public void Cookies_AddedCookies_ShouldBeVisibleOnSubsequentReads()
    {
        IHttpContext context = new BareHttpContext();

        context.Response.Cookies.Add(new HttpCookie("trace", "abc"));
        context.Response.Cookies.Add(new HttpCookie("session", "xyz"));

        IHttpCookieCollection cookies = context.Response.Cookies;
        cookies.Count.ShouldBe(2);
    }

    [Fact]
    public void Cookies_PreInstalledFeature_ShouldBeObservedByGetter()
    {
        IHttpContext context = new BareHttpContext();
        HttpCookieCollection seeded = new() { new HttpCookie("custom", "feature") };
        context.Features.Set<IHttpResponseCookieFeature>(new TestResponseCookieFeature(seeded));

        IHttpCookieCollection observed = context.Response.Cookies;

        observed.ShouldBeSameAs(seeded);
    }

    [Fact]
    public void Cookies_GetOnNullResponse_ShouldThrow()
    {
        IHttpResponse response = null!;

        Should.Throw<ArgumentNullException>(() => _ = response.Cookies);
    }

    private sealed class TestResponseCookieFeature : IHttpResponseCookieFeature
    {
        public TestResponseCookieFeature(IHttpCookieCollection cookies)
        {
            Cookies = cookies;
        }

        public string Name => nameof(TestResponseCookieFeature);
        public IHttpCookieCollection Cookies { get; }
    }

    private sealed class BareHttpContext : IHttpContext
    {
        public BareHttpContext()
        {
            Request = new BareHttpRequest(this);
            Response = new BareHttpResponse(this);
        }

        public HttpVersion Version => HttpVersion.Http11;
        public IHttpRequest Request { get; }
        public IHttpResponse Response { get; }
        public IHttpConnectionInfo ConnectionInfo => HttpConnectionInfo.Empty;
        public IHttpFeatureCollection Features { get; } = new HttpFeatureCollection();
        public IDictionary<string, object?> Items { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);
        public CancellationToken RequestCancelled => CancellationToken.None;

        public void Cancel()
        {
            
        }

        public Task CancelAsync()
        {
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class BareHttpRequest : IHttpRequest
    {
        public BareHttpRequest(IHttpContext context) { HttpContext = context; }
        public HttpHost Host => HttpHost.Empty;
        public HttpPath Path => HttpPath.Root;
        public HttpMethod Method => HttpMethod.Get;
        public HttpScheme Scheme => HttpScheme.Http;
        public IHttpQueryCollection Query { get; } = new HttpQueryCollection();
        public IHttpHeaderCollection Headers { get; } = new HttpHeaderCollection();
        public IHttpContext HttpContext { get; }
        public Stream Body => Stream.Null;
    }

    private sealed class BareHttpResponse : IHttpResponse
    {
        public BareHttpResponse(IHttpContext context) { HttpContext = context; }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Ok;
        public IHttpHeaderCollection Headers { get; } = new HttpHeaderCollection();
        public IHttpContext HttpContext { get; }
        public Stream Body { get; set; } = Stream.Null;
    }
}
```

## Walkthrough

- **Test entry point** — `Cookies_FirstRead_ShouldReturnEmptyMutableCollection` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Cookies_FirstRead_ShouldInstallFeature` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Cookies_RepeatedReads_ShouldReturnSameInstance` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Cookies_AddedCookies_ShouldBeVisibleOnSubsequentReads` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Cookies_PreInstalledFeature_ShouldBeObservedByGetter` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Cookies_GetOnNullResponse_ShouldThrow` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Cancel` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CancelAsync` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/tests/HttpResponseCookieExtensionsTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/tests/Assimalign.Cohesion.Http.Cookies.Tests.csproj`.
