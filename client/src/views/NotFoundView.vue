<template>
  <div class="flex flex-col items-center justify-center min-h-screen text-center">
    <h1 class="text-6xl font-bold text-red-600">404</h1>
    <h2 class="text-3xl font-semibold mt-4">Page Not Found</h2>
    <p class="mt-2 text-gray-600">The page you are looking for does not exist.</p>
    <router-link to="/dashboard" class="mt-6"
      ><Button label="Go to Dashboard" icon="pi pi-home"
    /></router-link>
  </div>
</template>
