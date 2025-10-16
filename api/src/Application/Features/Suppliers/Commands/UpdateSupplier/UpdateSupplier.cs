using InventorySys.Application.Common.Interfaces;
using InventorySys.Domain.Entities;

namespace InventorySys.Application.Features.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand>
{
    private readonly IRepository<Supplier> _repository;

    public UpdateSupplierCommandHandler(IRepository<Supplier> repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, supplier);

        supplier.Update(request.Name);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
