using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Assimalign.Cohesion.Viu.Markdown;
using Assimalign.Frontend;
using Assimalign.Viu.Components;
using Assimalign.Viu.Router;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Docs.Tests;

public sealed class CohesionDocsMicroFrontendTests
{
    private static readonly string[] ExpectedTopLevelTitles =
    [
        "Overview",
        "ApiManager",
        "ConfigurationStore",
        "Database",
        "EmailHub",
        "EventHub",
        "IdentityHub",
        "IoTHub",
        "LoadBalancer",
        "LogSpace",
        "MediaHub",
        "MessageHub",
        "NatGateway",
        "NotificationHub",
        "Rezolvr",
        "Scheduler",
        "SecretStore",
        "VpnGateway",
        "Web",
        "Platforms",
        ".NET APIs",
    ];

    [Fact]
    public void GeneratedCatalog_CoversEveryMarkdownFileAndUsesPackageAssetPrefix()
    {
        string docsRoot = FindDocsRoot();
        string[] markdownFiles = Directory.GetFiles(docsRoot, "*.md", SearchOption.AllDirectories);

        GeneratedMarkdownContent.Catalog.Pages.Count.ShouldBe(markdownFiles.Length);
        GeneratedMarkdownContent.Catalog.Pages.ShouldAllBe(page =>
            page.AssetPath.StartsWith("_content/Assimalign.Cohesion.Docs/docs/", StringComparison.Ordinal));
    }

    [Fact]
    public void CatalogAndComposedRoutes_UseExactlyOneCohesionPrefix()
    {
        CohesionDocsMicroFrontend module = new();
        MarkdownContentCatalog catalog = module.Catalog;

        catalog.TryGetPageByRoute("/cohesion", out MarkdownPage? home).ShouldBeTrue();
        home.ShouldNotBeNull().SourceRelativePath.ShouldBe("index.md");
        catalog.TryGetPageByRoute("/cohesion/database", out _).ShouldBeTrue();
        catalog.TryGetPageByRoute("/cohesion/database/sql", out _).ShouldBeTrue();
        catalog.TryGetPageByRoute("/database", out _).ShouldBeFalse();
        catalog.Pages.ShouldAllBe(page => page.RoutePath == "/cohesion"
            || page.RoutePath.StartsWith("/cohesion/", StringComparison.Ordinal));
        catalog.Pages.ShouldAllBe(page => !page.RoutePath.StartsWith("/cohesion/cohesion", StringComparison.Ordinal));

        IReadOnlyList<RouteRecord> routes = module.CreateRoutes(new MicroFrontendRouteContext(
            ComponentReference.ForName("AppShell"),
            module.Descriptor.RoutePrefix,
            EmptyServices.Instance));
        RouteRecord layout = routes.ShouldHaveSingleItem();
        layout.Path.ShouldBe("/cohesion");
        layout.Children.ShouldContain(route => route.Path == string.Empty);
        layout.Children.ShouldContain(route => route.Path == "database");
        layout.Children.ShouldContain(route => route.Path == "database/sql");
        layout.Children.ShouldNotContain(route => route.Path.StartsWith("cohesion", StringComparison.Ordinal));
    }

    [Fact]
    public void Navigation_FollowsRootLinkOrderAndResolvesContextLinks()
    {
        CohesionDocsMicroFrontend module = new();
        IMicroFrontendNavigation navigation = module.CreateNavigation();
        IReadOnlyList<NavigationRow> rows = navigation.GetRows(
            new HashSet<string>(StringComparer.Ordinal),
            "/cohesion");

        rows.Where(row => row.RoutePath != "/cohesion").Select(row => row.Title)
            .ShouldBe(ExpectedTopLevelTitles);
        navigation.GetBreadcrumbs("/cohesion/database/sql").Select(row => row.Title)
            .ShouldBe(["Cohesion Documentation", "Database", "SQL"]);
        IReadOnlyList<PageLink> links = navigation.GetPageLinks("/cohesion/database/sql");
        links.Select(link => link.Direction).ShouldBe(["Previous", "Next"]);
        links.ShouldAllBe(link => link.RoutePath.StartsWith("/cohesion", StringComparison.Ordinal));
    }

    [Fact]
    public void EveryStatusCallout_UsesOneOfTheDocumentedStatuses()
    {
        const string marker = "> **Status:** ";
        string docsRoot = FindDocsRoot();
        List<string> violations = [];
        foreach (string file in Directory.EnumerateFiles(docsRoot, "*.md", SearchOption.AllDirectories))
        {
            int lineNumber = 0;
            foreach (string line in File.ReadLines(file))
            {
                lineNumber++;
                bool looksLikeStatus = line.StartsWith("> ", StringComparison.Ordinal)
                    && line.Contains("Status", StringComparison.Ordinal);
                if (looksLikeStatus
                    && (!line.StartsWith(marker, StringComparison.Ordinal)
                        || !IsDocumentedStatus(line[marker.Length..])))
                {
                    violations.Add($"{Path.GetRelativePath(docsRoot, file)}:{lineNumber}: {line}");
                }
            }
        }

        violations.ShouldBeEmpty("Status callouts must start with one of the four forms documented in README.md."
            + Environment.NewLine + string.Join(Environment.NewLine, violations));
    }

    private static bool IsDocumentedStatus(string value) =>
        value.StartsWith("Implemented.", StringComparison.Ordinal)
        || value.StartsWith("Partial.", StringComparison.Ordinal)
        || value.StartsWith("Not yet implemented.", StringComparison.Ordinal)
        || value.StartsWith("Planned — not present in the codebase.", StringComparison.Ordinal);

    private static string FindDocsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, "docs");
            if (File.Exists(Path.Combine(candidate, "index.md")))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository docs directory.");
    }

    private sealed class EmptyServices : IServiceProvider
    {
        internal static EmptyServices Instance { get; } = new();

        public object? GetService(Type serviceType) => null;
    }
}
