# Document Database Scope Tests

This example exercises `Assimalign.Cohesion.Database.Documents` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentDatabaseScopeTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Session_commands_and_collection_handles_cannot_address_another_database.

## Source example

```csharp
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Documents.Tests;

public sealed class DocumentDatabaseScopeTests
{
    [Theory]
    [InlineData("USE other")]
    [InlineData("CREATE DATABASE injected")]
    [InlineData("DROP DATABASE other")]
    [InlineData("SELECT * FROM other.items")]
    [InlineData("SHOW DATABASES")]
    [InlineData("SELECT * FROM other/items")]
    public async Task Session_commands_and_collection_handles_cannot_address_another_database(string command)
    {
        await using var engine = DocumentDatabaseEngine.Create(new());
        var own = (IDocumentDatabase)await engine.CreateDatabaseAsync("own");
        var other = (IDocumentDatabase)await engine.CreateDatabaseAsync("other");
        var ownCollection = await own.CreateCollectionAsync("items");
        var otherCollection = await other.CreateCollectionAsync("items");
        await using var session = await own.CreateSessionAsync();
        await using var otherSession = await other.CreateSessionAsync();
        await ownCollection.PutAsync(session, "one", "\"own\""u8.ToArray());
        await otherCollection.PutAsync(otherSession, "one", "\"other\""u8.ToArray());
        session.Database.Name.ShouldBe(own.Name);
        await Should.ThrowAsync<DatabaseException>(async () => await session.ExecuteAsync(command));
        await Should.ThrowAsync<DatabaseException>(async () => await otherCollection.GetAsync(session, "one"));
        await Should.ThrowAsync<DatabaseException>(async () => await otherCollection.PutAsync(session, "one", "null"u8.ToArray()));
        await Should.ThrowAsync<DatabaseException>(async () => await otherCollection.DeleteAsync(session, "one"));
        var scoped = (IDocumentDatabase)session.Database;
        await Should.ThrowAsync<DatabaseException>(async () => await scoped.GetCollectionAsync("other.items"));
        Encoding.UTF8.GetString((await otherCollection.GetAsync(otherSession, "one")).ShouldNotBeNull().Content.Span).ShouldBe("\"other\"");
        engine.TryGetDatabase("other", out _).ShouldBeTrue();
        engine.TryGetDatabase("injected", out _).ShouldBeFalse();
        var bound = await scoped.GetCollectionAsync("items");
        await session.DisposeAsync();
        await Should.ThrowAsync<DatabaseException>(async () => await bound.GetAsync(session, "one"));
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentDatabaseScopeTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/Assimalign.Cohesion.Database.Documents.Tests.csproj`.
