export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref<string | null>(null)
  const expiresAt = ref<number | null>(null)

  const isAuthenticated = computed(() => {
    if (!accessToken.value || !expiresAt.value) {
      return false
    }
    return Date.now() < expiresAt.value
  })

  function setToken(token: string, expiresInSeconds: number) {
    accessToken.value = token
    expiresAt.value = Date.now() + expiresInSeconds * 1000
    if (import.meta.client) {
      localStorage.setItem('luckylot_access_token', token)
      localStorage.setItem('luckylot_expires_at', String(expiresAt.value))
    }
  }

  function clearToken() {
    accessToken.value = null
    expiresAt.value = null
    if (import.meta.client) {
      localStorage.removeItem('luckylot_access_token')
      localStorage.removeItem('luckylot_expires_at')
    }
  }

  function hydrate() {
    if (!import.meta.client) {
      return
    }
    const token = localStorage.getItem('luckylot_access_token')
    const expires = localStorage.getItem('luckylot_expires_at')
    if (token && expires) {
      accessToken.value = token
      expiresAt.value = Number(expires)
    }
  }

  return { accessToken, isAuthenticated, setToken, clearToken, hydrate }
})
