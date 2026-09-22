import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';

import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';

import { ProductService } from '../../../core/services/product';
import { OrderService } from '../../../core/services/order';


import { RouterLink,Router } from '@angular/router';

import {
  ProductCatalogItem
} from '../../../core/models/product-catalog.model';

import {
  CreateOrderRequest
} from '../../../core/models/order.model';

@Component({
  selector: 'app-catalog',
  imports: [
    FormsModule,
    DecimalPipe,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule,
    RouterLink
  ],
  templateUrl: './catalog.html',
  styleUrl: './catalog.scss'
})
export class Catalog implements OnInit {

  private readonly productService =
    inject(ProductService);

  private readonly orderService =
    inject(OrderService);

  products = signal<ProductCatalogItem[]>([]);

  loading = signal(false);

  creatingOrder = signal(false);

  errorMessage = signal('');

  successMessage = signal('');

  search = signal('');

  pageNumber = signal(1);

  pageSize = signal(10);

  totalCount = signal(0);

  totalPages = signal(0);

  // Products added to current order
  orderItems = signal<ProductCatalogItem[]>([]);

  // Quantity for each product
  quantities = signal<Record<number, number>>({});

  // Controls order summary visibility
  showOrderSummary = signal(false);

    private readonly router = inject(Router);


  ngOnInit(): void {

    this.loadProducts();
  }


  loadProducts(): void {

    this.loading.set(true);

    this.errorMessage.set('');

    this.productService
      .getCatalog(
        this.search(),
        this.pageNumber(),
        this.pageSize()
      )
      .subscribe({

        next: response => {

          this.products.set(
            response.items
          );

          this.totalCount.set(
            response.totalCount
          );

          this.totalPages.set(
            response.totalPages
          );

          this.loading.set(false);
        },

        error: error => {

          console.error(
            'Catalog loading failed:',
            error
          );

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to load products.'
          );

          this.loading.set(false);
        }

      });
  }


  searchProducts(): void {

    this.pageNumber.set(1);

    this.loadProducts();
  }


  clearSearch(): void {

    this.search.set('');

    this.pageNumber.set(1);

    this.loadProducts();
  }


  addToOrder(
    product: ProductCatalogItem
  ): void {

    const alreadyAdded =
      this.orderItems()
        .some(item => item.id === product.id);

    if (alreadyAdded) {
      return;
    }

    this.orderItems.update(
      items => [
        ...items,
        product
      ]
    );

    this.quantities.update(
      values => ({
        ...values,
        [product.id]: 1
      })
    );
  }


  removeFromOrder(
    productId: number
  ): void {

    this.orderItems.update(
      items =>
        items.filter(
          item => item.id !== productId
        )
    );

    this.quantities.update(
      values => {

        const updated = {
          ...values
        };

        delete updated[productId];

        return updated;
      }
    );
  }


  isAdded(
    productId: number
  ): boolean {

    return this.orderItems()
      .some(item => item.id === productId);
  }


  getQuantity(
    productId: number
  ): number {

    return this.quantities()[productId] ?? 1;
  }


  increaseQuantity(
    product: ProductCatalogItem
  ): void {

    const current =
      this.getQuantity(product.id);

    if (current >= product.availableStock) {
      return;
    }

    this.quantities.update(
      values => ({
        ...values,
        [product.id]: current + 1
      })
    );
  }


  decreaseQuantity(
    product: ProductCatalogItem
  ): void {

    const current =
      this.getQuantity(product.id);

    if (current <= 1) {
      return;
    }

    this.quantities.update(
      values => ({
        ...values,
        [product.id]: current - 1
      })
    );
  }


  getLineTotal(
    product: ProductCatalogItem
  ): number {

    return product.unitPrice *
      this.getQuantity(product.id);
  }


  getOrderTotal(): number {

    return this.orderItems()
      .reduce(
        (total, product) =>
          total + this.getLineTotal(product),
        0
      );
  }


  openOrderSummary(): void {

    if (this.orderItems().length === 0) {

      this.errorMessage.set(
        'Please add at least one product to the order.'
      );

      return;
    }

    this.errorMessage.set('');

    this.showOrderSummary.set(true);
  }


  closeOrderSummary(): void {

    this.showOrderSummary.set(false);
  }


  createDraftOrder(): void {

    if (this.orderItems().length === 0) {

      this.errorMessage.set(
        'Please add at least one product.'
      );

      return;
    }

    const request: CreateOrderRequest = {

      items: this.orderItems()
        .map(product => ({
          productId: product.id,
          quantity: this.getQuantity(product.id)
        }))

    };

    this.creatingOrder.set(true);

    this.errorMessage.set('');

    this.successMessage.set('');

    this.orderService
      .createDraft(request)
      .subscribe({

        next: response => {

          console.log(
            'Draft order created:',
            response
          );

          this.successMessage.set(
            `Draft order ${response.orderNumber} created successfully.`
          );

          this.orderItems.set([]);

          this.quantities.set({});

          this.showOrderSummary.set(false);

          this.creatingOrder.set(false);
        },

        error: error => {

          console.error(
            'Create draft order failed:',
            error
          );

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to create draft order.'
          );

          this.creatingOrder.set(false);
        }

      });
  }


  previousPage(): void {

    if (this.pageNumber() <= 1) {
      return;
    }

    this.pageNumber.update(
      page => page - 1
    );

    this.loadProducts();
  }


  nextPage(): void {

    if (
      this.pageNumber() >=
      this.totalPages()
    ) {
      return;
    }

    this.pageNumber.update(
      page => page + 1
    );

    this.loadProducts();
  }
  logout(): void {

    localStorage.removeItem('token');

    this.router.navigate(['/login']);

  }
}