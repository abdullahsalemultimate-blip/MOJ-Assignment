import type { QuantityUnit } from './enums' // Will create this file next
import type { LookupDto } from './LookupDto'

export interface ProductDto {
  id: number
  name: string
  supplierName: string
  unitPrice: number
  unitsInStock: number
  unitsOnOrder: number
  reorderLevel: number
}

export interface CreateProductCommand {
  name: string
  supplierId: number
  quantityPerUnit: QuantityUnit
  unitPrice: number
  unitsInStock: number
  unitsOnOrder: number
  reorderLevel: number
}

export interface UpdateProductCommand extends CreateProductCommand {
  id: number
}

export interface AdjustStockQuantityCommand {
  id: number
  adjustmentDirection: 1 | 2 // 1 for Increasing, 2 for Decreasing
  amount: number
}

export interface PaginatedListOfProductDto {
  items: ProductDto[]
  pageNumber: number
  totalPages: number
  totalCount: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface ProductFilterRequest {
  searchTerm: string | null
  supplierId: number | null
  pageNumber: number
  pageSize: number
}

export interface ProductDetailVm {
  quantityUnits: LookupDto[]
  productDetail: ProductDetailDto
}

export interface ProductDetailDto {
  id: number
  name: string
  quantityPerUnit: QuantityUnit
  unitsInStock: number
  unitsOnOrder: number
  reorderLevel: number
  unitPrice: number
  supplierId: number
  created: string
  lastModified: string | null
}
