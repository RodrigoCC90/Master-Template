<script setup lang="ts">
import { useAuth } from '~/composables/useAuth'

const { user, isAuthenticated, logout } = useAuth()
const api = useApi()

const apiStatus = ref<'idle' | 'success' | 'error'>('idle')
const apiMessage = ref('')
const isTestingApi = ref(false)

const getUserDisplayName = computed(() => {
  if (!user.value) return ''
  if ('displayName' in user.value) return user.value.displayName
  if ('name' in user.value) return user.value.name
  return ''
})

const getUserEmail = computed(() => {
  if (!user.value) return ''
  if ('email' in user.value) return user.value.email
  if ('username' in user.value) return user.value.username
  return ''
})

const getUserInitials = computed(() => {
  const name = getUserDisplayName.value
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
})

const testApiConnection = async () => {
  isTestingApi.value = true
  apiStatus.value = 'idle'
  try {
    const response = await api.get<{ message: string; user: string }>('/api/auth/ping-auth')
    apiStatus.value = 'success'
    apiMessage.value = `Connected as ${response.user}`
  } catch (error) {
    apiStatus.value = 'error'
    apiMessage.value = `${error}`
  } finally {
    isTestingApi.value = false
  }
}

// Redirect if not authenticated
watchEffect(() => {
  if (!isAuthenticated.value) {
    navigateTo('/')
  }
})
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 relative overflow-hidden">
    <!-- Animated background elements -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute -top-40 -right-40 w-80 h-80 bg-purple-500 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-pulse"></div>
      <div class="absolute -bottom-40 -left-40 w-80 h-80 bg-blue-500 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-pulse delay-1000"></div>
      <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-indigo-500 rounded-full mix-blend-multiply filter blur-3xl opacity-10 animate-pulse delay-500"></div>
    </div>

    <!-- Header -->
    <header class="relative backdrop-blur-xl bg-white/5 border-b border-white/10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-4">
          <!-- Logo -->
          <div class="flex items-center gap-3">
            <div class="flex items-center justify-center w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-blue-600 shadow-lg shadow-purple-500/30">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3c.132 0 .263 0 .393 0a7.5 7.5 0 0 0 7.92 12.446a9 9 0 1 1 -8.313-12.454z"></path>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 4a2 2 0 0 0 2 2a2 2 0 0 0 -2 2a2 2 0 0 0 -2 -2a2 2 0 0 0 2 -2"></path>
              </svg>
            </div>
            <div>
              <h1 class="text-xl font-bold text-white tracking-tight">OrbitOS</h1>
              <p class="text-white/40 text-xs">Dashboard</p>
            </div>
          </div>

          <!-- User menu -->
          <div class="flex items-center gap-4">
            <div class="hidden sm:block text-right">
              <p class="text-white font-medium text-sm">{{ getUserDisplayName }}</p>
              <p class="text-white/40 text-xs">{{ getUserEmail }}</p>
            </div>
            <div class="flex items-center justify-center w-10 h-10 rounded-full bg-gradient-to-br from-purple-500 to-blue-600 text-white font-semibold text-sm">
              {{ getUserInitials }}
            </div>
            <button
              @click="logout"
              class="flex items-center gap-2 py-2 px-4 bg-white/5 border border-white/10 rounded-xl text-white/70 hover:bg-white/10 hover:text-white transition-all duration-300 text-sm"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"></path>
              </svg>
              <span class="hidden sm:inline">Sign Out</span>
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Welcome Section -->
      <div class="mb-8">
        <h2 class="text-2xl font-bold text-white mb-2">Welcome back, {{ getUserDisplayName?.split(' ')[0] }}</h2>
        <p class="text-white/60">Here's what's happening with your workspace today.</p>
      </div>

      <!-- Stats Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="backdrop-blur-xl bg-white/5 border border-white/10 rounded-2xl p-6 hover:bg-white/10 transition-all duration-300">
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-purple-500/20">
              <svg class="w-6 h-6 text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path>
              </svg>
            </div>
            <span class="text-green-400 text-sm font-medium">+12%</span>
          </div>
          <p class="text-white/60 text-sm mb-1">Total Users</p>
          <p class="text-2xl font-bold text-white">1,234</p>
        </div>

        <div class="backdrop-blur-xl bg-white/5 border border-white/10 rounded-2xl p-6 hover:bg-white/10 transition-all duration-300">
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-blue-500/20">
              <svg class="w-6 h-6 text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
              </svg>
            </div>
            <span class="text-green-400 text-sm font-medium">+5%</span>
          </div>
          <p class="text-white/60 text-sm mb-1">Organizations</p>
          <p class="text-2xl font-bold text-white">56</p>
        </div>

        <div class="backdrop-blur-xl bg-white/5 border border-white/10 rounded-2xl p-6 hover:bg-white/10 transition-all duration-300">
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-emerald-500/20">
              <svg class="w-6 h-6 text-emerald-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <span class="text-green-400 text-sm font-medium">99.9%</span>
          </div>
          <p class="text-white/60 text-sm mb-1">System Uptime</p>
          <p class="text-2xl font-bold text-white">Online</p>
        </div>

        <div class="backdrop-blur-xl bg-white/5 border border-white/10 rounded-2xl p-6 hover:bg-white/10 transition-all duration-300">
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-amber-500/20">
              <svg class="w-6 h-6 text-amber-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
              </svg>
            </div>
            <span class="text-amber-400 text-sm font-medium">Active</span>
          </div>
          <p class="text-white/60 text-sm mb-1">API Status</p>
          <p class="text-2xl font-bold text-white">Healthy</p>
        </div>
      </div>

      <!-- Main Cards Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- API Connection Test Card -->
        <div class="backdrop-blur-xl bg-white/5 border border-white/10 rounded-2xl p-6">
          <div class="flex items-center gap-3 mb-6">
            <div class="flex items-center justify-center w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500/20 to-blue-500/20">
              <svg class="w-5 h-5 text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 12h14M5 12a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v4a2 2 0 01-2 2M5 12a2 2 0 00-2 2v4a2 2 0 002 2h14a2 2 0 002-2v-4a2 2 0 00-2-2m-2-4h.01M17 16h.01"></path>
              </svg>
            </div>
            <div>
              <h3 class="text-lg font-semibold text-white">API Connection</h3>
              <p class="text-white/40 text-sm">Test your backend connection</p>
            </div>
          </div>

          <button
            @click="testApiConnection"
            :disabled="isTestingApi"
            class="w-full flex items-center justify-center gap-2 py-3 px-4 bg-gradient-to-r from-purple-600 to-blue-600 rounded-xl font-semibold text-white shadow-lg shadow-purple-500/30 hover:shadow-purple-500/50 transition-all duration-300 hover:scale-[1.02] disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100"
          >
            <div v-if="isTestingApi" class="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin"></div>
            <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
            </svg>
            <span>{{ isTestingApi ? 'Testing...' : 'Test Connection' }}</span>
          </button>

          <!-- Status Message -->
          <div v-if="apiStatus !== 'idle'" class="mt-4">
            <div
              :class="[
                'px-4 py-3 rounded-xl text-sm',
                apiStatus === 'success' ? 'bg-green-500/10 border border-green-500/20 text-green-400' : 'bg-red-500/10 border border-red-500/20 text-red-400'
              ]"
            >
              <div class="flex items-center gap-2">
                <svg v-if="apiStatus === 'success'" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <span>{{ apiMessage }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions Card -->
        <div class="backdrop-blur-xl bg-white/5 border border-white/10 rounded-2xl p-6">
          <div class="flex items-center gap-3 mb-6">
            <div class="flex items-center justify-center w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500/20 to-teal-500/20">
              <svg class="w-5 h-5 text-emerald-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
              </svg>
            </div>
            <div>
              <h3 class="text-lg font-semibold text-white">Quick Actions</h3>
              <p class="text-white/40 text-sm">Common tasks at your fingertips</p>
            </div>
          </div>

          <div class="space-y-3">
            <button class="w-full flex items-center gap-3 py-3 px-4 bg-white/5 border border-white/10 rounded-xl text-white/70 hover:bg-white/10 hover:text-white transition-all duration-300 text-left">
              <svg class="w-5 h-5 text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"></path>
              </svg>
              <span>Invite Team Member</span>
            </button>
            <button class="w-full flex items-center gap-3 py-3 px-4 bg-white/5 border border-white/10 rounded-xl text-white/70 hover:bg-white/10 hover:text-white transition-all duration-300 text-left">
              <svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"></path>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
              </svg>
              <span>Organization Settings</span>
            </button>
            <button class="w-full flex items-center gap-3 py-3 px-4 bg-white/5 border border-white/10 rounded-xl text-white/70 hover:bg-white/10 hover:text-white transition-all duration-300 text-left">
              <svg class="w-5 h-5 text-amber-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
              <span>View Reports</span>
            </button>
          </div>
        </div>

        <!-- Recent Activity Card -->
        <div class="backdrop-blur-xl bg-white/5 border border-white/10 rounded-2xl p-6">
          <div class="flex items-center gap-3 mb-6">
            <div class="flex items-center justify-center w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500/20 to-orange-500/20">
              <svg class="w-5 h-5 text-amber-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <div>
              <h3 class="text-lg font-semibold text-white">Recent Activity</h3>
              <p class="text-white/40 text-sm">Your latest actions</p>
            </div>
          </div>

          <div class="space-y-4">
            <div class="flex items-start gap-3">
              <div class="flex items-center justify-center w-8 h-8 rounded-full bg-green-500/20 mt-0.5">
                <svg class="w-4 h-4 text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1"></path>
                </svg>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-white text-sm font-medium">Logged in successfully</p>
                <p class="text-white/40 text-xs">Just now</p>
              </div>
            </div>
            <div class="flex items-start gap-3">
              <div class="flex items-center justify-center w-8 h-8 rounded-full bg-purple-500/20 mt-0.5">
                <svg class="w-4 h-4 text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                </svg>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-white text-sm font-medium">Account created</p>
                <p class="text-white/40 text-xs">Welcome to OrbitOS!</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Footer -->
    <footer class="relative mt-12 py-6 border-t border-white/10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <p class="text-center text-white/30 text-sm">&copy; 2026 OrbitOS. All rights reserved.</p>
      </div>
    </footer>
  </div>
</template>

<style>
@keyframes pulse {
  0%, 100% { opacity: 0.2; transform: scale(1); }
  50% { opacity: 0.3; transform: scale(1.05); }
}

.animate-pulse {
  animation: pulse 4s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

.delay-500 {
  animation-delay: 500ms;
}

.delay-1000 {
  animation-delay: 1000ms;
}
</style>
