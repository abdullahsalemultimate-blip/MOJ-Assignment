import api from '@/lib/axios'
import type { SupplierDto, CreateSupplierCommand, UpdateSupplierCommand } from '../models/Supplier'
import type { LookupDto } from '@/models/LookupDto'

export class SupplierService {
  private readonly apiPath = '/api/Suppliers'

  async getAll(): Promise<SupplierDto[]> {
    const response = await api.get<SupplierDto[]>(this.apiPath)
    return response.data
  }

  async getSupplierLookup(): Promise<LookupDto[]> {
    const response = await api.get<LookupDto[]>(`${this.apiPath}/lookup`)
    return response.data
  }

  async create(data: CreateSupplierCommand): Promise<SupplierDto> {
    const response = await api.post<SupplierDto>(this.apiPath, data)
    return response.data
  }

  async update(data: UpdateSupplierCommand): Promise<void> {
    await api.put(`${this.apiPath}/${data.id}`, data)
  }

  async delete(id: number): Promise<void> {
    await api.delete(`${this.apiPath}/${id}`)
  }
}

export const supplierService = new SupplierService()
