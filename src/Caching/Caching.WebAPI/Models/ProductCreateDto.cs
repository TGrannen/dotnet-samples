namespace Caching.WebAPI.Models;

public class ProductCreateDto
{
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public decimal UnitPrice { get; set; }
}
