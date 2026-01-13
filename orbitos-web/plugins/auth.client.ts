import { useAuth } from '~/composables/useAuth'

export default defineNuxtPlugin(async () => {
  const { initializeAuth } = useAuth()
  await initializeAuth()
})
