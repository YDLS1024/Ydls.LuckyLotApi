<script setup lang="ts">
import type { NumberThreeDto } from '~/types/luckyLots'

definePageMeta({ layout: 'admin', middleware: 'admin' })

const api = useLuckyLotsApi()
const toast = useToast()

const form = reactive({
  openDate: new Date().toISOString().slice(0, 10),
  one: 0,
  two: 0,
  three: 0
})

const editingId = ref<string | null>(null)
const loading = ref(false)

const { data, refresh, pending } = await useAsyncData('admin-draws', () =>
  api.numberThree.list(0, 50)
)

function resetForm() {
  editingId.value = null
  form.openDate = new Date().toISOString().slice(0, 10)
  form.one = 0
  form.two = 0
  form.three = 0
}

function editRow(row: NumberThreeDto) {
  editingId.value = row.id
  form.openDate = row.openDate.slice(0, 10)
  form.one = row.one
  form.two = row.two
  form.three = row.three
}

async function submit() {
  loading.value = true
  try {
    const payload = {
      openDate: new Date(form.openDate).toISOString(),
      one: Number(form.one),
      two: Number(form.two),
      three: Number(form.three)
    }
    if (editingId.value) {
      await api.numberThree.update(editingId.value, payload)
      toast.add({ title: '已更新开奖记录' })
    } else {
      await api.numberThree.create(payload)
      toast.add({ title: '已添加开奖记录' })
    }
    resetForm()
    await refresh()
  } catch (e) {
    toast.add({ title: '操作失败', description: String(e), color: 'error' })
  } finally {
    loading.value = false
  }
}

async function removeRow(id: string) {
  if (!confirm('确认删除？')) return
  await api.numberThree.remove(id)
  await refresh()
}
</script>

<template>
  <div class="space-y-8">
    <h1 class="text-2xl font-bold">开奖管理</h1>

    <UCard>
      <template #header>{{ editingId ? '编辑开奖' : '新增开奖' }}</template>
      <form class="grid gap-4 md:grid-cols-5" @submit.prevent="submit">
        <UFormField label="开奖日期">
          <UInput v-model="form.openDate" type="date" />
        </UFormField>
        <UFormField label="百位">
          <UInput v-model.number="form.one" type="number" min="0" max="9" />
        </UFormField>
        <UFormField label="十位">
          <UInput v-model.number="form.two" type="number" min="0" max="9" />
        </UFormField>
        <UFormField label="个位">
          <UInput v-model.number="form.three" type="number" min="0" max="9" />
        </UFormField>
        <div class="flex items-end gap-2">
          <UButton type="submit" :loading="loading">{{ editingId ? '保存' : '添加' }}</UButton>
          <UButton v-if="editingId" variant="ghost" @click="resetForm">取消</UButton>
        </div>
      </form>
    </UCard>

    <div v-if="pending" class="text-slate-500">加载中...</div>
    <div v-else class="overflow-x-auto rounded-xl border bg-white dark:border-slate-800 dark:bg-slate-900">
      <table class="min-w-full text-sm">
        <thead class="border-b bg-slate-50 dark:border-slate-800">
          <tr>
            <th class="px-4 py-3 text-left">日期</th>
            <th class="px-4 py-3 text-left">号码</th>
            <th class="px-4 py-3 text-right">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in data?.items ?? []" :key="row.id" class="border-b dark:border-slate-800">
            <td class="px-4 py-3">{{ new Date(row.openDate).toLocaleDateString('zh-CN') }}</td>
            <td class="px-4 py-3">
              <div class="flex gap-1">
                <LotteryBall :value="row.one" size="sm" />
                <LotteryBall :value="row.two" size="sm" />
                <LotteryBall :value="row.three" size="sm" />
              </div>
            </td>
            <td class="px-4 py-3 text-right">
              <UButton size="xs" variant="ghost" @click="editRow(row)">编辑</UButton>
              <UButton size="xs" color="error" variant="ghost" @click="removeRow(row.id)">删除</UButton>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
