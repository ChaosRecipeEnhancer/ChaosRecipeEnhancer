using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChaosRecipeEnhancer.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace ChaosRecipeEnhancer.API.Helpers;

public static class AuthHelper
{
    public static string GenerateToken(string secret, CreTokenRequest creTokenRequest)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new Claim[] { new(ClaimTypes.Name, creTokenRequest.Name), }
            ),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var serializedToken = tokenHandler.WriteToken(token);
        return serializedToken;
    }
}
