// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: ['@nuxt/eslint', '@nuxt/ui', '@pinia/nuxt'],

  devtools: { enabled: true },

  css: ['~/assets/css/main.css'],

  devServer: {
    port: 3000
  },

  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'https://localhost:44364',
      oidcClientId: process.env.NUXT_PUBLIC_OIDC_CLIENT_ID || 'LuckyLotApi_App',
      oidcScope:
        process.env.NUXT_PUBLIC_OIDC_SCOPE || 'openid profile email roles LuckyLotApi offline_access'
    }
  },

  compatibilityDate: '2026-06-30',

  eslint: {
    config: {
      stylistic: {
        commaDangle: 'never',
        braceStyle: '1tbs'
      }
    }
  }
})
