namespace ProductsApi.Models.DTO;

// Same fields and rules as create. Inheriting also inherits IValidatableObject,
// so there is only one copy of the rules to maintain.
public class UpdateProductDTO : CreateProductDTO
{
}
