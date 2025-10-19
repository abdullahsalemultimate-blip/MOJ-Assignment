<template>
  <div v-if="hasErrors" class="mb-6">
    <!-- Main Error Alert -->
    <Message severity="error" :closable="false" class="mb-4 border-l-4 border-red-500 bg-red-50">
      <template #icon>
        <i class="pi pi-exclamation-triangle text-red-600 text-lg" />
      </template>

      <div class="ml-3">
        <h3 class="text-sm font-semibold text-red-800 mb-1">
          {{ errorData?.title || 'Validation Error' }}
        </h3>
        <p class="text-sm text-red-700">
          {{ errorData?.message || 'Please check the form for errors and try again.' }}
        </p>
      </div>
    </Message>

    <!-- Detailed Errors List -->
    <Card class="border border-red-200 bg-white">
      <template #title>
        <div class="flex items-center">
          <i class="pi pi-list text-red-500 mr-2"></i>
          <span class="text-red-800 font-semibold">Validation Errors</span>
        </div>
      </template>

      <template #content>
        <div class="space-y-3" v-if="errorData?.errors">
          <div
            v-for="(fieldErrors, fieldName) in errorData.errors"
            :key="fieldName"
            class="border-l-4 border-red-400 bg-red-25 pl-4 py-2"
          >
            <div class="flex items-start">
              <i class="pi pi-times-circle text-red-500 mt-1 mr-2 flex-shrink-0"></i>
              <div>
                <h4 class="font-medium text-red-800 capitalize">
                  {{ fieldName }}
                </h4>
                <ul class="mt-1 space-y-1">
                  <li
                    v-for="(error, index) in fieldErrors"
                    :key="index"
                    class="text-sm text-red-600 flex items-start"
                  >
                    <span
                      class="w-1.5 h-1.5 bg-red-400 rounded-full mt-2 mr-2 flex-shrink-0"
                    ></span>
                    {{ error }}
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import Message from 'primevue/message'
import Card from 'primevue/card'

interface ValidationError {
  type?: string
  title?: string
  status?: number
  errors: Record<string, string[]>
  message?: string
}

interface Props {
  errorData?: ValidationError | null
}

const props = withDefaults(defineProps<Props>(), {
  errorData: null,
})

const hasErrors = computed(() => {
  return (
    props.errorData !== null &&
    props.errorData !== undefined &&
    Object.keys(props.errorData.errors || {}).length > 0
  )
})
</script>

<style scoped>
.bg-red-25 {
  background-color: rgb(254 242 242 / 0.5);
}
</style>
