namespace InventorySys.Application.Features.Statistics.Dtos;

public class MinimumOrderProductDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int UnitsOnOrder { get; set; }
    public required string SupplierName { get; set; }
}
