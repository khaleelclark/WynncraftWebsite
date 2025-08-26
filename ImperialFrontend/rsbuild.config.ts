import { defineConfig } from '@rsbuild/core';

export default defineConfig({
  html: {
    template: './src/index.html',
  },
  server: {
    port: 5173,
    proxy: {
      '/odata': 'http://localhost:5032',
    },
  },
});
