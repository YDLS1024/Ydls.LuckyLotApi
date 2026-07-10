<script setup lang="ts">
const route = useRoute()
const api = useLuckyLotsApi()
const id = route.params.id as string

const { data: expert, pending: expertPending } = await useAsyncData(`expert-${id}`, () =>
  api.experts.get(id)
)

const { data: kills, pending: killsPending } = await useAsyncData(`expert-kills-${id}`, () =>
  api.killNumbers.list(0, 100, id)
)
</script>

<template>
  <div class="space-y-6">
    <div v-if="expertPending" class="text-slate-500">加载中...</div>
    <template v-else-if="expert">
      <div>
        <NuxtLink to="/experts" class="text-sm text-amber-600">← 返回专家列表</NuxtLink>
        <h1 class="mt-2 text-2xl font-bold">{{ expert.nickname }}</h1>
        <p class="mt-2 text-slate-500">
          胜率 {{ expert.winningRate?.toFixed(1) ?? '-' }}% · 命中 {{ expert.hitCount }}/{{ expert.killCount }}
        </p>
      </div>

      <section class="rounded-xl border bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
        <h2 class="mb-4 text-lg font-semibold">历史杀号</h2>
        <div v-if="killsPending" class="text-slate-500">加载杀号记录...</div>
        <div v-else-if="kills?.items?.length" class="space-y-4">
          <div
            v-for="item in kills.items"
            :key="item.id"
            class="flex flex-wrap items-center justify-between gap-3 border-b pb-4 last:border-0 dark:border-slate-800"
          >
            <div>
              <p class="font-medium">{{ new Date(item.killDate).toLocaleDateString('zh-CN') }}</p>
              <p class="text-xs text-slate-500">
                {{ item.isTrue === true ? '命中' : item.isTrue === false ? '未中' : '待开奖' }}
              </p>
            </div>
            <KillNumberBalls :numbers="item.killNumber" :is-true="item.isTrue" />
          </div>
        </div>
        <p v-else class="text-slate-500">暂无杀号记录</p>
      </section>
    </template>
  </div>
</template>
