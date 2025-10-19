export interface LargestSupplierDto {
    supplierId: number;
    supplierName: string;
    totalProducts: number;
    totalProductsInSystem: number;
}

export interface ReorderProductDto {
    id: number;
    name: string;
    unitsInStock: number;
    reorderLevel: number;
    supplierName: string;
}

export interface MinimumOrderProductDto {
    id: number;
    name: string;
    unitsOnOrder: number;
    supplierName: string;
}