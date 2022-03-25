using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Authentication
{
    public class TokenManager
    {
        public IConfiguration _configuration { get; }
        public UserManager<IdentityUser> _userManager;
        public RoleManager<IdentityRole> _roleManager;
        public TokenManager(IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken (IdentityUser user) 
        {
            var role = await _userManager.GetRolesAsync(user);
            var userClaim = new List<Claim>
            {
                new Claim("UserName", user.UserName),
                new Claim("Email", user.Email)
            };
            foreach(var roleUser in role)  //add role for claim
            {
                userClaim.Add(new Claim(ClaimTypes.Role, roleUser));
            }
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaims(userClaim);
            var jwtTokenHandle = new JwtSecurityTokenHandler();
            var secretkeyBytes = await Task.Run(()=> Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretkeyBytes),SecurityAlgorithms.HmacSha256Signature)
            };          
            var token = await Task.Run(()=> jwtTokenHandle.CreateToken(tokenDescription));
            return jwtTokenHandle.WriteToken(token);
        }
    }
}
