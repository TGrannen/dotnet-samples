namespace Caching.WebAPI.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime Created { get; set; }
}
