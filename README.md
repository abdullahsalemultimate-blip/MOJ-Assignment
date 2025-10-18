# Project Documentation Overview

This project is an implementation of a **MOJ assignment**, created to demonstrate a simple inventory management system. It focuses on managing **products** and **suppliers**, with functionality to **increase or decrease product stock quantities**, and to **display basic statistics**.


> [!WARNING]
> This documentation has been generated with the help of AI to save time and ensure clarity. All (key technical highlights, features, tools, and setup instructions) were passed to AI to create an easy-to-read, descriptive, and elegant guide.

# 🧾 InventorySys – Backend (.NET 9 + EF Core 9)

An advanced inventory management system built using **Clean Architecture** and **Domain-Driven Design (DDD)** principles with **CQRS** pattern using **MediatR**.
This project demonstrates modern enterprise-grade backend engineering practices — including auditing, caching, concurrency, logging, and robust domain event handling — implemented with clarity and modularity.

---

## 🚀 How to Run

1. **Requirements**

   * .NET SDK 9.0+
   * SQL Server running locally
   * Docker (optional, for containerized deployment)

2. **Clone & Build**

   ```bash
   git clone https://github.com/abdullahsalemultimate-blip/MOJ-Assignment.git
   cd api
   dotnet build
   ```

3. **Run the Application**

   ```bash
   cd src/Web
   dotnet run
   ```
Navigate to https://localhost:5001.

1. **Database Connection**

   * The app connects by default to a **local SQL Server** using:

     ```
     "Trusted_Connection=True;TrustServerCertificate=True;"
     ```
   * Ensure SQL Server is running locally, or update the connection string in:

     ```
     src/Web/appsettings.json
     ```

2. **Automatic Database Migration**

   * On startup, all EF Core migrations are **applied automatically**.

3. **Default Admin Credentials**

   ```
   Username: administrator@localhost
   Password: Administrator1!
   ```
---

## 🧱 Architecture Overview

The project is structured following **Clean Architecture + Domain-Driven Design (DDD)** with clear separation of concerns

### ⚙️ Technologies

* **.NET 9**
* **EF Core 9**
* **MediatR**
* **AutoMapper**
* **Dapper** (for statistical queries)
* **Serilog**
* **Bogus** (for data seeding)
* **ASP.NET Core Identity + JWT**
* **IMemoryCache**

---

## 🧩 Implemented Technical Features

### 🧱 **Architecture & Design**

* **Clean Architecture + DDD with Domain Events + SOLID Principles**

  * Layers are fully decoupled using abstractions.
  * Domain events dispatched via `DispatchDomainEventsInterceptor` using **MediatR**.

---

### 🕵️ **Auditing**

* **Automatic audit fields assignment** — handled via `AuditableEntityInterceptor`.

  * Updates `Created`, `CreatedBy`, `LastModified`, `LastModifiedBy` properties automatically.
  * Applied through the `BaseAuditableEntity` class used in **Product** and **Supplier** entities.

---

### 📜 **Audit Trail (Change History)**

* Full audit trail capturing **old values, new values, and changed columns**.

  * Implemented via `FullAuditTrailInterceptor`.
  * Tracks all Add, Update, and Delete operations.
  * Data stored in the `audit_trails` table.
  * Used by `/api/Audit/{entityName}/{entityId}` endpoint to show entity change history.
  * Applied to entities implementing `IFullAuditTrailEntity` ( Product Domain Only).

---

### 🗑️ **Soft Delete**

* Implemented via `ISoftDeletable` interface and `SoftDeleteInterceptor`.

  * On deletion, entities are marked as `IsDeleted = true` instead of being physically removed.
  * **Global EF Core query filter** applied on `Supplier` entity in `SupplierConfiguration.cs`.

---

### 🧾 **Logging**

* **Serilog** configured for file-based logging.

  * Creates **a new log file per day**.
  * Captures structured logs across all layers.

---

### 🧩 **Concurrency Control**

* EF Core **RowVersion** used for optimistic concurrency in `ProductConfiguration.cs`.

  * Prevents concurrent update conflicts at database level.
  * On conflict, EF Core throws `DbUpdateConcurrencyException` handled gracefully.

---

### 🔐 **Authentication & Authorization**

* **ASP.NET Core Identity + JWT Authentication**

  * Roles-based access control (Admin seeded automatically).
  * `GlobalAuthorizationFilter` ensures all controllers require authenticated admin users.

---

### ⚠️ **Centralized Error Handling**

* Global exception handler to return consistent API error responses.
* EF Core exception mapping for constraint violations implemented in
  **`GenericRepository.SaveChangesAsync()`**.

---

### ⚡ **Caching**

* **IMemoryCache** abstraction via `ICacheService`.

  * Caches supplier lookup data for fast access.
  * Auto-invalidation on supplier change using `SupplierModifiedEvent`.
  * Used in `SuppliersController.GetLookup()`.

---

### 📦 **Auto Migration**

* Database migrations applied automatically at application startup.

  * No manual EF command execution needed.

---

### 🧰 **Utilities & Enhancements**

* **Generic Repository Pattern** for common CRUD operations.
* **Custom Repositories** for more complex data (e.g., Products with search & paging).
* **Domain Events** (e.g., SupplierModifiedEvent, ReorderLevelReachedEvent) to handle cross-module reactions.
* **Bogus-based data seeding** for initial dataset population.
* **Serilog + file logging** ensures full observability.

---

## 💼 Implemented Business Features

### 👤 **Authentication**

* Login via `/api/Auth`
* JWT Token-based authentication (returned with expiry time)
* Global authorization required for all controllers

---

### 🧾 **Suppliers**

* CRUD APIs via `/api/Suppliers`
* Supplier lookup via `/api/Suppliers/lookup`
* Cached lookup responses using `ICacheService`

---

### 📦 **Products**

* CRUD APIs via `/api/Products`
* Supports:

  * Paging
  * Search by name and supplier
* Implements **Full Audit Trail** tracking all updates

---

### 📊 **Statistics**

* Analytical endpoints implemented using **Dapper** to showcase using Dapper with EfCore:

  * `/api/Statistics/largest-supplier` 
  * `/api/Statistics/minimum-orders-product`
  * `/api/Statistics/reorder-needed`

---

### 🕵️ **Audit Trail**

* `/api/Audit/{entityName}/{entityId}`
  Returns chronological history of entity changes

---

