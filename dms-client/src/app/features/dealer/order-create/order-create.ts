import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { ProductService } from '../../../core/services/product';
import { OrderService } from '../../../core/services/order';

import {
  ProductCatalogItem
} from '../../../core/models/product-catalog.model';

import {
  CreateOrderRequest
} from '../../../core/models/order.model';

@Component({
  selector: 'app-order-create',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    DecimalPipe
  ],
  templateUrl: './order-create.html',
  styleUrl: './order-create.scss'
})
export class OrderCreate implements OnInit {

  private readonly productService =
    inject(ProductService);

  private readonly orderService =
    inject(OrderService);

  private readonly fb =
    inject(FormBuilder);

    private readonly router = inject(Router);

  products = signal<ProductCatalogItem[]>([]);

  selectedProducts = signal<ProductCatalogItem[]>([]);

  quantities = signal<Record<number, number>>({});

  loading = signal(false);

  saving = signal(false);

  errorMessage = signal('');

  successMessage = signal('');
goToCatalog(): void {
  this.router.navigate(['/dealer/catalog']);
}
  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {

    this.loading.set(true);

    this.errorMessage.set('');

    this.productService
      .getCatalog('', 1, 100)
      .subscribe({

        next: response => {

          this.products.set(
            response.items
          );

          this.loading.set(false);
        },

        error: error => {

          console.error(
            'Products loading failed:',
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

  addProduct(product: ProductCatalogItem): void {

    const alreadyAdded =
      this.selectedProducts()
        .some(x => x.id === product.id);

    if (alreadyAdded) {
      return;
    }

    this.selectedProducts.update(
      products => [
        ...products,
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

  removeProduct(productId: number): void {

    this.selectedProducts.update(
      products =>
        products.filter(
          product => product.id !== productId
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

  getQuantity(productId: number): number {

    return this.quantities()[productId] ?? 1;
  }

  updateQuantity(
    productId: number,
    quantity: number
  ): void {

    if (!Number.isInteger(quantity)) {
      return;
    }

    if (quantity < 1) {
      return;
    }

    this.quantities.update(
      values => ({
        ...values,
        [productId]: quantity
      })
    );
  }

  getLineTotal(
    product: ProductCatalogItem
  ): number {

    const quantity =
      this.getQuantity(product.id);

    return product.unitPrice * quantity;
  }

  getOrderTotal(): number {

    return this.selectedProducts()
      .reduce(
        (total, product) =>
          total + this.getLineTotal(product),
        0
      );
  }

  createDraft(): void {

    if (this.selectedProducts().length === 0) {

      this.errorMessage.set(
        'Please add at least one product.'
      );

      return;
    }

    this.errorMessage.set('');
    this.successMessage.set('');

    const request: CreateOrderRequest = {

      items: this.selectedProducts()
        .map(product => ({
          productId: product.id,
          quantity: this.getQuantity(product.id)
        }))

    };

    this.saving.set(true);

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

          this.saving.set(false);

          this.selectedProducts.set([]);

          this.quantities.set({});
        },

        error: error => {

          console.error(
            'Create order failed:',
            error
          );

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to create order.'
          );

          this.saving.set(false);
        }

      });
  }
}