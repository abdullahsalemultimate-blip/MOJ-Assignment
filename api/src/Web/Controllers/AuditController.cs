using InventorySys.Application.Common.Models;
using InventorySys.Application.Features.AuditTrailEntries.Queries.GetEntityHistory;
using InventorySys.Application.Features.Auth.Commands.Login;
using Microsoft.AspNetCore.Mvc;

namespace InventorySys.Web.Controllers;

public class AuditController: ApiControllerBase
{

    [HttpGet("{entityName}/{entityId}")]
    public async Task<ActionResult<List<AuditEntryDto>>> GetEntityHistory(
        [FromRoute] string entityName,
        [FromRoute] string entityId,
        CancellationToken cancellationToken)
    {        
        var history = await Mediator.Send(new GetEntityHistoryQuery(entityName, entityId), cancellationToken);
        
        return Ok(history);
    }



}
