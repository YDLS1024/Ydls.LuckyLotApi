<script setup lang="ts">
definePageMeta({ layout: 'admin' })

const auth = useAuthStore()
const { login } = useApiClient()

const username = ref('admin')
const password = ref('')
const error = ref('')
const loading = ref(false)

onMounted(() => {
  auth.hydrate()
  if (auth.isAuthenticated) {
    navigateTo('/admin')
  }
})

async function onSubmit() {
  error.value = ''
  loading.value = true
  try {
    await login(username.value, password.value)
    await navigateTo('/admin')
  } catch (e) {
    error.value = e instanceof Error ? e.message : '登录失败'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-md rounded-2xl border bg-white p-8 shadow-sm dark:border-slate-800 dark:bg-slate-900">
    <h1 class="mb-6 text-2xl font-bold">管理后台登录</h1>
    <form class="space-y-4" @submit.prevent="onSubmit">
      <UFormField label="用户名">
        <UInput v-model="username" autocomplete="username" />
      </UFormField>
      <UFormField label="密码">
        <UInput v-model="password" type="password" autocomplete="current-password" />
      </UFormField>
      <p v-if="error" class="text-sm text-red-500">{{ error }}</p>
      <UButton type="submit" block :loading="loading">登录</UButton>
    </form>
  </div>
</template>
