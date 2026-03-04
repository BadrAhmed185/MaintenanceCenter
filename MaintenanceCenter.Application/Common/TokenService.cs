using MaintenanceCenter.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MaintenanceCenter.Application.Common
{
    public class TokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // The claim of User Public Key can be added if needed
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("Name", user.DisplayName),
                new Claim("NameIdentifier", user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               // new Claim("UserPublicKey", user.PublicKey)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
             //   audience: _jwtSettings.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.DurationInHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
      //  public string ValidAudience { get; set; } = string.Empty;
        public int DurationInHours { get; set; }
    }
}




// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYmFkclJlYyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiZTUwZjJmN2EtNDAxMS00MzNkLTk3ZGMtZjUzMzhiOTNiMTAyIiwiTmFtZSI6IkJhZHIgQWhtZWQiLCJOYW1lSWRlbnRpZmllciI6ImU1MGYyZjdhLTQwMTEtNDMzZC05N2RjLWY1MzM4YjkzYjEwMiIsImp0aSI6IjE0NzA1NDAxLTE1M2EtNGJiYy1hYTQ4LTZkNTQ2ODI5ZTY5MCIsImV4cCI6MTc3MjU5OTcxMX0.N34OLeV4c_bRsr6bvp2lr6lfU9NEU_MKqF05gZSEwDQ