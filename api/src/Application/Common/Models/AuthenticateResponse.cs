namespace InventorySys.Application.Common.Models;

public record AuthenticateResponse
{
    public required AuthTokenDto Jwt { get; set; } 
    
    public required string Username { get; init; }
}
