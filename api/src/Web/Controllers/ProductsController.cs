using InventorySys.Application.Common.Models;
using InventorySys.Application.Features.Products.Commands.AdjustStockQuantity;
using InventorySys.Application.Features.Products.Commands.CreateProduct;
using InventorySys.Application.Features.Products.Commands.DeleteProduct;
using InventorySys.Application.Features.Products.Commands.UpdateProduct;
using InventorySys.Application.Features.Products.Dtos;
using InventorySys.Application.Products.Queries.GetProductById;
using InventorySys.Application.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace InventorySys.Web.Controllers;

public class ProductsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetAll([FromQuery] GetProductsQuery request)
    {
        var products = await Mediator.Send(request);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailVm>> GetById(int id)
    {
        ProductDetailVm product = await Mediator.Send(new GetProductByIdQuery(id));
        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateProductCommand command)
        => Ok(await Mediator.Send(command));


    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await Mediator.Send(command);
        return NoContent();
    }


    [HttpPut("{id:int}/adjust-stock-quantity")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] AdjustStockQuantityCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
