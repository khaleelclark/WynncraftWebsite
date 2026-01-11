import { defineConfig } from "@rsbuild/core";
import { pluginReact } from "@rsbuild/plugin-react";

export default defineConfig({
  source: {
    entry: {
      index: "./src/index.tsx",
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
      "og:image": "https://khaleelclark.com/public/imperial-og-logo.png",
      "og:url": "https://khaleelclark.com",
      "og:type": "website",
      "twitter:title": "Imperial Guild",
      "twitter:description": "Imperial Guild",
      "twitter:card": "summary_large_image",
      "twitter:image": "https://khaleelclark.com/public/imperial-og-logo.png",
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
      "/api": "http://192.168.4.121:5032", //needs secret
    },
  },
});
