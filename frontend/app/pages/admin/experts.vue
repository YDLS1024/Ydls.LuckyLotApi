<script setup lang="ts">
import type { ExpertsDto } from '~/types/luckyLots'

definePageMeta({ layout: 'admin', middleware: 'admin' })

const api = useLuckyLotsApi()
const toast = useToast()

const form = reactive({ nickname: '' })
const editingId = ref<string | null>(null)
const loading = ref(false)

const { data, refresh, pending } = await useAsyncData('admin-experts', () =>
  api.experts.list(0, 100)
)

function resetForm() {
  editingId.value = null
  form.nickname = ''
}

function editRow(row: ExpertsDto) {
  editingId.value = row.id
  form.nickname = row.nickname
}

async function submit() {
  loading.value = true
  try {
    const payload = { nickname: form.nickname }
    if (editingId.value) {
      await api.experts.update(editingId.value, payload)
      toast.add({ title: '已更新专家' })
    } else {
      await api.experts.create(payload)
      toast.add({ title: '已添加专家' })
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
  if (!confirm('确认删除专家及其杀号？')) return
  await api.experts.remove(id)
  await refresh()
}
</script>

<template>
  <div class="space-y-8">
    <h1 class="text-2xl font-bold">专家管理</h1>
    <p class="text-sm text-slate-500">
      胜率由开奖后自动统计：杀号与开奖号码无交集（全正确）计入命中。
    </p>

    <UCard>
      <template #header>{{ editingId ? '编辑专家' : '新增专家' }}</template>
      <form class="grid gap-4 md:grid-cols-3" @submit.prevent="submit">
        <UFormField label="昵称">
          <UInput v-model="form.nickname" required />
        </UFormField>
        <div class="flex items-end gap-2 md:col-span-2">
          <UButton type="submit" :loading="loading">{{ editingId ? '保存' : '添加' }}</UButton>
          <UButton v-if="editingId" variant="ghost" @click="resetForm">取消</UButton>
        </div>
      </form>
    </UCard>

    <div v-if="pending" class="text-slate-500">加载中...</div>
    <div v-else class="space-y-3">
      <div
        v-for="row in data?.items ?? []"
        :key="row.id"
        class="flex items-center justify-between rounded-xl border bg-white px-4 py-3 dark:border-slate-800 dark:bg-slate-900"
      >
        <div>
          <p class="font-medium">{{ row.nickname }}</p>
          <p class="text-sm text-slate-500">
            胜率 {{ row.winningRate?.toFixed(1) ?? '-' }}% · 命中 {{ row.hitCount }}/{{ row.killCount }}
          </p>
        </div>
        <div class="flex gap-2">
          <UButton size="xs" variant="ghost" @click="editRow(row)">编辑</UButton>
          <UButton size="xs" color="error" variant="ghost" @click="removeRow(row.id)">删除</UButton>
        </div>
      </div>
    </div>
  </div>
</template>
