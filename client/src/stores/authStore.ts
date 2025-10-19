// src/stores/authStore.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '@/lib/axios'
import router from '@/router'

// Interfaces for API communication
interface LoginCredentials {
  username: string
  password: string
}

interface LoginResponse {
  jwt: AuthTokenDto
  username: string
}

interface AuthTokenDto {
  token: string
  expires: number // Unix timestamp
}

interface UserInfo {
  username: string
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const user = ref<UserInfo | null>(null)

  const isAuthenticated = computed(() => !!token.value)

  const initializeAuth = () => {
    const storedToken = localStorage.getItem('auth_token')
    const storedUsername = localStorage.getItem('user_username')

    if (storedToken && storedUsername) {
      token.value = storedToken
      user.value = { username: storedUsername }
    } else {
      token.value = null
      user.value = null
    }
  }

  const login = async (credentials: LoginCredentials): Promise<void> => {
    try {
      const response = await api.post<LoginResponse>('/api/Auth', credentials)

      const authTokenDto = response.data.jwt
      const tempUsername = response.data.username

      localStorage.setItem('auth_token', authTokenDto.token)
      localStorage.setItem('user_username', tempUsername)

      token.value = authTokenDto.token
      user.value = { username: tempUsername }
    } catch (error) {
      // Error is already handled by axios interceptor
      return Promise.reject(error)
    }
  }

  /**
   * Logs out the user, clears storage, and redirects.
   */
  const logout = (): void => {
    token.value = null
    user.value = null
    localStorage.removeItem('auth_token')
    localStorage.removeItem('user_username')

    // Redirect to login page
    router.push('/login')
  }

  return {
    token,
    user,
    isAuthenticated,
    login,
    logout,
    initializeAuth,
  }
})
