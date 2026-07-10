import type { TokenResponse } from '~/types/luckyLots'

export function useApiClient() {
  const config = useRuntimeConfig()
  const auth = useAuthStore()

  const baseUrl = config.public.apiBase as string

  async function request<T>(
    path: string,
    options: RequestInit & { auth?: boolean } = {}
  ): Promise<T> {
    const headers = new Headers(options.headers)
    if (!headers.has('Content-Type') && options.body) {
      headers.set('Content-Type', 'application/json')
    }

    if (options.auth !== false && auth.accessToken) {
      headers.set('Authorization', `Bearer ${auth.accessToken}`)
    }

    const response = await fetch(`${baseUrl}${path}`, {
      ...options,
      headers
    })

    if (!response.ok) {
      const text = await response.text()
      throw new Error(text || `HTTP ${response.status}`)
    }

    if (response.status === 204) {
      return undefined as T
    }

    return (await response.json()) as T
  }

  async function login(username: string, password: string) {
    const body = new URLSearchParams({
      grant_type: 'password',
      client_id: config.public.oidcClientId as string,
      username,
      password,
      scope: config.public.oidcScope as string
    })

    const response = await fetch(`${baseUrl}/connect/token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body
    })

    if (!response.ok) {
      throw new Error('登录失败，请检查用户名和密码')
    }

    const token = (await response.json()) as TokenResponse
    auth.setToken(token.access_token, token.expires_in)
    return token
  }

  function logout() {
    auth.clearToken()
  }

  return { request, login, logout, baseUrl }
}
