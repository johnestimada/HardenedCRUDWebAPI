## Test Results

| Test                                 | Expected Result               | Actual Result                  | Status |
|---                                   |---                            |---                             |---     |
| Invalid product input                | 400 Bad Request               | 400 Bad Request                | PASS   |
| Sale price higher than regular price | 400 Bad Request               | 400 Bad Request                | PASS   |
| Request for missing product ID       | 404 Not Found                 | 404 Not Found                  | PASS   |
| Create valid product                 | 201 Created                   | 201 Created                    | PASS   |
| Get products with pagination         | 200 OK with `data` and `meta` | 200 OK with `data` and `meta`  | PASS   |
| Delete existing product              | 204 No Content                | 204 No Content                 | PASS   |
| Get product after deletion           | 404 Not Found                 | 404 Not Found                  | PASS   |

### Reflection

Data annotations validate individual input fields, while cross-property validation 
checks rules involving more than one field. Centralized exception handling keeps controller code cleaner and 
helps return consistent error responses. Problem Details provides a standard structure for communicating errors 
without exposing internal implementation details. The `data` and `meta` pagination envelope makes collection responses 
more predictable, although it adds a wrapper that clients must handle.
