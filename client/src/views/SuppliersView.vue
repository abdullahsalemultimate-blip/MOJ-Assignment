<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useSupplierStore } from '@/stores/supplierStore'
import { useConfirm } from 'primevue/useconfirm'
import type { SupplierDto } from '@/models/Supplier'
import { FilterMatchMode } from '@primevue/core/api'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Toolbar from 'primevue/toolbar'
import SupplierDialog from '../components/suppliers/SupplierDialog.vue'
import InputGroup from 'primevue/inputgroup'
import InputGroupAddon from 'primevue/inputgroupaddon'

const store = useSupplierStore()
const confirm = useConfirm()

const filters = ref({
  global: { value: null, matchMode: FilterMatchMode.CONTAINS },
})

// State for filtering
const globalFilter = ref('')

// State for the dialog
const supplierDialogVisible = ref(false)
const currentSupplier = ref<SupplierDto | null>(null)

onMounted(() => {
  store.fetchSuppliers()
})

const openNew = () => {
  currentSupplier.value = null
  supplierDialogVisible.value = true
}

const editSupplier = (supplier: SupplierDto) => {
  debugger
  currentSupplier.value = { ...supplier }
  supplierDialogVisible.value = true
}

const confirmDeleteSupplier = (supplier: SupplierDto) => {
  confirm.require({
    message: `Are you sure you want to delete the supplier: ${supplier.name}? This action cannot be undone.`,
    header: 'Confirm Deletion',
    icon: 'pi pi-exclamation-triangle',
    acceptClass: 'p-button-danger',
    accept: async () => {
      await store.deleteSupplier(supplier.id)
    },
  })
}

const handleDialogClose = (saved: boolean) => {
  supplierDialogVisible.value = false
  currentSupplier.value = null
}
</script>

<template>
  <div class="p-4">
    <h1 class="text-3xl font-semibold mb-6 text-surface-700">Suppliers Management</h1>

    <Toolbar class="mb-4 rounded-lg shadow-sm">
      <template #start>
        <Button
          label="New Supplier"
          icon="pi pi-plus"
          severity="success"
          class="mr-2"
          @click="openNew"
        />
      </template>

      <template #end>
        <InputGroup>
          <InputGroupAddon>
            <i class="pi pi-search"></i>
          </InputGroupAddon>
          <InputText
            v-model="filters.global.value"
            placeholder="Search by Name..."
            class="w-full"
          />
        </InputGroup>
      </template>
    </Toolbar>

    <DataTable
      :value="store.suppliers"
      :loading="store.isLoading"
      v-model:filters="filters"
      filterDisplay="row"
      :globalFilterFields="['name']"
      dataKey="id"
      stripedRows
      paginator
      :rows="10"
      :rowsPerPageOptions="[10, 25, 50]"
      class="shadow-lg rounded-lg"
    >
      <template #empty>
        <div class="text-center p-4">No suppliers found.</div>
      </template>
      <template #loading>
        <div class="text-center p-4">Loading supplier data. Please wait...</div>
      </template>

      <Column field="id" header="ID" sortable class="w-20"></Column>
      <Column field="name" header="Supplier Name" sortable></Column>

      <Column header="Actions" class="w-40 text-right">
        <template #body="{ data }">
          <Button
            icon="pi pi-pencil"
            severity="info"
            text
            rounded
            class="mr-2"
            @click="editSupplier(data)"
            v-tooltip.top="'Edit Supplier'"
          />
          <Button
            icon="pi pi-trash"
            severity="danger"
            text
            rounded
            @click="confirmDeleteSupplier(data)"
            v-tooltip.top="'Delete Supplier'"
          />
        </template>
      </Column>
    </DataTable>

    <SupplierDialog
      :visible="supplierDialogVisible"
      :supplier="currentSupplier"
      @close="handleDialogClose"
    />
  </div>
</template>
