using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models.DTO;

public class CreateProductDTO : IValidatableObject
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string SKU { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Range(0.01, 1000000)]
    public decimal? SalePrice { get; set; }

    // Cross-property rule. It only runs after the attribute checks above pass.
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (SalePrice.HasValue && SalePrice.Value >= Price)
        {
            yield return new ValidationResult(
                "SalePrice must be less than Price.",
                new[] { nameof(SalePrice) });
        }
    }
}
