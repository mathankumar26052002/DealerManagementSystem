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

import { RouterLink , Router} from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';

import { DealerService } from '../../../core/services/dealer';

import {
  Dealer,
  CreateDealer,
  UpdateDealer
} from '../../../core/models/dealer.model';

@Component({
  selector: 'app-dealers',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule
  ],
  templateUrl: './dealers.html',
  styleUrl: './dealers.scss'
})
export class Dealers implements OnInit {

  private readonly dealerService =
    inject(DealerService);

  private readonly fb =
    inject(FormBuilder);


      private readonly router = inject(Router);

  dealers = signal<Dealer[]>([]);

  loading = signal(false);

  saving = signal(false);

  errorMessage = signal('');

  successMessage = signal('');

  editingId = signal<number | null>(null);

  displayedColumns = [
    'dealerCode',
    'companyName',
    'contactPerson',
    'email',
    'phone',
    'isActive',
    'actions'
  ];

  dealerForm = this.fb.nonNullable.group({

    dealerCode: [
      '',
      [
        Validators.required,
        Validators.maxLength(50)
      ]
    ],

    companyName: [
      '',
      [
        Validators.required,
        Validators.maxLength(200)
      ]
    ],

    contactPerson: [
      '',
      [
        Validators.required,
        Validators.maxLength(100)
      ]
    ],

    email: [
      '',
      [
        Validators.required,
        Validators.email,
        Validators.maxLength(200)
      ]
    ],

    phone: [
      '',
      [
        Validators.required,
        Validators.maxLength(20)
      ]
    ],

    address: [
      '',
      [
        Validators.required,
        Validators.maxLength(500)
      ]
    ]

  });

  ngOnInit(): void {
    this.loadDealers();
  }

  loadDealers(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.dealerService
      .getAll()
      .subscribe({

        next: response => {

          this.dealers.set(response);

          this.loading.set(false);
        },

        error: error => {

          console.error(
            'Dealer loading failed:',
            error
          );

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to load dealers.'
          );

          this.loading.set(false);
        }

      });
  }

   goBackToDashboard(): void {
  this.router.navigate(['/admin/dashboard']);
}


  saveDealer(): void {

    if (this.dealerForm.invalid) {

      return;
    }

    this.saving.set(true);

    this.errorMessage.set('');
    this.successMessage.set('');

    const request =
      this.dealerForm.getRawValue();

    const id = this.editingId();

    if (id === null) {

      this.createDealer(request);

    } else {

      this.updateDealer(id, request);
    }
  }

  private createDealer(
    request: CreateDealer
  ): void {

    this.dealerService
      .create(request)
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Dealer created successfully.'
          );

          this.resetForm();

          this.loadDealers();
        },

        error: error => {

          this.saving.set(false);

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to create dealer.'
          );
        }

      });
  }

  private updateDealer(
    id: number,
    request: UpdateDealer
  ): void {

    this.dealerService
      .update(id, request)
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Dealer updated successfully.'
          );

          this.resetForm();

          this.loadDealers();
        },

        error: error => {

          this.saving.set(false);

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to update dealer.'
          );
        }

      });
  }

  editDealer(dealer: Dealer): void {

    this.editingId.set(dealer.id);

    this.successMessage.set('');
    this.errorMessage.set('');

    this.dealerForm.patchValue({

      dealerCode: dealer.dealerCode,

      companyName: dealer.companyName,

      contactPerson: dealer.contactPerson,

      email: dealer.email,

      phone: dealer.phone,

      address: dealer.address

    });
  }

  toggleStatus(dealer: Dealer): void {

    this.errorMessage.set('');
    this.successMessage.set('');

    this.dealerService
      .updateStatus(
        dealer.id,
        {
          isActive: !dealer.isActive
        }
      )
      .subscribe({

        next: () => {

          this.successMessage.set(
            dealer.isActive
              ? 'Dealer deactivated successfully.'
              : 'Dealer activated successfully.'
          );

          this.loadDealers();
        },

        error: error => {

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to update dealer status.'
          );
        }

      });
  }
resetForm(): void {
  this.dealerForm.reset();

  Object.values(this.dealerForm.controls).forEach(control => {
    control.markAsPristine();
    control.markAsUntouched();
    control.setErrors(null);
  });

  this.dealerForm.updateValueAndValidity();

  this.editingId.set(null);
  this.saving.set(false);
}
}