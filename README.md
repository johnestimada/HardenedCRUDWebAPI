# Products API

## Overview

A .NET 8 ASP.NET Core Web API that demonstrates validated product CRUD operations,
centralized exception handling, standardized HTTP responses, and pagination.
Products are kept in memory, so data is lost when the app stops.

## Features

- Product CRUD endpoints
- Data Annotations validation, with the SalePrice < Price cross-property rule on create **and** update
- Unique SKU enforcement (case-insensitive)
- Global exception handling (`IExceptionHandler`)
- Problem Details (RFC 9457) for every error, including unknown routes
- Pagination with a `data` / `meta` envelope
- Thread-safe in-memory service

## Requirements

- .NET 8 SDK
- Git

## How to Run

1. Clone this repository.
2. `cd HardenedCRUDWebAPI` (the folder that contains `HardenedCRUDWebAPI.csproj`).
3. `dotnet restore`
4. `dotnet build`
5. `dotnet run`
6. Open `http://localhost:5250/swagger`, or send the requests in `HardenedCRUDWebAPI/HardenedCRUDWebAPI.http`.

## API Endpoints

| Method | Endpoint | Success | Errors |
|---|---|---|---|
| GET | /api/products?page=&pageSize= | 200 (`{ data, meta }`) | 400 (bad paging), 500 |
| GET | /api/products/{id} | 200 | 404, 500 |
| POST | /api/products | 201 + `Location` header | 400 (validation), 409 (duplicate SKU), 500 |
| PUT | /api/products/{id} | 200 | 400 (validation), 404, 409, 500 |
| DELETE | /api/products/{id} | 204 (no body) | 404, 500 |

## Validation Rules

| Field | Rule |
|---|---|
| Name | required, 2 to 100 characters |
| SKU | required, 3 to 20 characters, unique |
| Price | 0.01 to 1,000,000 |
| SalePrice | optional, 0.01 to 1,000,000, must be less than Price |
| page | 1 to 100000 |
| pageSize | 1 to 100 |

## Pagination

The response contains:

- `data`: the products for the requested page
- `meta`: `page`, `pageSize`, `totalItems`, `totalPages`

## Error Handling

| Cause | Status |
|---|---|
| Validation failure | 400 (`ValidationProblemDetails`, listing each invalid field) |
| `NotFoundException` | 404 |
| `ConflictException` | 409 |
| Any other exception | 500 with a generic message |

Unexpected error responses never expose stack traces. The full exception is written to
the server log only.

## Project Structure

| Folder / file | Purpose |
|---|---|
| `Models/DTO` | Request models with validation (`UpdateProductDTO` inherits `CreateProductDTO`) |
| `Models/Domain` | The `Product` entity |
| `Models/Responses` | The paged response envelope |
| `Services/ProductService.cs` | In-memory storage; throws domain exceptions |
| `Exceptions` | `NotFoundException`, `ConflictException` |
| `Exceptions/Handling` | `GlobalExceptionHandler` (maps exceptions to 404, 409, 500) |
| `Controllers/ProductsController.cs` | Thin actions that return the correct success codes |

## Contributors

- Andrick: API implementation and integration
- Jacob: DTO validation and validation tests
