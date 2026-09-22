import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
 
import { DecimalPipe , DatePipe } from '@angular/common';

import { OrderService } from '../../../core/services/order';
import { Order } from '../../../core/models/order.model';

import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [
    FormsModule,
    DecimalPipe,
    DatePipe,
    RouterLink
  ],
  templateUrl: './orders.html',
  styleUrl: './orders.scss'
})
export class Orders implements OnInit {

  private readonly orderService = inject(OrderService);

  orders = signal<Order[]>([]);
  loading = signal(false);
  errorMessage = signal('');

  search = signal('');
  selectedStatus = signal('');

  pageNumber = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  totalPages = signal(0);

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.orderService.getMyOrders(
      this.search(),
      this.selectedStatus(),
      this.pageNumber(),
      this.pageSize()
    ).subscribe({
      next: (response) => {

        this.orders.set(response.items ?? []);
        this.totalCount.set(response.totalCount ?? 0);
        this.totalPages.set(response.totalPages ?? 0);

        this.loading.set(false);
      },

      error: (error) => {

        console.error('Failed to load orders:', error);

        this.errorMessage.set(
          error?.error?.message ?? 'Failed to load orders.'
        );

        this.loading.set(false);
      }
    });
  }

  searchOrders(): void {
    this.pageNumber.set(1);
    this.loadOrders();
  }

  clearSearch(): void {
    this.search.set('');
    this.selectedStatus.set('');
    this.pageNumber.set(1);

    this.loadOrders();
  }

  changeStatus(): void {
    this.pageNumber.set(1);
    this.loadOrders();
  }

  previousPage(): void {

    if (this.pageNumber() > 1) {
      this.pageNumber.update(page => page - 1);
      this.loadOrders();
    }
  }

  nextPage(): void {

    if (this.pageNumber() < this.totalPages()) {
      this.pageNumber.update(page => page + 1);
      this.loadOrders();
    }
  }

  getStatusClass(status: string): string {

    switch (status) {
      case 'Draft':
        return 'status-draft';

      case 'Submitted':
        return 'status-submitted';

      case 'Approved':
        return 'status-approved';

      case 'Dispatched':
        return 'status-dispatched';

      case 'Delivered':
        return 'status-delivered';

      case 'Rejected':
        return 'status-rejected';

      case 'Cancelled':
        return 'status-cancelled';

      default:
        return '';
    }
  }
}