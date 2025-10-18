using InventorySys.Domain.Enums;

namespace InventorySys.Application.Features.Products.Commands.AdjustStockQuantity;

public class AdjustStockQuantityCommandValidator : AbstractValidator<AdjustStockQuantityCommand>
{
    public AdjustStockQuantityCommandValidator(IRepository<Product> repository)
    {
        RuleFor(v => v.Id)
            .GreaterThan(0);

        RuleFor(v => v.AdjustmentDirection)
            .IsInEnum()
            .NotEqual(AdjustQuantityDirection.None)
            .WithMessage("AdjustmentDirection must be a valid direction (1 increaseing/ 2 decreasing not None).");
                    
        RuleFor(v => v.Amount)
            .NotEqual(0).WithMessage("The Amount To Adjust Product Stock Quantity Cannot be zere eathier less than zero or .");

    }
}
