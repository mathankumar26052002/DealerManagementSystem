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

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { DecimalPipe } from '@angular/common';
import { ProductService } from '../../../core/services/product';
import { RouterLink , Router} from '@angular/router';

import {
  Product,
  CreateProduct,
  UpdateProduct
} from '../../../core/models/product.model';

@Component({
  selector: 'app-products',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
     DecimalPipe
  ],
  templateUrl: './products.html',
  styleUrl: './products.scss'
})
export class Products implements OnInit {

  private readonly productService =
    inject(ProductService);

  private readonly fb =
    inject(FormBuilder);


      private readonly router = inject(Router);


  products = signal<Product[]>([]);

  loading = signal(false);

  saving = signal(false);

  errorMessage = signal('');

  successMessage = signal('');

  editingId = signal<number | null>(null);

  displayedColumns = [
    'productCode',
    'name',
    'category',
    'unitPrice',
    'availableStock',
    'isActive',
    'actions'
  ];

  productForm = this.fb.nonNullable.group({

    productCode: [
      '',
      [
        Validators.required,
        Validators.maxLength(50)
      ]
    ],

    name: [
      '',
      [
        Validators.required,
        Validators.maxLength(200)
      ]
    ],

    category: [
      '',
      [
        Validators.required,
        Validators.maxLength(100)
      ]
    ],

    unitPrice: [
      0,
      [
        Validators.required,
        Validators.min(0.01)
      ]
    ],

    availableStock: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ]

  });

  ngOnInit(): void {
    this.loadProducts();
  }


   goBackToDashboard(): void {
  this.router.navigate(['/admin/dashboard']);
}

  loadProducts(): void {

    this.loading.set(true);

    this.errorMessage.set('');

    this.productService
      .getAll()
      .subscribe({

        next: response => {

          console.log('Products:',  response.length);
          this.products.set(response);

          this.loading.set(false);
                  

        },

        error: error => {

          console.error(
            'Product loading failed:',
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

  saveProduct(): void {

    if (this.productForm.invalid) {

      this.productForm.markAllAsTouched();

      return;
    }

    this.saving.set(true);

    this.errorMessage.set('');
    this.successMessage.set('');

    const request =
      this.productForm.getRawValue();

    const id = this.editingId();

    if (id === null) {

      this.createProduct(request);

    } else {

      this.updateProduct(id, request);
    }
  }

  private createProduct(
    request: CreateProduct
  ): void {

    this.productService
      .create(request)
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Product created successfully.'
          );

          this.resetForm();

          this.loadProducts();
        },

        error: error => {

          this.saving.set(false);

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to create product.'
          );
        }

      });
  }

  private updateProduct(
    id: number,
    request: UpdateProduct
  ): void {

    this.productService
      .update(id, request)
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Product updated successfully.'
          );

          this.resetForm();

          this.loadProducts();
        },

        error: error => {

          this.saving.set(false);

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to update product.'
          );
        }

      });
  }

  editProduct(product: Product): void {

    this.editingId.set(product.id);

    this.errorMessage.set('');
    this.successMessage.set('');

    this.productForm.patchValue({

      productCode: product.productCode,

      name: product.name,

      category: product.category,

      unitPrice: product.unitPrice,

      availableStock: product.availableStock

    });
  }

  toggleStatus(product: Product): void {

    this.errorMessage.set('');
    this.successMessage.set('');

    this.productService
      .updateStatus(
        product.id,
        {
          isActive: !product.isActive
        }
      )
      .subscribe({

        next: () => {

          this.successMessage.set(
            product.isActive
              ? 'Product deactivated successfully.'
              : 'Product activated successfully.'
          );

          this.loadProducts();
        },

        error: error => {

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to update product status.'
          );
        }

      });
  }

  deleteProduct(product: Product): void {

    const confirmed =
      window.confirm(
        `Delete product "${product.name}"?`
      );

    if (!confirmed) {
      return;
    }

    this.productService
      .delete(product.id)
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Product deleted successfully.'
          );

          this.loadProducts();
        },

        error: error => {

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to delete product.'
          );
        }

      });
  }

  resetForm(): void {

    this.productForm.reset({
      productCode: '',
      name: '',
      category: '',
      unitPrice: 0,
      availableStock: 0
    });

    this.editingId.set(null);

    this.saving.set(false);
  }
}