import {
  Component,
  OnInit,
  inject,
  signal
} from '@angular/core';

import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink , Router} from '@angular/router';

import { AdminOrderService } from '../../../core/services/admin-order';
import { AdminOrder } from '../../../core/models/order.model';

@Component({
  selector: 'app-admin-orders',
  standalone: true,

  imports: [
    FormsModule,
    DatePipe,
    DecimalPipe,
    RouterLink
  ],

  templateUrl: './orders.html',
  styleUrl: './orders.scss'
})
export class Orders implements OnInit {

  private readonly orderService =
    inject(AdminOrderService);


  // All orders received from backend
  allOrders = signal<AdminOrder[]>([]);

  // Orders displayed after filter
  orders = signal<AdminOrder[]>([]);


  loading = signal(false);

  errorMessage = signal('');


  private readonly router = inject(Router);

  search = signal('');

  selectedStatus = signal('');


  pageNumber = signal(1);

  pageSize = signal(10);

  totalCount = signal(0);

  totalPages = signal(0);


  ngOnInit(): void {
    this.loadOrders();
  }


  goBackToDashboard(): void {
  this.router.navigate(['/admin/dashboard']);
}


  // =====================================================
  // LOAD ALL ORDERS
  // =====================================================

  loadOrders(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.orderService
      .getOrders('', '', 1, 1000)
      .subscribe({

        next: (response) => {

          console.log(
            'Orders received:',
            response
          );

          this.allOrders.set(response);

          this.pageNumber.set(1);

          this.applyFilters();

          this.loading.set(false);
        },

        error: (error) => {

          console.error(error);

          this.errorMessage.set(
            error?.error?.message ??
            'Failed to load orders.'
          );

          this.loading.set(false);
        }

      });
  }


  // =====================================================
  // APPLY SEARCH + STATUS FILTER
  // =====================================================

  applyFilters(): void {

    let filtered =
      [...this.allOrders()];


    // -----------------------------
    // Search
    // -----------------------------

    const searchText =
      this.search()
        .trim()
        .toLowerCase();


    if (searchText) {

      filtered = filtered.filter(order =>
        order.orderNumber
          .toLowerCase()
          .includes(searchText)
      );

    }


    // -----------------------------
    // Status
    // -----------------------------

    const status =
      this.selectedStatus();


    if (status) {

      filtered = filtered.filter(order =>
        order.status.toLowerCase() ===
        status.toLowerCase()
      );

    }


    console.log(
      'Selected status:',
      status
    );

    console.log(
      'Filtered orders:',
      filtered
    );


    // -----------------------------
    // Pagination
    // -----------------------------

    this.totalCount.set(
      filtered.length
    );


    this.totalPages.set(
      Math.ceil(
        filtered.length /
        this.pageSize()
      )
    );


    if (
      this.totalPages() > 0 &&
      this.pageNumber() > this.totalPages()
    ) {

      this.pageNumber.set(
        this.totalPages()
      );

    }


    const startIndex =
      (this.pageNumber() - 1) *
      this.pageSize();


    const endIndex =
      startIndex +
      this.pageSize();


    this.orders.set(
      filtered.slice(
        startIndex,
        endIndex
      )
    );
  }


  // =====================================================
  // SEARCH
  // =====================================================

  searchOrders(): void {

    this.pageNumber.set(1);

    this.applyFilters();
  }


  // =====================================================
  // STATUS FILTER
  // =====================================================

  changeStatus(status: string): void {

    console.log(
      'Status selected:',
      status
    );

    this.selectedStatus.set(status);

    this.pageNumber.set(1);

    this.applyFilters();
  }


  // =====================================================
  // CLEAR
  // =====================================================

  clearFilters(): void {

    this.search.set('');

    this.selectedStatus.set('');

    this.pageNumber.set(1);

    this.applyFilters();
  }


  // =====================================================
  // PREVIOUS
  // =====================================================

  previousPage(): void {

    if (this.pageNumber() > 1) {

      this.pageNumber.update(
        page => page - 1
      );

      this.applyFilters();
    }
  }


  // =====================================================
  // NEXT
  // =====================================================

  nextPage(): void {

    if (
      this.pageNumber() <
      this.totalPages()
    ) {

      this.pageNumber.update(
        page => page + 1
      );

      this.applyFilters();
    }
  }


  // =====================================================
  // STATUS CSS
  // =====================================================

  getStatusClass(
    status: string
  ): string {

    return `status-${status.toLowerCase()}`;
  }

}