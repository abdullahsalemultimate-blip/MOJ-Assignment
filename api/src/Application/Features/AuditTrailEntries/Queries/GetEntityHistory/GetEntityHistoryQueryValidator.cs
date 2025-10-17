namespace InventorySys.Application.Features.AuditTrailEntries.Queries.GetEntityHistory;

public class GetEntityHistoryQueryValidator : AbstractValidator<GetEntityHistoryQuery>
{
    public GetEntityHistoryQueryValidator()
    {
        RuleFor(x => x.EntityId)
            .NotEmpty();

        RuleFor(x => x.EntityName)
            .NotEmpty();
    }
}


