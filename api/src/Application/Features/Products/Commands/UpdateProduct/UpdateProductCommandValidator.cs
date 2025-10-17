using InventorySys.Application.Common.Interfaces;
using InventorySys.Domain.Constants;
using InventorySys.Domain.Entities;
using InventorySys.Domain.Enums;

namespace InventorySys.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(IRepository<Product> repository)
    {
        RuleFor(v => v.Id)
            .GreaterThan(0);

        // These Validation Are Common between Create&Update and can be abstracted in one Validator then reused in both CreateProductCommandValidator, UpdateProductCommandValidator
        RuleFor(v => v.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");

        RuleFor(v => v.SupplierId)
            .GreaterThan(0).WithMessage("SupplierId is required.");

        RuleFor(v => v.QuantityPerUnit)
            .IsInEnum()
            .NotEqual(QuantityUnit.None)
            .WithMessage("QuantityPerUnit must be a valid unit (not None).");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Product Name is required.")
            .MaximumLength(DomainConsts.NameDefaultMaxLength)
            .MustAsync(async (req, nameProp, ct) =>
                !await repository.AnyAsync(
                    p => p.Name == nameProp &&
                         p.SupplierId == req.SupplierId &&
                         p.Id != req.Id, ct))
            .WithMessage("A product with the same name already exists for this supplier.");

        // Note:
        //  - incase of invalid SupplierId instaed to making round trip to check it i will relay on Database Exceptions 
        //  - look at  GenericRepository.SaveChanges method
    }
}
