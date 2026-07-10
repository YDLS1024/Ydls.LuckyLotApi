<script setup lang="ts">
import type { KillNumbersDto } from '~/types/luckyLots'

definePageMeta({ layout: 'admin', middleware: 'admin' })

const api = useLuckyLotsApi()
const toast = useToast()

const digits = Array.from({ length: 10 }, (_, i) => i)

const form = reactive({
  killDate: new Date().toISOString().slice(0, 10),
  expertId: '',
  selected: [] as number[],
  isTrue: null as boolean | null
})

const editingId = ref<string | null>(null)
const loading = ref(false)

const { data: experts } = await useAsyncData('admin-kill-experts', () => api.experts.list(0, 200))
const { data, refresh, pending } = await useAsyncData('admin-kills', () =>
  api.killNumbers.list(0, 100)
)

const expertOptions = computed(() =>
  (experts.value?.items ?? []).map((e) => ({ label: e.nickname, value: e.id }))
)

function toggleDigit(n: number) {
  const idx = form.selected.indexOf(n)
  if (idx >= 0) {
    form.selected.splice(idx, 1)
  } else if (form.selected.length < 9) {
    form.selected.push(n)
  }
}

function resetForm() {
  editingId.value = null
  form.killDate = new Date().toISOString().slice(0, 10)
  form.expertId = expertOptions.value[0]?.value ?? ''
  form.selected = []
  form.isTrue = null
}

watch(expertOptions, (opts) => {
  if (!form.expertId && opts[0]) {
    form.expertId = opts[0].value
  }
}, { immediate: true })

function editRow(row: KillNumbersDto) {
  editingId.value = row.id
  form.killDate = row.killDate.slice(0, 10)
  form.expertId = row.expertId
  form.selected = [...row.killNumber]
  form.isTrue = row.isTrue ?? null
}

async function submit() {
  if (!form.expertId || form.selected.length === 0) {
    toast.add({ title: '请选择专家和至少一个杀号', color: 'warning' })
    return
  }
  loading.value = true
  try {
    const payload = {
      killDate: new Date(form.killDate).toISOString(),
      expertId: form.expertId,
      killNumber: [...form.selected].sort((a, b) => a - b),
      isTrue: form.isTrue
    }
    if (editingId.value) {
      await api.killNumbers.update(editingId.value, payload)
      toast.add({ title: '已更新杀号' })
    } else {
      await api.killNumbers.create(payload)
      toast.add({ title: '已添加杀号' })
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
  await api.killNumbers.remove(id)
  await refresh()
}
</script>

<template>
  <div class="space-y-8">
    <h1 class="text-2xl font-bold">杀号管理</h1>

    <UCard>
      <template #header>{{ editingId ? '编辑杀号' : '新增杀号' }}</template>
      <form class="space-y-4" @submit.prevent="submit">
        <div class="grid gap-4 md:grid-cols-3">
          <UFormField label="日期">
            <UInput v-model="form.killDate" type="date" />
          </UFormField>
          <UFormField label="专家">
            <USelect v-model="form.expertId" :items="expertOptions" />
          </UFormField>
          <UFormField label="是否命中">
            <USelect
              v-model="form.isTrue"
              :items="[
                { label: '待开奖', value: null },
                { label: '命中', value: true },
                { label: '未中', value: false }
              ]"
            />
          </UFormField>
        </div>
        <div>
          <p class="mb-2 text-sm font-medium">杀号（点击 0-9 多选）</p>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="n in digits"
              :key="n"
              type="button"
              class="rounded-full border px-3 py-2 text-sm transition"
              :class="
                form.selected.includes(n)
                  ? 'border-rose-500 bg-rose-500 text-white'
                  : 'border-slate-300 hover:border-amber-400'
              "
              @click="toggleDigit(n)"
            >
              {{ n }}
            </button>
          </div>
        </div>
        <div class="flex gap-2">
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
        class="rounded-xl border bg-white p-4 dark:border-slate-800 dark:bg-slate-900"
      >
        <div class="mb-2 flex items-center justify-between">
          <div>
            <p class="font-medium">{{ row.expertNickname }}</p>
            <p class="text-sm text-slate-500">{{ new Date(row.killDate).toLocaleDateString('zh-CN') }}</p>
          </div>
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost" @click="editRow(row)">编辑</UButton>
            <UButton size="xs" color="error" variant="ghost" @click="removeRow(row.id)">删除</UButton>
          </div>
        </div>
        <KillNumberBalls :numbers="row.killNumber" :is-true="row.isTrue" />
      </div>
    </div>
  </div>
</template>
