using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace InventorySys.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly ILogger<IdentityService> _logger;
    
    private readonly IRequestParams _requestParams;

    private readonly ITokenService _tokenService;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<IdentityService> logger, IRequestParams requestParams, ITokenService tokenService)
    {
        _logger = logger;
        _requestParams = requestParams;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<AuthenticateResponse> LoginAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            _logger.LogWarning("Login attempt failed. Reason: User not found for UserName: {Username}, request IP Address: {RemoteIpAddress}", userName, _requestParams.RemoteIpAddress);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Login attempt failed. Reason: Account locked out for User ID: {UserId}, request IP Address: {RemoteIpAddress}", user.Id, _requestParams.RemoteIpAddress);
                throw new UnauthorizedAccessException("Account locked out due to too many failed login attempts.");
            }

            _logger.LogWarning("Login attempt failed. Reason: Invalid password for User ID: {UserId}, request IP Address: {RemoteIpAddress}", user.Id, _requestParams.RemoteIpAddress);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return new AuthenticateResponse
        {
            Jwt = await _tokenService.GetTokenAsync(user),
            Username = user.UserName!
        };
    }

}

