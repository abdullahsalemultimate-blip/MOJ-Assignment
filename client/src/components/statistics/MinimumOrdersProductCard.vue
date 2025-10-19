<script setup lang="ts">
import { ref, onMounted } from 'vue';
import Card from 'primevue/card';
import Tag from 'primevue/tag';
import Divider from 'primevue/divider';
import Skeleton from 'primevue/skeleton';
import { statisticService } from '@/services/StatisticService';
import type { MinimumOrderProductDto } from '@/models/Statistics';

const minOrderProduct = ref<MinimumOrderProductDto | null>(null);
const isLoading = ref(true);

onMounted(async () => {
    try {
        minOrderProduct.value = await statisticService.getMinimumOrdersProduct();
    } catch (error) {
        console.error('Error fetching min order product:', error);
    } finally {
        isLoading.value = false;
    }
});
</script>

<template>
    <Card class="shadow-lg min-h-[15rem] flex flex-col justify-between">
        <template #title>Product with Least Orders</template>
        <template #content>
            <div v-if="isLoading" class="flex flex-col gap-2">
                <Skeleton height="2rem" width="80%" class="mb-2"></Skeleton>
                <Skeleton height="1rem" width="50%"></Skeleton>
                <Skeleton height="1rem" width="70%"></Skeleton>
            </div>
            <div v-else-if="!minOrderProduct" class="flex flex-col items-center justify-center h-full text-center p-4">
                <i class="pi pi-chart-line text-4xl text-surface-400 mb-3"></i>
                <p class="text-surface-500">No product order data available.</p>
            </div>
            <div v-else class="flex flex-col items-center py-4">
                <i class="pi pi-star-fill text-4xl text-yellow-500 mb-3"></i>
                <h2 class="text-2xl font-bold text-surface-900 dark:text-surface-0 text-center">{{ minOrderProduct.name }}</h2>
                <p class="text-surface-600 dark:text-surface-400 text-sm mt-1">Supplier: {{ minOrderProduct.supplierName }}</p>
                <Divider class="w-2/3" />
                <div class="flex items-center gap-3 mt-4">
                    <Tag 
                        :value="`${minOrderProduct.unitsOnOrder}`" 
                        severity="info" 
                        rounded
                        icon="pi pi-send"
                    />
                    <span class="text-surface-600 dark:text-surface-400 font-semibold">Units on Order</span>
                </div>
            </div>
        </template>
    </Card>
</template>