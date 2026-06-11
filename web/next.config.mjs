/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  webpack: (config, { dev }) => {
    // Sur Windows, le cache webpack sur disque provoque des avertissements
    // ENOENT (...\.next\cache\webpack\...pack.gz) en mode dev à cause d'une
    // course sur les fichiers. On bascule sur un cache mémoire en dev.
    if (dev) {
      config.cache = { type: "memory" };
    }
    return config;
  },
};

export default nextConfig;
