import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const defaultProxyTarget =
    mode === 'modular' ? 'http://localhost:5220' : 'http://localhost:5213'
  const apiProxyTarget = env.VITE_API_PROXY_TARGET || defaultProxyTarget

  return {
    plugins: [react()],
    server: {
      port: 5173,
      proxy: {
        '/api': {
          target: apiProxyTarget,
          changeOrigin: true,
        },
      },
    },
  }
})
