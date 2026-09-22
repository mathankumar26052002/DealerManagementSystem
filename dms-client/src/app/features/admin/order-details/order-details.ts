import {
  Component,
  OnInit,
  inject,
  signal
} from '@angular/core';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import {
  DatePipe,
  DecimalPipe
} from '@angular/common';

import {
  AdminOrderService
} from '../../../core/services/admin-order';

import {
  AdminOrder
} from '../../../core/models/order.model';


@Component({
  selector: 'app-admin-order-details',
  standalone: true,

  imports: [
    DatePipe,
    DecimalPipe
  ],

  templateUrl: './order-details.html',
  styleUrl: './order-details.scss'
})
export class OrderDetails implements OnInit {

  // -----------------------------------------
  // Dependencies
  // -----------------------------------------

  private readonly route =
    inject(ActivatedRoute);

  private readonly router =
    inject(Router);

  private readonly orderService =
    inject(AdminOrderService);


  // -----------------------------------------
  // Signals
  // -----------------------------------------

  order = signal<AdminOrder | null>(null);

  loading = signal(false);

  processing = signal(false);

  errorMessage = signal('');

  successMessage = signal('');


  // -----------------------------------------
  // Initial Load
  // -----------------------------------------

  ngOnInit(): void {

    const orderId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!orderId) {

      this.errorMessage.set(
        'Invalid order ID.'
      );

      return;
    }

    this.loadOrder(orderId);
  }


  // -----------------------------------------
  // Load Order From Database
  // -----------------------------------------

  loadOrder(
    orderId?: number,
    showLoading: boolean = true
  ): void {

    const id =
      orderId ??
      Number(
        this.route.snapshot.paramMap.get('id')
      );


    if (!id) {

      this.errorMessage.set(
        'Invalid order ID.'
      );

      return;
    }


    if (showLoading) {

      this.loading.set(true);

    }


    this.errorMessage.set('');


    this.orderService
      .getOrder(id)
      .subscribe({

        next: response => {

          console.log(
            'Latest order from database:',
            response
          );


          // Update UI with latest DB data
          this.order.set(response);


          this.loading.set(false);

        },


        error: error => {

          console.error(
            'Failed to load order:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to load order.'
          );


          this.loading.set(false);

        }

      });
  }


  // -----------------------------------------
  // Back to Order List
  // -----------------------------------------

  goBack(): void {

    this.router.navigate([
      '/admin/orders'
    ]);

  }


  // -----------------------------------------
  // Approve Order
  // -----------------------------------------

  approveOrder(): void {

    const order =
      this.order();


    if (!order) {

      return;

    }


    if (
      !confirm(
        'Are you sure you want to approve this order?'
      )
    ) {

      return;

    }


    this.processing.set(true);

    this.errorMessage.set('');

    this.successMessage.set('');


    this.orderService
      .approve(order.id)
      .subscribe({

        next: () => {

          this.processing.set(false);


          this.successMessage.set(
            'Order approved successfully.'
          );


          // Get latest data from database
          this.loadOrder(
            undefined,
            false
          );

        },


        error: error => {

          console.error(
            'Approve order failed:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to approve order.'
          );


          this.processing.set(false);

        }

      });
  }


  rejectOrder(): void {
  const order = this.order();

  if (!order) {
    return;
  }

  const reason = prompt('Enter rejection reason:');

  if (!reason || !reason.trim()) {
    this.errorMessage.set('Rejection reason is required.');
    return;
  }

  this.processing.set(true);
  this.errorMessage.set('');
  this.successMessage.set('');

  this.orderService
    .reject(order.id, reason.trim())
    .subscribe({
      next: () => {
        this.processing.set(false);

        this.successMessage.set(
          'Order rejected successfully.'
        );

        // Reload latest data from database
        this.loadOrder(undefined, false);
      },

      error: (error) => {
        console.error(
          'Reject order failed:',
          error
        );

        this.errorMessage.set(
          error?.error?.message ??
          'Failed to reject order.'
        );

        this.processing.set(false);
      }
    });
}

  // -----------------------------------------
  // Dispatch Order
  // -----------------------------------------

  dispatchOrder(): void {

    const order =
      this.order();


    if (!order) {

      return;

    }


    if (
      !confirm(
        'Mark this order as dispatched?'
      )
    ) {

      return;

    }


    this.processing.set(true);

    this.errorMessage.set('');

    this.successMessage.set('');


    this.orderService
      .dispatch(order.id)
      .subscribe({

        next: () => {

          this.processing.set(false);


          this.successMessage.set(
            'Order marked as dispatched.'
          );


          // Get latest data from database
          this.loadOrder(
            undefined,
            false
          );

        },


        error: error => {

          console.error(
            'Dispatch order failed:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to dispatch order.'
          );


          this.processing.set(false);

        }

      });
  }


  // -----------------------------------------
  // Deliver Order
  // -----------------------------------------

  deliverOrder(): void {

    const order =
      this.order();


    if (!order) {

      return;

    }


    if (
      !confirm(
        'Mark this order as delivered?'
      )
    ) {

      return;

    }


    this.processing.set(true);

    this.errorMessage.set('');

    this.successMessage.set('');


    this.orderService
      .deliver(order.id)
      .subscribe({

        next: () => {

          this.processing.set(false);


          this.successMessage.set(
            'Order marked as delivered.'
          );


          // Get latest data from database
          this.loadOrder(
            undefined,
            false
          );

        },


        error: error => {

          console.error(
            'Deliver order failed:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to mark order as delivered.'
          );


          this.processing.set(false);

        }

      });
  }

}