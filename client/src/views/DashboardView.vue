<script setup lang="ts">
import LargestSupplierCard from '@/components/statistics/LargestSupplierCard.vue';
import MinimumOrdersProductCard from '@/components/statistics/MinimumOrdersProductCard.vue';
import ReorderNeededCard from '@/components/statistics/ReorderNeededCard.vue';
</script>

<template>
    <div class="p-6">
        <h1 class="text-3xl font-bold mb-6 text-surface-900 dark:text-surface-0">Inventory Dashboard</h1>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
            
            <LargestSupplierCard />

            <MinimumOrdersProductCard />
            
            <ReorderNeededCard />
        </div>

        <div class="p-4 border-2 border-dashed border-surface-200 rounded-lg dark:border-surface-700">
            <p class="text-surface-500 text-center">Charts or other detailed reports would go here.</p>
        </div>
    </div>
</template>

<style scoped>
</style>