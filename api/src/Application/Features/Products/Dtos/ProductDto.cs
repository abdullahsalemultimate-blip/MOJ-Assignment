using InventorySys.Domain.ValueObjects;

namespace InventorySys.Application.Features.Products.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public int UnitsInStock { get; set; }
    public int ReorderLevel { get; set; }
    public int UnitsOnOrder { get; set; }
    public decimal UnitPrice { get; set; }
}
