
// Audit DTOs
export interface AuditEntryDto {
  trailId: string;
  action: 'Create' | 'Update' | 'Delete';
  userId: string;
  dateUtc: string;
  entityName: string;
  primaryKey: string | null;
  changes: ChangeDetailDto[];
  fullSnapshot: string;
}

export interface ChangeDetailDto {
  property: string;
  oldValue: string | null;
  newValue: string | null;
}