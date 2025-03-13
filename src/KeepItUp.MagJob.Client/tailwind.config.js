/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        background: {
          DEFAULT: '#ffffff',
          dark: '#1a1a1a',
        },
        text: {
          DEFAULT: '#000000',
          dark: '#ffffff',
        },
      },
    },
  },
  plugins: [],
}