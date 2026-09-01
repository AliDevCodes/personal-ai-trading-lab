import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Proxy /api requests to the local ASP.NET backend during development
      '/api': {
        target: 'http://localhost:5176',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
