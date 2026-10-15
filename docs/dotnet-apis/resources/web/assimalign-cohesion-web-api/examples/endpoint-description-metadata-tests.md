# Endpoint Description Metadata Tests

This example exercises `Assimalign.Cohesion.Web.Api` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.Api/tests/EndpointDescriptionMetadataTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
Use it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — Description metadata: a parameter description exposes its arguments.
- **Case 2** — Description metadata: a parameter description rejects an empty name.
- **Case 3** — Description metadata: a parameter description rejects a null type.
- **Case 4** — Description metadata: a parameter description rejects an undefined source.
- **Case 5** — Description metadata: a response description defaults to no body.
- **Case 6** — Description metadata: a response description exposes a fixed content type.
- **Case 7** — Description metadata: a response description rejects an unset status code.
- **Case 8** — Description metadata: a response description rejects a wildcard content type.

## Source example

```csharp
using System;
using CohesionHttpStatusCode = Assimalign.Cohesion.Http.HttpStatusCode;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Http;

namespace Assimalign.Cohesion.Web.Api.Tests;

/// <summary>
/// Unit coverage for the endpoint description carriers, <see cref="EndpointParameterMetadata"/> and
/// <see cref="EndpointResponseMetadata"/>: what they expose and the invariants their constructors guard.
/// </summary>
public class EndpointDescriptionMetadataTests
{
    [Fact(DisplayName = "Cohesion Test [Web.Api] - Description metadata: a parameter description exposes its arguments")]
    public void EndpointParameterMetadata_ValidArguments_ShouldExposeThem()
    {
        // Act
        EndpointParameterMetadata metadata = new("X-Tenant", EndpointParameterSource.Header, typeof(string), isRequired: true);

        // Assert
        metadata.Name.ShouldBe("X-Tenant");
        metadata.Source.ShouldBe(EndpointParameterSource.Header);
        metadata.Type.ShouldBe(typeof(string));
        metadata.IsRequired.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Web.Api] - Description metadata: a parameter description rejects an empty name")]
    public void EndpointParameterMetadata_EmptyName_ShouldThrow()
    {
        // Act
        Action act = () => _ = new EndpointParameterMetadata("", EndpointParameterSource.Query, typeof(int), isRequired: false);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact(DisplayName = "Cohesion Test [Web.Api] - Description metadata: a parameter description rejects a null type")]
    public void EndpointParameterMetadata_NullType_ShouldThrow()
    {
        // Act
        Action act = () => _ = new EndpointParameterMetadata("id", EndpointParameterSource.Route, null!, isRequired: true);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact(DisplayName = "Cohesion Test [Web.Api] - Description metadata: a parameter description rejects an undefined source")]
    public void EndpointParameterMetadata_UndefinedSource_ShouldThrow()
    {
        // Act
        Action act = () => _ = new EndpointParameterMetadata("id", (EndpointParameterSource)42, typeof(int), isRequired: true);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact(DisplayName = "Cohesion Test [Web.Api] - Description metadata: a response description defaults to no body")]
    public void EndpointResponseMetadata_StatusOnly_ShouldDescribeNoBody()
    {
        // Act
        EndpointResponseMetadata metadata = new(CohesionHttpStatusCode.NoContent);

        // Assert
        metadata.StatusCode.ShouldBe(CohesionHttpStatusCode.NoContent);
        metadata.Type.ShouldBeNull();
        metadata.ContentType.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Web.Api] - Description metadata: a response description exposes a fixed content type")]
    public void EndpointResponseMetadata_FixedContentType_ShouldExposeIt()
    {
        // Act
        EndpointResponseMetadata metadata = new(CohesionHttpStatusCode.Ok, typeof(string), HttpMediaType.TextPlain);

        // Assert
        metadata.Type.ShouldBe(typeof(string));
        metadata.ContentType.ShouldBe(HttpMediaType.TextPlain);
    }

    [Fact(DisplayName = "Cohesion Test [Web.Api] - Description metadata: a response description rejects an unset status code")]
    public void EndpointResponseMetadata_DefaultStatusCode_ShouldThrow()
    {
        // Act
        Action act = () => _ = new EndpointResponseMetadata(default);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact(DisplayName = "Cohesion Test [Web.Api] - Description metadata: a response description rejects a wildcard content type")]
    public void EndpointResponseMetadata_WildcardContentType_ShouldThrow()
    {
        // Act
        Action act = () => _ = new EndpointResponseMetadata(CohesionHttpStatusCode.Ok, typeof(string), HttpMediaType.Any);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/tests/EndpointDescriptionMetadataTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/tests/Assimalign.Cohesion.Web.Api.Tests.csproj`.
