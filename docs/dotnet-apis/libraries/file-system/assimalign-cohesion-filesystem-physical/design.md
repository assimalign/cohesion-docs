# Assimalign.Cohesion.FileSystem.Physical design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem.Physical`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The provider delegates file operations to `System.IO` and reports capacity from the drive containing
its root. Change notifications use `FileSystemWatcher`, so latency follows the platform. Rooting and
provider ownership are explicit construction choices.

## Root containment

Every member that takes a path — `Exists`, `GetInfo`, `GetFile`, `GetDirectory`, `CreateFile`,
`CreateDirectory`, `DeleteFile`, `DeleteDirectory`, `CopyFile`, `Move`, and the directory-level
members that forward to them — passes it through one private resolver before any `System.IO` call.
The resolver turns the path into the full host path that will be opened and refuses it with
`FileSystemException` (`FileSystemErrorCode.PathOutsideRoot`) unless it is the root or lies under the
root on a segment boundary. Copy and move resolve both ends first.

The resolver runs in this order, and only its last step touches the disk:

1. **Relative or rooted.** A relative path is joined onto the root. A rooted path names a host
   location and must be fully qualified: on Windows `C:file` and `/file` resolve against the
   process's current directory or drive, so they cannot be shown to lie under the root and are
   refused outright.
2. **`Path.GetFullPath`** applies the host's own normalization, so the check runs on exactly the
   string that is opened. `.` and `..` collapse here and never reach the operating system. On
   Windows it also applies Win32's trimming of trailing dots and spaces, and a final `NUL` segment
   becomes the `\\.\NUL` device — which then fails the check instead of opening the device.
3. **The boundary check** compares ordinal ignore-case on Windows and the Apple platforms and
   ordinal elsewhere, the split the BCL uses for its own path comparisons. A volume root (`C:\`, `/`)
   contains everything on its volume.
4. **The checked path is rebuilt from the root's own text**, so a case-insensitive match can never
   hand the disk a differently cased root, which on a case-sensitive volume would be another
   directory. Then the `System.IO` call runs.

Nothing is decoded: `%2e%2e`, `..%2f`, and look-alike full stops are literal characters of an
in-root name. `RootDirectory.Parent` is `null`, as it is for every other provider: the host parent
lies outside the root, and entries enumerated or opened through it bypassed every path check.

With a root of `/srv/public`:

| Call | Result |
|---|---|
| `fs.CreateFile("css/site.css")` | `/srv/public/css/site.css` |
| `fs.GetFile("../public/css/site.css")` | `/srv/public/css/site.css` (back inside) |
| `fs.GetFile("/srv/public/css/site.css")` | `/srv/public/css/site.css` |
| `fs.GetFile("../secret.txt")` | `FileSystemException`: `PathOutsideRoot` |
| `fs.GetFile("/srv/public2/secret.txt")` | `FileSystemException`: `PathOutsideRoot` (sibling prefix) |
| `fs.GetFile("css/../secret.txt")` | `ArgumentException`: `FileSystemPath.Parse` refuses interior `..` |

Until #1180 the provider merged incoming paths onto its root with `FileSystemPath.Merge` and relied
on it for containment, which it never provided: a leading `..` climbed out of the root and a prefix
test without a segment boundary accepted sibling directories such as `public2/`. That shipped in
10.0.0-preview.1.

A path outside the root raises `PathOutsideRoot` from the resolver, before any `System.IO` call, so
there is no inner exception. Its message names only the caller's path, never the root or the
resolved host path.

### Symbolic links and junctions

**Containment is lexical.** It is decided on the normalized path text — the same text the disk APIs
receive — and the operating system then follows any symbolic link, junction, or mount point on that
path. A link inside the root that points outside it is therefore followed, and
`PhysicalFileSystemContainmentTests` pins that.

Rejected alternative: resolving links (`FileSystemInfo.ResolveLinkTarget`, segment by segment) and
checking the final target.

- **It is a time-of-check/time-of-use race.** Whoever can create a link inside the root can swap it
  between the check and the open, so the check would promise what it cannot keep. The only race-free
  mechanisms are kernel-side (`openat2` with `RESOLVE_BENEATH` on Linux; nothing equivalent on
  Windows), and .NET exposes neither.
- **It costs a system call per segment** on every operation.
- **It breaks legitimate layouts** that link shared content into a root, such as a web root whose
  `assets` directory is a link to a shared location.

The threat this provider defends against is an untrusted *path string* (CWE-22). Someone who can
create links inside the root already controls the root's content; the defense against that is the
operating system's: do not mount a root that untrusted parties can write to, and run under an
account that cannot read what it must not serve. No `..` ever reaches the operating system, so a link
cannot turn `link/..` into a different parent.

Case sensitivity follows the host file system for lookups; the containment check alone assumes the
platform default and rebuilds the checked path from the root's own text.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.FileSystem`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src/Assimalign.Cohesion.FileSystem.Physical.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/tests/PhysicalFileSystemContainmentTests.cs`.
