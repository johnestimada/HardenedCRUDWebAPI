using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models.Domain;
using ProductsApi.Models.DTO;
using ProductsApi.Models.Responses;
using ProductsApi.Services;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<PagedResponse<Product>> GetAll(
        [FromQuery, Range(1, 100000, ErrorMessage = "Page must be between 1 and 100000.")]
        int page = 1,
        [FromQuery, Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        int pageSize = 10)
    {
        var (items, totalItems) = _service.GetPage(page, pageSize);

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return Ok(new PagedResponse<Product>(
            items,
            new PageMeta(page, pageSize, totalItems, totalPages)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetById(int id)
    {
        return Ok(_service.GetById(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<Product> Create(CreateProductDTO request)
    {
        var created = _service.Create(ToProduct(request));

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<Product> Update(int id, UpdateProductDTO request)
    {
        return Ok(_service.Update(id, ToProduct(request)));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);

        return NoContent();
    }

    // Create and Update share the same fields, so one mapper serves both.
    private static Product ToProduct(CreateProductDTO request)
    {
        return new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            Price = request.Price,
            SalePrice = request.SalePrice
        };
    }
}
