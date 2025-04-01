/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#44bba4',
          50: '#edfaf7',
          100: '#d2f2ea',
          200: '#a9e5d7',
          300: '#76d1be',
          400: '#44bba4',
          500: '#2aa18a',
          600: '#1f8271',
          700: '#1c685c',
          800: '#1a544a',
          900: '#18463f',
          950: '#0a2924',
        },
        background: {
          light: '#ffffff',
          dark: '#111827',
        },
        surface: {
          light: '#f9fafb',
          dark: '#1f2937',
        },
        text: {
          light: {
            primary: '#1e293b',
            secondary: '#64748b',
          },
          dark: {
            primary: '#f9fafb',
            secondary: '#9ca3af',
          },
        },
        border: {
          light: '#e2e8f0',
          dark: '#374151',
        },
      },
    },
  },
  plugins: [],
}
