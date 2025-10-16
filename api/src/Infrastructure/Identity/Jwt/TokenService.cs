using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventorySys.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InventorySys.Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwtOptions;
    
    public TokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> options)
    {
        _userManager = userManager;
        _jwtOptions = options.Value;
    }

    public async Task<AuthTokenDto> GetTokenAsync(ApplicationUser user)
    {
        Guard.Against.Null(user, nameof(user));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Key);
        
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim> { 
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return new AuthTokenDto(tokenHandler.WriteToken(token), tokenDescriptor.Expires!.Value); ;
    }
}
