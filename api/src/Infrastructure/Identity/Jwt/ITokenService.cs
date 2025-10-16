using InventorySys.Application.Common.Models;

namespace InventorySys.Infrastructure.Identity;

public interface ITokenService
{
    Task<AuthTokenDto> GetTokenAsync(ApplicationUser user);
}
