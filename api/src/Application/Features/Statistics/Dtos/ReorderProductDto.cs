namespace InventorySys.Application.Features.Statistics.Dtos;

public class ReorderProductDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int UnitsInStock { get; set; }
    public int ReorderLevel { get; set; }
    public required string SupplierName { get; set; }
}
