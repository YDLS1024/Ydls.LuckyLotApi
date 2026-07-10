<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    value: number
    size?: 'sm' | 'md' | 'lg'
    variant?: 'draw' | 'kill' | 'hit' | 'miss' | 'neutral'
  }>(),
  { size: 'md', variant: 'draw' }
)

const sizeClass = computed(() => {
  if (props.size === 'sm') return 'h-8 w-8 text-sm'
  if (props.size === 'lg') return 'h-14 w-14 text-2xl'
  return 'h-10 w-10 text-lg'
})

const variantClass = computed(() => {
  switch (props.variant) {
    case 'kill':
      return 'bg-rose-500 text-white'
    case 'hit':
      return 'bg-emerald-500 text-white'
    case 'miss':
      return 'bg-slate-400 text-white'
    case 'neutral':
      return 'bg-slate-200 text-slate-800 dark:bg-slate-700 dark:text-white'
    default:
      return 'bg-amber-500 text-white'
  }
})
</script>

<template>
  <span
    class="inline-flex items-center justify-center rounded-full font-bold shadow-sm"
    :class="[sizeClass, variantClass]"
  >
    {{ value }}
  </span>
</template>
