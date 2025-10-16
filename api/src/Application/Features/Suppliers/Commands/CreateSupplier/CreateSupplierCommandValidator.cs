using InventorySys.Domain.Constants;

namespace InventorySys.Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
         RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Supplier Name is required.")
                .MaximumLength(DomainConsts.NameDefaultMaxLength).WithMessage($"The Maximum Legnth for Name field is {DomainConsts.NameDefaultMaxLength} characters long.");
    }
}
