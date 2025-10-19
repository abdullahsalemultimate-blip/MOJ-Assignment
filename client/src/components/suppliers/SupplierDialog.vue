<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { object, string, number } from 'yup'
import { yupResolver } from '@primevue/forms/resolvers/yup'
import { Form } from '@primevue/forms'
import { useSupplierStore } from '@/stores/supplierStore'
import type { SupplierDto, CreateSupplierCommand, UpdateSupplierCommand } from '@/models/Supplier'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import Message from 'primevue/message'

interface Props {
  visible: boolean
  supplier: SupplierDto | null // Data passed for editing (null for creation)
}

const props = defineProps<Props>()
const emit = defineEmits(['close'])

const store = useSupplierStore()
const isSaving = ref(false)

const supplierSchema = object({
  id: number().default(0),
  name: string()
    .required('Supplier Name is required.')
    .max(100, 'Name must be 100 characters or less.'),
})

const resolver = yupResolver(supplierSchema)

const dialogHeader = computed(() => (props.supplier ? `Edit Supplier` : 'Create New Supplier'))

const initialValues = ref({
  id: props.supplier?.id ?? 0,
  name: props.supplier?.name ?? '',
})

watch(
  () => props.supplier,
  (newSupplier) => {
    debugger
    if (newSupplier && newSupplier.id) {
      initialValues.value = { id: newSupplier.id, name: newSupplier.name }
    } else {
      initialValues.value = { id: 0, name: '' }
    }
  },
  { deep: true, immediate: true },
)

const onFormSubmit = async ({ values, valid }: { values: any; valid: boolean }) => {
  debugger
  if (!valid) return

  isSaving.value = true
  const supplierId = initialValues.value.id ?? null

  const command: CreateSupplierCommand | UpdateSupplierCommand = supplierId
    ? ({ id: supplierId, name: values.name } as UpdateSupplierCommand)
    : ({ name: values.name } as CreateSupplierCommand)

  const success = await store.saveSupplier(command)

  if (success) {
    emit('close', true)
  }

  isSaving.value = false
}

const closeDialog = () => {
  emit('close', false)
}
</script>

<template>
  <Dialog
    :visible="visible"
    :header="dialogHeader"
    modal
    class="w-full sm:w-11/12 md:w-3/5 lg:w-2/5 xl:w-1/4 p-4"
    @update:visible="closeDialog"
  >
    <Form
      v-slot="$form"
      :initialValues="initialValues"
      :resolver="resolver"
      @submit="onFormSubmit"
      class="space-y-6 p-fluid"
    >
      <div class="p-field flex flex-col gap-2">
        <label for="name">Supplier Name</label>
        <InputText
          name="name"
          type="text"
          placeholder="Enter supplier name"
          :invalid="$form.name?.invalid"
        />
        <Message v-if="$form.name?.invalid" severity="error" size="small" :closable="false">
          {{ $form.name.error.message }}
        </Message>
      </div>

      <div class="p-dialog-footer flex justify-end gap-2 pt-4 border-t border-surface-200">
        <Button label="Cancel" icon="pi pi-times" text @click="closeDialog" :disabled="isSaving" />
        <Button
          type="submit"
          label="Save"
          icon="pi pi-check"
          :loading="isSaving"
          :disabled="isSaving || !$form.valid"
          severity="primary"
        />
      </div>
    </Form>
  </Dialog>
</template>
