# Ydls.LuckyLotApi

**Languages / 语言**：[简体中文](README.md) · [English](README.en.md)

## About this solution

This is a layered startup solution based on [Domain Driven Design (DDD)](https://abp.io/docs/latest/framework/architecture/domain-driven-design) practises. All the fundamental ABP modules are already installed. Check the [Application Startup Template](https://abp.io/docs/latest/solution-templates/layered-web-application) documentation for more info.

### Pre-requirements

* [.NET10.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 or 20](https://nodejs.org/en)
* **PostgreSQL 17** (via [Npgsql](https://www.npgsql.org/) + EF Core; recommended Docker image `postgres:17`, verified on **17.x** in development)
  * Default database name: `LuckyLotApi`
  * Default port: `5432`
  * Create the database first, then run `DbMigrator` to apply migrations and seed data

### Configurations

The solution ships with placeholder values in `appsettings.json` (for example `REPLACE_ME_IN_APPSETTINGS_SECRETS_JSON`). **Never commit real passwords or passphrases to source control.**

For local development, store secrets with [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets). Values are kept outside the repo (under your user profile) and override `appsettings.json` when the environment is `Development`.

#### HttpApi.Host

Run once per machine to register a `UserSecretsId` for the host project:

```bash
cd src/Ydls.LuckyLotApi.HttpApi.Host
dotnet user-secrets init
```

Set the connection string and other secrets (use `:` for nested JSON keys):

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=LuckyLotApi;User ID=ydls;Password=<your-db-password>;"
dotnet user-secrets set "AuthServer:CertificatePassPhrase" "<your-openiddict-pfx-passphrase>"
dotnet user-secrets set "StringEncryption:DefaultPassPhrase" "<your-string-encryption-passphrase>"
```

Useful commands:

```bash
dotnet user-secrets list    # show keys (values are masked in recent SDKs)
dotnet user-secrets remove "ConnectionStrings:Default"
dotnet user-secrets clear   # remove all secrets for this project
```

Then run the API:

```bash
dotnet run --project src/Ydls.LuckyLotApi.HttpApi.Host
```

`WebApplication.CreateBuilder` loads user secrets automatically in `Development` after `dotnet user-secrets init`. The host also calls `AddAppSettingsSecretsJson()`, which optionally loads a gitignored `appsettings.secrets.json` in the project folder—user secrets and that file are alternatives; you only need one.

#### DbMigrator

The migrator needs the same database connection string. Initialize and set secrets in its project directory:

```bash
cd src/Ydls.LuckyLotApi.DbMigrator
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=LuckyLotApi;User ID=ydls;Password=<your-db-password>;"
```

Apply migrations and seed data:

```bash
dotnet run --project src/Ydls.LuckyLotApi.DbMigrator
```

#### Production and CI

Do **not** use user secrets in production. Provide configuration via environment variables, Docker `--env-file`, or the paths described in `Jenkinsfile` (`REMOTE_API_ENV_FILE`, `REMOTE_MIGRATOR_ENV_FILE`). Use the same key names as above (for example `ConnectionStrings__Default` with double underscores for environment variables).

### Before running the application

* Run `abp install-libs` command on your solution folder to install client-side package dependencies. This step is automatically done when you create a new solution, if you didn't especially disabled it. However, you should run it yourself if you have first cloned this solution from your source control, or added a new client-side package dependency to your solution.
* Run `Ydls.LuckyLotApi.DbMigrator` to create the initial database. This step is also automatically done when you create a new solution, if you didn't especially disabled it. This should be done in the first run. It is also needed if a new database migration is added to the solution later.

#### Generating a Signing Certificate

In the production environment, you need to use a production signing certificate. ABP Framework sets up signing and encryption certificates in your application and expects an `openiddict.pfx` file in your application.

To generate a signing certificate, you can use the following command:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p <passphrase>
```

> `<passphrase>` is the password of the certificate, you can change it to any password you want. The chosen passphrase must match `AuthServer:CertificatePassPhrase` in your local user secrets (see **Configurations** above) or environment configuration—not the placeholder in `appsettings.json`.

It is recommended to use **two** RSA certificates, distinct from the certificate(s) used for HTTPS: one for encryption, one for signing.

For more information, please refer to: [OpenIddict Certificate Configuration](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios)

> Also, see the [Configuring OpenIddict](https://abp.io/docs/latest/Deployment/Configuring-OpenIddict#production-environment) documentation for more information.

### Solution structure

This is a layered monolith application that consists of the following applications:

* `Ydls.LuckyLotApi.DbMigrator`: A console application which applies the migrations and also seeds the initial data. It is useful on development as well as on production environment.
* `Ydls.LuckyLotApi.HttpApi.Host`: ASP.NET Core API application that is used to expose the APIs to the clients.
* `frontend/`: Nuxt 4 website for PL3 draw and kill-number display (public + admin).

#### Frontend (`frontend/`)

Website for China Sports Lottery Arrangement 3 draws and expert kill numbers. Public pages are anonymous; admin CRUD requires OpenIddict login.

```bash
# 1. Start the API (separate terminal)
dotnet run --project src/Ydls.LuckyLotApi.HttpApi.Host

# 2. Start the frontend
cd frontend
npm install
npm run dev    # http://localhost:3000
```

Regenerate the TypeScript API client from Swagger (API must be running):

```bash
cd frontend
npm run generate:api
```

See [frontend/README.md](frontend/README.md).

#### Test Projects

The `test` folder contains the following test projects:

* `Ydls.LuckyLotApi.Application.Tests`: Application layer tests.
* `Ydls.LuckyLotApi.Domain.Tests`: Domain layer tests.
* `Ydls.LuckyLotApi.EntityFrameworkCore.Tests`: Entity Framework Core integration tests.

## Deploying the application

Deploying an ABP application follows the same process as deploying any .NET or ASP.NET Core application. However, there are important considerations to keep in mind. For detailed guidance, refer to ABP's [deployment documentation](https://abp.io/docs/latest/Deployment/Index).

### Additional resources

You can see the following resources to learn more about your solution and the ABP Framework:

* [Web Application Development Tutorial](https://abp.io/docs/latest/tutorials/book-store/part-1)
* [Application Startup Template](https://abp.io/docs/latest/startup-templates/application/index)
