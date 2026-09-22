import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [

  // Login
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login')
        .then(m => m.Login)
  },


  // Admin Dashboard
  {
    path: 'admin/dashboard',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' },
    loadComponent: () =>
      import('./features/admin/dashboard/dashboard')
        .then(m => m.Dashboard)
  },


  // Admin Dealers
  {
    path: 'admin/dealers',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' },
    loadComponent: () =>
      import('./features/admin/dealers/dealers')
        .then(m => m.Dealers)
  },


  // Admin Products
  {
    path: 'admin/products',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' },
    loadComponent: () =>
      import('./features/admin/products/products')
        .then(m => m.Products)
  },

  {
  path: 'admin/orders/:id',
  canActivate: [authGuard, roleGuard],
  data: { role: 'Admin' },
  loadComponent: () =>
    import('./features/admin/order-details/order-details')
      .then(m => m.OrderDetails)
},


  // ⭐ Admin Orders - ADD THIS
  {
    path: 'admin/orders',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' },
    loadComponent: () =>
      import('./features/admin/orders/orders')
        .then(m => m.Orders)
  },


  // Dealer Catalog
  {
    path: 'dealer/catalog',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Dealer' },
    loadComponent: () =>
      import('./features/dealer/catalog/catalog')
        .then(m => m.Catalog)
  },


  // Dealer Orders
  {
    path: 'dealer/orders',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Dealer' },
    loadComponent: () =>
      import('./features/dealer/orders/orders')
        .then(m => m.Orders)
  },


  // Dealer Order Details
  {
    path: 'dealer/orders/:id',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Dealer' },
    loadComponent: () =>
      import('./features/dealer/order-details/order-details')
        .then(m => m.OrderDetails)
  },


  // Default
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },


  // Wildcard - MUST BE LAST
  {
    path: '**',
    redirectTo: 'login'
  }

];