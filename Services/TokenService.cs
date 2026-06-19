using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using UCCD_App.Models;

namespace UCCD_App.Services;

public static class TokenService
{
    public static async Task<string> GenerateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        //Claims
        var Claims = new List<Claim>
        {
              new Claim(ClaimTypes.NameIdentifier,user.Id),
              new Claim(ClaimTypes.GivenName,user.FullName),
              new Claim(ClaimTypes.Email,user.Email!),
              new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };
        var userRoles = await userManager.GetRolesAsync(user);
        Claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));



        //Signing credential
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!));
        var SigningCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



        //create token
        var token = new JwtSecurityToken
            (
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: Claims,
            expires: DateTime.Now.AddMinutes(double.Parse(configuration["Jwt:DurationInMin"]!)),
            signingCredentials: SigningCred
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
