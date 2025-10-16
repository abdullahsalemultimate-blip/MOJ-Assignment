using InventorySys.Application.Common.Models;

namespace InventorySys.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<AuthenticateResponse> LoginAsync(string userName, string password);
}
