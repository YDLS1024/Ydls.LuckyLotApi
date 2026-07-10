<script setup lang="ts">
const auth = useAuthStore()
const { logout } = useApiClient()

onMounted(() => auth.hydrate())

const adminNav = [
  { label: '概览', to: '/admin' },
  { label: '开奖', to: '/admin/draws' },
  { label: '专家', to: '/admin/experts' },
  { label: '杀号', to: '/admin/kill-numbers' }
]

async function onLogout() {
  logout()
  await navigateTo('/admin/login')
}
</script>

<template>
  <div class="min-h-screen bg-slate-100 dark:bg-slate-950">
    <header class="border-b bg-white dark:border-slate-800 dark:bg-slate-900">
      <UContainer class="flex flex-wrap items-center justify-between gap-4 py-4">
        <div class="flex items-center gap-4">
          <NuxtLink to="/" class="font-bold text-amber-600">排列3杀号</NuxtLink>
          <span class="text-sm text-slate-500">管理后台</span>
        </div>
        <nav class="flex flex-wrap items-center gap-3 text-sm">
          <NuxtLink
            v-for="item in adminNav"
            :key="item.to"
            :to="item.to"
            active-class="font-semibold text-amber-600"
          >
            {{ item.label }}
          </NuxtLink>
          <UButton v-if="auth.isAuthenticated" size="xs" variant="ghost" @click="onLogout">
            退出
          </UButton>
        </nav>
      </UContainer>
    </header>
    <UContainer class="py-8">
      <slot />
    </UContainer>
  </div>
</template>
