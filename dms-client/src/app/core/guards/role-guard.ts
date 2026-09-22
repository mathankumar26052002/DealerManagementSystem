import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../services/auth';

export const roleGuard: CanActivateFn = (route) => {

  const authService = inject(AuthService);
  const router = inject(Router);

  const expectedRole =
    route.data['role'] as string;

  const currentRole =
    authService.getRole();

  if (currentRole === expectedRole) {
    return true;
  }

  if (currentRole === 'Admin') {
    return router.createUrlTree([
      '/admin/dashboard'
    ]);
  }

  if (currentRole === 'Dealer') {
    return router.createUrlTree([
      '/dealer/catalog'
    ]);
  }

  return router.createUrlTree([
    '/login'
  ]);
};