using InventorySys.Domain.Enums;

namespace InventorySys.Application.Features.Products.Dtos;


public class ProductDetailVm
{
    public required IReadOnlyCollection<LookupDto> QuantityUnits { get; init; }

    public required ProductDetailDto ProductDetail { get; init; }
}

public class ProductDetailDto
{
    public int Id { get; init; }

    public required string Name { get; set; }
    public required QuantityUnit QuantityPerUnit { get; set; }
    public required int UnitsInStock { get; set; }
    public required int UnitsOnOrder { get; set; }
    public required int ReorderLevel { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int SupplierId { get; set; }


    public DateTimeOffset Created { get; init; }
    public DateTimeOffset? LastModified { get; init; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDetailDto>()
             .ForMember(dest => dest.UnitsInStock, opt => opt.MapFrom(src => src.UnitsInStock.Value))
             .ForMember(dest => dest.UnitsOnOrder, opt => opt.MapFrom(src => src.UnitsOnOrder.Value))
             .ForMember(dest => dest.ReorderLevel, opt => opt.MapFrom(src => src.ReorderLevel.Value));
        }
    }
}
