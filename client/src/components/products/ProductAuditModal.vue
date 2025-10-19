<script setup lang="ts">
import { ref, watch } from 'vue';
import { TrailType } from "@/models/enums";
import Dialog from 'primevue/dialog';
import Timeline from 'primevue/timeline';
import Card from 'primevue/card';
import Tag from 'primevue/tag';
import ProgressSpinner from 'primevue/progressspinner';
import Accordion from 'primevue/accordion';
import AccordionTab from 'primevue/accordiontab';
import Button from 'primevue/button';

import { auditService } from '@/services/AuditService';
import { useProductStore } from '@/stores/productStore';
import type { AuditEntryDto } from '@/models/AuditEntryDto';

import { formatDate, getActionSeverity, formatAuditValue, getActionText } from '@/lib/auditFormatters';
import { useToast } from 'primevue/usetoast';

// --- Props & Emits ---
const props = defineProps<{
    visible: boolean;
    productId: number | null | undefined;
    productName: string | null | undefined;
}>();

const emit = defineEmits(['close']);
const toast = useToast();
const auditEntries = ref<AuditEntryDto[]>([]);
const isLoading = ref(false);
const productStore = useProductStore();

watch(() => props.visible, async (isVisible) => {
    if (isVisible && props.productId) {
        await fetchAuditData(props.productId);
    } else if (!isVisible) {
        // Clear state on close
        auditEntries.value = [];
    }
});

const fetchAuditData = async (productId: number) => {
    isLoading.value = true;
    try {
        const data = await auditService.getAuditTrail('Product', productId);
        auditEntries.value = data;
    } catch (error) {
        auditEntries.value = [];
    } finally {
        isLoading.value = false;
    }
};

const closeModal = () => {
    emit('close');
};


const readableProperty = (property: string): string => {
    if (!property) return '';
    const words = property.replace(/([A-Z])/g, ' $1').trim().split(' ');
    return words.map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
};

</script>

<template>
    <Dialog 
        :visible="visible" 
        :header="'Change History - ' + (productName ?? 'Product')" 
        :modal="true" 
        class="w-full lg:w-4/5 xl:w-3/4"
        :breakpoints="{'960px': '80vw', '640px': '90vw'}"
        :draggable="false"
        @update:visible="closeModal"
    >
        <div class="p-4">
            <div v-if="isLoading" class="flex justify-center items-center h-48">
                <ProgressSpinner />
                <p class="ml-4">Loading audit trail...</p>
            </div>
            <div v-else-if="auditEntries.length === 0" class="text-center p-8">
                <p class="text-xl text-surface-500">
                    <i class="pi pi-search text-3xl block mb-2"></i>
                    No audit history found for this product.
                </p>
            </div>
            
            <Timeline v-else :value="auditEntries" align="alternate" class="custom-timeline">
                <template #marker="{ item }">
                    <Tag :severity="getActionSeverity(item.action)" :value="getActionText(item.action)" />
                </template>
                <template #content="{ item }">
                    <Card class="mb-4 shadow-md">
                        <template #title>
                            {{ getActionText(item.action) }}
                            <span class="text-base font-normal ml-2 text-surface-600 dark:text-surface-400">
                                by User ID: {{ item.userId }}
                            </span>
                        </template>
                        <template #subtitle>
                            {{ formatDate(item.dateUtc) }} ({{ item.entityName }} ID: {{ item.primaryKey }})
                        </template>
                        <template #content>
                            
                            <div v-if="item.action === TrailType.Update && item.changes.length > 0">
                                <h4 class="font-semibold text-surface-700 dark:text-surface-300 mb-2">Changed Properties ({{ item.changes.length }}):</h4>
                                <ul class="list-disc ml-5 space-y-2">
                                    <li v-for="change in item.changes" :key="change.property" class="text-sm">
                                        <span class="font-medium text-primary-600 dark:text-primary-400">{{ readableProperty(change.property) }}:</span>
                                        <div class="inline-flex items-center ml-2">
                                            <Tag severity="secondary" :value="formatAuditValue(change.property, change.oldValue)" class="mr-2" />
                                            <i class="pi pi-arrow-right text-sm text-surface-400 mx-1"></i>
                                            <Tag severity="success" :value="formatAuditValue(change.property, change.newValue)" class="ml-2" />
                                        </div>
                                    </li>
                                </ul>
                            </div>
                            <div v-else-if="item.action === TrailType.Update && item.changes.length === 0" class="text-surface-500 italic">
                                No specific property changes were tracked for this update.
                            </div>

                            <Accordion v-if="item.action === TrailType.Create || item.action === TrailType.Delete" :activeIndex="0" class="mt-4">
                                <AccordionTab :header="item.action === TrailType.Create ? 'View Created Snapshot (JSON)' : 'View Deleted Snapshot (JSON)'">
                                    <pre class="p-2 border border-surface-200 dark:border-surface-700 rounded-md bg-surface-50 dark:bg-surface-800 overflow-x-auto text-sm">
{{ JSON.parse(item.fullSnapshot) }}
                                    </pre>
                                </AccordionTab>
                            </Accordion>
                        </template>
                    </Card>
                </template>
            </Timeline>
        </div>
        
        <template #footer>
            <Button label="Close" icon="pi pi-times" @click="closeModal" severity="secondary" />
        </template>
    </Dialog>
</template>

<style scoped>
/* Adjust timeline padding for better alignment in the card */
.custom-timeline :deep(.p-timeline-event-content) {
    padding-left: 1rem;
    padding-right: 1rem;
}
</style>