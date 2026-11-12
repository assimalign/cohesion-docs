# DatabaseProvisioningProgress

An immutable observation of a model's current provisioning work or held declaration.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

```csharp
public sealed class DatabaseProvisioningProgress
{
    public DatabaseProvisioningProgress(string operation, string objectName, int position,
        int stepCount, bool isRunning, long? rowsProcessed = null, long? totalRows = null,
        string? failureCode = null, string? detail = null);
    public string Operation { get; }
    public string ObjectName { get; }
    public int Position { get; }
    public int StepCount { get; }
    public bool IsRunning { get; }
    public long? RowsProcessed { get; }
    public long? TotalRows { get; }
    public string? FailureCode { get; }
    public string? Detail { get; }
}
```

## Behavior and exceptions

[DatabaseInstance.Provisioning](database-instance.md#provisioning) publishes a nullable snapshot
through the protected SetProvisioningProgress method. Names and detail belong to the model; this type carries no SQL
executor or schema capability. Position is one-based. Row counts are nullable when unavailable.
Blank operation/object names throw ArgumentException. Position must be positive and no greater
than StepCount; negative row counts or RowsProcessed greater than a known TotalRows throw
ArgumentOutOfRangeException. Detail is optional, stored unchanged, and null when unavailable.

SQL reports queued/running joint work, per-table/per-phase row progress and failure codes through
this observation. A held Build reports Operation=SchemaHeld, the schema name, position 1, the
complete plan's step count and IsRunning=false, with CS03405 for an unsafe rollback or CS03105
for a never-seen destructive declaration without opt-in. Detail retains both hashes and every
refused step in full. A hold survives reopen, rewrite and refused explicit apply while the stored
hash stays unchanged; redeploying the stored or a successfully applied newer declaration clears it.

Hosting reports Degraded for held provisioning while the database stays online. Event and health
detail are capped at 4,096 UTF-16 code units including an ellipsis, without splitting surrogate
pairs; this bound does not truncate the model's Provisioning.Detail. See
[rollback policy](../assimalign-cohesion-database-sql/design.md#applied-schema-history-and-rollback).

## Example

```csharp
var progress = database.Provisioning;
if (progress?.FailureCode is string code)
    Console.WriteLine($"{code}: {progress.Detail}");
```

## Related reference

[Assembly](index.md) · [DatabaseInstance](database-instance.md) · [Hosting design](../assimalign-cohesion-database-hosting/design.md#held-declarations)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseProvisioningProgress.cs`.
- `cohesion/docs/resources/Database/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Provisioning/SqlSchemaProvisioner.Rollback.cs`.
