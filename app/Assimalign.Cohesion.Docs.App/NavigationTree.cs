using System;
using System.Collections.Generic;
using System.Linq;

using Assimalign.Cohesion.Viu.Markdown;

namespace CohesionDocs;

public sealed record NavigationRow(string Key, int Depth, string Title, string RoutePath,
    bool IsSection, bool IsExpanded, bool IsActive, bool IsVisible);

public sealed record BreadcrumbRow(string Key, string Title, string RoutePath, bool IsCurrent);

public sealed record PageLink(string Direction, string Title, string RoutePath);

internal sealed class NavigationTree
{
    private readonly MarkdownContentCatalog _catalog;
    private readonly List<Entry> _entries = [];
    private readonly Dictionary<MarkdownSection, IReadOnlyList<MarkdownSection>> _chains = [];
    private readonly Dictionary<string, int> _pagePositions = new(StringComparer.Ordinal);

    internal NavigationTree(MarkdownContentCatalog catalog)
    {
        _catalog = catalog;
        foreach (MarkdownSection section in catalog.Sections)
        {
            AddSection(section, 0, []);
        }
        for (int index = 0; index < catalog.Pages.Count; index++)
        {
            _pagePositions.Add(catalog.Pages[index].RoutePath, index);
        }
    }

    internal IReadOnlyList<NavigationRow> GetRows(HashSet<string> expanded, string currentPath)
        => _entries.Where(entry => entry.Ancestors.All(expanded.Contains))
            .Select(entry => new NavigationRow(entry.Key, entry.Depth, entry.Title,
            entry.RoutePath, entry.IsSection, entry.IsSection && expanded.Contains(entry.Key),
            string.Equals(entry.RoutePath, currentPath, StringComparison.Ordinal), true)).ToArray();

    internal HashSet<string> ExpandAncestors(HashSet<string> expanded, string currentPath)
    {
        HashSet<string> next = new(expanded, StringComparer.Ordinal);
        if (_catalog.TryGetPageByRoute(currentPath, out MarkdownPage? page)
            && page.Section is MarkdownSection section && _chains.TryGetValue(section, out var chain))
        {
            foreach (MarkdownSection ancestor in chain)
            {
                if (ancestor.SourceRelativePath.Length > 0)
                {
                    next.Add(SectionKey(ancestor));
                }
            }
        }
        return next;
    }

    internal IReadOnlyList<BreadcrumbRow> GetBreadcrumbs(string currentPath)
    {
        List<BreadcrumbRow> breadcrumbs = [];
        if (!_catalog.TryGetPageByRoute(currentPath, out MarkdownPage? page))
        {
            if (_catalog.TryGetPageByRoute("/", out MarkdownPage? home))
            {
                breadcrumbs.Add(new BreadcrumbRow("/", home.Title, "/", false));
            }
            breadcrumbs.Add(new BreadcrumbRow("not-found", "Page not found", string.Empty, true));
            return breadcrumbs.AsReadOnly();
        }
        if (page.Section is MarkdownSection section && _chains.TryGetValue(section, out var chain))
        {
            foreach (MarkdownSection ancestor in chain)
            {
                MarkdownPage? landing = FindLanding(ancestor);
                if (landing?.RoutePath != page.RoutePath)
                {
                    breadcrumbs.Add(new BreadcrumbRow(SectionKey(ancestor), ancestor.Title,
                        landing?.RoutePath ?? string.Empty, false));
                }
            }
        }
        breadcrumbs.Add(new BreadcrumbRow(page.RoutePath, page.Title, page.RoutePath, true));
        return breadcrumbs.AsReadOnly();
    }

    internal IReadOnlyList<PageLink> GetPageLinks(string currentPath)
    {
        List<PageLink> links = [];
        if (_pagePositions.TryGetValue(currentPath, out int position))
        {
            if (position > 0)
            {
                MarkdownPage previous = _catalog.Pages[position - 1];
                links.Add(new PageLink("Previous", previous.Title, previous.RoutePath));
            }
            if (position + 1 < _catalog.Pages.Count)
            {
                MarkdownPage next = _catalog.Pages[position + 1];
                links.Add(new PageLink("Next", next.Title, next.RoutePath));
            }
        }
        return links.AsReadOnly();
    }

    private void AddSection(MarkdownSection section, int depth, IReadOnlyList<MarkdownSection> parents)
    {
        MarkdownSection[] chain = [.. parents, section];
        _chains.Add(section, chain);
        bool root = section.SourceRelativePath.Length == 0;
        MarkdownPage? landing = FindLanding(section);
        string[] ancestorKeys = parents.Where(parent => parent.SourceRelativePath.Length > 0)
            .Select(SectionKey).ToArray();
        if (!root)
        {
            _entries.Add(new Entry(SectionKey(section), depth, section.Title,
                landing?.RoutePath ?? string.Empty, true, ancestorKeys));
        }

        // Match MarkdownContentCatalog.AddSection: pages and children share the same Order space.
        // The root is a navigation hub, so its children do not sit beneath an extra toggle.
        int childDepth = root ? depth : depth + 1;
        string[] childAncestors = root ? ancestorKeys : [.. ancestorKeys, SectionKey(section)];
        IEnumerable<(int Order, MarkdownPage? Page, MarkdownSection? Section)> navigation = section.Pages
            .Select(page => (page.Order, (MarkdownPage?)page, (MarkdownSection?)null))
            .Concat(section.Sections.Select(child => (child.Order, (MarkdownPage?)null, (MarkdownSection?)child)))
            .OrderBy(item => item.Order);
        foreach ((int _, MarkdownPage? page, MarkdownSection? child) in navigation)
        {
            if (page is not null && (root || !ReferenceEquals(page, landing)))
            {
                _entries.Add(new Entry("page:" + page.RoutePath, childDepth, page.Title,
                    page.RoutePath, false, childAncestors));
            }
            else if (child is not null)
            {
                AddSection(child, childDepth, chain);
            }
        }
    }

    private static MarkdownPage? FindLanding(MarkdownSection section)
    {
        string path = section.SourceRelativePath.Length == 0
            ? "index.md" : section.SourceRelativePath + "/index.md";
        return section.Pages.FirstOrDefault(page => page.SourceRelativePath == path);
    }

    private static string SectionKey(MarkdownSection section) => "section:" + section.SourceRelativePath;

    private sealed record Entry(string Key, int Depth, string Title, string RoutePath,
        bool IsSection, IReadOnlyList<string> Ancestors);
}
