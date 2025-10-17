using InventorySys.Domain.Enums;

namespace InventorySys.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public QuantityUnit QuantityPerUnit { get; init; }
    public decimal UnitPrice { get; init; }
    public int SupplierId { get; init; }
    public int UnitsInStock { get; init; }
    public int UnitsOnOrder { get; init; }
    public int ReorderLevel { get; init; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _repository;

    public UpdateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, product);

        product.UpdateDetails(
            request.Name,
            request.QuantityPerUnit,
            request.UnitPrice,
            request.SupplierId,
            request.UnitsInStock,
            request.UnitsOnOrder,
            request.ReorderLevel
        );

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
