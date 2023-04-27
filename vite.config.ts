import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
})

// import { defineConfig } from 'vite';
// import react from '@vitejs/plugin-react';
// import path from 'path';

// // https://vitejs.dev/config/
// export default defineConfig({
//   plugins: [react()],
//   build: {
//     outDir: 'build',
//     emptyOutDir: true,
//     rollupOptions: {
//       input: {
//         index: path.resolve(__dirname, 'index.html'),
//         Signup: path.resolve(__dirname, 'Signup/index.html'),
//         Packages: path.resolve(__dirname, 'Packages/index.html'),
//       },
//     },
//   },
//   server: {
//     port: 3000,
//     strictPort: true,
//     fs: {
//       strict: true,
//     },
//     proxy: {
//       '/api': {
//         target: 'http://localhost:4000',
//         changeOrigin: true,
//       },
//     },
//   },
// });



// import { defineConfig } from 'vite'
// import react from '@vitejs/plugin-react'

// // https://vitejs.dev/config/
// export default defineConfig({
//   base: '/',
//   plugins: [react()],
//   server: {
//     port: 3000,
//     open: true,
//     proxy: {
//       '/api': 'http://localhost:8080'
//     }
//   },
//   build: {
//     outDir: 'dist',
//     assetsDir: 'assets',
//     rollupOptions: {
//       input: 'src/main.tsx'
//     }
//   }
// })


// // import { defineConfig } from 'vite';
// // import react from '@vitejs/plugin-react';

// // export default defineConfig({
// //   plugins: [react()],
// //   base: '/',
// //   build: {
// //     outDir: 'dist',
// //     assetsDir: 'assets',
// //     sourcemap: true,
// //     chunkSizeWarningLimit: 1500,
// //   },
// //   server: {
// //     port: 3000,
// //     open: true,
// //     proxy: {
// //       '/api': 'http://localhost:5000',
// //     },
// //   },
// //   optimizeDeps: {
// //     include: ['react', 'react-dom'],
// //   },
// //   // Add pages configuration
// //   pages: {
// //     index: {
// //       entry: 'src/index.tsx',
// //       template: 'public/index.html',
// //       filename: 'index.html',
// //     },
// //     app: {
// //       entry: 'src/App.tsx',
// //       template: 'public/index.html',
// //       filename: 'App/index.html',
// //     },
// //     signin: {
// //       entry: 'src/Signin.tsx',
// //       template: 'public/index.html',
// //       filename: 'Signin/index.html',
// //     },
// //     signup: {
// //       entry: 'src/Signup.tsx',
// //       template: 'public/index.html',
// //       filename: 'Signup/index.html',
// //     },
// //     packages: {
// //       entry: 'src/Packages.tsx',
// //       template: 'public/index.html',
// //       filename: 'Packages/index.html',
// //     },
// //     package_info: {
// //       entry: 'src/Package_info.tsx',
// //       template: 'public/index.html',
// //       filename: 'Package_info/index.html',
// //     },
// //   },
// // });
