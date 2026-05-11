# Log Server

A small log-collection HTTP API used to gather (web) client diagnostic logs.

The backend follows the conventions defined in [`CLAUDE.md`](./CLAUDE.md):
clean architecture, MediatR-based CQS, FluentValidation, and an
`ILogServerDbContext` abstraction over EF Core.

## Stack

- **.NET 9** / ASP.NET Core
- **MediatR 12** for command/query dispatching
- **FluentValidation 11** with a MediatR pipeline behavior
- **Entity Framework Core 9** (`Microsoft.EntityFrameworkCore.SqlServer`)
- **xUnit** + `Microsoft.AspNetCore.Mvc.Testing` for integration tests

## Solution layout

```
src/
  LogServer.Core/            Domain model (aggregates, domain events)
  LogServer.Application/     Commands, queries, validators, behaviors
  LogServer.Infrastructure/  EF Core DbContext + entity configurations
  LogServer.API/             ASP.NET Core controllers and host
test/
  IntegrationTests/          End-to-end API tests using WebApplicationFactory
```

## Build & test

```sh
dotnet build LogServer.sln
dotnet test  LogServer.sln
```

## Run

Provide a SQL Server connection string in `ConnectionStrings:LogServer`
(via `appsettings.json`, environment variables, or user secrets), then:

```sh
dotnet run --project src/LogServer.API
```
