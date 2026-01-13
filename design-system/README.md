# OrbitOS Design System

A modern glassmorphism design system built with Tailwind CSS.

## Quick Start

### 1. Copy the files to your project:
```
design-system/
├── tailwind.preset.js          # Tailwind preset with custom config
├── orbitos-design-system.css   # CSS classes
└── README.md                   # This file
```

### 2. Configure Tailwind
```js
// tailwind.config.js
module.exports = {
  presets: [require('./tailwind.preset.js')],
  // ... rest of your config
}
```

### 3. Import the CSS
```css
/* In your main CSS file */
@import './orbitos-design-system.css';
```

---

## Design Tokens

### Colors
The design uses a dark theme with purple/blue gradients:
- **Background**: `from-slate-900 via-purple-900 to-slate-900`
- **Glass surfaces**: `bg-white/5` to `bg-white/10`
- **Borders**: `border-white/10` to `border-white/20`
- **Primary accent**: Purple to Blue gradient
- **Text**: `text-white`, `text-white/60`, `text-white/40`, `text-white/30`

### Spacing & Radius
- Cards: `rounded-2xl` (16px)
- Modals/Large cards: `rounded-3xl` (24px)
- Buttons: `rounded-xl` (12px) to `rounded-2xl` (16px)
- Inputs: `rounded-xl` (12px)
- Icons: `rounded-xl` (12px)

---

## Component Classes

### Backgrounds
```html
<!-- Main page background -->
<div class="orbitos-bg">

<!-- Alternative colors -->
<div class="orbitos-bg-blue">
<div class="orbitos-bg-emerald">
```

### Animated Background Blobs
```html
<div class="absolute inset-0 overflow-hidden pointer-events-none">
  <div class="orbitos-blob-purple w-80 h-80 -top-40 -right-40"></div>
  <div class="orbitos-blob-blue w-80 h-80 -bottom-40 -left-40 animation-delay-1000"></div>
  <div class="orbitos-blob-indigo w-96 h-96 top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 animation-delay-500"></div>
</div>
```

### Glass Cards
```html
<!-- Primary glass card (for modals, login forms) -->
<div class="orbitos-glass p-8">

<!-- Subtle glass card (for dashboard cards) -->
<div class="orbitos-glass-subtle p-6">

<!-- Interactive card with hover -->
<div class="orbitos-card">

<!-- Header -->
<header class="orbitos-glass-header">
```

### Buttons
```html
<!-- Primary gradient button -->
<button class="orbitos-btn-primary">
  Sign In
</button>

<!-- Secondary outline button -->
<button class="orbitos-btn-secondary">
  Cancel
</button>

<!-- Ghost button (for lists) -->
<button class="orbitos-btn-ghost">
  <svg>...</svg>
  <span>Action</span>
</button>
```

### Form Inputs
```html
<label class="orbitos-label">Email</label>
<input class="orbitos-input" type="email" placeholder="you@company.com" />
```

### Icons
```html
<!-- Icon in colored container -->
<div class="orbitos-icon-container orbitos-icon-md orbitos-icon-purple">
  <svg class="w-5 h-5 text-purple-400">...</svg>
</div>

<!-- Gradient icon (for branding) -->
<div class="orbitos-icon-container orbitos-icon-xl orbitos-icon-gradient">
  <svg class="w-8 h-8 text-white">...</svg>
</div>
```

### Avatars
```html
<div class="orbitos-avatar orbitos-avatar-md">RC</div>
```

### Status Badges
```html
<div class="orbitos-badge-success">Connected successfully</div>
<div class="orbitos-badge-error">Connection failed</div>
<div class="orbitos-badge-warning">Warning message</div>
```

### Typography
```html
<h1 class="orbitos-heading-xl">Page Title</h1>
<h2 class="orbitos-heading-lg">Section Title</h2>
<h3 class="orbitos-heading-md">Card Title</h3>
<p class="orbitos-text">Body text</p>
<p class="orbitos-text-muted">Secondary text</p>
<p class="orbitos-text-xs">Footer text</p>
```

### Spinners
```html
<div class="orbitos-spinner orbitos-spinner-md"></div>
```

### Dividers
```html
<div class="orbitos-divider"></div>

<!-- With text -->
<div class="orbitos-divider-with-text">
  <span class="text-white/30 text-xs uppercase">or</span>
</div>
```

---

## Full Page Template

```html
<div class="orbitos-bg">
  <!-- Background blobs -->
  <div class="absolute inset-0 overflow-hidden pointer-events-none">
    <div class="orbitos-blob-purple w-80 h-80 -top-40 -right-40"></div>
    <div class="orbitos-blob-blue w-80 h-80 -bottom-40 -left-40 animation-delay-1000"></div>
  </div>

  <!-- Header -->
  <header class="relative orbitos-glass-header">
    <div class="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
      <div class="flex items-center gap-3">
        <div class="orbitos-icon-container orbitos-icon-md orbitos-icon-gradient">
          <svg class="w-5 h-5 text-white">...</svg>
        </div>
        <h1 class="orbitos-heading-md">App Name</h1>
      </div>
      <div class="flex items-center gap-4">
        <div class="orbitos-avatar orbitos-avatar-md">RC</div>
        <button class="orbitos-btn-secondary py-2 px-4 text-sm">Sign Out</button>
      </div>
    </div>
  </header>

  <!-- Main Content -->
  <main class="relative max-w-7xl mx-auto px-4 py-8">
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="orbitos-card">
        <h3 class="orbitos-heading-sm mb-4">Card Title</h3>
        <p class="orbitos-text">Card content here</p>
      </div>
    </div>
  </main>

  <!-- Footer -->
  <footer class="relative py-6 border-t border-white/10">
    <p class="text-center orbitos-text-xs">&copy; 2026 Your Company</p>
  </footer>
</div>
```

---

## Customization

### Changing the primary color
Replace `purple` with your brand color throughout:
- Gradients: `from-purple-600 to-blue-600` → `from-brand-600 to-blue-600`
- Shadows: `shadow-purple-500/30` → `shadow-brand-500/30`
- Blobs: `bg-purple-500` → `bg-brand-500`

### Light mode variant
For a light mode, invert the colors:
- Background: `from-slate-100 via-purple-50 to-slate-100`
- Glass: `bg-black/5` instead of `bg-white/5`
- Text: `text-slate-900`, `text-slate-600`, etc.
