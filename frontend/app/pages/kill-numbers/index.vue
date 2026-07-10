<script setup lang="ts">
const api = useLuckyLotsApi()

const { data: kills, pending } = await useAsyncData('kill-numbers-list', () =>
  api.killNumbers.list(0, 100)
)
</script>

<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold">杀号浏览</h1>
    <div v-if="pending" class="text-slate-500">加载中...</div>
    <div v-else class="space-y-4">
      <div
        v-for="item in kills?.items ?? []"
        :key="item.id"
        class="rounded-xl border bg-white p-5 dark:border-slate-800 dark:bg-slate-900"
      >
        <div class="mb-3 flex flex-wrap items-center justify-between gap-2">
          <div>
            <NuxtLink
              v-if="item.expertId"
              :to="`/experts/${item.expertId}`"
              class="font-semibold hover:text-amber-600"
            >
              {{ item.expertNickname || '未知专家' }}
            </NuxtLink>
            <p class="text-sm text-slate-500">
              {{ new Date(item.killDate).toLocaleDateString('zh-CN') }}
            </p>
          </div>
          <UBadge
            :color="item.isTrue === true ? 'success' : item.isTrue === false ? 'error' : 'neutral'"
            variant="subtle"
          >
            {{ item.isTrue === true ? '命中' : item.isTrue === false ? '未中' : '待开奖' }}
          </UBadge>
        </div>
        <KillNumberBalls :numbers="item.killNumber" :is-true="item.isTrue" />
      </div>
    </div>
  </div>
</template>
