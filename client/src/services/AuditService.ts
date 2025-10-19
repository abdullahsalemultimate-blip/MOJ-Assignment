// src/services/AuditService.ts
import api from '@/lib/axios';
import type { AuditEntryDto } from '../models/AuditEntryDto';

export class AuditService {
    private readonly apiPath = '/api/Audit';

    async getAuditTrail(entityName: string, primaryKey: number): Promise<AuditEntryDto[]> {
        const response = await api.get<AuditEntryDto[]>(
            `${this.apiPath}/${entityName}/${primaryKey}`
        );
        return response.data;
    }
}

export const auditService = new AuditService();