// src/services/StatisticService.ts
import api from '@/lib/axios';
import type { ReorderProductDto, LargestSupplierDto, MinimumOrderProductDto } from '@/models/Statistics';

export class StatisticService {
    private readonly apiPath = '/api/Statistics';

    async getReorderNeeded(): Promise<ReorderProductDto[]> {
        const response = await api.get<ReorderProductDto[]>(`${this.apiPath}/reorder-needed`);
        return response.data;
    }

    async getLargestSupplier(): Promise<LargestSupplierDto | null> {
        const response = await api.get<LargestSupplierDto | null>(`${this.apiPath}/largest-supplier`);
        return response.data;
    }

    async getMinimumOrdersProduct(): Promise<MinimumOrderProductDto | null> {
        const response = await api.get<MinimumOrderProductDto | null>(`${this.apiPath}/minimum-orders-product`);
        return response.data;
    }
}

export const statisticService = new StatisticService();