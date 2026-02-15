namespace Caching.WebAPI.Data;

public class ProductEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
