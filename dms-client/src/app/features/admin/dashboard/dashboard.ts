import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

import { DashboardService } from '../../../core/services/dashboard';
import { AdminDashboard } from '../../../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  imports: [
    MatCardModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {

  private readonly dashboardService =
    inject(DashboardService);

  dashboard = signal<AdminDashboard | null>(null);

  private readonly router = inject(Router);

  loading = signal(false);

  errorMessage = signal('');

  ngOnInit(): void {
    this.loadDashboard();
  }

    goToDealers(): void {
    this.router.navigate(['/admin/dealers']);
  }


  
  goToProducts(): void {
  this.router.navigate(['/admin/products']);
  
}

  goToOrders(): void {
    this.router.navigate(['/admin/orders']);
  }

  loadDashboard(): void {

    this.loading.set(true);

    this.errorMessage.set('');

    this.dashboardService
      .getDashboard()
      .subscribe({

        next: response => {

          console.log(
            'Dashboard data:',
            response
          );

          this.dashboard.set(response);

          this.loading.set(false);
        },

        error: error => {

          console.error(
            'Dashboard error:',
            error
          );

          this.loading.set(false);

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to load dashboard.'
          );
        }

      });
  }
   logout(): void {

    localStorage.removeItem('token');

    this.router.navigate(['/login']);

  }
}