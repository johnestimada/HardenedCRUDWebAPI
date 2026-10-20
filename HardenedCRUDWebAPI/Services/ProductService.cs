using ProductsApi.Exceptions;
using ProductsApi.Models.Domain;

namespace ProductsApi.Services;

public class ProductService
{
    private readonly List<Product> _products = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public (IReadOnlyList<Product> Items, int TotalItems) GetPage(
        int page, int pageSize)
    {
        lock (_lock)
        {
            var items = _products
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, _products.Count);
        }
    }

    public Product GetById(int id)
    {
        lock (_lock)
        {
            return FindOrThrow(id);
        }
    }

    public Product Create(Product product)
    {
        lock (_lock)
        {
            EnsureUniqueSku(product.SKU);

            product.Id = _nextId++;
            _products.Add(product);

            return product;
        }
    }

    public Product Update(int id, Product updatedProduct)
    {
        lock (_lock)
        {
            var existing = FindOrThrow(id);

            EnsureUniqueSku(updatedProduct.SKU, id);

            existing.Name = updatedProduct.Name;
            existing.SKU = updatedProduct.SKU;
            existing.Price = updatedProduct.Price;
            existing.SalePrice = updatedProduct.SalePrice;

            return existing;
        }
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            _products.Remove(FindOrThrow(id));
        }
    }

    // Callers must already hold the lock.
    private Product FindOrThrow(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id)
            ?? throw new NotFoundException(
                $"Product with ID {id} was not found.");
    }

    private void EnsureUniqueSku(string sku, int? excludeId = null)
    {
        var duplicate = _products.Any(p =>
            p.Id != excludeId &&
            string.Equals(p.SKU, sku, StringComparison.OrdinalIgnoreCase));

        if (duplicate)
        {
            throw new ConflictException(
                $"A product with SKU '{sku}' already exists.");
        }
    }
}
