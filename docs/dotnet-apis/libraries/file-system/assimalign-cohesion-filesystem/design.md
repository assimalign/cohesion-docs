# Assimalign.Cohesion.FileSystem design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Interface contracts allow different backing stores to expose files, directories, enumeration, and
events. The factory composes providers by name. Shared provider contract tests define the portable
behavior; provider-specific watch timing remains documented separately.

## Root containment

A provider rooted somewhere confines every path-taking operation to that root (#1180). The rule, in
the order it runs:

1. **Resolve.** An empty path is the root. A relative path is taken from the root. A rooted path
   names a location in the provider's namespace and is used as given. `.` and `..` segments are
   resolved, so `../public/index.html` from a root of `/srv/public` is back inside.
2. **Check.** The resolved path must equal the root or lie under it on a segment boundary:
   `/srv/public2/x` does not lie under `/srv/public`. Anything else throws `FileSystemException`
   with `FileSystemErrorCode.PathOutsideRoot`.
3. **Use.** Only the checked path reaches the backing store, rebuilt from the root's own text, so
   nothing outside the root is read, created, changed, or even probed for existence. `Exists`
   throws too; it does not answer `false`, because an answer about a location outside the root is
   still an answer about that location.

Copy and move resolve both ends before either is touched.

| Provider | Containment | How "resolve" works | Comparison |
|---|---|---|---|
| Physical | Enforced (#1180) | `Path.GetFullPath` — the host's own normalization, so the check runs on exactly the string `System.IO` opens. A rooted path must be fully qualified. | Ordinal ignore-case on Windows and Apple platforms, ordinal elsewhere |
| InMemory | Enforced (#1180) | Lexical `.`/`..` resolution beneath the namespace root; `..` at the namespace root stays there, as `/..` is `/` on every host | Ordinal, ignore-case when `IgnoreCase` is set |
| IsolatedStorage | Not yet | Merges onto `/` and hands the rest to `IsolatedStorageFile`, which does not confine `..` | — |
| Aggregate | Delegates | Routes by mount prefix on segment boundaries; the mounted provider confines its own paths | Ordinal |

The physical provider's link policy and Windows specifics are on its
[design page](../assimalign-cohesion-filesystem-physical/design.md).

### Why containment lives in each provider, not in `FileSystemPath`

**Decision:** `Merge` stays a navigation helper, its bugs fixed, and each provider owns the
containment check for its own namespace (recorded for #1180).

- **`Merge` keeps navigating.** It is public API with callers that rely on climbing above the base:
  `IFileSystemDirectory.Exists` and `CreateSubdirectory` (the `FileSystemExtensions` members) merge a
  caller's `../sibling` onto a subdirectory's path, which is legitimate as long as the provider then
  keeps the result inside its root. Making `Merge` refuse to leave its base would break that
  navigation for every caller to fix a problem only providers have.
- **`Merge`'s own bugs are fixed regardless.** Its prefix test now requires a segment boundary and
  is ordinal (a culture-aware match cannot decide a boundary); it honors `ignoreCase`, which it used
  to ignore. A leading `..` can no longer remove the base's root — `Merge("/srv/public", "../../../x")`
  used to return the relative path `x` — and a merged path keeps its root exactly instead of
  doubling it into a UNC-shaped `//srv/...`. Only an exact `..` segment counts as a parent
  reference, so `..config` is an ordinary name.
- **Rejected: a containment flag on `Merge`, or a public lexical `TryResolveWithin` on
  `FileSystemPath`.** A lexical check in a platform-neutral path type is the wrong primitive for
  host paths. Windows maps a final `NUL` segment to the `\\.\NUL` device and trims trailing dots and
  spaces, so a path that is lexically under the root can open something that is not. Only the host's
  normalization (`Path.GetFullPath`) answers "what will actually be opened", and that belongs in the
  provider that opens it.
- **Rejected: returning `false` from `Exists` for an outside path.** It answers a question about a
  location outside the root, and it makes a refused path indistinguishable from a missing one, which
  hides misuse.

### Consumers

- **`FileSystemPath.Merge`** (including the `+` operator, which calls it): the in-memory provider's
  entry paths and tree walks, which merge one plain segment at a time (unchanged);
  `IsolatedStoragePathHelper.ToAbsolute`, which merges onto `/`, where a leading `..` now throws
  `ArgumentException` instead of resolving to the store root; `FileSystemExtensions` (`Exists` and
  `CreateSubdirectory` on a directory), where in-root `..` navigation keeps working — and now works
  on Unix too, where the doubled `//` root used to make the provider reject it — while the provider
  refuses whatever leaves its root; and `FileSystemConfigurationProvider`, which merges a relative
  file name onto the root for its watch glob (unchanged).
- **`PhysicalFileSystem` path members:** `Web.StaticFiles` (gates its own request paths and passes
  mount-relative paths; it also catches `FileSystemException`, so a refusal is a `404`),
  `Web.Hosting` and `Database.Hosting` (read-only content roots for `appsettings*.json`),
  `Database.Storage` (roots a provider at a file's own directory and passes the bare file name), and
  `Configuration.FileSystem`. None passes a path that leaves its root, so none changes behavior.

## `FileSystemPath`

`FileSystemPath` uses `/` as the separator on every OS, and an optional leading `/` marks an absolute
path. `Parse` admits `..` only at the start of a relative path; an interior `..` is an
`ArgumentException` at conversion time. `Merge` navigates: it joins a relative path onto a base,
returns a path that already lies under the base on a segment boundary, and applies leading `..`
segments, which may climb above the base but never above its root (drive, leading `/`, or UNC share);
`Merge("/srv/public", "../secret.txt")` is `/srv/secret.txt`, and a `..` that would climb above the
root throws `ArgumentException` instead of producing a relative or `//`-prefixed result. The prefix
match is ordinal (ignore-case when asked), and the `CultureInfo` parameter no
longer takes part in it. `Merge` is not a containment primitive, and no provider uses it to resolve
incoming paths.

## Error codes and throw helpers

Every provider raises `FileSystemException` with one of the explicit codes in
`FileSystemErrorCode`. `PathOutsideRoot` means the path resolves outside the provider's root; it is
raised before the backing store is touched. It was appended after `ReadOnly`, so every earlier code
keeps its numeric value, and `FileSystemExceptionTests` pins the ordinals.

`[DoesNotReturn]` static helpers (`ThrowFileNotFound`, `ThrowReadOnly`, and so on) keep providers
from constructing exceptions inline. The original helpers are declared on the exception type. New
ones are static extension members in `FileSystemExceptionExtensions`, following the repository rule
against throw-helper types; `ThrowPathOutsideRoot` is the first. Both are called the same way
(`FileSystemException.ThrowPathOutsideRoot(path)`), so the older helpers can move to the extension
container without changing callers. `ThrowPathOutsideRoot`'s message names only the caller's path,
never the root or the resolved host location.

## Adding a provider

Route every path-taking member of a new provider through one containment check that follows the
rule above, and test escapes against every member, as the physical and in-memory containment tests
do.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Assimalign.Cohesion.FileSystem.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/COMPATIBILITY.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Extensions/FileSystemExceptionExtensions.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src`.
