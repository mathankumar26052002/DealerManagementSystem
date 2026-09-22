import { Component, inject, ChangeDetectorRef } from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { Router } from '@angular/router';

import { finalize } from 'rxjs';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-login',

  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],

  templateUrl: './login.html',

  styleUrl: './login.scss'
})
export class Login {

  private readonly fb =
    inject(FormBuilder);

  private readonly cdr =
    inject(ChangeDetectorRef);

  private readonly authService =
    inject(AuthService);

  private readonly router =
    inject(Router);


 

  loginForm = this.fb.nonNullable.group({

    username: [
      '',
      Validators.required
    ],

    password: [
      '',
      Validators.required
    ]

  });




  loading = false;

  errorMessage = '';




  clearError(): void {

    this.errorMessage = '';

  }




  submit(): void {


    this.errorMessage = '';




    if (this.loginForm.invalid) {

      this.loginForm.markAllAsTouched();

      return;
    }




    this.loading = true;


    const request =
      this.loginForm.getRawValue();


    console.log(
      'Login request:',
      request.username
    );


 

    this.authService
      .login(request)
      .pipe(

        finalize(() => {

          console.log('FINALIZE');

          this.loading = false;

          this.cdr.detectChanges();

        })

      )
      .subscribe({

      

        next: response => {

          console.log(
            'Login successful:',
            response
          );


          if (response.role === 'Admin') {

            this.router.navigate([
              '/admin/dashboard'
            ]);

            return;
          }


          if (response.role === 'Dealer') {

            this.router.navigate([
              '/dealer/catalog'
            ]);

            return;
          }


          this.errorMessage =
            'Unknown user role.';

        },




        error: error => {

          console.error(
            'Login failed:',
            error
          );


       

          if (error.status === 0) {

            this.errorMessage =
              'Unable to connect to the server. Please make sure the API is running.';

          }


        

          else if (
            error?.error?.message
          ) {

            this.errorMessage =
              error.error.message;

          }


        

          else if (error.status === 401) {

            this.errorMessage =
              'Invalid username or password.';

          }


          else if (error.status >= 500) {

            this.errorMessage =
              'Server error. Please try again later.';

          }


          else {

            this.errorMessage =
              'Login failed. Please try again.';

          }


          console.log(
            'Displayed error:',
            this.errorMessage
          );


          this.cdr.detectChanges();

        }

      });

  }
}