/**
 * OrbitOS Design System - Tailwind CSS Preset
 *
 * Usage in any project:
 * 1. Copy this file to your project
 * 2. In tailwind.config.js, add: presets: [require('./tailwind.preset.js')]
 */

module.exports = {
  theme: {
    extend: {
      // Custom animations
      animation: {
        'pulse-slow': 'pulse-slow 4s cubic-bezier(0.4, 0, 0.6, 1) infinite',
        'spin-slow': 'spin 2s linear infinite',
      },
      keyframes: {
        'pulse-slow': {
          '0%, 100%': { opacity: '0.2', transform: 'scale(1)' },
          '50%': { opacity: '0.3', transform: 'scale(1.05)' },
        },
      },
      // Glass morphism backdrop blur
      backdropBlur: {
        xs: '2px',
      },
      // Custom colors for the design system
      colors: {
        glass: {
          light: 'rgba(255, 255, 255, 0.1)',
          medium: 'rgba(255, 255, 255, 0.05)',
          border: 'rgba(255, 255, 255, 0.1)',
          'border-light': 'rgba(255, 255, 255, 0.2)',
        },
      },
      // Box shadows with color
      boxShadow: {
        'glow-purple': '0 10px 40px -10px rgba(168, 85, 247, 0.3)',
        'glow-purple-lg': '0 10px 40px -10px rgba(168, 85, 247, 0.5)',
        'glow-blue': '0 10px 40px -10px rgba(59, 130, 246, 0.3)',
        'glow-green': '0 10px 40px -10px rgba(34, 197, 94, 0.3)',
      },
    },
  },
}
