<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { object, number, mixed } from 'yup'
import { yupResolver } from '@primevue/forms/resolvers/yup'
import { Form } from '@primevue/forms'

import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputNumber from 'primevue/inputnumber'
import Message from 'primevue/message'
import RadioButton from 'primevue/radiobutton'
// import RadioButtonGroup from 'primevue/radiobuttongroup';    
import { useProductStore } from '@/stores/productStore'
import { AdjustQuantityDirection } from '@/models/enums'
import type { ProductDto, AdjustStockQuantityCommand } from '@/models/Product'
import { mapValidationErrors } from '@/lib/errorMapper'


 interface ProductToAdjust {
  id: number
  name: string
//   supplierName: string
//   unitPrice: number
  unitsInStock: number
//   unitsOnOrder: number
  reorderLevel: number
}

const props = defineProps<{
    visible: boolean
    product: ProductToAdjust | null // Product from the row data
}>()

const emit = defineEmits(['close'])

const productStore = useProductStore()
const apiError = ref<{ message: string } | null>(null)
const serverErrors = ref<Record<string, string>>({})

const directionOptions = [
    { label: 'Increase Stock (Add)', value: AdjustQuantityDirection.Increasing },
    { label: 'Decrease Stock (Remove)', value: AdjustQuantityDirection.Decreasing },
]

const adjustSchema = computed(() =>
    object({
        id: number().required(),
        adjustmentDirection: mixed<AdjustQuantityDirection>()
            .required('Adjustment Direction is required.')
            .notOneOf([AdjustQuantityDirection.None], 'Please select a direction.')
            .label('Direction'),
        amount: number()
            .required('Amount is required.')
            .min(1, 'Amount must be a positive number.')
            // Client-side validation: Check if decreasing amount exceeds current stock
            .test('sufficient-stock', 'Cannot remove more stock than currently available.', (value) => {
                if (
                    props.product &&
                    props.product.unitsInStock !== undefined &&
                    value &&
                    initialValues.value.adjustmentDirection === AdjustQuantityDirection.Decreasing
                ) {
                    return value <= props.product.unitsInStock
                }
                return true
            })
            .label('Amount'),
    }),
)

const resolver =  yupResolver(adjustSchema.value)


const getInitialValues = (product: ProductToAdjust | null) => ({
    id: product?.id ?? 0,
    adjustmentDirection: AdjustQuantityDirection.Increasing, // Default to Increasing
    amount: 1, // Default amount
})

const initialValues = ref(getInitialValues(null))
const formModel = ref<{adjustmentDirection: 1|2 , amount:number}>({
  adjustmentDirection: AdjustQuantityDirection.Increasing,
  amount: 1
});
// Watch for prop change to initialize the form
watch(
    () => props.product,
    (newProduct) => {
        initialValues.value = getInitialValues(newProduct)
        apiError.value = null
        serverErrors.value = {}
    },
    { immediate: true },
)

// Computed property for stock preview
const stockPreview = computed(() => {
    if (!props.product || initialValues.value.amount === null) {
        return props.product?.unitsInStock ?? 0
    }
    const current = props.product.unitsInStock
    const amount = initialValues.value.amount
    const direction = initialValues.value.adjustmentDirection

    if (direction === AdjustQuantityDirection.Increasing) {
        return current + amount
    } else if (direction === AdjustQuantityDirection.Decreasing) {
        return current - amount
    }
    return current
})

// Handle dialog close event and state reset
const closeModal = (refresh: boolean = false) => {
    // Note: The parent component (ProductView) handles clearing currentProduct ref
    initialValues.value = getInitialValues(null) // Reset form state
    apiError.value = null
    serverErrors.value = {}
    emit('close', refresh)
}

const calculateStockPreview = (
  current: number,
  amount: number | null | undefined,
  direction: AdjustQuantityDirection | null | undefined
) => {
  if (amount == null || direction == null) return current;

  return direction === AdjustQuantityDirection.Increasing
    ? current + amount
    : current - amount;
};

const isAmountValid = ($form: any) => {
    if (!$form || !props.product) return true;

    const direction = $form.adjustmentDirection?.value;
    const amount = $form.amount?.value;

    if (
        direction === AdjustQuantityDirection.Decreasing &&
        typeof amount === 'number'
    ) {
        return amount <= props.product.unitsInStock;
    }

    return true;
};
  
const onFormSubmit = async (data: { values: any; valid: boolean }) => {
    debugger;
    const { valid, values } = data;
    if (!valid || !props.product) return;

    const command: AdjustStockQuantityCommand = {
        id: props.product.id,
        adjustmentDirection: formModel.value.adjustmentDirection ,
        amount: formModel.value.amount ,
    };

    apiError.value = null
    serverErrors.value = {}
    productStore.isSaving = true

    try {
        await productStore.adjustStock(command)
        closeModal(true)
    } catch (error: any) {
        if (error.response && error.response.status === 400 && error.response.data.errors) {
            // Handle Validation Errors
            const mappedErrors = mapValidationErrors(error.response.data.errors)
            serverErrors.value = mappedErrors.formErrors
            apiError.value = mappedErrors.globalError
        } else {
            // Business rule or 500 errors handled by errorToast (e.g., "Cannot remove stock with orders")
            closeModal(false)
        }
    } finally {
        productStore.isSaving = false
    }
}
</script>

<template>
    <Dialog :visible="visible" :header="'Adjust Stock for: ' + (product?.name ?? '')" :modal="true"
        class="w-full max-w-lg p-fluid" @update:visible="closeModal(false)" :draggable="false">
        
        <Form
            v-if="product"
            v-slot="$form"
            :initialValues="initialValues"
            :resolver="resolver"
            @submit="onFormSubmit"
            class="p-4 space-y-4">
            <div v-if="apiError" class="col-span-full">
                <Message severity="error">{{ apiError.message }}</Message>
            </div>

            <div
                class="flex justify-between p-3 bg-surface-50 dark:bg-surface-800 rounded-md border border-surface-200 dark:border-surface-700">
                <div class="flex flex-col">
                    <span class="text-sm text-surface-500">Current Stock</span>
                    <span class="text-xl font-semibold">{{ product.unitsInStock }}</span>
                </div>
                <div class="flex flex-col text-right">
                    <span class="text-sm text-surface-500">Reorder Level</span>
                    <span class="text-xl font-semibold">{{ product.reorderLevel }}</span>
                </div>
            </div>


            <div class="flex flex-col gap-2">
    <label class="font-medium">Adjustment Direction</label>

    <div class="flex flex-wrap gap-4">
        <div class="flex items-center gap-2">
            <RadioButton
                name="adjustmentDirection"
                inputId="increasing"
                :value="AdjustQuantityDirection.Increasing"
                 v-model="formModel.adjustmentDirection"
            />
            <label for="increasing">Increase Stock (Add)</label>
        </div>
        <div class="flex items-center gap-2">
            <RadioButton
                name="adjustmentDirection"
                inputId="decreasing"
                :value="AdjustQuantityDirection.Decreasing"
                v-model="formModel.adjustmentDirection"
            />
            <label for="decreasing">Decrease Stock (Remove)</label>
        </div>
    </div>

    <Message
        v-if="$form.adjustmentDirection?.invalid"
        severity="error"
        size="small"
        variant="simple"
    >
        {{ $form.adjustmentDirection.error?.message }}
    </Message>
</div>

            <!-- <div class="flex flex-col gap-2">

                <RadioButtonGroup name="adjustmentDirection" class="flex flex-wrap gap-4"> 
                    <div class="flex items-center gap-2">
                        <RadioButton inputId="incr" :value="AdjustQuantityDirection.Increasing" /> 
                        <label for="incr">Increase Stock (Add)</label> 
                    </div>
                     <div class="flex items-center gap-2"> 
                        <RadioButton inputId="decr" :value="AdjustQuantityDirection.Decreasing" /> 
                        <label for="decr">Decrease Stock (Remove)</label> 
                    </div> 
                </RadioButtonGroup>
                <Message v-if="$form.adjustmentDirection?.invalid" severity="error" size="small">
                    {{ $form.adjustmentDirection.error?.message }}
                </Message>
            </div> -->


<div class="field flex flex-col gap-2">
    <label for="amount">Amount to Adjust</label>
    <InputNumber
        name="amount"
        :min="1"
        :useGrouping="false"
        :invalid="$form.amount?.invalid || !!serverErrors.amount || !isAmountValid($form)"
        v-model="formModel.amount"
    />

    <!-- Validation messages -->
    <Message
        v-if="$form.amount?.invalid"
        severity="error"
        size="small"
        variant="simple"
    >
        {{ $form.amount.error?.message }}
    </Message>

    <Message
        v-else-if="!isAmountValid($form)"
        severity="error"
        size="small"
        variant="simple"
    >
        Cannot remove more stock than currently available.
    </Message>
</div>

            <div class="p-3 bg-primary-100/50 dark:bg-primary-900/50 rounded-md text-primary-900 dark:text-primary-100">
                <p class="font-medium">
                    New Stock Total:
                    <span class="font-bold text-lg ml-2">
                        {{ 
                            calculateStockPreview(
                                product.unitsInStock,
                                $form.amount?.value,
                                $form.adjustmentDirection?.value)
                        }}
                    </span>
                </p>
            </div>

            <div class="col-span-full pt-4 border-t border-surface-200 flex justify-end gap-3">
                <Button label="Cancel" icon="pi pi-times" severity="secondary" @click="closeModal(false)"
                    :disabled="productStore.isSaving" />
                <Button type="submit" :label="productStore.isSaving ? 'Adjusting...' : 'Confirm Adjustment'"
                    :icon="productStore.isSaving ? 'pi pi-spin pi-spinner' : 'pi pi-check'"
                    :loading="productStore.isSaving"
                    :disabled="productStore.isSaving || !$form.valid || !isAmountValid($form)"
                    severity="primary" />
            </div>
        </Form>
        <div v-else class="text-center p-4 text-surface-500">
            Product data is missing. Please close and try again.
        </div>
    </Dialog>
</template>
