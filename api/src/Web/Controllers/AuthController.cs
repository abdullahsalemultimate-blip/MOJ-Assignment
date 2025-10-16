using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using InventorySys.Application.Features.Auth.Commands.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySys.Web.Controllers
{

    [AllowAnonymous]
    public class AuthController: ApiControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthenticateResponse>> Login(LoginCommand req)
        {
            var response = await Mediator.Send(req);
            return Ok(response);
        }
    }
}
