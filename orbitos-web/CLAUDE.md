# OrbitOS Web - AI Development Guide

## Overview

This is the **Nuxt 4 + Vuetify 3** web frontend for OrbitOS. It provides the primary user interface for business canvas management, process design, and organizational health analytics.

## Architecture

```
orbitos-web/
├── app/
│   ├── components/           # Vue components by feature
│   │   ├── canvas/          # Canvas editor components
│   │   ├── process/         # Process designer components
│   │   ├── resource/        # Resource registry components
│   │   ├── health/          # Health dashboard components
│   │   └── common/          # Shared UI components
│   ├── composables/         # Reusable composition functions
│   ├── layouts/             # Page layouts
│   ├── pages/               # File-based routing
│   ├── stores/              # Pinia state management
│   ├── types/               # TypeScript type definitions
│   └── utils/               # Helper functions
├── server/
│   ├── api/                 # Server API routes (BFF)
│   └── middleware/          # Server middleware
├── public/                  # Static assets
├── tests/
│   ├── unit/               # Vitest unit tests
│   └── e2e/                # Playwright E2E tests
└── .ai/                    # AI context files
```

## Tech Stack

| Component | Technology | Version |
|-----------|------------|---------|
| Framework | Nuxt | 4.x |
| UI Library | Vuetify | 3.x |
| State | Pinia | 2.x |
| HTTP | ofetch (built-in) | - |
| Forms | VeeValidate + Zod | - |
| Testing | Vitest + Playwright | - |
| Types | TypeScript | 5.x |

## Key Patterns

### 1. Page Structure

All pages follow a consistent pattern:

```vue
<!-- app/pages/canvases/[id].vue -->
<script setup lang="ts">
// 1. Route params
const route = useRoute()
const canvasId = computed(() => route.params.id as string)

// 2. Store
const canvasStore = useCanvasStore()

// 3. Data fetching
const { data: canvas, pending, error } = await useFetch(
  () => `/api/organizations/${useOrgId()}/canvases/${canvasId.value}`
)

// 4. SEO
useSeoMeta({
  title: () => canvas.value?.name ?? 'Canvas'
})
</script>

<template>
  <VContainer>
    <VProgressLinear v-if="pending" indeterminate />
    <VAlert v-else-if="error" type="error">{{ error.message }}</VAlert>
    <CanvasEditor v-else :canvas="canvas" />
  </VContainer>
</template>
```

### 2. API Composable

All API calls go through typed composables:

```typescript
// app/composables/useCanvasApi.ts
export function useCanvasApi() {
  const orgId = useOrgId()

  return {
    list: () => useFetch<CanvasListResponse>(
      `/api/organizations/${orgId}/canvases`
    ),

    get: (id: string) => useFetch<CanvasWithBlocks>(
      `/api/organizations/${orgId}/canvases/${id}`
    ),

    create: (data: CreateCanvasRequest) => $fetch<Canvas>(
      `/api/organizations/${orgId}/canvases`,
      { method: 'POST', body: data }
    ),

    update: (id: string, data: UpdateCanvasRequest) => $fetch<Canvas>(
      `/api/organizations/${orgId}/canvases/${id}`,
      { method: 'PUT', body: data }
    ),

    delete: (id: string) => $fetch(
      `/api/organizations/${orgId}/canvases/${id}`,
      { method: 'DELETE' }
    )
  }
}
```

### 3. Store Pattern (Pinia)

```typescript
// app/stores/canvas.ts
export const useCanvasStore = defineStore('canvas', () => {
  // State
  const canvases = ref<Canvas[]>([])
  const currentCanvas = ref<CanvasWithBlocks | null>(null)
  const loading = ref(false)

  // Getters
  const draftCanvases = computed(() =>
    canvases.value.filter(c => c.state === 'draft')
  )

  // Actions
  async function fetchCanvases() {
    loading.value = true
    try {
      const api = useCanvasApi()
      const { data } = await api.list()
      canvases.value = data.value?.items ?? []
    } finally {
      loading.value = false
    }
  }

  async function createCanvas(data: CreateCanvasRequest) {
    const api = useCanvasApi()
    const canvas = await api.create(data)
    canvases.value.push(canvas)
    return canvas
  }

  return {
    canvases,
    currentCanvas,
    loading,
    draftCanvases,
    fetchCanvases,
    createCanvas
  }
})
```

### 4. Component Pattern

```vue
<!-- app/components/canvas/CanvasCard.vue -->
<script setup lang="ts">
import type { Canvas } from '~/types/entities'

// Props with TypeScript
interface Props {
  canvas: Canvas
  selectable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  selectable: false
})

// Emits
const emit = defineEmits<{
  select: [canvas: Canvas]
  delete: [id: string]
}>()

// Composables
const { formatDate } = useDateFormat()

// Computed
const stateColor = computed(() => {
  switch (props.canvas.state) {
    case 'active': return 'success'
    case 'draft': return 'warning'
    case 'archived': return 'grey'
    default: return 'primary'
  }
})
</script>

<template>
  <VCard
    :class="{ 'cursor-pointer': selectable }"
    @click="selectable && emit('select', canvas)"
  >
    <VCardTitle>{{ canvas.name }}</VCardTitle>
    <VCardSubtitle>
      <VChip :color="stateColor" size="small">
        {{ canvas.state }}
      </VChip>
    </VCardSubtitle>
    <VCardText>{{ canvas.description }}</VCardText>
    <VCardActions>
      <VBtn
        color="primary"
        :to="`/canvases/${canvas.id}`"
      >
        Open
      </VBtn>
      <VSpacer />
      <VBtn
        icon="mdi-delete"
        color="error"
        variant="text"
        @click.stop="emit('delete', canvas.id)"
      />
    </VCardActions>
  </VCard>
</template>
```

## File Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Pages | `kebab-case.vue` | `canvases/index.vue` |
| Components | `PascalCase.vue` | `CanvasCard.vue` |
| Composables | `use{Name}.ts` | `useCanvasApi.ts` |
| Stores | `{name}.ts` | `canvas.ts` |
| Types | `{name}.ts` | `entities.ts` |

## Type Definitions

Types are generated from the OpenAPI spec:

```typescript
// app/types/entities.ts
// AUTO-GENERATED from ../contracts/openapi.yaml

export interface Canvas {
  id: string
  organizationId: string
  name: string
  description: string | null
  type: CanvasType
  state: CanvasState
  version: number
  createdAt: string
  updatedAt: string
  createdBy: string
  updatedBy: string
}

export type CanvasType = 'business_model' | 'lean' | 'value_proposition'
export type CanvasState = 'draft' | 'active' | 'archived'

export interface CreateCanvasRequest {
  name: string
  description?: string
  type: CanvasType
}
```

## Implementing a Screen from Specs

When implementing a screen (e.g., SCR-011 Canvas Editor):

### Step 1: Read the Spec
```bash
cat ../specs/L1-ui/screens/SCR-011-canvas-editor.json
```

### Step 2: Create the Page
```vue
<!-- app/pages/canvases/[id]/edit.vue -->
<script setup lang="ts">
// Map to spec: SCR-011 Canvas Editor

definePageMeta({
  middleware: ['auth', 'org-member']
})

const route = useRoute()
const canvasId = computed(() => route.params.id as string)

const api = useCanvasApi()
const { data: canvas, pending, error, refresh } = await api.get(canvasId.value)

// Auto-save with debounce
const { execute: saveCanvas } = useDebounceFn(async (blocks: CanvasBlocks) => {
  await api.update(canvasId.value, { blocks })
}, 500)

async function handleBlockUpdate(key: string, content: string) {
  if (!canvas.value) return
  canvas.value.blocks[key] = { ...canvas.value.blocks[key], content }
  await saveCanvas(canvas.value.blocks)
}
</script>

<template>
  <VContainer fluid>
    <!-- Spec: Header with canvas name and actions -->
    <VRow>
      <VCol>
        <h1>{{ canvas?.name }}</h1>
      </VCol>
      <VCol cols="auto">
        <VBtn color="primary" @click="/* export */">Export PDF</VBtn>
      </VCol>
    </VRow>

    <!-- Spec: 9-block grid layout -->
    <CanvasGrid
      v-if="canvas"
      :canvas="canvas"
      @update:block="handleBlockUpdate"
    />
  </VContainer>
</template>
```

### Step 3: Create Components
```vue
<!-- app/components/canvas/CanvasGrid.vue -->
<script setup lang="ts">
import type { CanvasWithBlocks } from '~/types/entities'

const props = defineProps<{
  canvas: CanvasWithBlocks
}>()

const emit = defineEmits<{
  'update:block': [key: string, content: string]
}>()

// Business Model Canvas layout (9 blocks)
const gridLayout = [
  { key: 'key_partners', row: 1, col: 1, rowSpan: 2 },
  { key: 'key_activities', row: 1, col: 2 },
  { key: 'key_resources', row: 2, col: 2 },
  { key: 'value_propositions', row: 1, col: 3, rowSpan: 2 },
  { key: 'customer_relationships', row: 1, col: 4 },
  { key: 'channels', row: 2, col: 4 },
  { key: 'customer_segments', row: 1, col: 5, rowSpan: 2 },
  { key: 'cost_structure', row: 3, col: 1, colSpan: 2 },
  { key: 'revenue_streams', row: 3, col: 3, colSpan: 3 }
]
</script>

<template>
  <div class="canvas-grid">
    <CanvasBlock
      v-for="block in gridLayout"
      :key="block.key"
      :block-key="block.key"
      :content="canvas.blocks[block.key]?.content ?? ''"
      :style="getGridStyle(block)"
      @update="content => emit('update:block', block.key, content)"
    />
  </div>
</template>

<style scoped>
.canvas-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  grid-template-rows: repeat(3, minmax(200px, auto));
  gap: 1rem;
}
</style>
```

## Running the App

```bash
# Development
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Type check
npm run typecheck
```

## Running Tests

```bash
# Unit tests
npm run test:unit

# Unit tests with coverage
npm run test:unit -- --coverage

# E2E tests (requires running app)
npm run test:e2e

# E2E tests in headed mode
npm run test:e2e -- --headed

# All tests
npm test
```

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `NUXT_PUBLIC_API_URL` | Backend API URL | http://localhost:5000/api/v1 |
| `NUXT_PUBLIC_AUTH_DOMAIN` | Auth0/Clerk domain | - |
| `NUXT_PUBLIC_AUTH_CLIENT_ID` | OAuth client ID | - |

## Vuetify Theme

```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  vuetify: {
    vuetifyOptions: {
      theme: {
        defaultTheme: 'light',
        themes: {
          light: {
            colors: {
              primary: '#0969da',
              secondary: '#8250df',
              success: '#1a7f37',
              warning: '#9a6700',
              error: '#cf222e'
            }
          }
        }
      }
    }
  }
})
```

## Accessibility (WCAG 2.1 AA)

Per NFR-USA-001:
- All interactive elements have focus states
- Color contrast ratio >= 4.5:1
- Keyboard navigation supported
- Screen reader labels on icons
- Form validation errors announced

```vue
<template>
  <VBtn
    aria-label="Delete canvas"
    icon="mdi-delete"
    @keydown.enter="handleDelete"
  />
</template>
```

## Security Checklist (SOC 2)

Before any PR:
- [ ] No secrets in client code
- [ ] User input sanitized (XSS prevention)
- [ ] Auth tokens stored securely (httpOnly cookies)
- [ ] API calls use HTTPS
- [ ] Sensitive data not logged to console
- [ ] CSP headers configured
