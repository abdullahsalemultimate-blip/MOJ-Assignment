import { defineStore } from 'pinia'
import { ref, watch, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import { productService } from '@/services/ProductService'
import type {
  ProductDto,
  CreateProductCommand,
  UpdateProductCommand,
  AdjustStockQuantityCommand,
  ProductFilterRequest,
  ProductDetailDto,
} from '@/models/Product'
import { useSupplierStore } from './supplierStore'
import { debounce } from 'lodash-es'
import app from '@/main'

export const useProductStore = defineStore('product', () => {
  const toast = app.config.globalProperties.$toast
  const supplierStore = useSupplierStore()

  const products = ref<ProductDto[]>([])
  const totalRecords = ref(0)
  const isLoading = ref(false)
  const isSaving = ref(false)

  const filterState = ref<ProductFilterRequest>({
    searchTerm: null,
    supplierId: null,
    pageNumber: 1,
    pageSize: 10,
  })

  const initializeDependencies = async () => {
    if (supplierStore.suppliers.length === 0) {
      await supplierStore.fetchSuppliers()
    }
  }

  const fetchProducts = async () => {
    isLoading.value = true
    try {
      const response = await productService.getAll(filterState.value)
      products.value = response.items
      totalRecords.value = response.totalCount
    } catch (error) {
      // Error is already handled by axios interceptor
      return Promise.reject(error)
    } finally {
      isLoading.value = false
    }
  }

  const debouncedFetch = debounce(fetchProducts, 300)

  const onLazyLoad = (event: { first: number; rows: number; filters?: any }) => {
    const newPageNumber = Math.floor(event.first / event.rows) + 1

    // Only update if value changes to prevent unnecessary fetches on initial load
    if (filterState.value.pageSize !== event.rows) {
      filterState.value.pageSize = event.rows
      filterState.value.pageNumber = 1 // Always reset to page 1 on page size change
    } else if (filterState.value.pageNumber !== newPageNumber) {
      filterState.value.pageNumber = newPageNumber
    } else {
      // Apply filtering logic if provided by PrimeVue's filter object
      // For now, we rely only on our custom input and dropdown for filtering
      return
    }

    fetchProducts()
  }

  const updateSearchTerm = (term: string) => {
    filterState.value.searchTerm = term.trim() || null
    filterState.value.pageNumber = 1 // Reset to page 1 on new search
    debouncedFetch()
  }

  const updateSupplierFilter = (supplierId: number | null) => {
    filterState.value.supplierId = supplierId
    filterState.value.pageNumber = 1 // Reset to page 1 on new filter
    fetchProducts()
  }

  const saveProduct = async (data: CreateProductCommand | UpdateProductCommand) => {
    isSaving.value = true
    debugger;
    try {
    
      if ('id' in data && data.id  ) {
        await productService.update(data as UpdateProductCommand)
        toast.add({
          severity: 'success',
          summary: 'Update Successful',
          detail: `Product '${data.name}' updated.`,
          life: 3000,
        })
      } else {
        await productService.create(data as CreateProductCommand)
        toast.add({
          severity: 'success',
          summary: 'Creation Successful',
          detail: `Product '${data.name}' created.`,
          life: 3000,
        })
      }

      await fetchProducts()
      return true
    } finally {
      isSaving.value = false
    }
  }

  const deleteProduct = async (id: number) => {
    isLoading.value = true
    try {
      await productService.delete(id)
      toast.add({
        severity: 'warn',
        summary: 'Delete Successful',
        detail: 'Product deleted.',
        life: 3000,
      })

      await fetchProducts()
    } catch (error) {
      // Error is already handled by axios interceptor
      return Promise.reject(error)
    } finally {
      isLoading.value = false
    }
  }

  const adjustStock = async (data: AdjustStockQuantityCommand) => {
    isSaving.value = true
    try {
      await productService.adjustStock(data)

      toast.add({
        severity: 'info',
        summary: 'Stock Adjusted',
        detail: 'Product stock quantity updated successfully.',
        life: 3000,
      })

      await fetchProducts()
      return true
    } catch (error) {
      // Error is already handled by axios interceptor
      return false
    } finally {
      isSaving.value = false
    }
  }

  // GetProductById For Edit
  const productToEdit = ref<ProductDetailDto | null>(null)
  const isFetchingEditProduct = ref(false)

  const fetchProductToEdit = async (id: number): Promise<ProductDetailDto | null> => {
    isFetchingEditProduct.value = true
    productToEdit.value = null
    try {
      // 1. Fetch supplier lookup data for the form's dropdown
      await supplierStore.fetchSupplierLookup()

      // 2. Fetch the product detail VM
      const productVm = await productService.getProductById(id)

      // 3. Extract the DTO from the VM
      productToEdit.value = productVm.productDetail

      return productVm.productDetail
    } catch (error) {
      // Error is already handled by axios interceptor
      return null
    } finally {
      isFetchingEditProduct.value = false
    }
  }

  const clearProductToEdit = () => {
    productToEdit.value = null
  }

  return {
    // State
    products,
    totalRecords,
    isLoading,
    isSaving,
    filterState,

    fetchProducts,
    saveProduct,
    deleteProduct,
    adjustStock,
    initializeDependencies,
    onLazyLoad,
    updateSearchTerm,
    updateSupplierFilter,

    productToEdit,
    isFetchingEditProduct,
    fetchProductToEdit,
    clearProductToEdit,
  }
})
