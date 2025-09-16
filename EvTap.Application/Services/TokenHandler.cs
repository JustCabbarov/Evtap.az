using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EvTap.Application.Exceptions;
using EvTap.Contracts.Options;
using EvTap.Contracts.Services;
using EvTap.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EvTap.Application.Services
{
    public class TokenHandler : ITokenHnadler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOption _jwtOptions;

        public TokenHandler(UserManager<ApplicationUser> userManager, IOptions<JwtOption> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<string> CreateAccessTokenAsync(ApplicationUser user)
        {
          
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtOptions.ExpiryMinutes),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);



        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
           var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
