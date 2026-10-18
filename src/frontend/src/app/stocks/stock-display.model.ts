import { PaginationModel } from "../shared/models/pagination.model";

export interface StockDisplayModel {
  categoryName: string;
  productName: string;
  productId: number;
  quantity: number;
  lowStock: boolean; // calculated by the backend (quantity <= 5)
  id: number;
  createDate: string;
  updateDate: string;
  isDeleted: boolean;
}

export interface PaginatedStocks extends PaginationModel {
  stocks: StockDisplayModel[];
}
