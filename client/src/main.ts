import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura'
import ToastService from 'primevue/toastservice'
import ConfirmationService from 'primevue/confirmationservice'
import { useAuthStore } from './stores/authStore'
import Tooltip from 'primevue/tooltip'

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      cssLayer: {
        name: 'primevue',
        order: 'theme, base, primevue',
      },
    },
  },
  ripple: true,
})
app.use(ToastService)
app.use(ConfirmationService)
app.directive('tooltip', Tooltip)

// 7. Initialize Auth Store (Critical for conditional rendering in AppLayout)
// Note: Pinia store creation must happen AFTER app.use(createPinia())
const authStore = useAuthStore()
// Call the initialization action immediately
authStore.initializeAuth()

app.mount('#app')

export default app
