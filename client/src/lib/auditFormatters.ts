import {AdjustQuantityDirection, QuantityUnit } from '@/models/enums'
import dayjs from 'dayjs';
import localizedFormat from 'dayjs/plugin/localizedFormat';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';

// Initialize dayjs plugins
dayjs.extend(localizedFormat);
dayjs.extend(utc);
dayjs.extend(timezone);

// --- ENUM MAPPING ---
const QuantityUnitMap: Record<number, string> = {
    [QuantityUnit.Kilo]: 'Kilo',
    [QuantityUnit.Box]: 'Box',
    [QuantityUnit.Can]: 'Can',
    [QuantityUnit.Liter]: 'Liter',
    [QuantityUnit.Bottle]: 'Bottle',
    [QuantityUnit.None]: 'None',
};

const AdjustQuantityDirectionMap: Record<number, string> = {
    [AdjustQuantityDirection.Increasing]: 'Increasing',
    [AdjustQuantityDirection.Decreasing]: 'Decreasing',
    [AdjustQuantityDirection.None]: 'None',
};

// --- CORE FORMATTING FUNCTIONS ---

/**
 * Formats an ISO 8601 date string to a local, readable format.
 * Requirement: "MMM DD, YYYY HH:mm AM/PM"
 */
export const formatDate = (dateUtc: string): string => {
    // Convert UTC string to local timezone and format
    return dayjs.utc(dateUtc).local().format('MMM DD, YYYY hh:mm A');
};


/**
 * Gets the severity (color) for the action badge.
 */
export const getActionSeverity = (action: 'Create' | 'Update' | 'Delete'): 'success' | 'info' | 'danger' | 'secondary' => {
    switch (action) {
        case 'Create': return 'success';
        case 'Update': return 'info';
        case 'Delete': return 'danger';
        default: return 'secondary';
    }
};

export const getActionText = (action: 'Create' | 'Update' | 'Delete') => {
    return action;
};

/**
 * Formats a raw audit value based on the property name.
 */
export const formatAuditValue = (property: string, rawValue: string | null): string => {
    if (rawValue === null || rawValue === undefined || rawValue === 'null') return '(null)';
    if (rawValue === '') return '(Empty String)';
    
    const numericValue = Number(rawValue);

    // 1. Currency/Decimal Formatting (e.g., UnitPrice)
    if (property.toLowerCase().includes('price') || property.toLowerCase().includes('cost')) {
        const value = parseFloat(rawValue);
        if (!isNaN(value)) {
            return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
        }
    }

    // 2. Enum Mapping (e.g., QuantityPerUnit, AdjustmentDirection)
    if (property === 'QuantityPerUnit' && !isNaN(numericValue) && mapQuantityUnit(numericValue)) {
        return mapQuantityUnit(numericValue);
    }
    if (property === 'AdjustmentDirection' && !isNaN(numericValue) && AdjustQuantityDirectionMap[numericValue]) {
        return mapAdjustQuantityDirection(numericValue);
    }
    
    // 3. Boolean Mapping
    if (rawValue.toLowerCase() === 'true' || rawValue.toLowerCase() === 'false') {
        return rawValue.toLowerCase() === 'true' ? 'Yes' : 'No';
    }

    // 4. Date/Time (Optional: if date/time properties are in the audit trail)
    if (property.toLowerCase().includes('date') || property.toLowerCase().includes('created') || property.toLowerCase().includes('modified')) {
        if (dayjs(rawValue).isValid()) {
            return formatDate(rawValue);
        }
    }

    // 5. Default: Return the raw value
    return rawValue;
};


/**
 * Maps the numeric QuantityUnit enum value to its readable string.
 * @param value The numeric enum value.
 * @returns The readable string (e.g., "Kilo").
 */
export const mapQuantityUnit = (value: number | string | null | undefined): string => {
  if (value === null || value === undefined) return ''
  const num = typeof value === 'string' ? parseInt(value, 10) : value

  switch (num) {
    case QuantityUnit.Kilo:
      return 'Kilo'
    case QuantityUnit.Box:
      return 'Box'
    case QuantityUnit.Can:
      return 'Can'
    case QuantityUnit.Liter:
      return 'Liter'
    case QuantityUnit.Bottle:
      return 'Bottle'
    case QuantityUnit.None:
    default:
      return 'N/A'
  }
}

/**
 * Maps the numeric AdjustQuantityDirection enum value to its readable string.
 * @param value The numeric enum value.
 * @returns The readable string (e.g., "Increasing").
 */
export const mapAdjustQuantityDirection = (value: number | string | null | undefined): string => {
  if (value === null || value === undefined) return ''
  const num = typeof value === 'string' ? parseInt(value, 10) : value

  switch (num) {
    case AdjustQuantityDirection.Increasing:
      return 'Increasing'
    case AdjustQuantityDirection.Decreasing:
      return 'Decreasing'
    case AdjustQuantityDirection.None:
    default:
      return 'N/A'
  }
}

/**
 * Returns a list of QuantityUnit options for a Dropdown.
 */
export const getQuantityUnitOptions = () => {
  // Exclude QuantityUnit.None = 0
  return [
    { label: 'Kilo', value: QuantityUnit.Kilo },
    { label: 'Box', value: QuantityUnit.Box },
    { label: 'Can', value: QuantityUnit.Can },
    { label: 'Liter', value: QuantityUnit.Liter },
    { label: 'Bottle', value: QuantityUnit.Bottle },
  ]
}
