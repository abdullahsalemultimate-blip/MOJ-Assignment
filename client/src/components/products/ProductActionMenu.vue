<script setup lang="ts">
import { ref } from 'vue'
import Menu from 'primevue/menu'
import Button from 'primevue/button'
import type { ProductDto } from '@/models/Product'

const props = defineProps<{
  product: ProductDto
}>()

const emit = defineEmits(['edit', 'stock-adjust', 'delete', 'view-history'])

const menu = ref()

const items = ref([
  {
    label: 'Edit Product',
    icon: 'pi pi-pencil',
    command: () => emit('edit', props.product),
  },
  {
    separator: true,
  },
  {
    label: 'Adjust Stock', // Combines Increase/Decrease into one dialog trigger
    icon: 'pi pi-box',
    command: () => emit('stock-adjust', props.product),
  },
  {
    separator: true,
  },
  {
    label: 'View History',
    icon: 'pi pi-history',
    command: () => emit('view-history', props.product),
  },
  {
    label: 'Delete',
    icon: 'pi pi-trash',
    class: 'text-red-500',
    command: () => emit('delete', props.product),
  },
])

const toggleMenu = (event: Event) => {
  menu.value.toggle(event)
}
</script>

<template>
  <div>
    <Button
      icon="pi pi-ellipsis-v"
      text
      rounded
      @click="toggleMenu"
      aria-haspopup="true"
      aria-controls="product_menu"
    />
    <Menu
      ref="menu"
      :model="items"
      :popup="true"
      :id="'product_menu_' + product.id"
      class="min-w-40"
    >
      <template #item="{ item, props }">
        <a
          v-bind="props.action"
          :class="[
            item.class,
            'flex items-center p-3 text-color hover:bg-surface-100 dark:hover:bg-surface-700',
          ]"
        >
          <span :class="item.icon" />
          <span class="ml-2">{{ item.label }}</span>
        </a>
      </template>
    </Menu>
  </div>
</template>
