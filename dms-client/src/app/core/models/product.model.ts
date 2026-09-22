export interface Product {
  id: number;
  productCode: string;
  name: string;
  category: string;
  unitPrice: number;
  availableStock: number;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateProduct {
  productCode: string;
  name: string;
  category: string;
  unitPrice: number;
  availableStock: number;
}

export interface UpdateProduct {
  productCode: string;
  name: string;
  category: string;
  unitPrice: number;
  availableStock: number;
}

export interface UpdateProductStatus {
  isActive: boolean;
}