# Dealer Management System

A full-stack Dealer Management System built using Angular, ASP.NET Core Web API, Entity Framework Core, and Microsoft SQL Server.

The application provides separate workflows for Administrators and Dealers.

Administrators can manage dealers, products, and orders, while Dealers can browse products, create orders, submit orders, and track their order status.

---

# 1. Technology Stack

## Frontend

- Angular 21
- TypeScript
- Angular Material
- RxJS
- Reactive Forms
- Vitest
- HTML5
- SCSS

## Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core
- JWT Authentication
- Role-Based Authorization
- BCrypt password hashing
- Swagger / OpenAPI
- LINQ
- Dependency Injection

## Database

- Microsoft SQL Server
- Entity Framework Core Migrations

## Testing

### Backend

- xUnit
- Entity Framework Core
- SQL Server test database

### Frontend

- Vitest
- Angular TestBed

---

# 2. Project Structure

```text
DealerManagementSystem/
│
├── DMS.API/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Properties/
│   ├── Program.cs
│   ├── appsettings.json
│   └── DMS.API.csproj
│
├── DMS.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   └── DMS.Application.csproj
│
├── DMS.Domain/
│   ├── Entities/
│   ├── Enum/
│   └── DMS.Domain.csproj
│
├── DMS.Infrastructure/
│   ├── Data/
│   ├── Migrations/
│   ├── Repositories/
│   ├── Seed/
│   └── DMS.Infrastructure.csproj
│
├── DMS.Tests/
│   ├── Helpers/
│   ├── OrderTests/
│   └── DMS.Tests.csproj
│
├── dms-client/
│   ├── src/
│   ├── package.json
│   ├── package-lock.json
│   └── angular.json
│
├── DealerManagementSystem.slnx
├── README.md
└── .gitignore
```

---

# 3. Architecture

The backend follows a layered architecture.

```text
                    Angular Frontend
                           |
                           v
                        DMS.API
                           |
                           v
                    DMS.Application
                           |
                           v
                       DMS.Domain
                           ^
                           |
                  DMS.Infrastructure
```

## Layer Responsibilities

### DMS.API

Responsible for:

- HTTP endpoints
- Controllers
- JWT authentication configuration
- Role-based authorization
- Exception handling middleware
- Swagger/OpenAPI configuration

### DMS.Application

Responsible for:

- DTOs
- Service interfaces
- Repository interfaces
- Business logic
- Application validation

### DMS.Domain

Responsible for:

- Domain entities
- Enums
- Core business models

### DMS.Infrastructure

Responsible for:

- Entity Framework Core
- SQL Server
- DbContext
- Repository implementations
- Entity configurations
- Database migrations
- Seed data

### DMS.Tests

Contains automated tests for important business scenarios.

### dms-client

Contains the Angular frontend application.

---

# 4. Features

## Authentication

The application provides:

- JWT-based authentication
- Role-based authorization
- Admin and Dealer roles
- BCrypt password hashing
- Active/inactive account validation
- Angular authentication interceptor
- Angular route guards
- Role guards

## Admin Features

Administrators can:

- View dashboard
- View dealer statistics
- View product statistics
- View order statistics
- Create dealers
- Update dealers
- Activate/deactivate dealers
- Create products
- Update products
- Activate/deactivate products
- View orders
- View order details
- Approve orders
- Reject orders
- Dispatch orders
- Mark orders as delivered

## Dealer Features

Dealers can:

- Login
- Browse active products
- Search products
- View paginated products
- Add multiple products to an order
- Increase/decrease quantities
- Remove products
- Create draft orders
- Submit orders
- Cancel eligible orders
- View their own orders
- Search orders
- Filter orders by status
- View order details
- View order status history

---

# 5. Order Workflow

The order workflow is:

```text
Draft
  |
  v
Submitted
  |
  v
Approved
  |
  v
Dispatched
  |
  v
Delivered
```

Additional supported transitions:

```text
Submitted --> Rejected

Draft -------> Cancelled

Submitted ----> Cancelled
```

Invalid status transitions are rejected by the backend.

---

# 6. Business Rules

## Dealer Rules

- Dealer code must be unique.
- Dealer email must be unique.
- Inactive dealers cannot create orders.
- Deactivating a dealer also deactivates its associated user accounts.
- A dealer can only access its own orders.

## Product Rules

- Only active products are available in the dealer catalog.
- Product quantity must be a positive whole number.
- Product stock cannot become negative.
- Product prices are calculated and validated by the backend.

## Order Rules

- An order must contain at least one product.
- Product quantities must be positive.
- Duplicate products are not allowed in the same order.
- Order totals are calculated by the backend.
- Submitted orders cannot be edited.
- Dealers cannot access another dealer's orders.
- Rejected orders require a reason.
- Invalid status transitions are rejected.

---

# 7. Price Snapshot

The order stores the product price at the time of order submission.

For example:

```text
Product Current Price
        |
        v
     ₹50,000
        |
        v
Order Submitted
        |
        v
OrderItem.UnitPrice = ₹50,000
```

If the administrator later changes the product price:

```text
Product Current Price

₹50,000
   |
   v
₹55,000
```

the existing submitted order continues to use:

```text
OrderItem.UnitPrice = ₹50,000
```

Therefore, changing the current product price does not change historical orders.

---

# 8. Concurrency-Safe Stock Deduction

During order approval, stock is deducted using an atomic conditional database update.

The update succeeds only when:

```text
AvailableStock >= RequestedQuantity
```

The stock deduction and order approval are executed inside a database transaction.

Example:

```text
Initial Stock = 1

Order A -> Quantity 1
Order B -> Quantity 1

Order A
    |
    +--> Stock condition succeeds
    +--> Stock becomes 0
    +--> Order Approved

Order B
    |
    +--> Stock condition fails
    +--> Approval rejected
```

This prevents:

- Negative stock
- Overselling
- Partial stock deduction
- Double stock deduction during repeated approval

---

# 9. Dealer Order Isolation

Dealers can only access their own orders.

The backend checks both:

```text
Order ID
+
Authenticated Dealer ID
```

For example:

```text
Dealer 1
   |
   +--> Order 101  ✓ Allowed

Dealer 2
   |
   +--> Order 202  ✓ Allowed
```

If Dealer 1 attempts to access Order 202, the backend verifies that the order does not belong to Dealer 1.

The order is therefore not returned.

This prevents a dealer from accessing another dealer's order by changing the order ID.

---

# 10. Invalid Status Transitions

The backend validates the current status before changing the order status.

Examples of invalid transitions:

```text
Draft       -> Delivered     ❌
Submitted   -> Delivered     ❌
Delivered   -> Approved      ❌
Rejected    -> Approved      ❌
```

Valid workflow:

```text
Draft
  -> Submitted
  -> Approved
  -> Dispatched
  -> Delivered
```

Status transition validation is enforced by the backend.

---

# 11. Inactive Dealer

When an administrator deactivates a dealer:

```text
Dealer.IsActive = false
```

the associated dealer users are also deactivated:

```text
User.IsActive = false
```

Therefore:

```text
Inactive Dealer
      |
      +----> Cannot login
      |
      +----> Cannot create orders
```

The restriction is enforced by the backend.

---

# 12. Authentication

The application uses JWT authentication.

Login endpoint:

```http
POST /api/Auth/login
```

Example request:

```json
{
  "username": "Admin",
  "password": "Admin@123"
}
```

After successful authentication, the API returns a JWT token.

The Angular HTTP interceptor sends the token with protected requests:

```http
Authorization: Bearer <JWT_TOKEN>
```

---

# 13. Role-Based Authorization

Admin APIs are protected using:

```csharp
[Authorize(Roles = "Admin")]
```

Dealer APIs are protected using:

```csharp
[Authorize(Roles = "Dealer")]
```

Example:

```text
Admin
  |
  +--> Admin APIs       ✓

Dealer
  |
  +--> Dealer APIs      ✓
  +--> Admin APIs       ✗ 403 Forbidden
```

Backend authorization is the security boundary.

Angular guards are used for frontend navigation protection but are not treated as the security boundary.

---

# 14. Prerequisites

Install the following before running the application:

- .NET SDK
- Node.js
- npm
- Angular CLI
- Microsoft SQL Server
- Git

Verify the installations:

```bash
dotnet --version
node --version
npm --version
ng version
```

---

# 15. Database Setup

The application uses Microsoft SQL Server with Entity Framework Core.

Migration files are located under:

```text
DMS.Infrastructure/Migrations/
```

The repository contains:

- EF Core migration
- Database model snapshot
- Seed data

To apply the database migration:

```bash
dotnet ef database update
```

If `dotnet ef` is not installed:

```bash
dotnet tool install --global dotnet-ef
```

The application contains seed data for:

- Admin user
- Dealer users
- Dealers
- Products

---

# 16. Backend Setup

Navigate to the API project:

```bash
cd DMS.API
```

Restore dependencies:

```bash
dotnet restore
```

Configure the SQL Server connection string in:

```text
DMS.API/appsettings.json
```

Apply the EF Core migrations:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

The API URL will be displayed in the terminal.

---

# 17. Frontend Setup

Navigate to the Angular application:

```bash
cd dms-client
```

Install dependencies:

```bash
npm install
```

Start the Angular application:

```bash
ng serve
```

Open the application:

```text
http://localhost:4200
```

---

# 18. Demo Credentials

## Admin

```text
Username: Admin
Password: Admin@123
Role: Admin
```

## Dealer 1

```text
Username: dealer01
Password: Dealer@123
Role: Dealer
Dealer Code: D001
```

## Dealer 2

```text
Username: dealer02
Password: Dealer@123
Role: Dealer
Dealer Code: D002
```

These credentials are intended for local/demo evaluation purposes only.

---

# 19. Swagger / OpenAPI

Swagger is enabled for API documentation and testing.

After starting the backend, open:

```text
https://localhost:<api-port>/swagger
```

For example:

```text
https://localhost:7255/swagger
```

The actual port depends on the local ASP.NET Core launch configuration.

## Swagger Authentication

1. Call `/api/Auth/login`.
2. Copy the JWT token from the response.
3. Click `Authorize` in Swagger.
4. Enter:

```text
Bearer <JWT_TOKEN>
```

5. Click `Authorize`.
6. Test the protected endpoints.

---

# 20. API Endpoints

## Authentication

```text
POST /api/Auth/login
```

## Dealers

```text
GET    /api/Dealers
GET    /api/Dealers/{id}
POST   /api/Dealers
PUT    /api/Dealers/{id}
PATCH  /api/Dealers/{id}/status
```

## Products

```text
GET    /api/Products
GET    /api/Products/{id}
POST   /api/Products
PUT    /api/Products/{id}
DELETE /api/Products/{id}
PATCH  /api/Products/{id}/status
```

## Dealer Product Catalog

```text
GET /api/Products/catalog
```

## Dealer Orders

```text
GET    /api/Orders
POST   /api/Orders
GET    /api/Orders/{id}
PUT    /api/Orders/{id}/items/{itemId}
DELETE /api/Orders/{id}/items/{itemId}
POST   /api/Orders/{id}/submit
POST   /api/Orders/{id}/cancel
```

## Admin Orders

```text
GET  /api/admin/orders
GET  /api/admin/orders/{id}
POST /api/admin/orders/{id}/approve
POST /api/admin/orders/{id}/reject
POST /api/admin/orders/{id}/dispatch
POST /api/admin/orders/{id}/deliver
```

---

# 21. Error Handling

The backend contains centralized exception handling middleware.

Common HTTP responses include:

```text
400 Bad Request
401 Unauthorized
403 Forbidden
404 Not Found
500 Internal Server Error
```

Example error response:

```json
{
  "statusCode": 400,
  "message": "Only submitted orders can be approved."
}
```

---

# 22. Logging

The application uses ASP.NET Core `ILogger` for application logging.

Unhandled exceptions are captured by the centralized exception handling middleware.

Important application and service errors are logged to assist with troubleshooting.

---

# 23. Automated Tests

## Backend Tests

Backend tests use xUnit.

From the solution root:

```bash
dotnet test
```

Important scenarios covered include:

- Dealer order isolation
- Invalid order status transitions
- Insufficient stock
- No partial stock deduction
- Repeated approval
- Concurrent approvals
- Price snapshot
- Database connectivity

Test project:

```text
DMS.Tests/
```

---

## Angular Tests

The Angular application uses Vitest.

Navigate to:

```bash
cd dms-client
```

Run:

```bash
ng test
```

Angular tests cover frontend behavior such as:

- Login component
- Form validation
- Authentication behavior
- Navigation
- Services
- Authentication guards
- Role guards
- Component behavior

---

# 24. Design Decisions

## Layered Architecture

The backend separates responsibilities between:

```text
DMS.API
    ↓
DMS.Application
    ↓
DMS.Domain

DMS.Infrastructure
    ↓
DMS.Application
    ↓
DMS.Domain
```

This keeps HTTP handling, business logic, domain models, and database access separated.

## DTOs

DTOs are used for API request and response contracts instead of exposing domain entities directly.

Benefits include:

- Controlled API contracts
- Validation
- Reduced data exposure
- Separation between API models and domain models

## Repository Pattern

Repositories handle database operations.

Services contain business rules.

Controllers handle HTTP requests and responses.

## JWT Authentication

JWT provides stateless authentication for the API.

The JWT contains relevant claims such as:

- User ID
- Username
- Role
- Dealer ID for Dealer users

## Backend Authorization

Authorization is enforced at the backend.

Angular guards provide frontend route protection, but backend authorization remains the actual security boundary.

## Transactional Order Approval

Order approval and stock deduction are performed inside a database transaction.

If any stock deduction fails, the transaction is rolled back.

## Atomic Stock Update

Stock deduction uses a conditional update:

```sql
UPDATE Products
SET AvailableStock = AvailableStock - @Quantity
WHERE Id = @ProductId
  AND AvailableStock >= @Quantity
```

This prevents overselling during concurrent approvals.

## Price Snapshot

The order item stores the price applicable to the order.

Therefore, future changes to the current product price do not modify historical submitted orders.

---

# 25. Known Limitations

- The application is implemented as a single application. Microservices are not required for this assignment.
- The current configuration is intended for local/demo usage.
- Production deployment would require secure secret management.
- Production deployment would require production database credentials and environment-specific configuration.
- Additional production monitoring and observability could be added.
- Demo credentials are included for evaluation purposes only.

---

# 26. Demo Walkthrough

The following flow can be used for the project demonstration.

## Step 1 — Admin Login

Login using:

```text
Username: Admin
Password: Admin@123
```

Show:

- Admin Dashboard
- Dealer Management
- Product Management
- Order Management

---

## Step 2 — Dealer Management

Show:

- Dealer list
- Create dealer
- Edit dealer
- Activate dealer
- Deactivate dealer

Demonstrate that an inactive dealer cannot login.

---

## Step 3 — Product Management

Show:

- Product list
- Create product
- Edit product
- Activate/deactivate product
- Product stock

---

## Step 4 — Dealer Login

Logout from Admin and login using:

```text
Username: dealer01
Password: Dealer@123
```

Show:

- Product catalog
- Search
- Pagination
- Add multiple products
- Quantity changes
- Order summary
- Draft order

---

## Step 5 — Submit Order

Submit the order:

```text
Draft
  |
  v
Submitted
```

Open the order details and show:

- Order number
- Products
- Quantities
- Unit prices
- Line totals
- Total amount
- Status history

---

## Step 6 — Admin Approval

Login as Admin.

Open the submitted order.

Approve the order:

```text
Submitted
    |
    v
Approved
```

Show that product stock has been deducted.

---

## Step 7 — Fulfillment

Move the order through:

```text
Approved
    |
    v
Dispatched
    |
    v
Delivered
```

Show the status history after each transition.

---

# 27. Important Technical Demonstrations

## Dealer Isolation

Use two different dealer accounts.

Create/view an order for Dealer 1.

Attempt to access the same order using Dealer 2.

Expected result:

```text
Order not found / access denied
```

This demonstrates backend dealer isolation.

---

## Price Snapshot

1. Create an order using the current product price.
2. Submit the order.
3. Login as Admin.
4. Change the product price.
5. Open the existing order.
6. Verify that the existing order still contains the original price.

Example:

```text
Product Price at Order Time = ₹50,000

Product Price Later          = ₹55,000

Existing Order Price         = ₹50,000
```

---

## Invalid Status Transition

Attempt an invalid transition such as:

```text
Draft -> Delivered
```

Expected result:

```text
400 Bad Request
```

The backend should reject the transition.

---

## Inactive Dealer

1. Login as Admin.
2. Deactivate a dealer.
3. Logout.
4. Attempt to login using the dealer credentials.

Expected result:

```text
User account is inactive.
```

---

## JWT and Role Authorization

Test the following:

```text
Admin -> Admin API       ✓
Dealer -> Dealer API     ✓
Dealer -> Admin API      ✗ 403
No JWT -> Protected API  ✗ 401
```

---

## Concurrency-Safe Stock

For a product with stock of 1:

```text
Initial Stock = 1

Order A -> Quantity 1
Order B -> Quantity 1
```

Attempt to approve both orders concurrently.

Expected result:

```text
One order -> Approved
One order -> Rejected

Final Stock = 0
```

Stock must never become negative.

---

# 28. Running the Complete Application

## Backend

Open a terminal:

```bash
cd DMS.API
dotnet run
```

## Frontend

Open another terminal:

```bash
cd dms-client
ng serve
```

## Application

Open:

```text
http://localhost:4200
```

## Swagger

Open:

```text
https://localhost:<api-port>/swagger
```

---

# 29. Submission Checklist

Before submitting the repository, verify:

- [x] Frontend code is present
- [x] Backend code is present
- [x] README.md is present
- [x] .gitignore is present
- [x] Database migration files are present
- [x] Seed data is present
- [x] Demo credentials are documented
- [x] Swagger/OpenAPI is enabled
- [x] Backend automated tests are present
- [ ] Angular unit tests are completed
- [x] `node_modules` is not committed
- [x] `bin/` and `obj/` are not committed
- [x] `.vs/` is not committed
- [x] Production secrets are not committed
- [x] Repository is public
- [x] GitHub repository URL is ready

---

# 30. Assignment Deliverables

The project contains the requested deliverables.

## 1. Git Repository

Contains:

- Angular frontend
- ASP.NET Core backend
- Domain layer
- Application layer
- Infrastructure layer
- Automated tests

## 2. README

Contains:

- Setup instructions
- Technology stack
- Project structure
- Architecture
- Business rules
- Design decisions
- Known limitations
- Demo credentials
- Swagger instructions
- Test instructions
- Demo walkthrough

## 3. Database Migrations and Seed Data

Contains:

- EF Core migrations
- Database model snapshot
- Seed data

## 4. Demo Credentials

Admin and Dealer credentials are documented in this README.

## 5. Swagger/OpenAPI

Swagger is enabled and available from the running backend.

## 6. Automated Tests

The project contains backend xUnit tests and Angular Vitest tests.

## 7. Demo / Live Walkthrough

The demonstration covers:

- Authentication
- Authorization
- Dealer management
- Product management
- Product catalog
- Order creation
- Order submission
- Order approval
- Stock deduction
- Order fulfillment
- Dealer isolation
- Price snapshot
- Invalid status transitions
- Inactive dealer handling
- Concurrency-safe stock deduction

---

# 31. Security Considerations

The application implements:

- JWT authentication
- Role-based authorization
- Password hashing using BCrypt
- Backend authorization
- Dealer-level data isolation
- Server-side validation
- Atomic stock deduction
- Transactional order approval
- Centralized exception handling

For production deployment, secrets such as JWT keys and database credentials should be stored using secure configuration or secret-management services rather than committed to source control.

---

# 32. Future Improvements

Possible future enhancements include:

- Refresh token support
- Password reset functionality
- Email notifications
- Advanced reporting
- Audit logging
- More comprehensive frontend unit test coverage
- Production monitoring
- Centralized configuration management
- Containerized deployment
- CI/CD pipeline
- Automated database deployment

---

# 33. Conclusion

The Dealer Management System demonstrates a full-stack implementation using Angular, ASP.NET Core Web API, Entity Framework Core, and SQL Server.

The application provides separate Admin and Dealer workflows with:

- JWT authentication
- Role-based authorization
- Dealer management
- Product management
- Product catalog
- Order management
- Order status workflow
- Price snapshot handling
- Transactional stock deduction
- Concurrency-safe stock management
- Dealer order isolation
- Database migrations
- Seed data
- Swagger/OpenAPI documentation
- Automated testing

The project is structured using a layered architecture to keep the API, application logic, domain models, and infrastructure concerns separated.