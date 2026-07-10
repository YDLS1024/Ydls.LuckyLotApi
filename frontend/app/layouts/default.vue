<script setup lang="ts">
const auth = useAuthStore()
onMounted(() => auth.hydrate())

const nav = [
  { label: '首页', to: '/' },
  { label: '开奖记录', to: '/draws' },
  { label: '专家', to: '/experts' },
  { label: '杀号', to: '/kill-numbers' }
]
</script>

<template>
  <div class="min-h-screen bg-slate-50 text-slate-900 dark:bg-slate-950 dark:text-slate-100">
    <header class="border-b border-slate-200 bg-white/80 backdrop-blur dark:border-slate-800 dark:bg-slate-900/80">
      <UContainer class="flex items-center justify-between gap-4 py-4">
        <NuxtLink to="/" class="text-lg font-bold text-amber-600">
          排列3杀号
        </NuxtLink>
        <nav class="flex flex-wrap items-center gap-3 text-sm">
          <NuxtLink
            v-for="item in nav"
            :key="item.to"
            :to="item.to"
            class="hover:text-amber-600"
            active-class="font-semibold text-amber-600"
          >
            {{ item.label }}
          </NuxtLink>
          <NuxtLink to="/admin" class="text-slate-500 hover:text-amber-600">管理</NuxtLink>
        </nav>
      </UContainer>
    </header>
    <main>
      <UContainer class="py-8">
        <slot />
      </UContainer>
    </main>
  </div>
</template>
