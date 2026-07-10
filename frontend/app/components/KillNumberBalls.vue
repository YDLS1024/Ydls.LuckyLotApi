<script setup lang="ts">
const props = defineProps<{
  numbers: number[]
  isTrue?: boolean | null
}>()

function variantForNumber() {
  if (props.isTrue === true) return 'hit' as const
  if (props.isTrue === false) return 'miss' as const
  return 'kill' as const
}
</script>

<template>
  <div class="flex flex-wrap gap-2">
    <LotteryBall
      v-for="n in numbers"
      :key="n"
      :value="n"
      :variant="variantForNumber()"
      size="sm"
    />
  </div>
</template>
