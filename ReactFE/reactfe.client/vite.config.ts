import { fileURLToPath, URL } from 'node:url'
import { defineConfig, loadEnv } from 'vite'
import plugin from '@vitejs/plugin-react'

export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '') 

    return {
        plugins: [plugin()],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url))
            }
        },
        server: {
            port: 100,
            host: true
        },
        define: {
            'import.meta.env.VITE_API_URL': JSON.stringify(
                process.env.VITE_API_URL ?? env.VITE_API_URL
            )
        }
    }
})