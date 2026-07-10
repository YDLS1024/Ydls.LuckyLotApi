<script setup lang="ts">
const api = useLuckyLotsApi()

const { data: latestDraw } = await useAsyncData('home-latest-draw', async () => {
  const result = await api.numberThree.list(0, 1)
  return result.items[0] ?? null
})

const { data: experts } = await useAsyncData('home-experts', async () => {
  const result = await api.experts.list(0, 5)
  return result.items
})
</script>

<template>
  <div class="space-y-10">
    <section>
      <h1 class="mb-2 text-3xl font-bold">中国体育彩票 · 排列3</h1>
      <p class="text-slate-600 dark:text-slate-400">开奖记录与专家杀号信息展示</p>
    </section>

    <section class="rounded-2xl border bg-white p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <h2 class="mb-4 text-xl font-semibold">最新开奖</h2>
      <div v-if="latestDraw" class="flex flex-wrap items-center gap-4">
        <p class="text-slate-600 dark:text-slate-400">
          {{ new Date(latestDraw.openDate).toLocaleDateString('zh-CN') }}
        </p>
        <div class="flex gap-2">
          <LotteryBall :value="latestDraw.one" />
          <LotteryBall :value="latestDraw.two" />
          <LotteryBall :value="latestDraw.three" />
        </div>
      </div>
      <p v-else class="text-slate-500">暂无开奖数据</p>
    </section>

    <section class="rounded-2xl border bg-white p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div class="mb-4 flex items-center justify-between">
        <h2 class="text-xl font-semibold">专家胜率榜</h2>
        <NuxtLink to="/experts" class="text-sm text-amber-600 hover:underline">查看全部</NuxtLink>
      </div>
      <div v-if="experts?.length" class="space-y-3">
        <div
          v-for="expert in experts"
          :key="expert.id"
          class="flex items-center justify-between rounded-lg border px-4 py-3 dark:border-slate-800"
        >
          <NuxtLink :to="`/experts/${expert.id}`" class="font-medium hover:text-amber-600">
            {{ expert.nickname }}
          </NuxtLink>
          <div class="text-sm text-slate-500">
            胜率 {{ expert.winningRate?.toFixed(1) ?? '-' }}% · 命中 {{ expert.hitCount }}/{{ expert.killCount }}
          </div>
        </div>
      </div>
      <p v-else class="text-slate-500">暂无专家数据</p>
    </section>
  </div>
</template>
