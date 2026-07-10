import { defineConfig } from '@hey-api/openapi-ts'

export default defineConfig({
  input: process.env.OPENAPI_URL || 'https://localhost:44364/swagger/v1/swagger.json',
  output: {
    path: 'app/api/generated',
    format: 'prettier'
  },
  plugins: ['@hey-api/typescript', '@hey-api/sdk']
})
