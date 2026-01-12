import { defineConfig } from "@rsbuild/core";
import { pluginReact } from "@rsbuild/plugin-react";

// Read from environment at build time
const BACKEND_URL = process.env.BACKEND_URL ?? "http://localhost:5032";
const FRONTEND_URL = process.env.FRONTEND_URL ?? "http://localhost:5173";

export default defineConfig({
  source: {
    entry: {
      index: "./src/index.tsx",
    },
    define: {
      // Define as simple global constants that will be replaced at build time
      RSBUILD_PUBLIC_API_URL: JSON.stringify(BACKEND_URL),
      RSBUILD_BACKEND_URL: JSON.stringify(BACKEND_URL),
      RSBUILD_FRONTEND_URL: JSON.stringify(FRONTEND_URL),
    },
  },
  html: {
    template: "./src/index.html",
    favicon: "./public/favicon.ico",
    meta: {
      description:
        "Imperial Website. Designed and developed by esteemed members of the Imperial Guild Community.",
      author: "pto, thop",
      "og:title": "Imperial Website",
      "og:description": "Imperial Website",
      "og:image": `${FRONTEND_URL}/public/imperial-og-logo.png`,
      "og:url": FRONTEND_URL,
      "og:type": "website",
      "twitter:title": "Imperial Guild",
      "twitter:description": "Imperial Guild",
      "twitter:card": "summary_large_image",
      "twitter:image": `${FRONTEND_URL}/public/imperial-og-logo.png`,
    },
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
      "/api": BACKEND_URL,
    },
  },
});
