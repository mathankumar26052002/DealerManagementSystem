export interface ProductCatalogItem {
  id: number;
  productCode: string;
  name: string;
  category: string;
  unitPrice: number;
  availableStock: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface ProductCatalogResponse {
  items: ProductCatalogItem[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}