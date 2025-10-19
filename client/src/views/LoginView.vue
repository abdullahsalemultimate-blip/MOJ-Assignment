<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useRouter, useRoute } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { object, string } from 'yup'

import { yupResolver } from '@primevue/forms/resolvers/yup'
import { Form } from '@primevue/forms' // Corrected named import

// PrimeVue Components
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import Card from 'primevue/card'
import Message from 'primevue/message'

const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()
const toast = useToast()

const isLoginLoading = ref(false)

// Yup Validation Schema
const loginSchema = object({
  username: string().required('Username is required.').email('Must be a valid email address.'),
  password: string().required('Password is required.'),
})

const resolver = yupResolver(loginSchema)

const initialValues = {
  username: 'administrator@localhost',
  password: 'Administrator1!',
}

const onFormSubmit = async ({ values, valid }: { values: any; valid: boolean }) => {
  if (!valid) return

  isLoginLoading.value = true
  try {
    await authStore.login(values as { username: string; password: string })

    toast.add({
      severity: 'success',
      summary: 'Login Successful',
      detail: 'Welcome to InventorySys!',
      life: 3000,
    })

    const redirectPath = (route.query.redirect as string) || '/dashboard'
    router.push(redirectPath)
  } catch (error) {
    console.error('Login error caught in component:', error)
  } finally {
    isLoginLoading.value = false
  }
}

const isFormLoading = computed(() => isLoginLoading.value)
</script>

<template>
  <div class="min-h-screen flex items-center justify-center p-4 bg-surface-100">
    <Card class="w-full max-w-md shadow-2xl">
      <template #title>
        <div class="text-center text-2xl font-semibold text-primary-600">InventorySys Login</div>
      </template>
      <template #subtitle>
        <div class="text-center text-sm text-surface-500">
          Enter your credentials to access the system
        </div>
      </template>
      <template #content>
        <Form
          v-slot="$form"
          :initialValues="initialValues"
          :resolver="resolver"
          @submit="onFormSubmit"
          class="space-y-6"
        >
          <div class="flex flex-col gap-2">
            <label for="username" class="font-medium text-surface-700">Username (Email)</label>
            <InputText
              name="username"
              type="text"
              placeholder="administrator@localhost"
              :invalid="$form.username?.invalid"
            />
            <Message v-if="$form.username?.invalid" severity="error" size="small" :closable="false">
              {{ $form.username.error.message }}
            </Message>
          </div>

          <div class="flex flex-col gap-2">
            <label for="password" class="font-medium text-surface-700">Password</label>
            <InputText
              name="password"
              type="password"
              placeholder="Password"
              :invalid="$form.password?.invalid"
            />
            <Message v-if="$form.password?.invalid" severity="error" size="small" :closable="false">
              {{ $form.password.error.message }}
            </Message>
          </div>

          <Button
            type="submit"
            label="Log In"
            :loading="isFormLoading"
            :disabled="isFormLoading || !$form.valid"
            icon="pi pi-sign-in"
            class="w-full"
          />
        </Form>
        <div class="mt-4 text-center text-sm text-surface-500">
          <p>Default Credentials:</p>
          <p class="font-mono text-xs">U: administrator@localhost, P: Administrator1!</p>
        </div>
      </template>
    </Card>
  </div>
</template>
