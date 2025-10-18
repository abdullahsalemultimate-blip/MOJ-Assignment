using InventorySys.Application.Features.Suppliers.Commands.CreateSupplier;
using InventorySys.Application.Features.Suppliers.Commands.DeleteSupplier;
using InventorySys.Application.Features.Suppliers.Commands.UpdateSupplier;
using InventorySys.Application.Features.Suppliers.Dtos;
using InventorySys.Application.Features.Suppliers.Queries.GetAllSuppliers;
using InventorySys.Application.Features.Suppliers.Queries.GetSupplierById;
using InventorySys.Application.Features.Suppliers.Queries.GetSuppliersLookup;
using Microsoft.AspNetCore.Mvc;

namespace InventorySys.Web.Controllers;

public class SuppliersController: ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SupplierDto>>> GetAll()
        => Ok(await Mediator.Send(new GetAllSuppliersQuery()));

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierDto?>> GetById(int id)
    {
        var supplier = await Mediator.Send(new GetSupplierByIdQuery(id));
        return Ok(supplier);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create(CreateSupplierCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, UpdateSupplierCommand command)
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
        await Mediator.Send(new DeleteSupplierCommand(id));
        return NoContent();
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<SupplierDto>>> GetLookup()
        => Ok(await Mediator.Send(new GetSuppliersLookupQuery()));
}
