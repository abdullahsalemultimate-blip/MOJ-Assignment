using InventorySys.Application.Common.Interfaces;
using InventorySys.Domain.Entities;

namespace InventorySys.Application.Features.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
}

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, int>
{
    private readonly IRepository<Supplier> _repository;

    public CreateSupplierCommandHandler(IRepository<Supplier> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        Supplier supplier = new(request.Name);
        _repository.Add(supplier);
        await _repository.SaveChangesAsync(cancellationToken);
        return supplier.Id;
    }
}
