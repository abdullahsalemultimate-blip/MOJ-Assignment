using InventorySys.Domain.Constants;

namespace InventorySys.Application.Features.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(DomainConsts.NameDefaultMaxLength);
    }
}
