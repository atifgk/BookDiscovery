import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

const isDevelopment =
    env.NODE_ENV === 'development' ||
    env.ASPNETCORE_ENVIRONMENT === 'Development';

const baseFolder =
    env.APPDATA && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = 'bookdiscovery.client';

const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

const target = env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS
        ? env.ASPNETCORE_URLS.split(';')[0]
        : 'https://localhost:7229';

// ONLY run certificate logic locally
if (isDevelopment) {

    if (!fs.existsSync(baseFolder)) {
        fs.mkdirSync(baseFolder, { recursive: true });
    }

    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {

        const result = child_process.spawnSync(
            'dotnet',
            [
                'dev-certs',
                'https',
                '--export-path',
                certFilePath,
                '--format',
                'Pem',
                '--no-password'
            ],
            { stdio: 'inherit' }
        );

        if (result.status !== 0) {
            throw new Error('Could not create certificate.');
        }
    }
}

// https://vitejs.dev/config/
export default defineConfig({

    plugins: [react()],

    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },

    // ONLY enable dev HTTPS server locally
    server: isDevelopment
        ? {
            proxy: {
                '^/api': {
                    target,
                    secure: false
                }
            },

            port: parseInt(env.DEV_SERVER_PORT || '65037'),

            https: {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath),
            }
        }
        : undefined
});