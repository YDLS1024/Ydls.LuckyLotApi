# AGENTS.md

ABP Framework 10.4.0 layered "app" template, .NET 10.0, PostgreSQL, LeptonXLite theme. Built with ABP Studio 3.0.3.

## Stack

- **Framework**: ABP 10.4.0 (`common.props`, `Ydls.LuckyLotApi.abpmdl`). Target: `net10.0`. LangVersion: `latest`.
- **DB**: PostgreSQL via Npgsql. Dev connection string lives in `src/Ydls.LuckyLotApi.HttpApi.Host/appsettings.json` (hardcoded `localhost:5432`, user `ydls`). Override locally via `appsettings.secrets.json` (gitignored).
- **Solution files**: `Ydls.LuckyLotApi.slnx` (XML solution) and `Ydls.LuckyLotApi.abpsln` (ABP Studio). The legacy `.sln` and `.abpmdl` are part of the same template.
- **Host URL**: `https://localhost:44364` (`src/Ydls.LuckyLotApi.HttpApi.Host/Properties/launchSettings.json`).
- **Health endpoints**: `/health-status` (JSON), `/health-ui` (UI dashboard).
- **No UI framework** (`uiFramework: no-ui` in `Ydls.LuckyLotApi.abpsln`). The host is API-only with a single landing `Pages/Index.cshtml` and LeptonXLite theme assets under `wwwroot/libs`.

## Project layout (layered DDD)

Strict layer boundaries - see `.cursor/rules/framework/common/dependency-rules.mdc` for the full graph.

```
src/Ydls.LuckyLotApi.Domain.Shared      # constants, enums, localization, ETOs, MultiTenancy
src/Ydls.LuckyLotApi.Domain             # entities, domain services, settings, OpenIddict seeding
src/Ydls.LuckyLotApi.Application.Contracts # DTOs, service interfaces, permissions
src/Ydls.LuckyLotApi.Application        # app service impls (Mapperly), base LuckyLotApiAppService
src/Ydls.LuckyLotApi.EntityFrameworkCore # DbContext, repos, migrations
src/Ydls.LuckyLotApi.HttpApi            # REST controllers (currently only a base controller)
src/Ydls.LuckyLotApi.HttpApi.Client     # C# client proxies (regenerate with `abp generate-proxy`)
src/Ydls.LuckyLotApi.HttpApi.Host       # ASP.NET Core host (the entrypoint)
src/Ydls.LuckyLotApi.DbMigrator         # migration + seed console app

test/Ydls.LuckyLotApi.TestBase          # shared test base + data builder
test/Ydls.LuckyLotApi.{Domain,Application,EntityFrameworkCore}.Tests
test/Ydls.LuckyLotApi.HttpApi.Client.ConsoleTestApp
```

## Pre-seeded domain (do not re-add)

`src/Ydls.LuckyLotApi.Domain/LuckyLots/` already has three entities wired into `LuckyLotApiDbContext` and three migrations:

- `NumberThree` - `FullAuditedAggregateRoot<Guid>` (3-digit lottery drawing).
- `Experts` - `Entity<Guid>` (NOT an aggregate root - does not own a repository by default).
- `KillNumbers` - `Entity<Guid>` with FK to `Experts` (cascade delete set in `LuckyLotApiDbContext.OnModelCreating`).

Inconsistency to be aware of: only `NumberThree` is an aggregate root. New entities should pick the right base class deliberately.

## Repo-specific deviations from ABP defaults

- `addDefaultRepositories(includeAllEntities: true)` is enabled in `LuckyLotApiEntityFrameworkCoreModule.ConfigureServices` - the ABP framework convention in `.cursor/rules/framework/data/ef-core.mdc` recommends the opposite. Don't "fix" without confirming intent.
- Password policy is **intentionally relaxed** in `src/Ydls.LuckyLotApi.Domain/Identity/ChangeIdentityPasswordPolicySettingDefinitionProvider.cs` (no non-alphanumeric, no digit, no case requirements). Default admin password is `1q2w3E*` (`LuckyLotApiConsts.AdminPasswordDefaultValue`).
- Multi-tenancy **enabled** (`MultiTenancyConsts.IsEnabled = true`); the DbContext implements `ITenantManagementDbContext` and `IIdentityDbContext` via `[ReplaceDbContext]`.
- Mapperly is the mapper (no AutoMapper). See `.abpstudio/ai-rules/app.mdc` for the mapper shape, and `LuckyLotApiApplicationMappers.cs` for the file location.
- OpenIddict passphrase: see `AuthServer:CertificatePassPhrase` in your local `src/Ydls.LuckyLotApi.HttpApi.Host/appsettings.secrets.json`. For production generate `openiddict.pfx` with `dotnet dev-certs https -v -ep openiddict.pfx -p <passphrase>` (file is gitignored).

## Common commands

All run from the repo root unless noted.

```bash
# Restore JS/CSS client libs (LeptonXLite). Required after a fresh clone.
abp install-libs

# Build
dotnet build

# Run all tests (xUnit + Shouldly). EF Core tests use in-memory SQLite - no DB needed.
dotnet test

# Single test project
dotnet test test/Ydls.LuckyLotApi.Application.Tests

# Add EF Core migration
cd src/Ydls.LuckyLotApi.EntityFrameworkCore
dotnet ef migrations add <Name>     # IDesignTimeDbContextFactory is present; no -s needed

# Apply migrations + seed (requires reachable PostgreSQL)
cd ../..
dotnet run --project src/Ydls.LuckyLotApi.DbMigrator

# Run the API
dotnet run --project src/Ydls.LuckyLotApi.HttpApi.Host
# Swagger at https://localhost:44364/swagger
```

`etc/scripts/initialize-solution.ps1` and `migrate-database.ps1` are PowerShell wrappers used by ABP Studio "Initialize Solution" / "Migrate Database" tasks (`etc/abp-studio/run-profiles/Default.abprun.json`). On Linux/macOS run the underlying `dotnet`/`abp` commands directly - `pwsh` is not preinstalled.

## Adding a feature

Full walkthrough: `.cursor/rules/template/app.mdc` + `.cursor/rules/framework/common/development-flow.mdc`. The bare minimum order:

1. Entity in `src/Ydls.LuckyLotApi.Domain/<Feature>/` - inherit `AggregateRoot<Guid>` (or `Entity<Guid>` if child of an existing aggregate).
2. Constants in `src/Ydls.LuckyLotApi.Domain.Shared/`.
3. (Optional) custom repo interface in `Domain` + impl in `EntityFrameworkCore/EntityFrameworkCore/`.
4. Add `DbSet` + `builder.Entity<T>(b => { b.ToTable(...); b.ConfigureByConvention(); ... })` in `LuckyLotApiDbContext.OnModelCreating`.
5. `dotnet ef migrations add <Name>` (from `src/Ydls.LuckyLotApi.EntityFrameworkCore`) then `dotnet run --project src/Ydls.LuckyLotApi.DbMigrator`.
6. DTOs + `IXxxAppService` in `Application.Contracts`; impl in `Application` extending `LuckyLotApiAppService`. ABP auto-generates the REST controller - no manual controller needed.
7. Permissions in `Application.Contracts/Permissions/LuckyLotApiPermissions.cs` + provider registration in `LuckyLotApiPermissionDefinitionProvider.cs`.
8. Use `IClock`/`Clock.Now` (not `DateTime.Now`); use `CurrentUser`/`CurrentTenant` (already on base classes); use Mapperly mappers.

## ABP Studio MCP

A `abp-studio` MCP server is registered in `.vscode/mcp.json`. Tool reference: `.cursor/rules/mcp-studio.mdc`. The MCP requires an open solution in ABP Studio; without it every tool returns an error. The Default run profile lives at `etc/abp-studio/run-profiles/Default.abprun.json` (defines the host app and tasks; no `containers` are declared, so `start_containers` is a no-op).

## Reference docs already in this repo (read these, don't duplicate)

- ABP core / DI / base classes: `.cursor/rules/framework/common/abp-core.mdc`
- DDD patterns: `.cursor/rules/framework/common/ddd-patterns.mdc`
- Layer dependency rules: `.cursor/rules/framework/common/dependency-rules.mdc`
- Application services / DTOs: `.cursor/rules/framework/common/application-layer.mdc`
- Authorization / permissions: `.cursor/rules/framework/common/authorization.mdc`
- Multi-tenancy: `.cursor/rules/framework/common/multi-tenancy.mdc`
- Infrastructure (settings, features, cache, events, jobs): `.cursor/rules/framework/common/infrastructure.mdc`
- EF Core: `.cursor/rules/framework/data/ef-core.mdc`
- Testing: `.cursor/rules/framework/testing/patterns.mdc`
- CLI commands (`abp install-libs`, `generate-proxy`, `suite`, `update`): `.cursor/rules/framework/common/cli-commands.mdc`
- Layered template specifics: `.cursor/rules/template/app.mdc` and `.abpstudio/ai-rules/app.mdc`
- MCP tool workflows: `.cursor/rules/mcp-studio.mdc`

## Cautions

- Never put `DbContext` in `Application`; never reference `EntityFrameworkCore` from `Application` or `Domain`. Use repository interfaces.
- Custom repositories: only for aggregate roots. Child entities (like `KillNumbers`) are accessed through their parent aggregate.
- Migrations are auto-generated in `src/Ydls.LuckyLotApi.EntityFrameworkCore/Migrations/` - don't hand-edit.
- Keep `*.pfx`, `appsettings.secrets.json`, and any local `*.pfx` signing certs out of the repo (gitignored, see `.abpignore` for the ABP Studio AI view of what to ignore).
- Logs go to `src/Ydls.LuckyLotApi.HttpApi.Host/Logs/logs.txt` (Serilog, gitignored).
