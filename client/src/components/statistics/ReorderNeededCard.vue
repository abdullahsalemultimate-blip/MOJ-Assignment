<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import Card from 'primevue/card';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Tag from 'primevue/tag';
import Skeleton from 'primevue/skeleton';
import { statisticService } from '@/services/StatisticService';
import type { ReorderProductDto } from '@/models/Statistics';

const reorderList = ref<ReorderProductDto[]>([]);
const isLoading = ref(true);

const sortedReorderList = computed(() => {    
    return [...reorderList.value].sort((a, b) => 
        (a.unitsInStock - a.reorderLevel) - (b.unitsInStock - b.reorderLevel)
    );
});

onMounted(async () => {
    try {
        reorderList.value = await statisticService.getReorderNeeded();
    } catch (error) {
        console.error('Error fetching reorder list:', error);
    } finally {
        isLoading.value = false;
    }
});

const getStockSeverity = (unitsInStock: number, reorderLevel: number): 'danger' | 'warning' | 'info' => {
    if (unitsInStock <= 0) return 'danger';
    if (unitsInStock <= reorderLevel) return 'warning';
    return 'info';
};
</script>

<template>
    <Card class="shadow-lg min-h-[15rem] flex flex-col justify-between">
        <template #title>Products Needing Reorder</template>
        <template #content>
            <div v-if="isLoading" class="h-full">
                <Skeleton height="2.5rem" class="mb-2"></Skeleton>
                <Skeleton height="2.5rem" class="mb-2"></Skeleton>
                <Skeleton height="2.5rem"></Skeleton>
            </div>
            <div v-else-if="reorderList.length === 0" class="flex flex-col items-center justify-center h-full text-center p-4">
                <i class="pi pi-check-circle text-4xl text-green-500 mb-3"></i>
                <p class="text-surface-600">All products are stocked adequately.</p>
            </div>
            <div v-else class="h-full">
                <DataTable 
                    :value="sortedReorderList" 
                    :rows="5" 
                    :paginator="sortedReorderList.length > 5" 
                    class="p-datatable-sm"
                >
                    <Column field="name" header="Product" class="font-medium"></Column>
                    <Column field="unitsInStock" header="Stock" style="width: 30%;">
                        <template #body="{ data }">
                            <Tag 
                                :value="`${data.unitsInStock} / ${data.reorderLevel} units`"
                                :severity="getStockSeverity(data.unitsInStock, data.reorderLevel)"
                                icon="pi pi-exclamation-triangle"
                                class="text-xs"
                            />
                        </template>
                    </Column>
                </DataTable>
                <p class="text-right text-sm text-surface-500 mt-2">Total: {{ reorderList.length }} products</p>
            </div>
        </template>
    </Card>
</template>