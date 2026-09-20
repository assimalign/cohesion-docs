using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Assimalign.Cohesion.Content.Markdown;
using Assimalign.Cohesion.Viu.Markdown;
using Assimalign.Viu.Components;

namespace CohesionDocs;

// The Content.Markdown node family has no table node. Transport tables through its fenced-code
// node, then expand only that marker in the immutable rendered tree. Never inject raw HTML.
internal static class PipeTables
{
    private static readonly Regex Delimiter = new(
        @"^\s*\|?\s*:?-{1,}:?\s*(\|\s*:?-{1,}:?\s*)*\|?\s*$",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    internal static string Extract(string markdown)
    {
        ArgumentNullException.ThrowIfNull(markdown);
        string[] lines = NormalizeLines(markdown);
        List<string> output = [];
        char fenceCharacter = '\0';
        int fenceLength = 0;

        for (int index = 0; index < lines.Length; index++)
        {
            string line = lines[index];
            if (fenceCharacter != '\0')
            {
                output.Add(line);
                if (TryFence(line, out char character, out int length, out string remainder)
                    && character == fenceCharacter && length >= fenceLength
                    && string.IsNullOrWhiteSpace(remainder))
                {
                    fenceCharacter = '\0';
                }
                continue;
            }

            if (TryFence(line, out char opening, out int openingLength, out string information)
                && (opening != '`' || !information.Contains('`')))
            {
                fenceCharacter = opening;
                fenceLength = openingLength;
                output.Add(line);
                continue;
            }

            if (IsIndentedCode(line) || index + 1 >= lines.Length
                || !HasSeparator(line) || !Delimiter.IsMatch(lines[index + 1])
                || SplitCells(line).Count != SplitCells(lines[index + 1]).Count)
            {
                output.Add(line);
                continue;
            }

            int end = index + 2;
            while (end < lines.Length && !IsIndentedCode(lines[end])
                && !string.IsNullOrWhiteSpace(lines[end]) && HasSeparator(lines[end])
                && !TryFence(lines[end], out _, out _, out _))
            {
                end++;
            }

            // A longer tilde fence cannot be closed by authored cell content.
            int longestRun = 2;
            for (int row = index; row < end; row++)
            {
                int run = 0;
                foreach (char character in lines[row])
                {
                    run = character == '~' ? run + 1 : 0;
                    longestRun = Math.Max(longestRun, run);
                }
            }
            string marker = new('~', longestRun + 1);
            output.Add(string.Empty);
            output.Add(marker + "gfm-table");
            for (int row = index; row < end; row++)
            {
                output.Add(lines[row]);
            }
            output.Add(marker);
            output.Add(string.Empty);
            index = end - 1;
        }
        return string.Join("\n", output);
    }

    internal static VirtualNode Expand(
        VirtualNode article, MarkdownVirtualNodeRenderer renderer, MarkdownPage page)
    {
        if (article is ElementNode element)
        {
            if (element.Name.LocalName == "pre" && element.Children.Count == 1
                && element.Children[0] is ElementNode code && code.Name.LocalName == "code"
                && code.Bindings.Any(binding => binding.Name.LocalName == "class"
                    && binding.Value is string value && value == "language-gfm-table"))
            {
                string raw = string.Concat(code.Children.OfType<TextNode>().Select(node => node.Text));
                VirtualNode? table = RenderTable(raw, renderer, page);
                if (table is not null)
                {
                    return table;
                }
            }

            return new ElementNode(element.Name, element.Bindings,
                element.Children.Select(child => Expand(child, renderer, page)), element.Directives,
                element.Key, element.MountReference, element.RenderPlan);
        }
        if (article is FragmentNode fragment)
        {
            return new FragmentNode(fragment.Children.Select(child => Expand(child, renderer, page)),
                fragment.Key, fragment.RenderPlan);
        }
        // Inline RouterLink slots contain no block tables and retain their original invocation.
        return article;
    }

    private static VirtualNode? RenderTable(
        string markdown, MarkdownVirtualNodeRenderer renderer, MarkdownPage page)
    {
        string[] lines = NormalizeLines(markdown.TrimEnd('\r', '\n'));
        if (lines.Length < 2 || !Delimiter.IsMatch(lines[1]))
        {
            return null;
        }
        IReadOnlyList<string> headers = SplitCells(lines[0]);
        IReadOnlyList<string> delimiters = SplitCells(lines[1]);
        if (headers.Count != delimiters.Count)
        {
            return null;
        }

        string[] alignments = delimiters.Select(cell =>
            cell.StartsWith(':') && cell.EndsWith(':') ? "center"
            : cell.EndsWith(':') ? "right" : "left").ToArray();
        List<VirtualNode> rows = [];
        for (int row = 2; row < lines.Length; row++)
        {
            rows.Add(RenderRow(SplitCells(lines[row]), false, alignments, renderer, page));
        }
        ElementNode table = Element("table",
            [Element("thead", [RenderRow(headers, true, alignments, renderer, page)]),
             Element("tbody", rows)], [Attribute("class", "markdown-table")]);
        return Element("div", [table],
            [Attribute("class", "table-scroll"), Attribute("tabindex", "0"),
             Attribute("role", "region"), Attribute("aria-label", "Scrollable table")]);
    }

    private static ElementNode RenderRow(IReadOnlyList<string> cells, bool header,
        IReadOnlyList<string> alignments, MarkdownVirtualNodeRenderer renderer, MarkdownPage page)
    {
        List<VirtualNode> children = [];
        for (int column = 0; column < alignments.Count; column++)
        {
            string text = column < cells.Count ? cells[column] : string.Empty;
            List<ElementBinding> bindings = [Attribute("class", "table-align-" + alignments[column])];
            if (header)
            {
                bindings.Add(Attribute("scope", "col"));
            }
            children.Add(Element(header ? "th" : "td", RenderInline(text, renderer, page), bindings));
        }
        return Element("tr", children);
    }

    private static IReadOnlyList<VirtualNode> RenderInline(
        string text, MarkdownVirtualNodeRenderer renderer, MarkdownPage page)
    {
        if (text.Length == 0)
        {
            return Array.Empty<VirtualNode>();
        }
        // The same renderer retains page-relative links and the application's fragment policy.
        if (renderer.Render(MarkdownText.Parse(text), page) is ElementNode article)
        {
            ElementNode? paragraph = article.Children.OfType<ElementNode>()
                .FirstOrDefault(node => node.Name.LocalName == "p");
            if (paragraph is not null)
            {
                return paragraph.Children;
            }
        }
        // Block-looking cell text is literal; table cells only admit inline content.
        return [new TextNode(text)];
    }

    private static IReadOnlyList<string> SplitCells(string line)
    {
        string text = line.Trim();
        int start = text.StartsWith('|') ? 1 : 0;
        int end = text.Length;
        if (end > start && text[end - 1] == '|' && !IsEscaped(text, end - 1))
        {
            end--;
        }
        List<string> cells = [];
        StringBuilder cell = new();
        for (int index = start; index < end; index++)
        {
            char character = text[index];
            if (character == '|')
            {
                if (!IsEscaped(text, index))
                {
                    cells.Add(cell.ToString().Trim());
                    cell.Clear();
                    continue;
                }
                // Unescape pipes before inline parsing, including inside code spans.
                cell.Length--;
            }
            cell.Append(character);
        }
        cells.Add(cell.ToString().Trim());
        return cells;
    }

    private static bool HasSeparator(string text)
    {
        for (int index = 0; index < text.Length; index++)
        {
            if (text[index] == '|' && !IsEscaped(text, index))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsEscaped(string text, int position)
    {
        int backslashes = 0;
        while (position > 0 && text[--position] == '\\')
        {
            backslashes++;
        }
        return (backslashes & 1) != 0;
    }

    private static bool TryFence(string line, out char character, out int length, out string remainder)
    {
        string text = line.TrimStart(' ');
        character = '\0';
        length = 0;
        remainder = string.Empty;
        if (line.Length - text.Length > 3 || text.Length < 3 || text[0] is not ('`' or '~'))
        {
            return false;
        }
        character = text[0];
        while (length < text.Length && text[length] == character)
        {
            length++;
        }
        remainder = text[length..];
        return length >= 3;
    }

    private static bool IsIndentedCode(string line) => line.StartsWith("    ", StringComparison.Ordinal)
        || line.StartsWith('\t');

    private static string[] NormalizeLines(string text) => text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

    private static ElementBinding Attribute(string name, string value) => ElementBinding.Attribute(new QualifiedName(name), value);

    private static ElementNode Element(string name, IEnumerable<VirtualNode> children,
        IEnumerable<ElementBinding>? bindings = null) => new(new QualifiedName(name), bindings, children);
}
