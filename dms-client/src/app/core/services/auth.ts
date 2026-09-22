import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment/environment'
import {
  LoginRequest,
  LoginResponse
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Auth`;

  login(request: LoginRequest): Observable<LoginResponse> {

    return this.http
      .post<LoginResponse>(
        `${this.apiUrl}/login`,
        request
      )
      .pipe(
        tap(response => {

          localStorage.setItem(
            'token',
            response.token
          );

          localStorage.setItem(
            'username',
            response.username
          );

          localStorage.setItem(
            'role',
            response.role
          );

          if (response.dealerId != null) {

  localStorage.setItem(
    'dealerId',
    response.dealerId.toString()
  );
}
        })
      );
  }

  logout(): void {

    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    localStorage.removeItem('dealerId');
  }

  getToken(): string | null {

    return localStorage.getItem('token');
  }

  getUsername(): string | null {

    return localStorage.getItem('username');
  }

  getRole(): string | null {

    return localStorage.getItem('role');
  }

  getDealerId(): number | null {

    const dealerId =
      localStorage.getItem('dealerId');

    return dealerId
      ? Number(dealerId)
      : null;
  }

  isLoggedIn(): boolean {

    return !!this.getToken();
  }

  isAdmin(): boolean {

    return this.getRole() === 'Admin';
  }

  isDealer(): boolean {

    return this.getRole() === 'Dealer';
  }

  
}