import { AdjustQuantityDirection, QuantityUnit } from '@/models/enums'
import dayjs from 'dayjs';
import localizedFormat from 'dayjs/plugin/localizedFormat';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';

// Initialize dayjs plugins
dayjs.extend(localizedFormat);
dayjs.extend(utc);
dayjs.extend(timezone);


/**
 * Formats an ISO 8601 date string to a local, readable format.
 * Requirement: "MMM DD, YYYY HH:mm AM/PM"
 */
export const formatDate = (dateUtc: string): string => {
    // Convert UTC string to local timezone and format
    return dayjs.utc(dateUtc).local().format('MMM DD, YYYY hh:mm A');
};

/**
 * Formats a numeric value as SAR currency.
 * @param value The number to format.
 * @returns Formatted currency string (e.g., "SAR 1,234.50").
 */
// Formats a numeric value as US Dollar currency.
export const formatCurrency = (value: number | string | null | undefined): string => {
  if (!value) return ''

  const num = typeof value === 'string' ? parseFloat(value) : value
  if (isNaN(num)) return ''

  return new Intl.NumberFormat('en-SA', {
    style: 'currency',
    currency: 'SAR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(num)
}

/**
 * Formats a UTC date string into a local, readable format.
 * @param dateString The ISO 8601 date string.
 * @returns Formatted date string (e.g., "Oct 18, 2025 09:01 PM").
 */
export const formatDateManual = (dateString: string | null | undefined): string => {
  if (!dateString) return ''
  try {
    const date = new Date(dateString)
    return date.toLocaleString(undefined, {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: 'numeric',
      minute: '2-digit',
      hour12: true,
    })
  } catch (e) {
    return dateString
  }
}
