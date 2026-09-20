# Example: Dns Question Tests

Exercise Dns Question behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `DnsQuestionTests.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures. The using block below makes the test-framework import explicit where the original
project supplies it globally.

## Code

```csharp
using Xunit;
using Assimalign.Cohesion.Dns;

namespace Assimalign.Cohesion.Dns.Tests;

/// <summary>
/// Covers the <see cref="DnsQuestion"/> value-type contract.
/// </summary>
public class DnsQuestionTests
{
    [Fact(DisplayName = "Cohesion Test [Dns] - DnsQuestion: defaults class to IN")]
    public void DefaultClass_IsIN()
    {
        var question = new DnsQuestion("example.com", DnsRecordType.A);
        Assert.Equal(DnsClass.IN, question.Class);
    }

    [Fact(DisplayName = "Cohesion Test [Dns] - DnsQuestion: equality compares all three fields")]
    public void Equality_AllFields()
    {
        var a = new DnsQuestion("example.com", DnsRecordType.A, DnsClass.IN);
        var b = new DnsQuestion("EXAMPLE.COM", DnsRecordType.A, DnsClass.IN);
        var differentType = new DnsQuestion("example.com", DnsRecordType.AAAA, DnsClass.IN);
        var differentClass = new DnsQuestion("example.com", DnsRecordType.A, DnsClass.CH);

        Assert.Equal(a, b);                // case-insensitive name match
        Assert.NotEqual(a, differentType);
        Assert.NotEqual(a, differentClass);
    }

    [Fact(DisplayName = "Cohesion Test [Dns] - DnsQuestion: ToString uses zone-file convention")]
    public void ToString_ZoneFileFormat()
    {
        var question = new DnsQuestion("example.com", DnsRecordType.MX);
        // Zone-file convention: NAME CLASS TYPE
        Assert.Equal("example.com IN MX", question.ToString());
    }
}
```

## Walkthrough

- **Covered behavior** — DnsQuestion: defaults class to IN.
- **Covered behavior** — DnsQuestion: equality compares all three fields.
- **Covered behavior** — DnsQuestion: ToString uses zone-file convention.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/tests/DnsQuestionTests.cs`.
- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/tests/Assimalign.Cohesion.Dns.Tests.csproj`.
