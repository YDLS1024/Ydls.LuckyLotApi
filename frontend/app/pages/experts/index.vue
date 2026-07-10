<script setup lang="ts">
const api = useLuckyLotsApi()

const { data: experts, pending } = await useAsyncData('experts-list', () =>
  api.experts.list(0, 100)
)
</script>

<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold">专家列表</h1>
    <div v-if="pending" class="text-slate-500">加载中...</div>
    <div v-else class="grid gap-4 md:grid-cols-2">
      <NuxtLink
        v-for="expert in experts?.items ?? []"
        :key="expert.id"
        :to="`/experts/${expert.id}`"
        class="rounded-xl border bg-white p-5 transition hover:border-amber-400 dark:border-slate-800 dark:bg-slate-900"
      >
        <h2 class="text-lg font-semibold">{{ expert.nickname }}</h2>
        <p class="mt-2 text-sm text-slate-500">
          胜率 {{ expert.winningRate?.toFixed(1) ?? '-' }}%
        </p>
        <p class="mt-1 text-sm text-slate-500">
          已结算 {{ expert.killCount }} 期 · 全正确 {{ expert.hitCount }} 期
        </p>
      </NuxtLink>
    </div>
  </div>
</template>
