import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment/environment';

import {
  CreateOrderRequest,
  Order,
  UpdateOrderItemRequest
} from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Orders`;


  // Create Draft Order
  createDraft(
    request: CreateOrderRequest
  ): Observable<Order> {

    return this.http.post<Order>(
      this.apiUrl,
      request
    );
  }


  // Get Dealer Orders
  getMyOrders(
    search: string = '',
    status: string = '',
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<any> {

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

    return this.http.get<any>(
      this.apiUrl,
      { params }
    );
  }


  // Get single order
  getMyOrder(
    orderId: number
  ): Observable<Order> {

    return this.http.get<Order>(
      `${this.apiUrl}/${orderId}`
    );
  }


  // Update order item quantity
  updateItem(
    orderId: number,
    itemId: number,
    request: UpdateOrderItemRequest
  ): Observable<Order> {

    return this.http.put<Order>(
      `${this.apiUrl}/${orderId}/items/${itemId}`,
      request
    );
  }


  // Remove order item
  removeItem(
    orderId: number,
    itemId: number
  ): Observable<void> {

    return this.http.delete<void>(
      `${this.apiUrl}/${orderId}/items/${itemId}`
    );
  }


  // Submit order
  submit(
    orderId: number
  ): Observable<Order> {

    return this.http.post<Order>(
      `${this.apiUrl}/${orderId}/submit`,
      {}
    );
  }


  // Cancel order
  cancel(
    orderId: number
  ): Observable<Order> {

    return this.http.post<Order>(
      `${this.apiUrl}/${orderId}/cancel`,
      {}
    );
  }
}