interface ServerValidationErrors {
  [key: string]: string[]
}

interface MappedErrors {
  formErrors: Record<string, string>
  globalError: { message: string } | null
}

export const mapValidationErrors = (serverErrors: ServerValidationErrors): MappedErrors => {
  const formErrors: Record<string, string> = {}
  let globalError: { message: string } | null = null

  // The server uses PascalCase (Name, SupplierId), client uses camelCase (name, supplierId)
  const toCamelCase = (s: string) => s.charAt(0).toLowerCase() + s.slice(1)

  for (const serverKey in serverErrors) {
    if (serverErrors.hasOwnProperty(serverKey)) {
      const clientKey: string = toCamelCase(serverKey) as any
      const messages = serverErrors[serverKey]
      formErrors[clientKey] = ''
      if (messages && messages.length > 0) {
        formErrors[clientKey] = messages[0] as unknown as any
      } else {
        globalError = { message: `Validation Error for ${serverKey}: ${messages?.join(' ')}` }
      }
    }
  }

  if (Object.keys(formErrors).length === 0 && Object.keys(serverErrors).length > 0) {
    const allMessages = Object.values(serverErrors).flat().join('; ')
    globalError = { message: allMessages || 'One or more validation errors occurred.' }
  }

  return { formErrors, globalError }
}
