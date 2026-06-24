# Project Context: Aegis ERP

## 1. Tech Stack
* **Backend:** .NET Web API (C#)
* **Database:** Microsoft SQL Server (LocalDB/Express for Dev, Azure SQL for Prod)
* **ORM:** Entity Framework Core
* **Frontend:** React + Vite
* **Storage (Future):** Cloudflare R2 (Zero egress fees)

## 2. Architecture: Modular Monolith
* The system is split into logical modules (e.g., `Erp.Module.Core`, `Erp.Module.GL`) as Class Libraries.
* The main `ERP` project acts as the API host and Startup Project.
* EF Core migrations belong strictly to their respective modules, delegated via `MigrationsAssembly`.
* **Vertical Slice Architecture:** The main API project groups files by feature (e.g., `ERP/Features/Auth/`), not by type. A feature folder contains its Controller, Service, Interface, and a `DTOs/` subfolder.
* **Shared Entities:** Entities that need to be referenced across multiple modules (like `BaseEntity` or `Permissions`) belong in `Erp.Shared`.
* **Modular Dependency Injection:** Each module/feature must have its own `DependencyInjection.cs` with an extension method (e.g., `AddFeatureServices()`) to keep `Program.cs` clean.

## 3. Database & Entity Rules
* **Wide Tables over JOINs:** For 1-to-1 relationships (like Company Financial Setup or VAT details), we use EF Core `[Owned]` types to flatten data into Wide Tables for blazing-fast read performance.
* **Global Query Filters:** Multi-tenancy is strictly enforced. Entities have a `TenantId`, and queries are globally filtered by the injected `ICurrentUserService` unless bypassed with `IgnoreQueryFilters()` for login/startup logic.
* **Soft Deletes:** We use `IsActive` flags instead of physically deleting records.

## 4. UI & State Management Strategy
* **Forms:** Multi-step wizards use React Hook Form + Zod for validation.
* **Continuous Save (Drafts):** Complex forms are not held in local state. Step 1 generates a POST request to create a "Draft" record. Subsequent steps use PUT requests to continuously update the SQL database, ensuring zero data loss if the user exits.

## 5. Coding Philosophy
* **YAGNI (You Aren't Gonna Need It):** We build for the MVP first. We skip complex setups (like cloud document storage) until the core engine is running perfectly.
* **Security:** Passwords use .NET `IPasswordHasher`. Authentication relies on JWTs with strict validation (ClockSkew = Zero for testing).
* **Thin Controllers, Fat Services:** API controllers must not contain business logic or direct DB access. Always abstract database transactions and operations into Services (and optionally Repositories) via Dependency Injection.