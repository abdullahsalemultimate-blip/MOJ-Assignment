<template>
  <div class="w-64 bg-white border-r border-surface-200 p-4 shadow-lg">
    <nav class="space-y-2">
      <router-link
        to="/dashboard"
        class="flex items-center p-3 rounded-md text-surface-700 hover:bg-primary-50 hover:text-primary-500 transition-colors duration-200"
        active-class="bg-primary-50 text-primary-600 font-semibold"
      >
        <i class="pi pi-chart-pie mr-3 text-lg"></i>
        Dashboard
      </router-link>
      <router-link
        to="/products"
        class="flex items-center p-3 rounded-md text-surface-700 hover:bg-primary-50 hover:text-primary-500 transition-colors duration-200"
        active-class="bg-primary-50 text-primary-600 font-semibold"
      >
        <i class="pi pi-box mr-3 text-lg"></i>
        Products
      </router-link>
      <router-link
        to="/suppliers"
        class="flex items-center p-3 rounded-md text-surface-700 hover:bg-primary-50 hover:text-primary-500 transition-colors duration-200"
        active-class="bg-primary-50 text-primary-600 font-semibold"
      >
        <i class="pi pi-users mr-3 text-lg"></i>
        Suppliers
      </router-link>
    </nav>
  </div>
</template>
