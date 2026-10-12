# Assimalign.Cohesion.Http

The transport-independent HTTP contracts and protocol value objects used by
Cohesion clients, transports, and Web features. The package depends on Core;
feature-specific contracts live in the corresponding Http feature packages.
See [Design](design.md) for protocol semantics and compatibility decisions.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Headers and trailers are distinct ordered field sections using compatible collection primitives.
Optional concerns attach through feature and interceptor seams rather than widening the protocol
root. The core remains independent of hosting and resource platforms.

The core holds base contracts only: no transport-produced feature contract is declared here. A
transport publishes further facts about a connection as *facets* on `IHttpConnectionInfo`:
additional interfaces the object it hands out also implements, found with a type test. The server
transport (`Assimalign.Cohesion.Http.Connections`) publishes what a TLS handshake negotiated as the
Connections library's `ITlsConnectionInfo`, which `context.TlsConnection` in
`Assimalign.Cohesion.Http.Tls` reads; `IHttpTlsConnectionFeature` and its accessor moved there
(#1367). `HttpConnectionInfo` is unsealed so a transport can publish a facet on a subclass, and code
that wraps an `IHttpContext` forwards the inner context's connection info rather than building a new
object, which would hide the facets.

For the HTTP/2 and HTTP/3 extended CONNECT (RFC 8441, RFC 9220) the core carries two generic seam
members (#1368): `HttpExchangeInterceptorRequestContext.Protocol`, the `:protocol` the transport
validated, and `IHttpExchangeControl.CanAcceptTunnel` / `AcceptTunnelAsync`, which answers `200`
without ending the stream and returns the stream as a duplex tunnel. `IHttpExtendedConnectFeature`
(`context.ExtendedConnect`) moved back to `Http.ExtendedConnect`, whose interceptor installs it.
Adding the two control members is a source break for an `IHttpExchangeControl` implementer outside
this repository. `HttpFieldNormalization.ValidateExtendedConnect`, which HTTP/2 and HTTP/3 share,
treats a present but empty `:protocol` as malformed (#1369); only an absent field means the request
is not an extended CONNECT. See the [design](design.md#the-extended-connect-seam).

`Request.Trailers` and `Response.Trailers` report per exchange whether a trailer section is
supported (`IsSupported`). The transports fill request trailers on every version (on HTTP/1.1 for a
chunked request) once the body has been read to its end, hold a received trailer section to one
rule set on every version, and send response trailers on HTTP/2 and HTTP/3 only. See the
[design](design.md#trailers).

`HttpQuery.Parse`, which every transport uses for a request's query, skips a parameter with an empty
name (`?=1`) rather than failing the request as its head is read (#1323). See the
[design](design.md#query-parameters).

A request-parse hook can add an interceptor to its own exchange's response phase
(`HttpExchangeInterceptorRequestContext.AddResponseInterceptor`), so an interceptor that needs the
response phase for a few exchanges keeps every other exchange on the transport's fast path. See the
[design](design.md#per-exchange-response-interceptors).

`HttpFieldNormalization` states the one field-syntax rule every reader and writer applies
(`IsValidFieldName`, `IsValidFieldValue`, `IndexOfInvalidControlCharacter`; #1341): a name is a
token, and a value holds no control character but HTAB. HTTP/1.1 refuses a request that breaks it
with `400` (#1341), HTTP/2 and HTTP/3 reset its stream (#1376), and every response writer refuses
such a field before it writes a byte, with an `HttpException` whose code is the new
`HttpErrorCode.InvalidResponseField` (#1183). See the [design](design.md#field-syntax).

`HttpMethod` is case-sensitive (RFC 9110 §9.1, #1301): it keeps its token as sent and compares it
ordinally, so `get` is an unknown extension method rather than `GET`, on every version. This is a
breaking change for code that compared methods without regard to case. See the
[design](design.md#methods-are-case-sensitive-rfc-9110-91).

`HttpContentTypes` reads its table through two lookups, by file name (`TryGetFromFileName`) and by
extension (`TryGetFromExtension`), and never guesses between them: a file named `html` maps to
nothing (#1186). The single `TryGetContentType` is gone. See the
[design](design.md#content-types-two-lookups).

`IHttpExchangeControl.ClientFaultStatusCode` reports the `4xx` status a transport answers because
the client's request was at fault, or `null` (#1340). It is a default interface member returning
`null`, so adding it is not a source break; the server transport reports it on every version, HTTP/2
and HTTP/3 since #1378, which report `400` for a malformed body they reset.
`HttpExchangeInterceptorRequestContext.CanHaveBody` tells a host's gate whether content may follow
the head, which an HTTP/2 or HTTP/3 request need not declare. See the
[design](design.md#the-client-fault-report).

`HttpMinDataRate` moved here from `Assimalign.Cohesion.Http.Connections` (#1383), a source and
binary break: code that names it with only `using Assimalign.Cohesion.Http.Connections;` adds
`using Assimalign.Cohesion.Http;`. The request-parse context carries `MinRequestBodyDataRate`
(#1383) and `MinResponseDataRate` (#1384), seeded from the listener's limits and frozen at the
body's first read and the response's first write, so a hook, middleware or endpoint can change
either rate for one request. See the [design](design.md#the-request-contexts-data-rates-1383-1384).

`HttpFieldNormalization.ValidateRequestTarget` (#1334) holds the HTTP/2 and HTTP/3 request targets
to HTTP/1.1's: a classic CONNECT carries `host:port` and no `:scheme` or `:path`, asterisk-form is
`OPTIONS *` alone, `:path`, `:authority` and `Host` hold only VCHAR now that HPACK decodes obs-text
as Latin-1 like QPACK (#1387), and `:method` is a token of at most 32 characters. See the
[design](design.md#request-target-forms-1334).

`HttpPath.ToUriComponent()` and `HttpQuery.Create` write a decoded path and query back into a URI,
the inverses of the transports' decode, so a redirect's `Location` keeps the request's escapes
(#1324). See the
[design](design.md#writing-a-path-and-a-query-back-out-touricomponent-httpquerycreate).

`HttpIfRange.Matches` applies a date only when it exactly matches a strong `Last-Modified`, one at
least a second before the `responseDate` a new third parameter supplies (#1185); the two-argument
overload is gone, a source break. See the [design](design.md#if-range-is-step-5-rfc-9110-1315).

`HttpStatusCode` carries the IANA registry's reason phrases, RFC 9110's wording, and marks the
reserved `Unused` (`306`) obsolete (#1204). See the
[design](design.md#status-codes-and-reason-phrases-rfc-9110-15).

`HttpHeaderValue.Concat` appends in amortized constant time, so a field repeated `n` times combines
in time linear in `n` (#1082); `HttpHost` trims SP and HTAB only (#1341); and
`Features.Get<TFeature>()` allocates nothing on an `HttpFeatureCollection`. See the
[design](design.md#repeated-fields-combine-in-linear-time).

The first field in a parameterless `HttpHeaderCollection` stays inline, so seeding a response with
a process-cached `Date` allocates nothing and consumes none of the dictionary capacity needed by
application fields (#1417). The core owns only the collection storage; the transport owns the Date
policy and clock. See the [design](design.md#small-header-collections-and-cached-fields-1417).

## Query values

Parse with `new HttpQuery("?tag=first&tag=&tag=last").Parse()`.
The collection has one key and its `query["tag"]` value contains three ordered
strings: `"first"`, `""`, and `"last"`. Enumerate that value or use its
integer indexer and `Count` to read all occurrences. Its `Value`, `ToString()`,
and typed scalar getters return the first occurrence (decision 31).

`HttpQueryCollection.Add` appends occurrences; assigning its key indexer
replaces all values for that key. `HttpQuery.Create(query)` encodes every
occurrence for a redirect or rewritten URL. Collection `Count` counts keys.
These behaviors replace preview.1's last-value parsing; see the compatibility
and caller audit in the [design](design.md#repeated-values-1335-decision-31)
before upgrading code that sends duplicate keys.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpAcceptParser` | `src/HttpAcceptParser.cs` |
| `HttpAcceptQuery` | `src/ValueObjects/HttpAcceptQuery.cs` |
| `HttpAltService` | `src/ValueObjects/HttpAltService.cs` |
| `HttpCacheControl` | `src/HttpCacheControl.cs` |
| `HttpCacheControlExtension` | `src/ValueObjects/HttpCacheControlExtension.cs` |
| `HttpConditionalRequest` | `src/HttpConditionalRequest.cs` |
| `HttpConditionalRequestContext` | `src/HttpConditionalRequestContext.cs` |
| `HttpConnectionInfo` | `src/HttpConnectionInfo.cs` |
| `HttpContentNegotiation` | `src/HttpContentNegotiation.cs` |
| `HttpContentRange` | `src/ValueObjects/HttpContentRange.cs` |
| `HttpContentTypes` | `src/HttpContentTypes.cs` |
| `HttpContext` | `src/HttpContext.cs` |
| `HttpContextExtensions` | `src/Extensions/HttpContextExtensions.cs` |
| `HttpDate` | `src/HttpDate.cs` |
| `HttpEntityTag` | `src/ValueObjects/HttpEntityTag.cs` |
| `HttpEntityTagCondition` | `src/HttpEntityTagCondition.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Assimalign.Cohesion.Http.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpAcceptParser.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpAcceptQuery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpAltService.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpCacheControl.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpCacheControlExtension.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConditionalRequest.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConditionalRequestContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConnectionInfo.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentNegotiation.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpContentRange.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentTypes.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Extensions/HttpContextExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpDate.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpEntityTag.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpEntityTagCondition.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpConnectionInfo.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpExchangeControl.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpFieldNormalization.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpResponse.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpExchangeInterceptorRequestContext.cs`.

- **Source** — `cohesion/docs/libraries/Http/DECISIONS.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpQuery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpHeaderValue.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpHost.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpMethod.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpFeatureCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Extensions/HttpFeatureCollectionExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Exceptions/HttpErrorCode.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpMinDataRate.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpPath.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpIfRange.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpStatusCode.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpHeaderCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpQueryCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpQueryValue.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpQueryCollection.cs`.
