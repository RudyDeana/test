import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import toast from 'react-hot-toast'

const API_BASE = '/api'

export const useAuthStore = create(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      isLoading: false,

      login: async (credentials) => {
        set({ isLoading: true })
        try {
          const response = await fetch(`${API_BASE}/login`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(credentials),
          })

          if (!response.ok) {
            const error = await response.json()
            throw new Error(error.error || 'Login fallito')
          }

          const data = await response.json()
          set({ user: data.user, token: data.token, isLoading: false })
          toast.success(`Benvenuto, ${data.user.username}!`)
          return data
        } catch (error) {
          set({ isLoading: false })
          toast.error(error.message)
          throw error
        }
      },

      register: async (userData) => {
        set({ isLoading: true })
        try {
          const response = await fetch(`${API_BASE}/register`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(userData),
          })

          if (!response.ok) {
            const error = await response.json()
            throw new Error(error.error || 'Registrazione fallita')
          }

          const data = await response.json()
          set({ isLoading: false })
          toast.success('Registrazione completata! Effettua il login.')
          return data
        } catch (error) {
          set({ isLoading: false })
          toast.error(error.message)
          throw error
        }
      },

      logout: () => {
        set({ user: null, token: null })
        toast.success('Logout effettuato')
      },

      checkAuth: () => {
        const { token } = get()
        if (!token) {
          set({ user: null, token: null })
          return false
        }

        try {
          // Basic token validation (in a real app, you'd validate with the server)
          const payload = JSON.parse(atob(token.split('.')[1]))
          if (payload.exp * 1000 < Date.now()) {
            set({ user: null, token: null })
            return false
          }
          return true
        } catch {
          set({ user: null, token: null })
          return false
        }
      },

      getAuthHeaders: () => {
        const { token } = get()
        return token ? { Authorization: `Bearer ${token}` } : {}
      },
    }),
    {
      name: 'voip-auth',
      partialize: (state) => ({
        user: state.user,
        token: state.token,
      }),
    }
  )
)