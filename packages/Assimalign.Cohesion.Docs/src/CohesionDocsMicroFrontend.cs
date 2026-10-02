using System;
using System.Net.Http;

using Assimalign.Cohesion.Viu.Markdown;
using Assimalign.Frontend;

namespace Assimalign.Cohesion.Docs;

/// <summary>Provides the Cohesion documentation catalog, content source, routes, and navigation.</summary>
public sealed class CohesionDocsMicroFrontend : DocumentationMicroFrontend
{
    private const string PackageVersion = "1.0.0-preview.1";

    /// <summary>
    /// Creates the Cohesion documentation module with an unconfigured HTTP client for catalog-only
    /// hosts and tooling. Browser hosts should use <see cref="CohesionDocsMicroFrontend(HttpClient)"/>.
    /// </summary>
    public CohesionDocsMicroFrontend()
        : this(new HttpClient())
    {
    }

    /// <summary>Creates the Cohesion documentation module using the host's borrowed HTTP client.</summary>
    /// <param name="httpClient">
    /// The client whose base address identifies the host root. The module borrows and never disposes it.
    /// </param>
    public CohesionDocsMicroFrontend(HttpClient httpClient)
        : base(
            new MicroFrontendDescriptor(
                "cohesion",
                "Cohesion",
                "/cohesion",
                BrandAccent.Cohesion,
                10,
                "Cohesion is a code-first, multi-service application framework for .NET.",
                new Uri("https://github.com/assimalign/cohesion", UriKind.Absolute),
                PackageVersion),
            GeneratedMarkdownContent.Catalog,
            new HttpMarkdownContentSource(httpClient))
    {
        ArgumentNullException.ThrowIfNull(httpClient);
    }
}
