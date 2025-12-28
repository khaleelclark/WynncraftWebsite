import { defineConfig } from "@rsbuild/core";
import { pluginReact } from "@rsbuild/plugin-react";

export default defineConfig({
  source: {
    entry: {
      // name "index" just matches the default HTML entry
      index: "./src/index.tsx",
    },
  },
  html: {
    template: "./src/index.html",
  },
  plugins: [pluginReact()],
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
