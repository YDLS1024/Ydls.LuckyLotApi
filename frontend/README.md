# 排列3杀号 · 前端

Nuxt 4 + Nuxt UI 前端，对接 `Ydls.LuckyLotApi` 后端。

## 环境要求

- Node.js 18+
- 后端 API 运行于 `https://localhost:44364`

## 快速开始

```bash
cd frontend
cp .env.example .env   # 可选，默认已指向本地 API
npm install
npm run dev            # http://localhost:3000
```

## 从 Swagger 自动生成 API 客户端

先启动后端 Host，再执行：

```bash
npm run generate:api
```

生成文件位于 `app/api/generated/`（已 gitignore）。当前业务 API 封装在 `app/composables/useLuckyLotsApi.ts`，生成后可逐步迁移到 SDK。

## 页面

| 路由 | 说明 |
|------|------|
| `/` | 首页：最新开奖、专家榜 |
| `/draws` | 开奖历史 |
| `/experts` | 专家列表 |
| `/experts/:id` | 专家杀号详情 |
| `/kill-numbers` | 杀号浏览 |
| `/admin/login` | 管理登录（默认 admin / 见后端种子密码） |
| `/admin/*` | 管理 CRUD |

## 认证

管理后台使用 OpenIddict **密码模式**（`LuckyLotApi_App` 客户端）。公开浏览接口无需登录。
