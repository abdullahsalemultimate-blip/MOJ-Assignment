<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import Button from 'primevue/button'
import { useToast } from 'primevue/usetoast'

const authStore = useAuthStore()
const toast = useToast()

const handleLogout = () => {
  authStore.logout()
  toast.add({
    severity: 'info',
    summary: 'Logged Out',
    detail: 'You have been successfully logged out.',
    life: 3000,
  })
}
</script>

<template>
  <header
    class="flex items-center justify-between h-16 px-6 bg-primary-600 text-white shadow-lg z-10"
  >
    <div class="text-xl font-bold">InventorySys</div>
    <div class="flex items-center space-x-4">
      <span v-if="authStore.user?.username" class="text-sm font-medium">
        Welcome, {{ authStore.user.username }}
      </span>
      <Button
        label="Logout"
        icon="pi pi-sign-out"
        severity="danger"
        text
        @click="handleLogout"
        class="hover:bg-red-700"
      />
    </div>
  </header>
</template>
