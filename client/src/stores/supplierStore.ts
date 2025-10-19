// src/stores/supplierStore.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import { supplierService } from '@/services/SupplierService'
import type { SupplierDto, CreateSupplierCommand, UpdateSupplierCommand } from '@/models/Supplier'
import app from '@/main'
import type { LookupDto } from '@/models/LookupDto'

export const useSupplierStore = defineStore('supplier', () => {
  const suppliers = ref<SupplierDto[]>([])
  const isLoading = ref(false)
  const toast = app.config.globalProperties.$toast

  const fetchSuppliers = async () => {
    isLoading.value = true
    try {
      suppliers.value = await supplierService.getAll()
    } catch (error) {
      // Error is already handled by axios interceptor
      return Promise.reject(error)
    } finally {
      isLoading.value = false
    }
  }

  const saveSupplier = async (data: CreateSupplierCommand | UpdateSupplierCommand) => {
    isLoading.value = true
    try {
      if ('id' in data) {
        // Update
        await supplierService.update(data as UpdateSupplierCommand)
        toast.add({
          severity: 'success',
          summary: 'Update Successful',
          detail: `Supplier '${data.name}' updated.`,
          life: 3000,
        })
      } else {
        // Create
        await supplierService.create(data as CreateSupplierCommand)
        toast.add({
          severity: 'success',
          summary: 'Creation Successful',
          detail: `Supplier '${data.name}' created.`,
          life: 3000,
        })
      }

      // Refresh list after successful operation
      await fetchSuppliers()
      return true
    } catch (error) {
      // Error is already handled by axios interceptor
      return false
    } finally {
      isLoading.value = false
    }
  }

  const deleteSupplier = async (id: number) => {
    isLoading.value = true
    try {
      await supplierService.delete(id)
      toast.add({
        severity: 'warn',
        summary: 'Delete Successful',
        detail: 'Supplier deleted.',
        life: 3000,
      })

      // Optimistic update: remove the item from the local array
      suppliers.value = suppliers.value.filter((s) => s.id !== id)
    } catch (error) {
      // Error is already handled by axios interceptor
      return Promise.reject(error)
    } finally {
      isLoading.value = false
    }
  }

  // Fetch Supplier  Lookup
  const supplierLookup = ref<LookupDto[]>([]) // For Create/Edit dropdowns
  const isLookupLoading = ref(false)

  const fetchSupplierLookup = async () => {
    if (supplierLookup.value.length > 0) return // Simple caching
    isLookupLoading.value = true
    try {
      supplierLookup.value = await supplierService.getSupplierLookup()
    } catch (error) {
      // Error is already handled by axios interceptor
      return Promise.reject(error)
    } finally {
      isLookupLoading.value = false
    }
  }

  return {
    suppliers,
    isLoading,
    fetchSuppliers,
    saveSupplier,
    deleteSupplier,

    supplierLookup,
    isLookupLoading,
    fetchSupplierLookup,
  }
})
