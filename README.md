# Ydls.LuckyLotApi

**语言 / Languages**：[简体中文](README.md) · [English](README.en.md)

## 关于本解决方案

本项目是基于[领域驱动设计（DDD）](https://abp.io/docs/latest/framework/architecture/domain-driven-design)的分层启动解决方案，已集成 ABP 框架核心模块。更多信息请参阅 [分层 Web 应用启动模板](https://abp.io/docs/latest/solution-templates/layered-web-application)文档。

### 环境要求

* [.NET 10.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 或 v20](https://nodejs.org/en)

### 配置说明

解决方案在 `appsettings.json` 中使用占位符（例如 `REPLACE_ME_IN_APPSETTINGS_SECRETS_JSON`）。**切勿将真实密码或口令提交到版本库。**

本地开发请使用 [.NET User Secrets](https://learn.microsoft.com/zh-cn/aspnet/core/security/app-secrets) 保存机密。机密存储在用户目录下，不会进入仓库；在 `Development` 环境下会覆盖 `appsettings.json` 中的同名配置。

#### HttpApi.Host

每台机器只需执行一次，为 Host 项目注册 `UserSecretsId`：

```bash
cd src/Ydls.LuckyLotApi.HttpApi.Host
dotnet user-secrets init
```

设置连接字符串及其他机密（嵌套 JSON 键使用 `:` 分隔）：

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=LuckyLotApi;User ID=ydls;Password=<你的数据库密码>;"
dotnet user-secrets set "AuthServer:CertificatePassPhrase" "<openiddict.pfx-口令>"
dotnet user-secrets set "StringEncryption:DefaultPassPhrase" "<字符串加密口令>"
```

常用命令：

```bash
dotnet user-secrets list    # 查看已配置的键（新版 SDK 会屏蔽值）
dotnet user-secrets remove "ConnectionStrings:Default"
dotnet user-secrets clear   # 清除本项目的全部机密
```

启动 API：

```bash
dotnet run --project src/Ydls.LuckyLotApi.HttpApi.Host
```

执行 `dotnet user-secrets init` 后，`WebApplication.CreateBuilder` 会在 `Development` 环境下自动加载 user secrets。Host 还调用了 `AddAppSettingsSecretsJson()`，可额外加载项目目录下 gitignored 的 `appsettings.secrets.json`——user secrets 与该文件二选一即可。

#### DbMigrator

数据库迁移工具需要相同的连接字符串。在其项目目录中初始化并设置机密：

```bash
cd src/Ydls.LuckyLotApi.DbMigrator
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=LuckyLotApi;User ID=ydls;Password=<你的数据库密码>;"
```

执行迁移并种子数据：

```bash
dotnet run --project src/Ydls.LuckyLotApi.DbMigrator
```

#### 生产环境与 CI

生产环境**不要**使用 user secrets。请通过环境变量、Docker `--env-file`，或 `Jenkinsfile` 中描述的路径（`REMOTE_API_ENV_FILE`、`REMOTE_MIGRATOR_ENV_FILE`）提供配置。键名与上文相同；环境变量中使用双下划线，例如 `ConnectionStrings__Default`。

### 运行前准备

* 在解决方案根目录执行 `abp install-libs`，安装前端依赖。首次克隆仓库或新增客户端包后需手动执行。
* 运行 `Ydls.LuckyLotApi.DbMigrator` 创建初始数据库。首次运行必须执行；后续新增 EF Core 迁移后也需再次运行。

#### 生成签名证书

生产环境需使用正式签名证书。ABP 会在应用中配置签名与加密证书，并期望存在 `openiddict.pfx` 文件。

生成签名证书：

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p <口令>
```

> `<口令>` 为证书密码，可自定义。该口令须与本地 user secrets 中的 `AuthServer:CertificatePassPhrase`（见上文**配置说明**）或环境配置一致，而非 `appsettings.json` 中的占位符。

建议为加密与签名各使用一张 RSA 证书，且与 HTTPS 证书分开。

更多信息：[OpenIddict 证书配置](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios)

> 另请参阅 ABP [配置 OpenIddict](https://abp.io/docs/latest/Deployment/Configuring-OpenIddict#production-environment) 文档。

### 解决方案结构

分层单体应用，包含以下可执行项目：

* `Ydls.LuckyLotApi.DbMigrator`：控制台应用，执行数据库迁移并种子初始数据；开发与生产均可使用。
* `Ydls.LuckyLotApi.HttpApi.Host`：ASP.NET Core API 宿主，对外暴露接口。
* `frontend/`：Nuxt 4 排列3杀号展示网站（公开浏览 + 管理后台）。

#### 前端（frontend/）

中国体育彩票排列3开奖与杀号信息展示站点。公开页无需登录；管理后台通过 OpenIddict 登录录入数据。

```bash
# 1. 启动后端 API（另开终端）
dotnet run --project src/Ydls.LuckyLotApi.HttpApi.Host

# 2. 启动前端
cd frontend
npm install
npm run dev    # http://localhost:3000
```

从 Swagger 自动生成 TypeScript 客户端（需 API 已运行）：

```bash
cd frontend
npm run generate:api
```

详见 [frontend/README.md](frontend/README.md)。

#### 测试项目

`test` 目录包含：

* `Ydls.LuckyLotApi.Application.Tests`：应用层测试
* `Ydls.LuckyLotApi.Domain.Tests`：领域层测试
* `Ydls.LuckyLotApi.EntityFrameworkCore.Tests`：EF Core 集成测试

## 部署

ABP 应用部署方式与普通 .NET / ASP.NET Core 应用相同。详见 ABP [部署文档](https://abp.io/docs/latest/Deployment/Index)。

### 延伸阅读

* [Web 应用开发教程](https://abp.io/docs/latest/tutorials/book-store/part-1)
* [应用启动模板](https://abp.io/docs/latest/startup-templates/application/index)
