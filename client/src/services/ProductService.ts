// src/services/ProductService.ts
import api from '@/lib/axios'
import type {
  ProductDto,
  CreateProductCommand,
  UpdateProductCommand,
  AdjustStockQuantityCommand,
  ProductFilterRequest,
  PaginatedListOfProductDto,
  ProductDetailVm,
} from '@/models/Product'

export class ProductService {
  private readonly apiPath = '/api/Products'

  async getAll(request: ProductFilterRequest): Promise<PaginatedListOfProductDto> {
    const params = new URLSearchParams()
    if (request.searchTerm) params.append('SearchTerm', request.searchTerm)
    if (request.supplierId) params.append('SupplierId', request.supplierId.toString())
    params.append('PageNumber', request.pageNumber.toString())
    params.append('PageSize', request.pageSize.toString())

    const response = await api.get<PaginatedListOfProductDto>(this.apiPath, { params })
    return response.data
  }

  async getProductById(id: number): Promise<ProductDetailVm> {
    const response = await api.get<ProductDetailVm>(`${this.apiPath}/${id}`)
    return response.data
  }

  async create(data: CreateProductCommand): Promise<ProductDto> {
    const response = await api.post<ProductDto>(this.apiPath, data)
    return response.data
  }

  async update(data: UpdateProductCommand): Promise<void> {
    await api.put(`${this.apiPath}/${data.id}`, data)
  }

  async delete(id: number): Promise<void> {
    await api.delete(`${this.apiPath}/${id}`)
  }

  async adjustStock(data: AdjustStockQuantityCommand): Promise<void> {
    await api.put(`${this.apiPath}/${data.id}/adjust-stock-quantity`, data)
  }
}

export const productService = new ProductService()
