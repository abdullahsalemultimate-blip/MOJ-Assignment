using InventorySys.Domain.Entities;

namespace InventorySys.Application.Features.Suppliers.Dtos;

public class SupplierDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Supplier, SupplierDto>();
        }
    }
}
