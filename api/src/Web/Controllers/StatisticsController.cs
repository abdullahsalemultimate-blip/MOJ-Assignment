using InventorySys.Application.Features.Statistics.Dtos;
using InventorySys.Application.Features.Statistics.Queries.GetLargestSupplierQuery;
using InventorySys.Application.Features.Statistics.Queries.GetProductsNeedingReorderQuery;
using InventorySys.Application.Features.Statistics.Queries.GetProductWithMinimumOrdersQuery;
using Microsoft.AspNetCore.Mvc;

namespace InventorySys.Web.Controllers;

public class StatisticsController : ApiControllerBase
{
    [HttpGet("reorder-needed")]
    public async Task<IActionResult> GetReorderNeeded()
    {
        var result = await Mediator.Send(new GetProductsNeedingReorderQuery());
        return Ok(result);
    }

    [HttpGet("largest-supplier")]
    public async Task<IActionResult> GetLargestSupplier()
    {
        var result = await Mediator.Send(new GetLargestSupplierQuery());
        return Ok(result);
    }

    [HttpGet("minimum-orders-product")]
    public async Task<IActionResult> GetMinimumOrdersProduct()
    {
        var result = await Mediator.Send(new GetProductWithMinimumOrdersQuery());
        return Ok(result);
    }
}
