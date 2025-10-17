using InventorySys.Application.Features.Products.Commands.CreateProduct;
using InventorySys.Application.Features.Products.Commands.DeleteProduct;
using InventorySys.Application.Features.Products.Commands.UpdateProduct;
using InventorySys.Application.Products.Queries.GetProductById;
using InventorySys.Application.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace InventorySys.Web.Controllers;

public class ProductsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery request)
    {
        var products = await Mediator.Send(request);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await Mediator.Send(new GetProductByIdQuery(id));
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
        => Ok(await Mediator.Send(command));


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
