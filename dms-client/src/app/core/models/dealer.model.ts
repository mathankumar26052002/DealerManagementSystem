export interface Dealer {
  id: number;
  dealerCode: string;
  companyName: string;
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateDealer {
  dealerCode: string;
  companyName: string;
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
}

export interface UpdateDealer {
  dealerCode: string;
  companyName: string;
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
}

export interface UpdateDealerStatus {
  isActive: boolean;
}