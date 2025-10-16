using InventorySys.Application.Common.Interfaces;
using InventorySys.Domain.Entities;

namespace InventorySys.Application.Features.Suppliers.Commands.DeleteSupplier;

public record DeleteSupplierCommand(int Id) : IRequest;


public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand>
{
    private readonly IRepository<Supplier> _repository;

    public DeleteSupplierCommandHandler(IRepository<Supplier> repository)
    {
        _repository = repository;
    }


    public async Task Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, supplier);

        _repository.Remove(supplier);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
