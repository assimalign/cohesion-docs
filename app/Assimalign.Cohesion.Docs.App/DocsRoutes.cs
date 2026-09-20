using System;
using System.Collections.Generic;

using Assimalign.Cohesion.Viu.Markdown;
using Assimalign.Viu.Components;
using Assimalign.Viu.Router;

namespace CohesionDocs;

// Adapted from Assimalign.Cohesion.Viu.Markdown/src/MarkdownRoutes.cs (10.0.0-beta.4).
// Route names, layout nesting, and catch-all arguments stay identical; the page component differs.
/// <summary>Creates eagerly declared, named route records without activating any components.</summary>
public static class DocsRoutes
{
    /// <summary>Creates one exact page record in catalog order, optionally nested beneath a layout.</summary>
    /// <param name="catalog">The immutable page catalog.</param>
    /// <param name="options">Composition options, or null for defaults.</param>
    /// <returns>A read-only route table; page arguments are fixed and the catch-all forwards the requested path.</returns>
    public static IReadOnlyList<RouteRecord> Create(MarkdownContentCatalog catalog, MarkdownRouteOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        options ??= new MarkdownRouteOptions();
        ArgumentNullException.ThrowIfNull(options.NamePrefix);
        List<RouteRecord> records = [];
        string layoutPath = MarkdownContentCatalog.NormalizeRoute(options.LayoutPath);
        foreach (MarkdownPage page in catalog.Pages)
        {
            string path = options.LayoutComponent is not null && page.RoutePath == layoutPath
                ? string.Empty : page.RoutePath;
            records.Add(new RouteRecord(path, options.NamePrefix + page.RoutePath,
                component: new ComponentNode(DocsPageView.Registration.Reference),
                argumentsResolver: RouteComponentArguments.FromValues(("route", page.RoutePath))));
        }
        if (options.IncludeNotFoundRoute)
        {
            records.Add(new RouteRecord("/:pathMatch(.*)*", options.NamePrefix + "not-found",
                component: new ComponentNode(options.NotFoundComponent ?? DocsPageView.Registration.Reference),
                argumentsResolver: static route => new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["route"] = route.Path,
                }));
        }
        if (options.LayoutComponent is not null)
        {
            return Array.AsReadOnly(new[]
            {
                new RouteRecord(layoutPath, options.NamePrefix + "layout",
                    children: records.AsReadOnly(), component: new ComponentNode(options.LayoutComponent)),
            });
        }
        return records.AsReadOnly();
    }
}
