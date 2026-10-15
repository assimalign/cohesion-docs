# Web Event Source Support Tests

This example exercises `Assimalign.Cohesion.Sdk.Web` through the ApplicationModel SDK’s area-default test project.

> **Status:** Implemented.

The example reproduces
`cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/tests/WebEventSourceSupportTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
Use it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — NativeAOT EventSource support defaults to true when no value is supplied.
- **Case 2** — An explicit true before the import is preserved.
- **Case 3** — An explicit false before the import is preserved.
- **Case 4** — An explicit true after the import overrides the default.
- **Case 5** — An explicit false after the import overrides the default.
- **Case 6** — False after the import overrides true before it.
- **Case 7** — True after the import overrides false before it.
- **Case 8** — A command-line true overrides the unset project values.
- **Case 9** — A command-line false overrides the unset project values.
- **Case 10** — A command-line true overrides false before and after the import.
- **Case 11** — A command-line false overrides true before and after the import.

## Source example

```csharp
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Sdk.ApplicationModel.Tests;

/// <summary>Evaluates Sdk.Web's NativeAOT event-delivery default and consumer overrides with MSBuild.</summary>
public sealed class WebEventSourceSupportTests
{
    [Theory(DisplayName = "Cohesion Test [Sdk.Web] - NativeAOT EventSource support defaults on and preserves explicit choices")]
    [InlineData(null, null, null, "true")]
    [InlineData("true", null, null, "true")]
    [InlineData("false", null, null, "false")]
    [InlineData(null, "true", null, "true")]
    [InlineData(null, "false", null, "false")]
    [InlineData("true", "false", null, "false")]
    [InlineData("false", "true", null, "true")]
    [InlineData(null, null, "true", "true")]
    [InlineData(null, null, "false", "false")]
    [InlineData("false", "false", "true", "true")]
    [InlineData("true", "true", "false", "false")]
    public async Task Evaluate_PublishAotWithConsumerChoice_ShouldPreserveEventSourceSupport(
        string? beforeImport,
        string? afterImport,
        string? commandLine,
        string expected)
    {
        // Arrange — evaluate the shipped props directly, with no package feed or restore required.
        string root = FindRepositoryRoot();
        string props = Path.Combine(root, "sdks", "Assimalign.Cohesion.Sdk.Web", "Targets", "Sdk.Web.props");
        DirectoryInfo workspace = Directory.CreateTempSubdirectory("cohesion-web-eventsource-");
        try
        {
            string project = Path.Combine(workspace.FullName, "WebEventSourceSupport.proj");
            XElement contents = new("Project",
                new XElement("PropertyGroup",
                    new XElement("PublishAot", "true"),
                    beforeImport is null ? null : new XElement("EventSourceSupport", beforeImport)),
                new XElement("Import", new XAttribute("Project", props)),
                afterImport is null ? null : new XElement("PropertyGroup", new XElement("EventSourceSupport", afterImport)));
            await File.WriteAllTextAsync(project, new XDocument(contents).ToString());
            ProcessStartInfo start = new()
            {
                FileName = "dotnet",
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            start.ArgumentList.Add("msbuild");
            start.ArgumentList.Add(project);
            start.ArgumentList.Add("-nologo");
            start.ArgumentList.Add("-verbosity:quiet");
            start.ArgumentList.Add("-getProperty:EventSourceSupport");
            if (commandLine is not null)
            {
                start.ArgumentList.Add($"-p:EventSourceSupport={commandLine}");
            }
            start.Environment["MSBUILDDISABLENODEREUSE"] = "1";
            using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(1));

            // Act
            using Process process = Process.Start(start) ?? throw new InvalidOperationException("Could not start MSBuild evaluation.");
            Task<string> output = process.StandardOutput.ReadToEndAsync(timeout.Token);
            Task<string> error = process.StandardError.ReadToEndAsync(timeout.Token);
            try
            {
                await process.WaitForExitAsync(timeout.Token);

                // Assert
                process.ExitCode.ShouldBe(0, await output + Environment.NewLine + await error);
                (await output).Trim().ShouldBe(expected);
            }
            finally
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    await process.WaitForExitAsync(CancellationToken.None);
                }
            }
        }
        finally
        {
            workspace.Delete(recursive: true);
        }
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Assimalign.Cohesion.slnx")))
        {
            directory = directory.Parent;
        }
        return directory?.FullName ?? throw new InvalidOperationException("Could not locate the repository root.");
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/tests/WebEventSourceSupportTests.cs`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/tests/Assimalign.Cohesion.Sdk.ApplicationModel.Tests.csproj`.
