export interface SupplierDto {
  id: number
  name: string
}

export interface CreateSupplierCommand {
  name: string
}

export interface UpdateSupplierCommand {
  id: number
  name: string
}
