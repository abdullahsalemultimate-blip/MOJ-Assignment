<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { RouterView } from 'vue-router'
import Toast from 'primevue/toast'
import ConfirmDialog from 'primevue/confirmdialog'

// Components (To be created in src/components/layout)
import AppSidebar from '../components/layout/AppSidebar.vue'
import AppTopbar from '../components/layout/AppTopbar.vue'

const authStore = useAuthStore()

onMounted(() => {
  // Initialize auth state on app mount
  authStore.initializeAuth()
})
</script>

<template>
  <div v-if="authStore.isAuthenticated" class="min-h-screen flex flex-col">
    <Toast position="top-right" />
    <ConfirmDialog />

    <AppTopbar />

    <div class="flex flex-1 overflow-hidden">
      <AppSidebar />

      <main class="flex-1 p-6 overflow-y-auto bg-gray-50">
        <RouterView />
      </main>
    </div>
  </div>
  <div v-else class="min-h-screen">
    <RouterView />
  </div>
</template>
