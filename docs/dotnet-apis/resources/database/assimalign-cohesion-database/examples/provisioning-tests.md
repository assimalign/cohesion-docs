# Provisioning Tests

This example exercises `Assimalign.Cohesion.Database` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database/tests/ProvisioningTests.cs`. It retains
the test class and assertions so the setup, operation, and expected outcome stay together. `Use` it in
the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — CompiledSchema_WithDifferentModelShapes_HashesOnlyTheCanonicalDocument.
- **Case 2** — ObjectOwnership_SeparatesAdhocAndSchemaObjects.
- **Case 3** — SchemaMigrationResult_RetainsTheModelIndependentOutcome.

## Source example

```csharp
using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Tests;

public class ProvisioningTests
{
    [Fact]
    public void CompiledSchema_WithDifferentModelShapes_HashesOnlyTheCanonicalDocument()
    {
        const string document = "{\"shape\":\"model-owned\"}";
        CompiledSchema first = new TestSchema("first", EngineModel.Document, document);
        CompiledSchema second = new TestSchema("second", EngineModel.Graph, document);
        CompiledSchema changed = new TestSchema("first", EngineModel.Document, document + " ");

        first.Format.ShouldBe("tests/schema/v1");
        first.Name.ShouldBe("first");
        first.Model.ShouldBe(EngineModel.Document);
        first.AllowsDestructiveChanges.ShouldBeFalse();
        first.CanonicalDocument.ShouldBe(document);
        first.Hash.ShouldBe(System.Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(document))));
        second.Hash.ShouldBe(first.Hash);
        changed.Hash.ShouldNotBe(first.Hash);
    }

    [Fact]
    public void ObjectOwnership_SeparatesAdhocAndSchemaObjects()
    {
        ((byte)DatabaseObjectOwner.Adhoc).ShouldBe((byte)0);
        ((byte)DatabaseObjectOwner.Schema).ShouldBe((byte)1);

        var exception = new DatabaseObjectLockedException("Customers", "AppSchema", "DROP TABLE");
        exception.ShouldBeAssignableTo<DatabaseException>();
        exception.ObjectName.ShouldBe("Customers");
        exception.OwningSchema.ShouldBe("AppSchema");
        exception.Operation.ShouldBe("DROP TABLE");
        exception.Message.ShouldBe(
            "Object 'Customers' is owned by schema 'AppSchema' and cannot be changed by DROP TABLE. Alter the schema and redeploy it.");
    }

    [Fact]
    public void SchemaMigrationResult_RetainsTheModelIndependentOutcome()
    {
        var result = new SchemaMigrationResult("before", "after", 2, false);
        var (fromHash, toHash, operationCount, wasAlreadyApplied) = result;

        fromHash.ShouldBe("before");
        toHash.ShouldBe("after");
        operationCount.ShouldBe(2);
        wasAlreadyApplied.ShouldBeFalse();
        result.ShouldBe(new SchemaMigrationResult("before", "after", 2, false));
    }

    private sealed class TestSchema(string name, EngineModel model, string document)
        : CompiledSchema("tests/schema/v1", name, model, false)
    {
        public override string CanonicalDocument { get; } = document;
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/ProvisioningTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/Assimalign.Cohesion.Database.Tests.csproj`.
