<script setup lang="ts">
definePageMeta({ layout: 'admin', middleware: 'admin' })

const api = useLuckyLotsApi()

const { data: draws } = await useAsyncData('admin-draw-count', () => api.numberThree.list(0, 1))
const { data: experts } = await useAsyncData('admin-expert-count', () => api.experts.list(0, 1))
const { data: kills } = await useAsyncData('admin-kill-count', () => api.killNumbers.list(0, 1))
</script>

<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold">管理概览</h1>
    <div class="grid gap-4 md:grid-cols-3">
      <UCard>
        <p class="text-sm text-slate-500">开奖记录</p>
        <p class="text-3xl font-bold">{{ draws?.totalCount ?? 0 }}</p>
        <NuxtLink to="/admin/draws" class="mt-2 inline-block text-sm text-amber-600">管理 →</NuxtLink>
      </UCard>
      <UCard>
        <p class="text-sm text-slate-500">专家</p>
        <p class="text-3xl font-bold">{{ experts?.totalCount ?? 0 }}</p>
        <NuxtLink to="/admin/experts" class="mt-2 inline-block text-sm text-amber-600">管理 →</NuxtLink>
      </UCard>
      <UCard>
        <p class="text-sm text-slate-500">杀号记录</p>
        <p class="text-3xl font-bold">{{ kills?.totalCount ?? 0 }}</p>
        <NuxtLink to="/admin/kill-numbers" class="mt-2 inline-block text-sm text-amber-600">管理 →</NuxtLink>
      </UCard>
    </div>
  </div>
</template>
