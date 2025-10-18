using InventorySys.Domain.Enums;

namespace InventorySys.Application.Features.Products.Commands.AdjustStockQuantity;

public record AdjustStockQuantityCommand : IRequest
{
    public int Id { get; init; }
    public AdjustQuantityDirection AdjustmentDirection { get; init; }
    public int Amount { get; init; }
}
public class AdjustStockQuantityCommandHandler : IRequestHandler<AdjustStockQuantityCommand>
{
    private readonly IProductRepository _repository;

    public AdjustStockQuantityCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AdjustStockQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, product);

        if (request.AdjustmentDirection == AdjustQuantityDirection.Increasing)
            product.IncreaseStock(request.Amount);
        else
            product.DecreaseStock(request.Amount) ;
         

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
