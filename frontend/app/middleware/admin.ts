export default defineNuxtRouteMiddleware(() => {
  const auth = useAuthStore()
  auth.hydrate()

  if (!auth.isAuthenticated) {
    return navigateTo('/admin/login')
  }
})
