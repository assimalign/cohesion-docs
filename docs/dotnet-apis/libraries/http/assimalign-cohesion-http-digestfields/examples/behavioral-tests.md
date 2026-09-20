# Example: Http Digest Header Rules Tests

Exercise Http Digest Header Rules behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpDigestHeaderRulesTests.cs` listing from the package test project. Keep it
in that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Http.DigestFields.Tests;

using Assimalign.Cohesion.Http;

public class HttpDigestHeaderRulesTests
{
    [Theory(DisplayName = "Cohesion Test [Http.DigestFields] - Keys: Header keys emit their exact RFC 9530 field names")]
    [InlineData("Content-Digest")]
    [InlineData("Repr-Digest")]
    [InlineData("Want-Content-Digest")]
    [InlineData("Want-Repr-Digest")]
    public void HeaderKeys_MatchRfcFieldNames(string expected)
    {
        HttpHeaderKey key = expected switch
        {
            "Content-Digest" => HttpHeaderKey.ContentDigest,
            "Repr-Digest" => HttpHeaderKey.ReprDigest,
            "Want-Content-Digest" => HttpHeaderKey.WantContentDigest,
            _ => HttpHeaderKey.WantReprDigest,
        };

        key.Value.ShouldBe(expected);
    }

    [Fact(DisplayName = "Cohesion Test [Http.DigestFields] - Trailers: Digest fields are allowed in the trailer section")]
    public void DigestFields_AreNotProhibitedInTrailers()
    {
        // RFC 9530 §2.1 permits Content-Digest / Repr-Digest as trailers so a streamed body can be
        // hashed as it is written; HttpFieldRules must not classify them as trailer-prohibited.
        HttpFieldRules.IsProhibitedInTrailers(HttpHeaderKey.ContentDigest).ShouldBeFalse();
        HttpFieldRules.IsProhibitedInTrailers(HttpHeaderKey.ReprDigest).ShouldBeFalse();
    }
}
```

## Walkthrough

- **Covered behavior** — Keys: Header keys emit their exact RFC 9530 field names.
- **Covered behavior** — Trailers: Digest fields are allowed in the trailer section.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/tests/HttpDigestHeaderRulesTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/tests/Assimalign.Cohesion.Http.DigestFields.Tests.csproj`.
