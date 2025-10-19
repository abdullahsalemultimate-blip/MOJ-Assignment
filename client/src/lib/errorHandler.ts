import { type AxiosError } from 'axios'

export interface ProblemDetails {
  type?: string
  title?: string
  status: number
  detail?: string
  traceId?: string
  errors?: Record<string, string[]>
}

export interface StructuredError {
  status: number
  title?: string
  detail?: string
  fieldErrors: Record<string, string>
}

// Centralized function to process Axios error responses and format them
export const errorHandler = (error: AxiosError): StructuredError => {
  debugger
  // Default error structure
  const structuredError: StructuredError = {
    status: error.response?.status || 500,
    title: 'Error',
    detail: 'An unexpected error occurred.',
    fieldErrors: {},
  }

  if (error.response && error.response.data) {
    const problemDetails = error.response.data as ProblemDetails

    structuredError.title = problemDetails.title || 'API Error'
    structuredError.detail = problemDetails.detail || 'The server returned an error.'

    // --- 400 Validation Errors Handling ---
    if (structuredError.status === 400 && problemDetails.errors) {
      structuredError.title = 'Validation Failed'
      structuredError.detail = 'Please correct the highlighted errors.'

      const fieldErrors: Record<string, string> = {}
      for (const key in problemDetails.errors) {
        if (problemDetails.errors.hasOwnProperty(key)) {
          //let errorMsg: string[] =  as string[]
          fieldErrors[key.toLowerCase()] = problemDetails.errors[key]![0] as string
        }
      }
      structuredError.fieldErrors = fieldErrors
    } else if (structuredError.status === 400 && problemDetails.detail) {
      structuredError.title = problemDetails.title || 'Business Rule Violation'
      structuredError.detail = problemDetails.detail
    } else if (structuredError.status === 404) {
      structuredError.title = problemDetails.title || 'Not Found'
      structuredError.detail = problemDetails.detail || 'The requested resource was not found.'
    } else if (structuredError.status === 500) {
      structuredError.title = problemDetails.title || 'Internal Server Error'
      structuredError.detail =
        problemDetails.detail || 'An unexpected error occurred on the server.'
    }
  }

  console.error('API Error Handled:', structuredError, error)

  return structuredError
}
