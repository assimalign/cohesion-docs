# Glob

`Glob` is a lightweight, generic string pattern matcher.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `System.IO`.

Assembly: `Assimalign.Cohesion.Core`.

## Remarks

`Glob` is a lightweight, generic string pattern matcher. It is not limited to file paths. This is
called out explicitly because users often assume file-system globbing semantics, which can vary by
OS and can lead to surprising mismatches.

File systems impose platform-specific rules (case sensitivity, separators, reserved names). `Glob`
is designed to be reusable beyond file systems—for example, for registry paths, URLs, message topic
names, or any other string-based identifiers—while providing predictable, platform-agnostic
behavior.


## Patterns

The following core patterns are supported (adapted from Wikipedia):

| Wildcard | Description                                                           | Example       | Matches                                                  | Does not match                        |
| -------- | --------------------------------------------------------------------- | ------------- | -------------------------------------------------------- | ------------------------------------- |
| `*`      | Matches any number of characters, including none                      | `Law*`        | `Law`, `Laws`, `Lawyer`                                  |                                         |
| `?`      | Matches any single character                                          | `?at`         | `Cat`, `cat`, `Bat`, `bat`                               | `at`                                   |
| `[abc]`  | Matches one character given in the bracket                            | `[CB]at`      | `Cat`, `Bat`                                             | `cat`, `bat`                           |
| `[0-9]`  | Matches one character from the range given in the bracket             | `Letter[0-9]` | `Letter0`…`Letter9`                                      | `Letters`, `Letter`, `Letter10`        |
| `[!abc]` | Matches one character that is not given in the bracket                | `[!C]at`      | `Bat`, `bat`, `cat`                                      | `Cat`                                  |
| `[!3-5]` | Matches one character that is not from the range given in the bracket | `Letter[!3-5]` | `Letter1`, `Letter2`, `Letter6`…`Letter9`, `Letterx`   | `Letter3`, `Letter4`, `Letter5`, `Letterxx` |

In addition, `Glob` also supports:

| Wildcard | Description                                                                                                         | Example      | Matches                                               | Does not match               |
| -------- | ------------------------------------------------------------------------------------------------------------------- | ------------ | ----------------------------------------------------- | ---------------------------- |
| `**`     | Matches across zero or more hierarchical segments. Must be the only content of a segment when used with separators. | `/**/some.*` | `/foo/bar/baz/some.txt`, `/some.txt`, `/foo/some.txt` | `some.txt`, `C:/foo/som.txt` |

Notes:

- **Contract** — When you use `**` with path-like inputs, treat `/` as the segment separator in examples. If your domain uses a different separator, normalize input accordingly.

- **Contract** — For non-hierarchical strings, `**` behaves equivalently to `*`.


## Common Pitfalls

| Scenario | What happens | How to fix |
| --- | --- | --- |
| `**/*.{md,cs}` | Brace expansion is not interpreted as a single pattern. | Provide multiple patterns, e.g., `**/*.md` and `**/*.cs` (two separate globs). |
| Mixing OS-specific path rules | Assuming Windows or POSIX case/separator rules applies universally. | Normalize your input and be explicit about separators before matching. |
| Segment `**` misuse | Using `**` as part of a segment (e.g., `foo**bar`). | Use `**` as its own segment or prefer `*` within a segment. |

## Design Rationale: Keep `Glob` Generic

Keeping the `Glob` type generic (not tied to the file system) has a few clear benefits:

- **Contract** — Consistent semantics: Avoids OS-specific surprises (case sensitivity, special directories, drive letters).

- **Contract** — Reuse across domains: Works for registry paths, URLs, topics/queues, logical resource names, etc.

- **Contract** — Simpler mental model: A small, predictable pattern set that behaves the same everywhere.

If you are using `Glob` for actual file paths, consider normalizing path separators and, if needed,
applying your own case-normalization to align with the host file system’s behavior.

## Members

| Member | Responsibility |
|---|---|
| `Parse(string)` | Parses one pattern into a reusable matcher. |
| `Count` | Number of parsed tokens. |
| `Tokens` | Parsed token sequence. |
| `IsMatch(ReadOnlySpan<char>)` | Matches with invariant culture and case-sensitive comparison. |
| `IsMatch(ReadOnlySpan<char>, bool)` | Selects case sensitivity with invariant culture. |
| `IsMatch(ReadOnlySpan<char>, CultureInfo)` | Selects culture with case-sensitive comparison. |
| `IsMatch(ReadOnlySpan<char>, CultureInfo, bool)` | Selects both culture and case sensitivity. |
| `ToString()` | Returns the pattern representation. |
| Implicit conversion from `string` | Parses a pattern. |
| Implicit conversion to `string` | Returns the pattern representation. |

## Sources

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/docs/Assembly/System.IO/Glob/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/IO/Glob.cs`.
