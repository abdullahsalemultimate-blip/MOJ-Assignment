import axios, { type AxiosInstance, type AxiosError } from 'axios'
import router from '@/router'
import { useAuthStore } from '@/stores/authStore'
import { errorHandler, type ProblemDetails } from './errorHandler'
import app from '@/main'

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001'

const api: AxiosInstance = axios.create({
  baseURL: BASE_URL,
  timeout: 30000, // 30 seconds timeout
  headers: {
    'Content-Type': 'application/json',
  },
})

// Token Interceptor
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('auth_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

//  Global Error Handler Interceptor for response
api.interceptors.response.use(
  (response) => {
    return response
  },
  (error: AxiosError) => {
    debugger
    const authStore = useAuthStore()
    // const toast = useToast()
    const toast = app.config.globalProperties.$toast

    if (error.response) {
      const { status } = error.response

      // Global 401 Unauthorized handling
      if (status === 401) {
        // Check if user was authenticated (i.e., had a token)
        if (authStore.isAuthenticated) {
          toast.add({
            severity: 'error',
            summary: 'Session Expired',
            detail: 'Your session has expired. Please log in again.',
            life: 5000,
          })
        } else {
          toast.add({
            severity: 'error',
            summary: 'Error',
            detail: (error.response.data as ProblemDetails).detail,
            life: 5000,
          })
        }

        // Trigger global logout (clears state, localStorage, and redirects)
        authStore.logout()

        // Redirect to login, preserving the path user tried to access
        if (
          router.currentRoute.value.meta.requiresAuth &&
          router.currentRoute.value.path !== '/login'
        ) {
          router.push({
            path: '/login',
            query: { redirect: router.currentRoute.value.fullPath },
          })
        }
        return Promise.reject(error)
      }

      // Handle other HTTP errors (400, 404, 500) using the centralized error handler
      const handledError = errorHandler(error)

      // Display error in a toast
      if (handledError.detail || handledError.title) {
        
        if(!handledError.fieldErrors){
            toast.add({
              severity: 'error',
              summary: handledError.title || 'Error',
              detail: handledError.detail || 'An unexpected error occurred.',
              life: 5000,
            })
        }
        else {
          for (let key in handledError.fieldErrors) {
                    toast.add({
                      severity: 'error',
                      summary: handledError.title || 'Error',
                      detail:`${key}: ${ handledError.fieldErrors[key]}`,
                      life: 5000,
                    })
          }
        }
        
      }

      return Promise.reject(error)
    } else if (error.request) {
      // Network Error (No response received)
      toast.add({
        severity: 'error',
        summary: 'Network Error',
        detail: 'Connection error. Please check your internet or API server status.',
        life: 5000,
      })
      return Promise.reject({ detail: 'Network connection failed.' })
    } else {
      // Something happened in setting up the request that triggered an Error
      return Promise.reject(error)
    }
  },
)

export default api
