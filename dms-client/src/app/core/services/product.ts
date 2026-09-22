import { Injectable, inject } from '@angular/core';
import { HttpClient,HttpParams  } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment/environment';

import {
  Product,
  CreateProduct,
  UpdateProduct,
  UpdateProductStatus
} from '../models/product.model';


import {
  ProductCatalogItem,
  ProductCatalogResponse
} from '../models/product-catalog.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Products`;

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(
      this.apiUrl
    );
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(
      `${this.apiUrl}/${id}`
    );
  }

 getCatalog(
  search: string = '',
  pageNumber: number = 1,
  pageSize: number = 10
): Observable<ProductCatalogResponse> {

  let params = new HttpParams()
    .set('pageNumber', pageNumber)
    .set('pageSize', pageSize);

  if (search.trim()) {
    params = params.set(
      'search',
      search.trim()
    );
  }

  return this.http.get<ProductCatalogResponse>(
    `${this.apiUrl}/catalog`,
    {
      params
    }
  );
}

  create(request: CreateProduct): Observable<Product> {
    return this.http.post<Product>(
      this.apiUrl,
      request
    );
  }

  update(
    id: number,
    request: UpdateProduct
  ): Observable<Product> {
    return this.http.put<Product>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }

  updateStatus(
    id: number,
    request: UpdateProductStatus
  ): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/${id}/status`,
      request
    );
  }

  
}