<script setup lang="ts">
import type { ExpertsDto, KillNumbersDto, NumberThreeDto } from '~/types/luckyLots'

const api = useLuckyLotsApi()

const { data: home } = await useAsyncData('home-dashboard', async () => {
  const drawResult = await api.numberThree.list(0, 1)
  const latestDraw: NumberThreeDto | null = drawResult.items[0] ?? null

  let periodDate: Date | null = null
  if (latestDraw?.openDate) {
    periodDate = new Date(latestDraw.openDate)
  } else {
    const latestKill = await api.killNumbers.list(0, 1)
    if (latestKill.items[0]) {
      periodDate = new Date(latestKill.items[0].killDate)
    }
  }

  if (!periodDate) {
    return {
      latestDraw,
      periodDate: null as string | null,
      items: [] as KillNumbersDto[],
      experts: [] as ExpertsDto[]
    }
  }

  const day = periodDate.toISOString().slice(0, 10)
  const killDateMin = `${day}T00:00:00.000Z`
  const killDateMax = `${day}T23:59:59.999Z`

  const [kills, experts] = await Promise.all([
    api.killNumbers.list(0, 50, undefined, 'KillDate DESC', killDateMin, killDateMax),
    api.experts.list(0, 100)
  ])

  const expertMap = new Map(experts.items.map((e) => [e.id, e]))
  const items = [...kills.items].sort((a, b) => {
    const rateA = expertMap.get(a.expertId)?.winningRate ?? -1
    const rateB = expertMap.get(b.expertId)?.winningRate ?? -1
    return rateB - rateA
  })

  return {
    latestDraw,
    periodDate: day,
    items,
    experts: experts.items
  }
})

function expertRate(expertId: string) {
  return home.value?.experts.find((e) => e.id === expertId)?.winningRate
}

function statusText(isTrue: boolean | null | undefined) {
  if (isTrue === true) return '全正确'
  if (isTrue === false) return '未中'
  return '待开奖'
}
</script>

<template>
  <div class="space-y-10">
    <section>
      <h1 class="mb-2 text-3xl font-bold">中国体育彩票 · 排列3</h1>
      <p class="text-slate-600 dark:text-slate-400">开奖记录与专家杀号信息展示</p>
    </section>

    <section class="rounded-2xl border bg-white p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <h2 class="mb-4 text-xl font-semibold">最新开奖</h2>
      <div v-if="home?.latestDraw" class="flex flex-wrap items-center gap-4">
        <p class="text-slate-600 dark:text-slate-400">
          {{ new Date(home.latestDraw.openDate).toLocaleDateString('zh-CN') }}
        </p>
        <div class="flex gap-2">
          <LotteryBall :value="home.latestDraw.one" />
          <LotteryBall :value="home.latestDraw.two" />
          <LotteryBall :value="home.latestDraw.three" />
        </div>
      </div>
      <p v-else class="text-slate-500">暂无开奖数据</p>
    </section>

    <section class="rounded-2xl border bg-white p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div class="mb-4 flex items-center justify-between">
        <div>
          <h2 class="text-xl font-semibold">当期专家杀号</h2>
          <p v-if="home?.periodDate" class="mt-1 text-sm text-slate-500">
            {{ new Date(home.periodDate).toLocaleDateString('zh-CN') }}
          </p>
        </div>
        <NuxtLink to="/experts" class="text-sm text-amber-600 hover:underline">查看全部专家</NuxtLink>
      </div>

      <div v-if="home?.items?.length" class="space-y-3">
        <div
          v-for="kill in home.items"
          :key="kill.id"
          class="flex flex-wrap items-center justify-between gap-3 rounded-lg border px-4 py-3 dark:border-slate-800"
        >
          <div class="min-w-0">
            <NuxtLink
              :to="`/experts/${kill.expertId}`"
              class="font-medium hover:text-amber-600"
            >
              {{ kill.expertNickname }}
            </NuxtLink>
            <p class="text-sm text-slate-500">
              胜率 {{ expertRate(kill.expertId)?.toFixed(1) ?? '-' }}%
              · {{ statusText(kill.isTrue) }}
            </p>
          </div>
          <KillNumberBalls :numbers="kill.killNumber" :is-true="kill.isTrue" />
        </div>
      </div>
      <p v-else class="text-slate-500">暂无当期杀号数据</p>
    </section>
  </div>
</template>
