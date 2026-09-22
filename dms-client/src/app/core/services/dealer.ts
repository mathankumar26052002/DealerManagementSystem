import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment/environment';

import {
  Dealer,
  CreateDealer,
  UpdateDealer,
  UpdateDealerStatus
} from '../models/dealer.model';

@Injectable({
  providedIn: 'root'
})
export class DealerService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Dealers`;

  getAll(): Observable<Dealer[]> {

    return this.http.get<Dealer[]>(
      this.apiUrl
    );
  }

  getById(id: number): Observable<Dealer> {

    return this.http.get<Dealer>(
      `${this.apiUrl}/${id}`
    );
  }

  create(request: CreateDealer): Observable<Dealer> {

    return this.http.post<Dealer>(
      this.apiUrl,
      request
    );
  }

  update(
    id: number,
    request: UpdateDealer
  ): Observable<Dealer> {

    return this.http.put<Dealer>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  updateStatus(
    id: number,
    request: UpdateDealerStatus
  ): Observable<void> {

    return this.http.patch<void>(
      `${this.apiUrl}/${id}/status`,
      request
    );
  }
}