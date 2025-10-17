using InventorySys.Application.Common.Interfaces;
using InventorySys.Domain.Entities;
using InventorySys.Domain.Enums;

namespace InventorySys.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public QuantityUnit QuantityPerUnit { get; set; }
    public int SupplierId { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public int UnitsOnOrder { get; set; }
    public int ReorderLevel { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {

        var product = Product.Create(
            request.Name,
            request.QuantityPerUnit,
            request.SupplierId,
            request.UnitPrice,
            request.UnitsInStock,
            request.UnitsOnOrder,
            request.ReorderLevel);

        _repository.Add(product);

        await _repository.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}