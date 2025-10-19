<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useProductStore } from '@/stores/productStore'
import { useSupplierStore } from '@/stores/supplierStore'
import { useConfirm } from 'primevue/useconfirm'
import type { ProductDetailDto, ProductDto } from '@/models/Product'
import { QuantityUnit } from '@/models/enums'
import { useRouter } from 'vue-router' // To view history later

import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Toolbar from 'primevue/toolbar'
import Tag from 'primevue/tag'
import Dropdown from 'primevue/dropdown'
import InputGroup from 'primevue/inputgroup'
import InputGroupAddon from 'primevue/inputgroupaddon'
import ProductActionMenu from '../components/products/ProductActionMenu.vue'
import ProductDialog from '../components/products/ProductDialog.vue';
import AdjustStockDialog from '../components/products/AdjustStockDialog.vue';
import ProductAuditModal from '@/components/products/ProductAuditModal.vue';

const productStore = useProductStore()
const supplierStore = useSupplierStore()
const confirm = useConfirm()
const router = useRouter()

const productDialogVisible = ref(false)
const adjustStockDialogVisible = ref(false)
const isAuditModalVisible = ref(false);
const currentProduct = ref<ProductDetailDto | null>(null)

const supplierOptions = computed(() => {
  return [
    { name: 'All Suppliers', id: null },
    ...supplierStore.suppliers.map((s) => ({ name: s.name, id: s.id })),
  ]
})

onMounted(async () => {
  await productStore.initializeDependencies()
  await productStore.fetchProducts()
})

const openAuditModal = (product: ProductDetailDto) => {
    currentProduct.value = product;
    isAuditModalVisible.value = true;
};

const getStockSeverity = (unitsInStock: number, reorderLevel: number) => {
  if (unitsInStock <= reorderLevel) return 'danger'
  if (unitsInStock < reorderLevel * 2) return 'warning'
  return 'success'
}

const handleEdit = async (product: ProductDetailDto | null) => {
  if (product?.id) {
    const fetchedProduct = await productStore.fetchProductToEdit(product.id)
    if (fetchedProduct) {
      currentProduct.value = fetchedProduct
      productDialogVisible.value = true
    }
  } else {
    currentProduct.value = null
    productDialogVisible.value = true
  }
}

const handleStockAdjustment = (product: ProductDetailDto) => {
  currentProduct.value = product
  adjustStockDialogVisible.value = true
}

const handleDelete = (product: ProductDto) => {
  confirm.require({
    message: `Are you sure you want to delete the product: ${product.name}?`,
    header: 'Confirm Deletion',
    icon: 'pi pi-exclamation-triangle',
    acceptClass: 'p-button-danger',
    accept: async () => {
      await productStore.deleteProduct(product.id)
    },
  })
}

const handleDialogClose = () => {
  productDialogVisible.value = false
  adjustStockDialogVisible.value = false
  isAuditModalVisible.value = false; 
  currentProduct.value = null
  productStore.clearProductToEdit()
}
const onSearchInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  productStore.updateSearchTerm(target.value)
}

const onSupplierChange = (event: { value: { id: number | null } }) => {
  productStore.updateSupplierFilter(event.value.id)
}
</script>

<template>
  <div class="p-4">
    <h1 class="text-3xl font-semibold mb-6 text-surface-700">Products Inventory</h1>

    <Toolbar class="mb-4 rounded-lg shadow-sm">
      <template #start>
        <Button
          label="New Product"
          icon="pi pi-plus"
          severity="success"
          class="mr-2"
          @click="handleEdit(null)"
        />
      </template>

      <template #end>
        <div class="flex gap-3">
          <Dropdown
            :options="supplierOptions"
            optionLabel="name"
            placeholder="Select Supplier"
            class="w-48"
            :modelValue="{
              id: productStore.filterState.supplierId,
              name:
                supplierStore.suppliers.find((s) => s.id === productStore.filterState.supplierId)
                  ?.name || 'All Suppliers',
            }"
            @change="onSupplierChange"
          />
          <InputGroup>
            <InputGroupAddon>
              <i class="pi pi-search"></i>
            </InputGroupAddon>
            <InputText
              :value="productStore.filterState.searchTerm"
              @input="onSearchInput"
              placeholder="Search products..."
              class="w-full"
            />
          </InputGroup>
        </div>
      </template>
    </Toolbar>

    <DataTable
      :value="productStore.products"
      :loading="productStore.isLoading"
      :totalRecords="productStore.totalRecords"
      :rows="productStore.filterState.pageSize"
      :first="(productStore.filterState.pageNumber - 1) * productStore.filterState.pageSize"
      lazy
      paginator
      @page="productStore.onLazyLoad"
      dataKey="id"
      stripedRows
      :rowsPerPageOptions="[10, 25, 50]"
      class="shadow-lg rounded-lg"
    >
      <template #empty>
        <div class="text-center p-4">No products found matching your criteria.</div>
      </template>
      <template #loading>
        <div class="text-center p-4">Loading product data. Please wait...</div>
      </template>

      <Column field="name" header="Name" style="min-width: 15rem"></Column>
      <Column field="supplierName" header="Supplier"></Column>
      <Column field="unitPrice" header="Unit Price" data-type="numeric" class="w-32">
        <template #body="{ data }">
          {{ data.unitPrice.toFixed(2) }}
        </template>
      </Column>

      <Column field="unitsInStock" header="Stock" data-type="numeric" class="w-32">
        <template #body="{ data }">
          <Tag
            :value="data.unitsInStock"
            :severity="getStockSeverity(data.unitsInStock, data.reorderLevel)"
          />
        </template>
      </Column>

      <Column field="reorderLevel" header="Reorder Lvl" data-type="numeric" class="w-32"></Column>
      <Column field="unitsOnOrder" header="On Order" data-type="numeric" class="w-32"></Column>

      <Column header="Actions" class="w-20 text-right">
        <template #body="{ data }">
          <ProductActionMenu
            :product="data"
            @edit="handleEdit"
            @stock-adjust="handleStockAdjustment"
            @delete="handleDelete"
            @view-history="openAuditModal"
          />
        </template>
      </Column>
    </DataTable>

    <ProductDialog
      :visible="productDialogVisible"
      :product="currentProduct"
      :loading="productStore.isFetchingEditProduct"
      @close="handleDialogClose"
    />

    <AdjustStockDialog
      :visible="adjustStockDialogVisible"
      :product="currentProduct"
      @close="handleDialogClose"
    />

    
    <ProductAuditModal
            :visible="isAuditModalVisible"
            :productId="currentProduct?.id"
            :productName="currentProduct?.name"
            @close="handleDialogClose"
        />

    
  </div>
</template>
