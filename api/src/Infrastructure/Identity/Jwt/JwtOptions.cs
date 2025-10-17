using System.ComponentModel.DataAnnotations;

namespace InventorySys.Infrastructure.Identity;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required, MinLength(16), MaxLength(32)]
    public string Key { get; set; } = null!;

    [Required]
    public string Issuer { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;

    [Range(1,43800000)] // 43800 Eqal 1 month
    public int ExpireMinutes { get; set; }
}
