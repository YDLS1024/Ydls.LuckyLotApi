<script setup lang="ts">
const api = useLuckyLotsApi()
const page = ref(1)
const pageSize = 20

const { data, pending, refresh } = await useAsyncData(
  'draws-list',
  async () => api.numberThree.list((page.value - 1) * pageSize, pageSize),
  { watch: [page] }
)
</script>

<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold">开奖历史</h1>
    <div v-if="pending" class="text-slate-500">加载中...</div>
    <div v-else class="overflow-x-auto rounded-xl border bg-white dark:border-slate-800 dark:bg-slate-900">
      <table class="min-w-full text-left text-sm">
        <thead class="border-b bg-slate-50 dark:border-slate-800 dark:bg-slate-800/50">
          <tr>
            <th class="px-4 py-3">开奖日期</th>
            <th class="px-4 py-3">号码</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="draw in data?.items ?? []"
            :key="draw.id"
            class="border-b dark:border-slate-800"
          >
            <td class="px-4 py-3">{{ new Date(draw.openDate).toLocaleDateString('zh-CN') }}</td>
            <td class="px-4 py-3">
              <div class="flex gap-2">
                <LotteryBall :value="draw.one" size="sm" />
                <LotteryBall :value="draw.two" size="sm" />
                <LotteryBall :value="draw.three" size="sm" />
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div class="flex items-center gap-3">
      <UButton :disabled="page <= 1" variant="outline" @click="page--">上一页</UButton>
      <span class="text-sm text-slate-500">第 {{ page }} 页 · 共 {{ data?.totalCount ?? 0 }} 条</span>
      <UButton
        :disabled="(data?.items?.length ?? 0) < pageSize"
        variant="outline"
        @click="page++"
      >
        下一页
      </UButton>
    </div>
  </div>
</template>
