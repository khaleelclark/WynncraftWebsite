import { defineConfig } from "@rsbuild/core";

export default defineConfig({
  html: {
    template: "./src/index.html",
  },
  tools: {
    rspack: {
      watchOptions: {
        poll: 1000, // check for changes every 1s
        ignored: /node_modules/,
      },
    },
  },
  server: {
    port: 5173,
    proxy: {
      "/api": "http://backend:5032",
    },
  },
});
