# What was improved and why

| # | Area | Before | After | Why |
|---|---|---|---|---|
| 1 | UpdateProductDTO | Full copy of CreateProductDTO | Inherits it | One copy of the rules; they cannot drift apart |
| 2 | GlobalExceptionHandler | Sets ContentType, then WriteAsJsonAsync | Passes contentType to WriteAsJsonAsync | WriteAsJsonAsync overwrites a ContentType set before it, so the response was application/json, not application/problem+json |
| 3 | GlobalExceptionHandler | Everything logged as Error | 404/409 are warnings, 500 is an error | Expected problems should not look like crashes in the log |
| 4 | GlobalExceptionHandler | Type = httpstatuses.com URL | RFC 9110 links; Instance added | httpstatuses.com is unofficial; Instance is a standard Problem Details field |
| 5 | Paging | Manual if-checks in the controller; skip computed in the controller | `[Range]` on the parameters; paging moved into the service | Thinner controller; `(page - 1) * pageSize` can no longer overflow |
| 6 | List response | Anonymous object | `PagedResponse<T>` record | Typed, and Swagger can show the shape |
| 7 | ProductService | No locking | `lock` around all access | A singleton is shared by concurrent requests |
| 8 | ProductService | GetById called inside other methods through public API | Private FindOrThrow | One lookup, one message |
| 9 | Controller | Duplicate Product mapping in Create and Update | One ToProduct helper | No repeated code |
| 10 | Swagger | Mostly bare ProducesResponseType | Typed (ProblemDetails / ValidationProblemDetails) | Swagger documents the real error bodies |
| 11 | Program.cs | Unknown routes returned an empty 404 | `UseStatusCodePages()` | Every error is Problem Details |
| 12 | Template leftovers | WeatherForecast files, weather-only .http | Deleted; .http has a request for every checkpoint | Clutter and nothing to test with |
| 13 | LAB.md | Mixed encoding (garbled characters) | Clean UTF-8, same structure, still blank | Fill it with real output |
| 14 | README | "Swagger UI" listed as a requirement; no validation table | Cleaned, structure and validation tables | Accuracy |
| 15 | Repo hygiene | `ProductsApi.csproj.user` in the folder | Removed (already git-ignored) | Personal IDE file |

Unchanged on purpose: the folder structure, class names, DTO and SKU naming, routes, status
codes, and the in-memory service. LAB.md checkpoints and your reflection must still be done by you.
