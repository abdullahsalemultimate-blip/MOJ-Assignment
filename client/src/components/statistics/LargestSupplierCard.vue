<script setup lang="ts">
import { ref, onMounted } from 'vue';
import Card from 'primevue/card';
import Tag from 'primevue/tag';
import Divider from 'primevue/divider';
import Skeleton from 'primevue/skeleton';
import { statisticService } from '@/services/StatisticService';
import type { LargestSupplierDto } from '@/models/Statistics';

const largestSupplier = ref<LargestSupplierDto | null>(null);
const isLoading = ref(true);

onMounted(async () => {
    try {
        largestSupplier.value = await statisticService.getLargestSupplier();
    } catch (error) {
        console.error('Error fetching largest supplier:', error);
    } finally {
        isLoading.value = false;
    }
});
</script>

<template>
    <Card class="shadow-lg min-h-[15rem] flex flex-col justify-between">
        <template #title>Largest Supplier</template>
        <template #content>
            <div v-if="isLoading" class="flex flex-col gap-2">
                <Skeleton height="2rem" width="80%" class="mb-2"></Skeleton>
                <Skeleton height="1rem" width="50%"></Skeleton>
                <Skeleton height="1rem" width="70%"></Skeleton>
            </div>
            <div v-else-if="!largestSupplier" class="flex flex-col items-center justify-center h-full text-center p-4">
                <i class="pi pi-box text-4xl text-surface-400 mb-3"></i>
                <p class="text-surface-500">No supplier data available.</p>
            </div>
            <div v-else class="flex flex-col items-center py-4">
                <i class="pi pi-users text-4xl text-primary-500 mb-3"></i>
                <h2 class="text-4xl font-bold text-surface-900 dark:text-surface-0 mb-4">{{ largestSupplier.supplierName }}</h2>
                <Divider class="w-2/3" />
                <div class="flex items-center gap-3 mt-4">
                    <Tag 
                        :value="`${largestSupplier.totalProducts}`" 
                        severity="success" 
                        rounded
                        icon="pi pi-shopping-bag"
                    />
                    <span class="text-surface-600 dark:text-surface-400 font-semibold">Products Supplied</span>
                </div>
            </div>
        </template>
    </Card>
</template>