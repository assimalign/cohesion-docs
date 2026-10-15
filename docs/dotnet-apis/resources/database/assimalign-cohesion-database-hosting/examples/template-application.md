# Template application

The shipped `cohesion-database` template demonstrates the Database executable entry point.

> **Status:** Partial.

This is the `Program.cs` from `cohesion-database`. The template’s resource-name token is replaced
with `orders`. `Use` the matching `Assimalign.Cohesion.Sdk.Database` project from that template.

```csharp
using System;
using System.IO;
using Assimalign.Cohesion.Database.Hosting;
using Assimalign.Cohesion.Database.Sql;
using Assimalign.Cohesion.Database.Sql.Schema;
using Assimalign.Cohesion.Hosting;

DatabaseApplicationBuilder builder = DatabaseApplication.CreateBuilder(args);

builder.AddSql((_, options) =>
{
    options.EngineName = "orders";
    options.RootPath = Path.Combine(AppContext.BaseDirectory, "data");
    options.AddServer(engine => SqlDatabaseServer.Create(
        (SqlDatabaseEngine)engine, new SqlDatabaseServerOptions().Listen(new Uri("cohesion-db://localhost:5740"))));
});

SqlCompiledSchema schema = SqlSchema.Compile("customers", database =>
{
    database.Table<Customer>("Customers", table =>
    {
        table.Key(customer => customer.Id);
        table.Index(customer => customer.Email);
    });
    database.Principal(
        "orders-api",
        principal => principal.Grant(SqlPermission.ReadWrite, "Customers"));
});

builder.AddDatabase("orders", "customers", schema);

await using DatabaseApplication application = builder.Build();
await application.RunAsync();

internal sealed record Customer(long Id, string Name, string Email);
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-database/Program.cs`.
