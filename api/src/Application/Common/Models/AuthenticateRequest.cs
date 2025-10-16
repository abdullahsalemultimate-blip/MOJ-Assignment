using System.ComponentModel.DataAnnotations;

namespace InventorySys.Application.Common.Models;

public class AuthenticateRequest
{
    [Required]
    public string Username { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}
