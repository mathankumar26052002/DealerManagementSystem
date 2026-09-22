import {
  Component,
  OnInit,
  inject,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { OrderService } from '../../../core/services/order';
import { Order } from '../../../core/models/order.model';


@Component({
  selector: 'app-order-details',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule
  ],

  templateUrl: './order-details.html',

  styleUrl: './order-details.scss'
})
export class OrderDetails implements OnInit {



  private readonly route =
    inject(ActivatedRoute);

  private readonly router =
    inject(Router);

  private readonly orderService =
    inject(OrderService);




  order = signal<Order | null>(null);

  loading = signal(false);

  processing = signal(false);

  errorMessage = signal('');

  successMessage = signal('');



  editingItemId =
    signal<number | null>(null);

  editingQuantity =
    signal<number>(1);


  

  ngOnInit(): void {

    const orderId =
      Number(
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
      .getMyOrder(id)
      .subscribe({

        next: (response) => {

          console.log(
            'Latest order from database:',
            response
          );


          this.order.set(response);


          this.loading.set(false);

          this.processing.set(false);
        },


        error: (error) => {

          console.error(
            'Failed to load order:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to load order.'
          );


          this.loading.set(false);

          this.processing.set(false);
        }

      });
  }




  goBack(): void {

    this.router.navigate([
      '/dealer/orders'
    ]);
  }




  isDraft(): boolean {

    return this.order()?.status === 'Draft';
  }


  isSubmitted(): boolean {

    return this.order()?.status === 'Submitted';
  }


  canCancel(): boolean {

    const status =
      this.order()?.status;

    return (
      status === 'Draft' ||
      status === 'Submitted'
    );
  }




  startEdit(
    itemId: number,
    quantity: number
  ): void {

    this.editingItemId.set(itemId);

    this.editingQuantity.set(quantity);

    this.errorMessage.set('');
  }




  cancelEdit(): void {

    this.editingItemId.set(null);

    this.editingQuantity.set(1);

    this.errorMessage.set('');
  }




  saveQuantity(
    itemId: number
  ): void {

    const order =
      this.order();


    if (!order) {
      return;
    }


    const quantity =
      Number(
        this.editingQuantity()
      );


   

    if (
      !Number.isInteger(quantity) ||
      quantity <= 0
    ) {

      this.errorMessage.set(
        'Quantity must be a positive whole number.'
      );

      return;
    }


    this.processing.set(true);

    this.errorMessage.set('');

    this.successMessage.set('');


    const orderId =
      order.id;


    this.orderService
      .updateItem(
        orderId,
        itemId,
        {
          quantity: quantity
        }
      )
      .subscribe({

        next: () => {

          this.editingItemId.set(null);

          this.successMessage.set(
            'Quantity updated successfully.'
          );


          // Get latest data from database

          this.loadOrder(
            orderId,
            false
          );
        },


        error: (error) => {

          console.error(
            'Update quantity failed:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to update quantity.'
          );


          this.processing.set(false);
        }

      });
  }




  removeItem(
    itemId: number
  ): void {

    const order =
      this.order();


    if (!order) {
      return;
    }


    if (
      !confirm(
        'Are you sure you want to remove this item?'
      )
    ) {

      return;
    }


    this.processing.set(true);

    this.errorMessage.set('');

    this.successMessage.set('');


    const orderId =
      order.id;


    this.orderService
      .removeItem(
        orderId,
        itemId
      )
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Item removed successfully.'
          );


        

          this.loadOrder(
            orderId,
            false
          );
        },


        error: (error) => {

          console.error(
            'Remove item failed:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to remove item.'
          );


          this.processing.set(false);
        }

      });
  }




  submitOrder(): void {

    const order =
      this.order();


    if (!order) {
      return;
    }


    if (
      !confirm(
        'Are you sure you want to submit this order?'
      )
    ) {

      return;
    }


    this.processing.set(true);

    this.errorMessage.set('');

    this.successMessage.set('');


    const orderId =
      order.id;


    this.orderService
      .submit(orderId)
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Order submitted successfully.'
          );




          this.loadOrder(
            orderId,
            false
          );
        },


        error: (error) => {

          console.error(
            'Submit order failed:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to submit order.'
          );


          this.processing.set(false);
        }

      });
  }



  cancelOrder(): void {

    const order =
      this.order();


    if (!order) {
      return;
    }


    if (
      !confirm(
        'Are you sure you want to cancel this order?'
      )
    ) {

      return;
    }


    this.processing.set(true);

    this.errorMessage.set('');

    this.successMessage.set('');


    const orderId =
      order.id;


    this.orderService
      .cancel(orderId)
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Order cancelled successfully.'
          );


       

          this.loadOrder(
            orderId,
            false
          );
        },


        error: (error) => {

          console.error(
            'Cancel order failed:',
            error
          );


          this.errorMessage.set(
            error?.error?.message ??
            'Failed to cancel order.'
          );


          this.processing.set(false);
        }

      });
  }

}