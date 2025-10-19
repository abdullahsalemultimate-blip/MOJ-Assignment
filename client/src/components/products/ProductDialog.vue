<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { object, string, number, mixed } from 'yup'
import { yupResolver } from '@primevue/forms/resolvers/yup' // Corrected resolver import
import { Form } from '@primevue/forms' // Import the Form component

// PrimeVue Components
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import InputNumber from 'primevue/inputnumber'
import Message from 'primevue/message'
import VTooltip from 'primevue/tooltip'

import { useProductStore } from '@/stores/productStore'
import { useSupplierStore } from '@/stores/supplierStore'
import { QuantityUnit } from '@/models/enums'
import type {
  ProductDto,
  CreateProductCommand,
  UpdateProductCommand,
  ProductDetailDto,
} from '@/models/Product'
import { mapValidationErrors } from '@/lib/errorMapper' // Utility for mapping 400 errors

const props = defineProps<{
  visible: boolean
  product: ProductDetailDto | null
}>()

const emit = defineEmits(['close'])

const productStore = useProductStore()
const supplierStore = useSupplierStore()
const apiError = ref<{ message: string } | null>(null)
const serverErrors = ref<Record<string, string>>({}) // For field-specific server errors

// QuantityUnit options for dropdown (exclude None)
const quantityUnitOptions = Object.keys(QuantityUnit)
  .filter((key) => isNaN(Number(key)) && key !== 'None')
  .map((key) => ({ label: key, value: QuantityUnit[key as keyof typeof QuantityUnit] }))

const supplierOptions = computed(() =>
  supplierStore.supplierLookup.map((s) => ({ label: s.title, value: s.id })),
)

const productSchema = object({
  id: number().nullable().default(0),
  name: string()
    .required('Product Name is required.')
    .max(100, 'Product Name must be 100 characters or less.')
    .label('Product Name'),
  supplierId: number()
    .required('Supplier is required.')
    .min(1, 'Supplier is required.')
    .label('Supplier'),
  quantityPerUnit: mixed<QuantityUnit>()
    .required('Quantity Unit is required.')
    .notOneOf([QuantityUnit.None as any], 'Quantity Unit must be a valid unit.')
    .label('Quantity Unit'),
  unitPrice: number()
    .required('Unit Price is required.')
    .min(0, 'Unit price cannot be negative.')
    .label('Unit Price'),
  unitsInStock: number()
    .min(0, 'Units in Stock cannot be negative.')
    .default(0)
    .label('Units In Stock'),
  unitsOnOrder: number()
    .min(0, 'Units On Order cannot be negative.')
    .default(0)
    .label('Units On Order'),
  reorderLevel: number()
    .min(0, 'Reorder Level cannot be negative.')
    .default(0)
    .label('Reorder Level'),
})

const resolver = yupResolver(productSchema)

const getProductInitialValues = (product: ProductDetailDto | null) => ({
  id: product?.id ?? null,
  name: product?.name ?? '',
  supplierId: product?.supplierId ?? null,
  quantityPerUnit: product?.quantityPerUnit ?? QuantityUnit.None,
  unitPrice: product?.unitPrice ?? 0,
  unitsInStock: product?.unitsInStock ?? 0,
  unitsOnOrder: product?.unitsOnOrder ?? 0,
  reorderLevel: product?.reorderLevel ?? 0,
})

const initialValues = ref(getProductInitialValues(null))

const dialogHeader = computed(() =>
  productStore.productToEdit && productStore.productToEdit.id
    ? `Edit Product: ${productStore.productToEdit.name}`
    : 'Create New Product',
)

onMounted(async () => {
  await supplierStore.fetchSupplierLookup()
})

watch(
  () => productStore.productToEdit,
  (newProduct) => {
    debugger;
    initialValues.value = getProductInitialValues(newProduct)
    serverErrors.value = {} // Clear server errors on new data load
  },
  { deep: true, immediate: true },
)

const closeModal = (refresh: boolean = false) => {
  productStore.clearProductToEdit()
  initialValues.value = getProductInitialValues(null) // Reset form state
  apiError.value = null
  serverErrors.value = {}
  emit('close', refresh)
}

const onFormSubmit = async ({ values, valid }: { values: any; valid: boolean }) => {
  if (!valid || productStore.isFetchingEditProduct) return

  apiError.value = null
  serverErrors.value = {} // Clear server errors before submission
  productStore.isSaving = true
  debugger;
  const isUpdate = productStore.productToEdit?.id && productStore.productToEdit.id > 0

  const command: CreateProductCommand | UpdateProductCommand = isUpdate
    ? ({ ...values, id: productStore!.productToEdit!.id } as UpdateProductCommand)
    :   {
            name: values?.name ?? '',
            supplierId: values?.supplierId ?? null,
            quantityPerUnit: values?.quantityPerUnit ?? QuantityUnit.None,
            unitPrice: values?.unitPrice ?? 0,
            unitsInStock: values?.unitsInStock ?? 0,
            unitsOnOrder: values?.unitsOnOrder ?? 0,
            reorderLevel: values?.reorderLevel ?? 0,
    }

  try {
    const success = await productStore.saveProduct(command)

    if (success) {
      closeModal(true)
    }
  } catch (error: any) {
    debugger
    if (error.response && error.response.status === 400 && error.response.data.errors) {
      const mappedErrors = mapValidationErrors(error.response.data.errors)

      // Map the field errors to the local ref
      serverErrors.value = mappedErrors.formErrors
      apiError.value = mappedErrors.globalError ;
    } else {
      // If it's a non-validation error (404, 500, business rule), the store shows a toast
      closeModal(false)
    }
  } finally {
    productStore.isSaving = false
  }
}
</script>

<template>
  <Dialog
    :visible="visible"
    :header="dialogHeader"
    :modal="true"
    class="p-fluid w-full max-w-4xl"
    @update:visible="closeModal(false)"
    :draggable="false"
  >
    <Form
      v-slot="$form"
      :initialValues="initialValues"
      :resolver="resolver"
      @submit="onFormSubmit"
      class="p-4 grid grid-cols-1 md:grid-cols-2 gap-6"
    >
      <div v-if="apiError" class="col-span-full">
          <small class="p-error">
            {{ apiError.message }}
          </small>
      </div>

      <div v-if="productStore.isFetchingEditProduct" class="col-span-full text-center p-6">
        <i class="pi pi-spin pi-spinner text-4xl text-primary-500"></i>
        <p class="mt-2">Loading product details...</p>
      </div>

      <template v-else>
        <div class="field flex flex-col gap-2">
          <label for="name">Product Name</label>
          <InputText
            name="name"
            type="text"
            placeholder="Product Name"
            :invalid="$form.name?.invalid || !!serverErrors.name"
            maxlength="100"
          />
        <Message v-if="$form.name?.invalid" severity="error" size="small" :closable="false">
          {{ $form.name.error.message || serverErrors.name }}
        </Message>
        
        </div>

        <div class="field flex flex-col gap-2">
          <label for="supplierId">Supplier</label>
          <Dropdown
            name="supplierId"
            :options="supplierOptions"
            optionLabel="label"
            optionValue="value"
            placeholder="Select a Supplier"
            :invalid="$form.supplierId?.invalid || !!serverErrors.supplierId"
            :loading="supplierStore.isLookupLoading"
            :showClear="true"
          />
          <Message v-if="$form.supplierId?.invalid" severity="error" size="small" :closable="false">
            {{ $form.supplierId?.error?.message || serverErrors.supplierId }}
          </Message>
        </div>

        <div class="field flex flex-col gap-2">
          <label for="quantityPerUnit">Quantity Unit</label>
          <Dropdown
            name="quantityPerUnit"
            :options="quantityUnitOptions"
            optionLabel="label"
            optionValue="value"
            placeholder="Select Unit"
            :invalid="$form.quantityPerUnit?.invalid || !!serverErrors.quantityPerUnit"
          />
          <Message v-if="$form.quantityPerUnit?.invalid" severity="error" size="small" :closable="false">
            {{ $form.quantityPerUnit?.error?.message || serverErrors.quantityPerUnit }}
          </Message>
        </div>

        <div class="field flex flex-col gap-2">
          <label for="unitPrice">Unit Price ($)</label>
          <InputNumber
            name="unitPrice"
            mode="currency"
            currency="USD"
            locale="en-US"
            :min="0"
            :maxFractionDigits="2"
            :invalid="$form.unitPrice?.invalid || !!serverErrors.unitPrice"
          />
          <Message v-if="$form.unitPrice?.invalid" severity="error" size="small" :closable="false">
                {{ $form.unitPrice?.error?.message || serverErrors.unitPrice }}
          </Message>          
        </div>

        <div class="field flex flex-col gap-2">
          <label for="unitsInStock">Units In Stock</label>
          <InputNumber
            name="unitsInStock"
            :min="0"
            :useGrouping="false"
            :invalid="$form.unitsInStock?.invalid || !!serverErrors.unitsInStock"
            :disabled="!!(initialValues.id && initialValues.id > 0)"
            v-tooltip.top="'Stock can only be adjusted via the dedicated Stock Adjustment action.'"
          />
            <Message v-if="$form.unitsInStock?.invalid" severity="error" size="small" :closable="false">
                {{ $form.unitsInStock?.error?.message || serverErrors.unitsInStock }}
          </Message>  
        </div>

        <div class="field flex flex-col gap-2">
          <label for="reorderLevel">Reorder Level</label>
          <InputNumber
            name="reorderLevel"
            :min="0"
            :useGrouping="false"
            :invalid="$form.reorderLevel?.invalid || !!serverErrors.reorderLevel"
          />
            <Message v-if="$form.reorderLevel?.invalid" severity="error" size="small" :closable="false">
                {{ $form.reorderLevel?.error?.message || serverErrors.reorderLevel }}
          </Message>  
        </div>

        <div class="field md:col-span-1 flex flex-col gap-2">
          <label for="unitsOnOrder">Units On Order</label>
          <InputNumber
            name="unitsOnOrder"
            :min="0"
            :useGrouping="false"
            :invalid="$form.unitsOnOrder?.invalid || !!serverErrors.unitsOnOrder"
          />
            <Message v-if="$form.unitsOnOrder?.invalid" severity="error" size="small" :closable="false">
                {{ $form.unitsOnOrder?.error?.message || serverErrors.unitsOnOrder }}
          </Message>  
        </div>

        <input type="hidden" name="id" />
      </template>

      <div class="col-span-full pt-4 border-t border-surface-200 flex justify-end gap-3">
        <Button
          label="Cancel"
          icon="pi pi-times"
          severity="secondary"
          @click="closeModal(false)"
          :disabled="productStore.isSaving || productStore.isFetchingEditProduct"
        />
        <Button
          type="submit"
          :label="productStore.isSaving ? 'Saving...' : 'Save Product'"
          :icon="productStore.isSaving ? 'pi pi-spin pi-spinner' : 'pi pi-check'"
          :loading="productStore.isSaving"
          :disabled="productStore.isSaving || productStore.isFetchingEditProduct || !$form.valid"
          severity="primary"
        />
      </div>
    </Form>
  </Dialog>
</template>