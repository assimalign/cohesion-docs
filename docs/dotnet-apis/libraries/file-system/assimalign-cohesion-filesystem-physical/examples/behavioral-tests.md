# Example: Physical File System Tests

Exercise physical file-system operations, extended-length roots, and change notifications using
the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `PhysicalFileSystemWatchTests.cs` and `PhysicalFileSystemTests.cs` listings
from the package test project. Keep them in that project when running them: the project supplies its
package references, generated sources, and any shared fixtures. The using blocks below make the
test-framework import explicit where the original project supplies it globally.

## Code

```csharp
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.FileSystem.Physical.Tests;

/// <summary>
/// Verifies caller-owned native watcher disposal and concurrent notification cleanup.
/// </summary>
public class PhysicalFileSystemWatchTests
{
    [Theory(DisplayName = "Cohesion Test [PhysicalFileSystem] - Watch: Token and provider disposal are safe in either order")]
    [InlineData(true)]
    [InlineData(false)]
    public void Dispose_TokenAndProviderInEitherOrder_ShouldBeIdempotent(bool tokenFirst)
    {
        using var fixture = new Fixture();
        var token = fixture.FileSystem.Watch(Glob.Parse("**"));
        using var disposable = (IDisposable)token;
        using var changed = token.OnChange(_ => { }, null);
        using var created = token.OnCreate<object>(_ => { }, null);
        using var deleted = token.OnDelete<object>(_ => { }, null);
        using var renamed = token.OnRename<object>(_ => { }, null);

        Should.NotThrow(() =>
        {
            if (tokenFirst)
            {
                disposable.Dispose();
                fixture.FileSystem.CreateFile("still-usable.txt");
                fixture.FileSystem.Exists("still-usable.txt").ShouldBeTrue();
            }

            fixture.FileSystem.Dispose();
            disposable.Dispose();
            fixture.FileSystem.Dispose();
            disposable.Dispose();
            changed.Dispose();
            created.Dispose();
            deleted.Dispose();
            renamed.Dispose();
        });

        Should.Throw<ObjectDisposedException>(() => token.OnCreate<object>(_ => { }, null));
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Watch: A callback can dispose its own token")]
    public async Task Dispose_FromCallback_ShouldCompleteWithoutSubscriberMutationFailure()
    {
        using var fixture = new Fixture();
        var token = fixture.FileSystem.Watch(Glob.Parse("**"));
        using var disposable = (IDisposable)token;
        var disposed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var subscription = token.OnCreate<object>(_ =>
        {
            var exception = Record.Exception(disposable.Dispose);
            if (exception is null)
            {
                disposed.TrySetResult();
            }
            else
            {
                disposed.TrySetException(exception);
            }
        }, null);
        using var otherSubscription = token.OnCreate<object>(_ => { }, null);

        fixture.FileSystem.CreateFile("trigger.txt");

        await disposed.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Should.Throw<ObjectDisposedException>(() => token.OnCreate<object>(_ => { }, null));
        fixture.FileSystem.CreateFile("after-disposal.txt");
        fixture.FileSystem.Exists("after-disposal.txt").ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Watch: Removing a registration from a callback preserves notification")]
    public async Task OnCreate_UnsubscribeFromCallback_ShouldNotifyRemainingSubscriber()
    {
        using var fixture = new Fixture();
        var token = fixture.FileSystem.Watch(Glob.Parse("**"));
        using var disposable = (IDisposable)token;
        var notified = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        IDisposable? first = null;
        first = token.OnCreate<object>(_ => first!.Dispose(), null);
        using var firstSubscription = first;
        using var secondSubscription = token.OnCreate<object>(_ => notified.TrySetResult(), null);

        fixture.FileSystem.CreateFile("trigger.txt");

        await notified.Task.WaitAsync(TimeSpan.FromSeconds(10));
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Watch: Disposal is safe while a callback is in flight")]
    public async Task Dispose_WithCallbackInFlight_ShouldReleaseWatcherWithoutWaitingForUserCode()
    {
        using var fixture = new Fixture();
        var token = fixture.FileSystem.Watch(Glob.Parse("**"));
        using var disposable = (IDisposable)token;
        using var release = new ManualResetEventSlim();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var finished = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var subscription = token.OnCreate<object>(_ =>
        {
            entered.TrySetResult();
            finished.TrySetResult(release.Wait(TimeSpan.FromSeconds(10)));
        }, null);

        fixture.FileSystem.CreateFile("trigger.txt");
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));

        try
        {
            await Task.Run(disposable.Dispose).WaitAsync(TimeSpan.FromSeconds(5));
            fixture.FileSystem.CreateFile("after-disposal.txt");
            fixture.FileSystem.Exists("after-disposal.txt").ShouldBeTrue();
        }
        finally
        {
            release.Set();
            (await finished.Task.WaitAsync(TimeSpan.FromSeconds(10))).ShouldBeTrue();
        }
    }

    private sealed class Fixture : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "CohesionPhysicalWatchTests", Guid.NewGuid().ToString("N"));

        public Fixture()
        {
            Directory.CreateDirectory(_root);
            FileSystem = new PhysicalFileSystem(_root);
        }

        public PhysicalFileSystem FileSystem { get; }

        public void Dispose()
        {
            FileSystem.Dispose();
            Directory.Delete(_root, true);
        }
    }
}
```

## Walkthrough

- **Covered behavior** — Watch: Token and provider disposal are safe in either order.
- **Covered behavior** — Watch: A callback can dispose its own token.
- **Covered behavior** — Watch: Removing a registration from a callback preserves notification.
- **Covered behavior** — Watch: Disposal is safe while a callback is in flight.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Code: PhysicalFileSystemTests

```csharp
using System;
using System.IO;
using System.Linq;
using System.Text;

using Shouldly;

namespace Assimalign.Cohesion.FileSystem.Physical.Tests;

using Xunit;

public class PhysicalFileSystemTests
{
    private static string TestRoot => Path.Combine(Path.GetTempPath(), "CohesionPhysicalFsTests", Guid.NewGuid().ToString());

    private IFileSystem CreateFileSystem(string? root = null, bool isReadOnly = false)
    {
        var testRoot = root ?? TestRoot;
        Directory.CreateDirectory(testRoot);

        var factory = new FileSystemFactoryBuilder()
            .AddPhysicalFileSystem(options =>
            {
                options.Root = testRoot;
                options.IsReadOnly = isReadOnly;
            })
            .Build();

        return factory.Create("PhysicalFileSystem");
    }

    private void Cleanup(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    // ================================================================
    // Factory and Initialization Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Factory: Should create file system via factory builder")]
    public void Factory_Create_ShouldReturnFileSystem()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);

        try
        {
            var factory = new FileSystemFactoryBuilder()
                .AddPhysicalFileSystem(options =>
                {
                    options.Root = root;
                })
                .Build();

            var fileSystem = factory.Create("PhysicalFileSystem");

            Assert.NotNull(fileSystem);
            Assert.Equal("PhysicalFileSystem", fileSystem.Name);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Factory: Should create with default options")]
    public void Factory_CreateDefault_ShouldReturnFileSystem()
    {
        var factory = new FileSystemFactoryBuilder()
            .AddPhysicalFileSystem()
            .Build();

        var fileSystem = factory.Create("PhysicalFileSystem");

        Assert.NotNull(fileSystem);
        Assert.False(fileSystem.IsReadOnly);
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Properties: Should report size information")]
    public void Properties_Size_ShouldReportDriveInfo()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);

        try
        {
            var fileSystem = CreateFileSystem(root);

            Assert.True(fileSystem.Size.Length > 0);
            Assert.True(fileSystem.SpaceAvailable.Length > 0);
            Assert.True(fileSystem.SpaceUsed.Length >= 0);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Properties: RootDirectory should not be null")]
    public void Properties_RootDirectory_ShouldNotBeNull()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);

        try
        {
            var fileSystem = CreateFileSystem(root);

            Assert.NotNull(fileSystem.RootDirectory);
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // CreateDirectory Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - CreateDirectory: Should create a directory")]
    public void CreateDirectory_ValidPath_ShouldCreateDirectory()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var dir = fileSystem.CreateDirectory("testdir");

            Assert.NotNull(dir);
            Assert.True(Directory.Exists(Path.Combine(root, "testdir")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - CreateDirectory: Should throw on duplicate")]
    public void CreateDirectory_Duplicate_ShouldThrowConflict()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateDirectory("duplicate");

            var ex = Assert.Throws<FileSystemException>(() => fileSystem.CreateDirectory("duplicate"));
            Assert.Equal(FileSystemErrorCode.Conflict, ex.Code);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - CreateDirectory: Should create nested directories")]
    public void CreateDirectory_Nested_ShouldCreateAllLevels()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var dir = fileSystem.CreateDirectory("level1/level2/level3");

            Assert.NotNull(dir);
            Assert.True(Directory.Exists(Path.Combine(root, "level1", "level2", "level3")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // CreateFile Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - CreateFile: Should create a file")]
    public void CreateFile_ValidPath_ShouldCreateFile()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var file = fileSystem.CreateFile("test.txt");

            Assert.NotNull(file);
            Assert.True(File.Exists(Path.Combine(root, "test.txt")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - CreateFile: Should round-trip a file under an extended-length root past MAX_PATH")]
    public void CreateFile_ExtendedLengthRootPastMaxPath_ShouldRoundTrip()
    {
        if (!OperatingSystem.IsWindows())
        {
            return; // "\\?\" is the Win32 extended-length prefix; other platforms have no such form.
        }

        // Arrange
        var root = TestRoot;
        var deepRoot = Path.Combine([root, .. Enumerable.Repeat("directory-segment-0123456789", 9)]);
        Directory.CreateDirectory(deepRoot);
        var fileSystem = CreateFileSystem(@"\\?\" + deepRoot);
        var payload = Encoding.UTF8.GetBytes("extended-length");

        try
        {
            // Act
            var file = fileSystem.CreateFile("payload.txt");
            using (var stream = file.Open(FileMode.Open, FileAccess.Write))
            {
                stream.Write(payload);
            }

            // Assert
            var fullPath = Path.Combine(deepRoot, "payload.txt");
            fullPath.Length.ShouldBeGreaterThan(260);
            fileSystem.RootDirectory.Path.ToString().ShouldNotStartWith("//?/");
            File.ReadAllBytes(fullPath).ShouldBe(payload);
            fileSystem.Exists("payload.txt").ShouldBeTrue();
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - CreateFile: Should throw on duplicate")]
    public void CreateFile_Duplicate_ShouldThrowConflict()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateFile("duplicate.txt");

            var ex = Assert.Throws<FileSystemException>(() => fileSystem.CreateFile("duplicate.txt"));
            Assert.Equal(FileSystemErrorCode.Conflict, ex.Code);
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // DeleteDirectory Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - DeleteDirectory: Should delete directory")]
    public void DeleteDirectory_Existing_ShouldDelete()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateDirectory("todelete");
            Assert.True(Directory.Exists(Path.Combine(root, "todelete")));

            fileSystem.DeleteDirectory("todelete");

            Assert.False(Directory.Exists(Path.Combine(root, "todelete")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - DeleteDirectory: Should throw when not found")]
    public void DeleteDirectory_NotFound_ShouldThrow()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            Assert.Throws<FileSystemException>(() => fileSystem.DeleteDirectory("nonexistent"));
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // DeleteFile Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - DeleteFile: Should delete file")]
    public void DeleteFile_Existing_ShouldDelete()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateFile("todelete.txt");
            Assert.True(File.Exists(Path.Combine(root, "todelete.txt")));

            fileSystem.DeleteFile("todelete.txt");

            Assert.False(File.Exists(Path.Combine(root, "todelete.txt")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // Exists Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Exists: Should return true for existing file")]
    public void Exists_ExistingFile_ShouldReturnTrue()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateFile("exists.txt");

            Assert.True(fileSystem.Exists("exists.txt"));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Exists: Should return true for existing directory")]
    public void Exists_ExistingDirectory_ShouldReturnTrue()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateDirectory("existsdir");

            Assert.True(fileSystem.Exists("existsdir"));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Exists: Should return false for non-existing")]
    public void Exists_NonExisting_ShouldReturnFalse()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            Assert.False(fileSystem.Exists("nope.txt"));
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // GetInfo / GetFile / GetDirectory Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - GetFile: Should return file info")]
    public void GetFile_Existing_ShouldReturnFile()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateFile("info.txt");

            var file = fileSystem.GetFile("info.txt");

            Assert.NotNull(file);
            Assert.Equal("info.txt", file.Name.ToString());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - GetDirectory: Should return directory info")]
    public void GetDirectory_Existing_ShouldReturnDirectory()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateDirectory("infodir");

            var dir = fileSystem.GetDirectory("infodir");

            Assert.NotNull(dir);
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // File Open/Read/Write Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - File: Should write and read content")]
    public void File_WriteAndRead_ShouldRoundTrip()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var file = fileSystem.CreateFile("content.txt");
            var content = "Hello, Cohesion!"u8.ToArray();

            using (var stream = file.Open(FileMode.Open, FileAccess.Write))
            {
                stream.Write(content, 0, content.Length);
            }

            byte[] readBuffer = new byte[content.Length];
            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                stream.Read(readBuffer, 0, readBuffer.Length);
            }

            Assert.Equal(content, readBuffer);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - File: Size should reflect written content")]
    public void File_Size_ShouldReflectContent()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var file = fileSystem.CreateFile("sized.txt");
            var content = "12345"u8.ToArray();

            using (var stream = file.Open(FileMode.Open, FileAccess.Write))
            {
                stream.Write(content, 0, content.Length);
            }

            file = fileSystem.GetFile("sized.txt");

            Assert.Equal(5, file.Size.Length);
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // CopyFile Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - CopyFile: Should copy file content")]
    public void CopyFile_ValidSource_ShouldCopy()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var file = fileSystem.CreateFile("original.txt");
            var content = "Copy me"u8.ToArray();

            using (var stream = file.Open(FileMode.Open, FileAccess.Write))
            {
                stream.Write(content, 0, content.Length);
            }

            FileSystemPath srcPath = file.Path;
            FileSystemPath destPath = fileSystem.RootDirectory.Path.Join("copy.txt");

            fileSystem.CopyFile(srcPath, destPath);

            Assert.True(File.Exists(Path.Combine(root, "copy.txt")));
            Assert.Equal(content, File.ReadAllBytes(Path.Combine(root, "copy.txt")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // Move Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Move: Should move file")]
    public void Move_File_ShouldRelocate()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateFile("moveme.txt");
            fileSystem.CreateDirectory("dest");

            FileSystemPath srcPath = fileSystem.RootDirectory.Path.Join("moveme.txt");
            FileSystemPath destPath = fileSystem.RootDirectory.Path.Join("dest/moveme.txt");

            fileSystem.Move(srcPath, destPath);

            Assert.False(File.Exists(Path.Combine(root, "moveme.txt")));
            Assert.True(File.Exists(Path.Combine(root, "dest", "moveme.txt")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // ReadOnly Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - ReadOnly: Should throw on write operations")]
    public void ReadOnly_WriteOperations_ShouldThrow()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root, isReadOnly: true);

        try
        {
            Assert.True(fileSystem.IsReadOnly);
            AssertReadOnly(() => fileSystem.CreateDirectory("test"));
            AssertReadOnly(() => fileSystem.CreateFile("test.txt"));
            AssertReadOnly(() => fileSystem.DeleteDirectory("test"));
            AssertReadOnly(() => fileSystem.DeleteFile("test.txt"));
        }
        finally
        {
            Cleanup(root);
        }

        static void AssertReadOnly(Action action)
        {
            var exception = Assert.Throws<FileSystemException>(action);
            Assert.Equal(FileSystemErrorCode.ReadOnly, exception.Code);
        }
    }

    // ================================================================
    // Enumeration Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Enumerate: Should list created entries")]
    public void Enumerate_WithEntries_ShouldListAll()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateDirectory("dir1");
            fileSystem.CreateDirectory("dir2");
            fileSystem.CreateFile("file1.txt");

            var entries = fileSystem.EnumerateFileSystem().ToList();

            Assert.Equal(3, entries.Count);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - RootDirectory: GetFiles should list files")]
    public void RootDirectory_GetFiles_ShouldReturnFiles()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateFile("a.txt");
            fileSystem.CreateFile("b.txt");
            fileSystem.CreateDirectory("subdir");

            var files = fileSystem.RootDirectory.GetFiles().ToList();

            Assert.Equal(2, files.Count);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - RootDirectory: GetDirectories should list directories")]
    public void RootDirectory_GetDirectories_ShouldReturnDirectories()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.CreateDirectory("sub1");
            fileSystem.CreateDirectory("sub2");
            fileSystem.CreateFile("file.txt");

            var dirs = fileSystem.RootDirectory.GetDirectories().ToList();

            Assert.Equal(2, dirs.Count);
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // Directory Operations Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Directory: CreateFile via directory should work")]
    public void Directory_CreateFile_ShouldCreateInSubdir()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var dir = fileSystem.CreateDirectory("subdir");

            var file = dir.CreateFile("nested.txt");

            Assert.NotNull(file);
            Assert.True(File.Exists(Path.Combine(root, "subdir", "nested.txt")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Directory: CreateDirectory via directory should work")]
    public void Directory_CreateDirectory_ShouldCreateSubdir()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var dir = fileSystem.CreateDirectory("parent");

            var child = dir.CreateDirectory("child");

            Assert.NotNull(child);
            Assert.True(Directory.Exists(Path.Combine(root, "parent", "child")));
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // Timestamps Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Timestamps: Should have valid creation time")]
    public void Timestamps_CreatedOn_ShouldBeRecent()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var before = DateTime.UtcNow.AddSeconds(-2);

            var file = fileSystem.CreateFile("timed.txt");
            var info = fileSystem.GetFile("timed.txt");

            Assert.True(info.CreatedOn >= before);
            Assert.True(info.UpdatedOn >= before);
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // SetAttributes Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - SetAttributes: Should set file attributes")]
    public void SetAttributes_File_ShouldApply()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            var file = fileSystem.CreateFile("attrs.txt");
            var info = fileSystem.GetInfo("attrs.txt");

            info.SetAttributes(FileAttributes.ReadOnly);

            var updated = fileSystem.GetInfo("attrs.txt");
            Assert.True((updated.Attributes & FileAttributes.ReadOnly) != 0);

            // Cleanup attribute so file can be deleted
            info.SetAttributes(FileAttributes.Normal);
        }
        finally
        {
            Cleanup(root);
        }
    }

    // ================================================================
    // Dispose Tests
    // ================================================================

    [Fact(DisplayName = "Cohesion Test [PhysicalFileSystem] - Dispose: Should not throw")]
    public void Dispose_ShouldNotThrow()
    {
        var root = TestRoot;
        Directory.CreateDirectory(root);
        var fileSystem = CreateFileSystem(root);

        try
        {
            fileSystem.Dispose();
        }
        finally
        {
            Cleanup(root);
        }
    }
}
```

## Walkthrough: PhysicalFileSystemTests

- **Test entry point** — `Factory_Create_ShouldReturnFileSystem` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Factory_CreateDefault_ShouldReturnFileSystem` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Properties_Size_ShouldReportDriveInfo` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Properties_RootDirectory_ShouldNotBeNull` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CreateDirectory_ValidPath_ShouldCreateDirectory` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CreateDirectory_Duplicate_ShouldThrowConflict` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CreateDirectory_Nested_ShouldCreateAllLevels` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CreateFile_ValidPath_ShouldCreateFile` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CreateFile_ExtendedLengthRootPastMaxPath_ShouldRoundTrip` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CreateFile_Duplicate_ShouldThrowConflict` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `DeleteDirectory_Existing_ShouldDelete` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `DeleteDirectory_NotFound_ShouldThrow` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `DeleteFile_Existing_ShouldDelete` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Exists_ExistingFile_ShouldReturnTrue` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Exists_ExistingDirectory_ShouldReturnTrue` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Exists_NonExisting_ShouldReturnFalse` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `GetFile_Existing_ShouldReturnFile` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `GetDirectory_Existing_ShouldReturnDirectory` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `File_WriteAndRead_ShouldRoundTrip` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `File_Size_ShouldReflectContent` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `CopyFile_ValidSource_ShouldCopy` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Move_File_ShouldRelocate` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `ReadOnly_WriteOperations_ShouldThrow` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Enumerate_WithEntries_ShouldListAll` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RootDirectory_GetFiles_ShouldReturnFiles` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RootDirectory_GetDirectories_ShouldReturnDirectories` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Directory_CreateFile_ShouldCreateInSubdir` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Directory_CreateDirectory_ShouldCreateSubdir` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Timestamps_CreatedOn_ShouldBeRecent` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `SetAttributes_File_ShouldApply` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Dispose_ShouldNotThrow` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/tests/PhysicalFileSystemWatchTests.cs`.
- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/tests/Assimalign.Cohesion.FileSystem.Physical.Tests.csproj`.
- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/tests/PhysicalFileSystemTests.cs`.
