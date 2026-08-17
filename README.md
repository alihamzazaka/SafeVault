# SafeVault

Secure ASP.NET Core order-management API created for the Microsoft Copilot security capstone.

## Features

- JWT authentication with 30-minute access tokens.
- Role-based authorization for Admin and User roles.
- Secure password hashing with PBKDF2-HMAC-SHA256 and unique salts.
- Entity Framework Core persistence with parameterized LINQ queries.
- Input validation through strongly typed DTOs and data annotations.
- In-memory caching for frequently requested product data.
- Protected order endpoints scoped to the authenticated user.
- Security-focused automated tests.
- Swagger API documentation in development.

## Security vulnerabilities addressed

### SQL Injection
Unsafe string-concatenated SQL was avoided. Database access uses Entity Framework Core LINQ expressions, which parameterize user input.

### XSS
API inputs are constrained with DTO validation and the API does not render user input as executable HTML. Client applications must still HTML-encode untrusted values before rendering them.

### Unauthorized access
JWT Bearer authentication protects private endpoints. `[Authorize(Roles = "Admin")]` protects administrative operations, while normal users can access only their own orders.

### Password exposure
Passwords are never stored as plaintext. PBKDF2-HMAC-SHA256 with a random salt and 100,000 iterations is used for password hashing.

### Sensitive configuration
Production deployments should provide `Jwt:Key` through environment variables or a secret manager. The development fallback key in `Program.cs` is intentionally non-production and must be replaced before deployment.

## Copilot usage

Microsoft Copilot was used during development to generate and review secure validation patterns, JWT authentication and RBAC examples, EF Core query patterns, caching approaches, and security tests. Generated suggestions were reviewed, corrected where necessary, and tested before inclusion.

## Performance

Product reads use `AsNoTracking()` and projection to return only required fields. A five-minute in-memory cache reduces repeated database reads. Order queries filter by the authenticated user's ID at the database level rather than loading unrelated records.

## Run locally

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/SafeVault
```

Swagger is available in the development environment.

## Demo accounts

- Admin: `admin` / `Admin@12345`
- User: `user` / `User@12345`

These accounts are development-only seed data. Change or remove them before production deployment.

## Assignment coverage

1. Public GitHub repository: this repository.
2. Secure input validation and SQL injection prevention: DTO validation and EF Core parameterized queries.
3. Authentication and RBAC: JWT Bearer authentication and Admin/User roles.
4. Vulnerability remediation: SQL injection and XSS risks addressed; authorization enforced.
5. Security tests: password hashing and verification tests included.
6. Vulnerability/fix/Copilot summary: documented above.
