import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment/environment';
import { AdminOrder } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class AdminOrderService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/admin/orders`;


  getOrders(
    search: string = '',
    status: string = '',
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<AdminOrder[]> {

    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (search.trim()) {
      params = params.set(
        'search',
        search.trim()
      );
    }

    if (status) {
      params = params.set(
        'status',
        status
      );
    }

    return this.http.get<AdminOrder[]>(
      this.apiUrl,
      { params }
    );
  }


  getOrder(orderId: number): Observable<AdminOrder> {

    return this.http.get<AdminOrder>(
      `${this.apiUrl}/${orderId}`
    );
  }


  approve(orderId: number): Observable<AdminOrder> {

    return this.http.post<AdminOrder>(
      `${this.apiUrl}/${orderId}/approve`,
      {}
    );
  }


  reject(
    orderId: number,
    reason: string
  ): Observable<AdminOrder> {

    return this.http.post<AdminOrder>(
      `${this.apiUrl}/${orderId}/reject`,
      { reason }
    );
  }


  dispatch(orderId: number): Observable<AdminOrder> {

    return this.http.post<AdminOrder>(
      `${this.apiUrl}/${orderId}/dispatch`,
      {}
    );
  }


  deliver(orderId: number): Observable<AdminOrder> {

    return this.http.post<AdminOrder>(
      `${this.apiUrl}/${orderId}/deliver`,
      {}
    );
  }
}