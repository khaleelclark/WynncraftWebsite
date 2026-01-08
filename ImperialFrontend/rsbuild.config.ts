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
    favicon: "./public/favicon.ico",
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
      "/api": "http://192.168.4.121:5032", //needs secret
    },
  },
});
