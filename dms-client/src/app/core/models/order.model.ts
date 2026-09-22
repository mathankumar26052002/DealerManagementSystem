export interface CreateOrderItemRequest {
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  items: CreateOrderItemRequest[];
}

export interface UpdateOrderItemRequest {
  quantity: number;
}

export interface OrderItem {
  id: number;
  productId: number;
  productCodeSnapshot: string;
  productNameSnapshot: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface OrderStatusHistory {
  id: number;
  previousStatus: string | null;
  newStatus: string;
  changedByUserId?: number;
  changedByUsername: string;
  changedAt: string;
  remarks: string | null;
}

export interface Order {
  id: number;
  orderNumber: string;
  dealerId: number;
  dealerCode: string;
  companyName: string;

  status: string;
  totalAmount: number;

  createdAt: string;
  submittedAt: string | null;
  approvedAt: string | null;

  items: OrderItem[];

  statusHistory: OrderStatusHistory[];
}
export interface AdminOrderItem {
  id: number;
  productId: number;
  productCode: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface AdminOrder {
  id: number;
  orderNumber: string;

  dealerId: number;
  dealerCode: string;
  companyName: string;

  status: string;

  totalAmount: number;

  createdAt: string;
  submittedAt: string | null;
  approvedAt: string | null;

  items: AdminOrderItem[];

  statusHistory: OrderStatusHistory[];
}