namespace InventorySys.Application.Features.Statistics.Dtos;

public class LargestSupplierDto
{
    public int SupplierId { get; set; }
    public required string SupplierName { get; set; }
    public int TotalProducts { get; set; }
    public int TotalProductsInSystem { get; set; }
}