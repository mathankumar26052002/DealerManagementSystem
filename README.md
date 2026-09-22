# Dealer Management System

A full-stack Dealer Management System built using Angular, ASP.NET Core Web API, Entity Framework Core, and SQL Server.

The system allows dealers to browse active products, create and submit orders, and track order status. Administrators can manage dealers, products, and orders.

---

## Technology Stack

### Frontend

- Angular 21
- TypeScript
- Angular Material
- RxJS
- Reactive Forms

### Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core
- JWT Authentication
- BCrypt password hashing
- Swagger / OpenAPI

### Database

- Microsoft SQL Server
- Entity Framework Core Migrations

---

# Project Structure

```text
DealerManagementSystem/
│
├── DMS.API/
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
│
├── DMS.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
│
├── DMS.Domain/
│   ├── Entities/
│   └── Enums/
│
├── DMS.Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   └── Configurations/
│
├── DMS.Tests/
│   └── ...
│
├── dealer-management-ui/
│   ├── src/
│   └── ...
│
└── README.md